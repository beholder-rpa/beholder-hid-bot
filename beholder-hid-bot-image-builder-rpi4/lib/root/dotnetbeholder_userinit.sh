#!/bin/bash

git clone --depth=1 https://github.com/beholder-rpa/beholder-hid-bot ~/beholder-hid-bot

# If an init script exists in the cloned beholder folder, run it
if [ -f ~/beholder-hid-bot/init.sh ]; then
  ~/beholder-hid-bot/init.sh
fi