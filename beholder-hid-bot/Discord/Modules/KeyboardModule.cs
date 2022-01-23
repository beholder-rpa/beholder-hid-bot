namespace beholder_hid_bot.Discord.Modules
{
  using beholder_hid_bot.HardwareInterfaceDevices;
  using global::Discord.Commands;
  using System.Threading.Tasks;

  public class KeyboardModule : ModuleBase<SocketCommandContext>
  {
    private readonly Keyboard _keyboard;

    public KeyboardModule(Keyboard keyboard)
    {
      _keyboard = keyboard ?? throw new ArgumentNullException(nameof(keyboard));
    }

    [Command("sendkeys")]
    [Summary("Sends a sequence of keyboard commands")]
    [Remarks("{Left-Shift down}h{Left-Shift up}ello, !world{!}")]
    public async Task SendKeys([Remainder][Summary("The text to echo")] string echo)
    {
      await _keyboard.SendKeys(echo);
    }

    [Command("sendkey")]
    [Summary("Sends a single keyboard")]
    [Remarks("{oemtilde}")]
    public async Task SendKey([Remainder][Summary("The key to send")] string key)
    {
      await _keyboard.SendKey(key);
    }

    [Command("sendkeysreset")]
    [Summary("Resets the keyboard")]
    [Remarks("{oemtilde}")]
    public async Task SendKeyReset()
    {
      _keyboard.SendKeysReset();

      await ReplyAsync("Keys Reset.");
    }
  }
}
