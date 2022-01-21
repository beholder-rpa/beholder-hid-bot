#!/bin/bash

git clone --depth=1 https://github.com/beholder-rpa/beholder-hid-bot ~/beholder-hid-bot

# TODO: Consider using a sparse checkout to download the helm charts
# git clone \
#   --depth 1  \
#   --filter=blob:none  \
#   --sparse \
#   https://github.com/beholder-rpa/beholder-hid-bot \
# ;
# cd beholder-hid-bot
# git sparse-checkout init --cone
# git sparse-checkout set charts

# If a boot script exists in the cloned beholder-hid-bot folder, run it
if [ -f ~/beholder-hid-bot/boot.sh ]; then
  ~/beholder-hid-bot/boot.sh
fi
