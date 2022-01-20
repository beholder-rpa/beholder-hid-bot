namespace beholder_hid_bot.Protos
{
  using System.ComponentModel.DataAnnotations;

  [MetadataType(typeof(KeypressMetadata))]
  public partial class Keypress
  {
  }

  partial class KeypressDuration : IDuration
  {
    internal static KeypressDuration Infinite = new KeypressDuration() { Delay = uint.MaxValue, Min = uint.MaxValue, Max = uint.MaxValue };
  }
}