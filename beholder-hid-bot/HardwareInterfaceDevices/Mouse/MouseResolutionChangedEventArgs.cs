namespace beholder_hid_bot.HardwareInterfaceDevices
{
  using System;

  public class MouseResolutionChangedEventArgs : EventArgs
  {
    public MouseResolutionChangedEventArgs(int horizontalResolution, int verticalResolution)
        : base()
    {
      HorizontalResolution = horizontalResolution;
      VerticalResolution = verticalResolution;
    }

    public int HorizontalResolution { get; private set; }

    public int VerticalResolution { get; private set; }
  }
}