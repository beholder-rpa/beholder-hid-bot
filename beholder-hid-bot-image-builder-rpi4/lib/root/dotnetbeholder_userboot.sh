#!/bin/bash

wget --retry-connrefused --waitretry=2 --read-timeout=20 --timeout=15 -t 5 --continue -O ~/docker-compose.yml https://github.com/beholder-rpa/beholder-hid-bot/releases/download/cd-prerelease/docker-compose.yml
while [ 1 ]; do
    wget --retry-connrefused --waitretry=2 --read-timeout=20 --timeout=15 -t 5 --continue -O ~/beholder-hid-bot-arm64.tar.gz https://github.com/beholder-rpa/beholder-hid-bot/releases/download/cd-prerelease/beholder-hid-bot-arm64.tar.gz
    if [ $? = 0 ]; then break; fi; # check return value, break if successful (0)
    sleep 1s;
done;
mkdir -p ~/beholder-hid-bot
tar -xvf ~/beholder-hid-bot-arm64.tar.gz -C ~/beholder-hid-bot
rm ~/beholder-hid-bot-arm64.tar.gz

# If a boot script exists in the cloned beholder-hid-bot folder, run it
if [ -f ~/beholder-hid-bot/boot.sh ]; then
  ~/beholder-hid-bot/boot.sh
fi
