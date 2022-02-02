namespace beholder_hid_bot.Bot
{
  public record BlockSession
  {
    public string Name { get; init; } = string.Empty;

    public double Duration { get; init; } = 1;

    public DateTime ExpiresAt { get; init; } = DateTime.MinValue;
  }
}
