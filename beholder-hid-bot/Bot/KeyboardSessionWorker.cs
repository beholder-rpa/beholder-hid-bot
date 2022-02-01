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
    private readonly ConcurrentDictionary<string, KeyboardSession> _keyboardSessions = new ConcurrentDictionary<string, KeyboardSession>();
    private readonly Timer _keyboardSessionTimer = new(100);

    public KeyboardSessionWorker(Keyboard keyboard)
    {
      _keyboard = keyboard ?? throw new ArgumentNullException(nameof(keyboard));
      _keyboardSessionTimer.Elapsed += ProcessKeyboardSessions;
      _keyboardSessionTimer.AutoReset = true;
      _keyboardSessionTimer.Enabled = true;
    }

    public bool TryAdd(string name, KeyboardSession session)
    {
      return _keyboardSessions.TryAdd(name, session);
    }

    public void AddOrUpdate(string name, KeyboardSession session)
    {
      _keyboardSessions.AddOrUpdate(name, session, (name, ks) => session);
    }

    public bool Remove(string name)
    {
      return _keyboardSessions.Remove(name, out KeyboardSession _);
    }

    private void ProcessKeyboardSessions(object? source, ElapsedEventArgs e)
    {
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
  }
}
