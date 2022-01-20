namespace beholder_hid_bot.Models
{
  using System.Text.Json.Serialization;

  public record Rectangle
  {
    [JsonPropertyName("x")]
    public int X
    {
      get;
      init;
    }

    [JsonPropertyName("y")]
    public int Y
    {
      get;
      init;
    }

    [JsonPropertyName("width")]
    public int Width
    {
      get;
      init;
    }

    [JsonPropertyName("height")]
    public int Height
    {
      get;
      init;
    }
  }
}