#!/usr/bin/env bash
# One-time setup on the Iowa Bins Pi, run with sudo. Safe to re-run.
# Adds a second site beside Iowa Bins without touching it: a service user, an app folder the
# existing deploy user can write, the systemd unit, and a sudo rule that lets the deploy user
# restart this one service and nothing else.
set -euo pipefail

id scottbush >/dev/null 2>&1 || useradd --system --no-create-home --shell /usr/sbin/nologin scottbush
install -d -o deploy -g deploy -m 755 /opt/scottbush /opt/scottbush/app
install -m 644 "$(dirname "$0")/scottbush.service" /etc/systemd/system/scottbush.service
echo 'deploy ALL=(root) NOPASSWD: /usr/bin/systemctl restart scottbush' > /etc/sudoers.d/scottbush-deploy
chmod 440 /etc/sudoers.d/scottbush-deploy
visudo -cf /etc/sudoers.d/scottbush-deploy
systemctl daemon-reload
systemctl enable scottbush
echo "Ready. Deploy a build, then: systemctl restart scottbush"
