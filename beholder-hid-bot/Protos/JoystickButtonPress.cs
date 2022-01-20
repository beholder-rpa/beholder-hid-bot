﻿namespace beholder_hid_bot.Protos
{
  public partial class JoystickButtonPress : IJoystickAction
  {
  }

  public partial class JoystickMove : IJoystickAction
  {
  }

  public partial class JoystickThrottle : IJoystickAction
  {
  }

  partial class JoystickButtonPressDuration : IDuration
  {
    internal static JoystickButtonPressDuration Infinite = new JoystickButtonPressDuration() { Delay = uint.MaxValue, Min = uint.MaxValue, Max = uint.MaxValue };
  }
}