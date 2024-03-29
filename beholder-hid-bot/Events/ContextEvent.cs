﻿using beholder_hid_bot.Models;

namespace beholder_stalk_v2
{
  /// <summary>
  /// Represents an abstract event that is produced by Context Controller.
  /// Use Pattern Matching to handle specific instances of this type.
  /// </summary>
  public abstract record ContextEvent
  {
  }

  /// <summary>
  /// Represents an event that is raised when the pointer position has changed
  /// </summary>
  public record PointerPositionChangedEvent : ContextEvent
  {
    public string Source { get; init; } = string.Empty;

    public Point OldPointerPosition { get; init; } = Point.Empty;

    public Point NewPointerPosition { get; init; } = Point.Empty;
  }

  /// <summary>
  /// Represents an event that is raised when the system information has changed
  /// </summary>
  public record SystemInformationChangedEvent : ContextEvent
  {
    public SysInfo SysInfo { get; init; } = new SysInfo();
  }
}