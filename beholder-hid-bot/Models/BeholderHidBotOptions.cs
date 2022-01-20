namespace beholder_hid_bot.Models
{
  using System.Collections.Generic;
  using System.Text.Json.Serialization;

  /// <summary>
  /// Represents Beholder Daemon options used by various areas of the framework.
  /// </summary>
  public record BeholderHidBotOptions
  {
    public string DiscordToken { get; set; } = string.Empty;

    public string MouseUpdatesChannel { get; set; } = "beholder-hid-bot-mouse";

    /// <summary>
    /// Gets or sets any additional properties not previously described.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object> ExtensionData { get; set; } = new Dictionary<string, object>();
  }
}