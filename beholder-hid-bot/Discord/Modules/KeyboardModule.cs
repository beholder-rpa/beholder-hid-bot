namespace beholder_hid_bot.Discord.Modules
{
  using beholder_hid_bot.HardwareInterfaceDevices;
  using global::Discord.Commands;
  using System.Collections.Concurrent;
  using System.Timers;

  public class KeyboardModule : ModuleBase<SocketCommandContext>
  {
    private readonly object _sessionLock = new();
    private readonly Keyboard _keyboard;
    private readonly ILogger<KeyboardModule> _logger;
    private readonly IDictionary<string, KeyboardSession> _keyboardSessions = new ConcurrentDictionary<string, KeyboardSession>();
    private readonly Timer _keyboardSessionTimer = new(100);

    public KeyboardModule(Keyboard keyboard, ILogger<KeyboardModule> logger)
    {
      _keyboard = keyboard ?? throw new ArgumentNullException(nameof(keyboard));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _keyboardSessionTimer.Elapsed += ProcessKeyboardSessions;
      _keyboardSessionTimer.AutoReset = true;
      _keyboardSessionTimer.Enabled = true;
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
      if (_keyboardSessions.TryAdd(name, new KeyboardSession()
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
      if (_keyboardSessions.Remove(name))
      {
        _logger.LogInformation("Removed keyboard session for {sessionName}", name);
      }
      return Task.CompletedTask;
    }

    private void ProcessKeyboardSessions(object? source, ElapsedEventArgs e)
    {
      if (Monitor.TryEnter(_sessionLock, -1))
      {
        foreach (var session in _keyboardSessions.Values)
        {
          if (session.Presses >= session.MaxRepeats)
          {
            _keyboardSessions.Remove(session.Name);
            continue;
          }

          if (session.Presses == 0 || e.SignalTime - session.LastPress >= TimeSpan.FromSeconds(session.RepeatDelaySeconds))
          {
            _keyboard.SendKeys(session.Keys).GetAwaiter().GetResult();
            session.LastPress = DateTime.Now;
            session.Presses++;
            continue;
          }
        }

        Monitor.Exit(_sessionLock);
      }
    }
  }
}
