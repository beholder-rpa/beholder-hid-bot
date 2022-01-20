namespace beholder_hid_bot.Models
{
  using System.Text.Json.Serialization;

  public record Size
  {
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

    public static Size Empty = new Size { Height = 0, Width = 0 };
  }
}