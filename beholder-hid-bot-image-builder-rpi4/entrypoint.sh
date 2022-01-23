#!/bin/bash

echo "# Building Beholder Raspberry Pi 4 image..."
IMAGE_SUFFIX=$1
WPA_SSID=$2
WPA_PASSPHRASE=$3
RPI_HOSTNAME=$4
RPI_TIMEZONE=$5
BEHOLDER_MODE=$6

IMAGE_PATH=$(find /create/images/*${IMAGE_SUFFIX}.img -type f -print -quit)

if [[ -z "${IMAGE_PATH// }" ]]; then
  echo "No Raspberry Pi OS image detected at ${IMAGE_PATH}!"
  exit 1
fi

echo "Using ${IMAGE_PATH} as the mountable Raspberry Pi OS image"

# Capture the patition details.
BOOT_PARTITION=`fdisk -l "${IMAGE_PATH}" | grep "c W95 FAT32 (LBA)"`
ROOT_PARTITION=`fdisk -l "${IMAGE_PATH}" | grep "83 Linux"`

# Grab the starting sector of the partitions.
BOOT_START_SECTOR=`echo "$BOOT_PARTITION" | awk '{print $2}'`
ROOT_START_SECTOR=`echo "$ROOT_PARTITION" | awk '{print $2}'`

# Calculate the start byte of the partitions.
((BOOT_START_BYTE=$BOOT_START_SECTOR * 512))
((ROOT_START_BYTE=$ROOT_START_SECTOR * 512))

# Grab the sector length of the partitions.
BOOT_SECTOR_LENGTH=`echo "$BOOT_PARTITION" | awk '{print $4}'`
ROOT_SECTOR_LENGTH=`echo "$ROOT_PARTITION" | awk '{print $4}'`

# Calculate the byte length of the partitions.
((BOOT_BYTE_LENGTH=$BOOT_SECTOR_LENGTH * 512))
((ROOT_BYTE_LENGTH=$ROOT_SECTOR_LENGTH * 512))

# Create the mount points.
mkdir -p /mnt/image/boot
mkdir -p /mnt/image/root

# Mount the partitions to the mount points.
mount -v -o offset=$BOOT_START_BYTE,sizelimit=$BOOT_BYTE_LENGTH -t vfat "${IMAGE_PATH}" /mnt/image/boot
mount -v -o offset=$ROOT_START_BYTE,sizelimit=$ROOT_BYTE_LENGTH -t ext4 "${IMAGE_PATH}" /mnt/image/root

./index.js

# Copy generated files on over to the boot image
cp ./lib/boot/wpa_supplicant.conf /mnt/image/boot
touch /mnt/image/boot/ssh

# Copy configuration data on over to the root image
if (( $BEHOLDER_MODE == "dotnetbeholder" )); then
  echo "# Setting up dotnet Beholder..."
  cp ./lib/root/dotnetbeholder_init.sh /mnt/image/root/etc/beholder_init.sh
  cp ./lib/root/dotnetbeholder_boot.sh /mnt/image/root/usr/bin/beholder_boot.sh

  cp ./lib/root/dotnetbeholder_userinit.sh /mnt/image/root/etc/
  chmod +x /mnt/image/root/etc/dotnetbeholder_userinit.sh

  cp ./lib/root/dotnetbeholder_userboot.sh /mnt/image/root/etc/
  chmod +x /mnt/image/root/etc/dotnetbeholder_userboot.sh
else
  echo "# Unknown Mode: $BEHOLDER_MODE"
  exit 1
fi

chmod +x /mnt/image/root/etc/beholder_init.sh
chmod +x /mnt/image/root/usr/bin/beholder_boot.sh

cp ./lib/root/beholder_boot.service /mnt/image/root/etc/systemd/system/
chmod 644 /mnt/image/root/etc/systemd/system/beholder_boot.service

cp ./lib/root/hostname /mnt/image/root/etc/
cp ./lib/root/timezone /mnt/image/root/etc/

echo "127.0.0.1       $RPI_HOSTNAME" | tee -a /mnt/image/root/etc/hosts

cp ./lib/root/avahi-daemon.conf /mnt/image/root/etc/avahi/
chmod 644 /mnt/image/root/etc/avahi/avahi-daemon.conf

cp ./lib/root/avahi-alias@.service /mnt/image/root/etc/systemd/system/
chmod 644 /mnt/image/root/etc/systemd/system/avahi-alias@.service

# Add the boot script
sed -i -e '$i\/etc/beholder_init.sh' /mnt/image/root/etc/rc.local

${@:7}

# Unmount the partitions
umount /mnt/image/boot
umount /mnt/image/root
if (( $BEHOLDER_MODE == "dotnetbeholder" )); then
  echo "# dotnet Beholder Raspberry Pi 4 image build completed."
else
  echo "# Unknown Mode: $BEHOLDER_MODE"
  exit 1
fi

echo "# Create a bootable SD or USB drive from the image located at ${IMAGE_PATH/\/create\//.\/} using balenaEtcher or your favourite image creator."