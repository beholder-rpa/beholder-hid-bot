namespace beholder_hid_bot.Discord.Modules
{
  using beholder_hid_bot.HardwareInterfaceDevices;
  using global::Discord.Commands;
  using System.Collections.Concurrent;
  using System.Timers;

  public class KeyboardModule : ModuleBase<SocketCommandContext>
  {
    private readonly Keyboard _keyboard;
    private readonly KeyboardSessionWorker _keyboardSessionWorker;
    private readonly ILogger<KeyboardModule> _logger;

    public KeyboardModule(Keyboard keyboard, KeyboardSessionWorker keyboardSessionWorker, ILogger<KeyboardModule> logger)
    {
      _keyboard = keyboard ?? throw new ArgumentNullException(nameof(keyboard));
      _keyboardSessionWorker = keyboardSessionWorker ?? throw new ArgumentNullException(nameof(keyboardSessionWorker));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

    [Command("kstart")]
    [Summary("Starts a keyboard button mashing session")]
    [Remarks("{oemtilde}")]
    public Task KeyboardStart(string name, string keys, double delay, int max)
    {
      if (_keyboardSessionWorker.TryAdd(name, new KeyboardSession()
      {
        Name = name,
        Keys = keys,
        RepeatDelaySeconds = delay,
        MaxRepeats = max,
      }))
      {
        _logger.LogInformation("Started keyboard session for {sessionName} - Press '{keys}' every {delay}s until {max} or ended.", name, keys, delay, max);
      }
      return Task.CompletedTask;
    }

    [Command("kend")]
    [Summary("Ends a keyboard button mashing session")]
    [Remarks("{oemtilde}")]
    public Task KeyboardEnd(string name)
    {
      if (_keyboardSessionWorker.Remove(name))
      {
        _logger.LogInformation("Removed keyboard session for {sessionName}", name);
      }
      return Task.CompletedTask;
    }
  }
}
