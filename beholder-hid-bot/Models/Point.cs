namespace beholder_hid_bot.Models
{
  using System.Text.Json.Serialization;

  public record Point
  {
    [JsonPropertyName("x")]
    public int X
    {
      get;
      set;
    }

    [JsonPropertyName("y")]
    public int Y
    {
      get;
      set;
    }

    public static Point Empty = new Point() { X = 0, Y = 0 };
  }
}