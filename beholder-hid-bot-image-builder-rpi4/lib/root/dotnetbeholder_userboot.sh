#!/bin/bash

rm -rf ~/beholder-hid-bot
wget https://github.com/beholder-rpa/beholder-hid-bot/releases/download/cd-prerelease/beholder-hid-bot-arm64.tar.gz
mkdir ~/beholder-hid-bot
tar -xvf ./beholder-hid-bot-arm64.tar.gz -C ~/beholder-hid-bot
rm beholder-hid-bot-arm64.tar.gz

# If a boot script exists in the cloned beholder-hid-bot folder, run it
if [ -f ~/beholder-hid-bot/boot.sh ]; then
  ~/beholder-hid-bot/boot.sh
fi
