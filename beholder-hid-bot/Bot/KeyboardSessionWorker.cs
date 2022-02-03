namespace beholder_hid_bot.Bot
{
  using beholder_hid_bot.HardwareInterfaceDevices;
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Timers;

  public class KeyboardSessionWorker
  {
    private readonly Keyboard _keyboard;
    private readonly object _sessionLock = new();
    private readonly object _blockLock = new();
    private readonly object _repeatLock = new();
    private readonly ConcurrentDictionary<string, KeyboardSession> _keyboardSessions = new();
    private readonly ConcurrentDictionary<string, BlockSession> _blockSessions = new();
    private readonly ConcurrentDictionary<string, RepeatSession> _repeatSessions = new();
    private readonly Timer _keyboardSessionTimer = new(100);
    private readonly Timer _blockSessionTimer = new(25);
    private readonly Timer _repeatSessionTimer = new(25);

    public KeyboardSessionWorker(Keyboard keyboard)
    {
      _keyboard = keyboard ?? throw new ArgumentNullException(nameof(keyboard));
      _keyboardSessionTimer.Elapsed += ProcessKeyboardSessions;
      _keyboardSessionTimer.AutoReset = true;
      _keyboardSessionTimer.Enabled = true;

      _blockSessionTimer.Elapsed += ProcessBlockSessions;
      _blockSessionTimer.AutoReset = true;
      _blockSessionTimer.Enabled = true;

      _repeatSessionTimer.Elapsed += ProcessRepeatSessions;
      _repeatSessionTimer.AutoReset = true;
      _repeatSessionTimer.Enabled = true;
    }

    public bool TryAdd(string name, KeyboardSession session)
    {
      return _keyboardSessions.TryAdd(name, session);
    }

    public bool TryAddBlock(string name, BlockSession session)
    {
      return _blockSessions.TryAdd(name, session with { ExpiresAt = DateTime.Now.AddSeconds(session.Duration)});
    }

    public bool TryAddRepeat(string name, RepeatSession session)
    {
      return _repeatSessions.TryAdd(name, session);
    }

    public void AddOrUpdate(string name, KeyboardSession session)
    {
      _keyboardSessions.AddOrUpdate(name, session, (name, ks) => session);
    }

    public void AddOrUpdateBlock(string name, BlockSession session)
    {
      var ns = session with { ExpiresAt = DateTime.Now.AddSeconds(session.Duration) };
      _blockSessions.AddOrUpdate(name, ns, (name, ks) => ns);
    }

    public bool Remove(string name)
    {
      return _keyboardSessions.Remove(name, out KeyboardSession _);
    }

    public bool RemoveBlock(string name)
    {
      return _blockSessions.Remove(name, out BlockSession _);
    }

    public bool RemoveRepeat(string name)
    {
      return _repeatSessions.Remove(name, out RepeatSession _);
    }

    private void ProcessKeyboardSessions(object? source, ElapsedEventArgs e)
    {
      // If there are any block session, skip.
      if (_blockSessions.Count > 0)
      {
        return;
      }

      if (Monitor.TryEnter(_sessionLock, -1))
      {
        foreach (var session in _keyboardSessions.Values)
        {
          if (session.Presses >= session.MaxRepeats)
          {
            _keyboardSessions.Remove(session.Name, out KeyboardSession _);
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

    private void ProcessBlockSessions(object? source, ElapsedEventArgs e)
    {
      if (Monitor.TryEnter(_blockLock, -1))
      {
        foreach (var session in _blockSessions.Values)
        {
          if (session.ExpiresAt < e.SignalTime)
          {
            _blockSessions.Remove(session.Name, out BlockSession _);
            continue;
          }
        }

        Monitor.Exit(_blockLock);
      }
    }

    private void ProcessRepeatSessions(object? source, ElapsedEventArgs e)
    {
      if (Monitor.TryEnter(_repeatLock, -1))
      {
        foreach (var session in _repeatSessions.Values)
        {
          if (session.LastTriggered == null || e.SignalTime - session.LastTriggered >= TimeSpan.FromSeconds(session.IntervalSeconds))
          {
            AddOrUpdate(session.Name, new KeyboardSession()
            {
              Name = session.Name,
              Keys = session.Keys,
              RepeatDelaySeconds = session.RepeatDelaySeconds,
              MaxRepeats = session.MaxRepeats,
            });
            session.LastTriggered = DateTime.Now;
            continue;
          }
        }

        Monitor.Exit(_repeatLock);
      }
    }
  }
}
