#!/usr/bin/env bash
# Build, test and ship to the Pi over Tailscale, then wait for the public site to report the
# new commit. Rollback is: check out the previous commit and run this again.
set -euo pipefail
cd "$(dirname "$0")/.."

PI_HOST="${PI_HOST:-iowabins.tail0e1b9d.ts.net}"
SITE="${SITE:-https://scottbush.dev}"
commit=$(git rev-parse HEAD)

dotnet test --configuration Release
rm -rf publish
dotnet publish src/ScottBush.Web --configuration Release --runtime linux-arm64 --self-contained \
  -p:SourceRevisionId="$commit" --output publish

rsync -az --delete --rsync-path="sudo -u deploy rsync" publish/ "$PI_HOST:/opt/scottbush/app/"
ssh "$PI_HOST" sudo /usr/bin/systemctl restart scottbush

for _ in $(seq 1 30); do
  body=$(curl -fsS --max-time 5 "$SITE/healthz" || true)
  if [[ "$body" == *'"status":"ok"'* && "$body" == *"\"commit\":\"$commit\""* ]]; then
    echo "Live: $commit"; exit 0
  fi
  sleep 2
done
echo "The site did not report commit $commit within a minute." >&2
exit 1
