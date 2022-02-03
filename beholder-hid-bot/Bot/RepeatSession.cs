namespace beholder_hid_bot.Bot
{
  using System;

  public record RepeatSession
  {
    public string Name { get; init; } = string.Empty;

    public string Keys { get; init; } = string.Empty;

    public double IntervalSeconds { get; init; } = 1;

    public double RepeatDelaySeconds { get; init; } = 1;

    public int MaxRepeats { get; init; } = 1;

    public DateTime? LastTriggered { get; set; } = null;
  }
}
