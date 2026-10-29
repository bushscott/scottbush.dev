#!/usr/bin/env bash
# One-time setup on the Iowa Bins Pi, run with sudo. Safe to re-run.
# Usage: sudo bash pi-setup.sh <deploy-key.pub>
#
# Adds this site beside Iowa Bins and shares nothing with it but the machine and the tunnel:
#   scottbush             runs the app; no login, no home
#   scottbush-portfolio   receives deploys; key-only, owns /opt/scottbush/app and nothing else,
#                         and may restart this one service and nothing else
# Iowa Bins' own deploy user and setup script never touch either account.
set -euo pipefail
here="$(dirname "$0")"
pubkey="${1:?pass the deploy public key file}"

id scottbush >/dev/null 2>&1 || useradd --system --no-create-home --shell /usr/sbin/nologin scottbush
id scottbush-portfolio >/dev/null 2>&1 || \
  useradd --create-home --shell /bin/bash --comment "Scott Bush Portfolio deploy" scottbush-portfolio

# The key is restricted: tailnet addresses only, no terminal, no forwarding.
install -d -o scottbush-portfolio -g scottbush-portfolio -m 700 /home/scottbush-portfolio/.ssh
echo "restrict,from=\"100.64.0.0/10,fd7a:115c:a1e0::/48\" $(cat "$pubkey")" > /home/scottbush-portfolio/.ssh/authorized_keys
chown scottbush-portfolio:scottbush-portfolio /home/scottbush-portfolio/.ssh/authorized_keys
chmod 600 /home/scottbush-portfolio/.ssh/authorized_keys

install -d -o root -g root -m 755 /opt/scottbush
install -d -m 755 /opt/scottbush/app
chown -R scottbush-portfolio:scottbush-portfolio /opt/scottbush/app

install -m 644 "$here/scottbush.service" /etc/systemd/system/scottbush.service
rm -f /etc/sudoers.d/scottbush-deploy   # replaced: the shared deploy user no longer deploys this site
echo 'scottbush-portfolio ALL=(root) NOPASSWD: /usr/bin/systemctl restart scottbush' > /etc/sudoers.d/scottbush-portfolio
chmod 440 /etc/sudoers.d/scottbush-portfolio
visudo -cf /etc/sudoers.d/scottbush-portfolio
systemctl daemon-reload
systemctl enable scottbush
echo "Ready."
