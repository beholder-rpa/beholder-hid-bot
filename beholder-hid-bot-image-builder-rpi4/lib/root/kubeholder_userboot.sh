#!/bin/bash

git clone --depth=1 https://github.com/beholder-rpa/beholder-hid-bot ~/beholder-hid-bot

# If a boot script exists in the cloned beholder-hid-bot folder, run it
if [ -f ~/beholder-hid-bot/boot.sh ]; then
  ~/beholder-hid-bot/boot.sh
fi
