namespace beholder_hid_bot_tests;

using Xunit;
using beholder_hid_bot.HardwareInterfaceDevices;
using System.IO;

public class KeyboardTests
{
  [Fact]
  public void KeyboardShouldParseModifiers()
  {
    // Arrange
    var keys = "!hello";

    File.Create("./keys.bin");
    // Act
  }
}