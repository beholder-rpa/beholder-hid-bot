namespace beholder_hid_bot.Bot
{
  using System;

  public record KeyboardSession
  {
    public string Name { get; init; } = string.Empty;

    public string Keys { get; init; } = string.Empty;

    public double RepeatDelaySeconds { get; init; } = 1;

    public int MaxRepeats { get; init; } = 1;

    public DateTime? LastPress { get; set; } = null;

    public int Presses { get; set; } = 0;
  }
}
