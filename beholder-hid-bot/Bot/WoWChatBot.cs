namespace beholder_hid_bot.Bot;

using beholder_hid_bot.HardwareInterfaceDevices;
using beholder_hid_bot.Models;
using WoWChat.Net.Common;
using System;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using WoWChat.Net.Game.Events;

public class WoWChatBot : IObserver<IWoWChatEvent>
{
  private static readonly Regex SendKeysRegex = new(@"^%sendkeys\s+(?<Keys>.*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
  private static readonly Regex SendKeyRegex = new(@"^%sendkey\s+(?<Key>.*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
  private static readonly Regex SendKeysResetRegex = new(@"^%sendkeysreset$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
  private static readonly Regex KstartRegex = new(@"^%kstart\s+""?(?<Name>.*)""?\s+(?<Keys>.*)\s+(?<Delay>((\d+(\.\d*)?)|(\.\d+))*)\s+(?<Max>\d*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
  private static readonly Regex KupdateRegex = new(@"^%kupdate\s+""?(?<Name>.*)""?\s+(?<Keys>.*)\s+(?<Delay>((\d+(\.\d*)?)|(\.\d+))*)\s+(?<Max>\d*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
  private static readonly Regex KendRegex = new(@"^%kend\s+""?(?<Name>.*)""?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

  private readonly Keyboard _keyboard;
  private readonly KeyboardSessionWorker _keyboardSessionWorker;
  private readonly WoWChatBotOptions _options;
  private readonly GameNameLookup _gameNameLookup;
  private readonly ILogger<WoWChatBot> _logger;

  public WoWChatBot(
    Keyboard keyboard,
    KeyboardSessionWorker keyboardSessionWorker,
    GameNameLookup gameNameLookup,
    IOptions<WoWChatBotOptions> options,
    ILogger<WoWChatBot> logger)
  {
    _keyboard = keyboard ?? throw new ArgumentNullException(nameof(keyboard));
    _keyboardSessionWorker = keyboardSessionWorker ?? throw new ArgumentNullException(nameof(keyboardSessionWorker));
    _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    _gameNameLookup = gameNameLookup ?? throw new ArgumentNullException(nameof(gameNameLookup));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  public async Task ProcessPlayerCommand(GameNameQuery player, GameChatMessage message)
  {
    switch(message.Message)
    {
      case var txt when SendKeysRegex.IsMatch(txt):
        var sendKeys = SendKeyRegex.Match(txt).Groups["Keys"].Value;
        await _keyboard.SendKeys(sendKeys);
        break;
      case var txt when SendKeyRegex.IsMatch(txt):
        var sendKey = SendKeyRegex.Match(txt).Groups["Key"].Value;
        await _keyboard.SendKey(sendKey);
        break;
      case var txt when SendKeysResetRegex.IsMatch(txt):
        _keyboard.SendKeysReset();
        break;
      case var txt when KstartRegex.IsMatch(txt):
        {
          var kstartMatch = KstartRegex.Match(txt);
          var kstartName = kstartMatch.Groups["Name"].Value;
          var keys = kstartMatch.Groups["Keys"].Value;
          var delay = double.Parse(kstartMatch.Groups["Delay"].Value);
          var max = int.Parse(kstartMatch.Groups["Max"].Value);
          if (_keyboardSessionWorker.TryAdd(kstartName, new KeyboardSession()
          {
            Name = kstartName,
            Keys = keys,
            RepeatDelaySeconds = delay,
            MaxRepeats = max,
          }))
          {
            _logger.LogInformation("Started keyboard session for {sessionName} - Press '{keys}' every {delay}s until {max} or ended.", kstartName, keys, delay, max);
          }
        }
        break;
      case var txt when KupdateRegex.IsMatch(txt):
        {
          var kupdateMatch = KupdateRegex.Match(txt);
          var kupdatename = kupdateMatch.Groups["Name"].Value;
          var keys = kupdateMatch.Groups["Keys"].Value;
          var delay = double.Parse(kupdateMatch.Groups["Delay"].Value);
          var max = int.Parse(kupdateMatch.Groups["Max"].Value);
          _keyboardSessionWorker.AddOrUpdate(kupdatename, new KeyboardSession()
          {
            Name = kupdatename,
            Keys = keys,
            RepeatDelaySeconds = delay,
            MaxRepeats = max,
          });
          _logger.LogInformation("Add/Updated keyboard session for {sessionName} - Press '{keys}' every {delay}s until {max} or ended.", kupdatename, keys, delay, max);
        }
        break;
      case var txt when KendRegex.IsMatch(txt):
        var kendName = KendRegex.Match(txt).Groups["Name"].Value;
        if (_keyboardSessionWorker.Remove(kendName))
        {
          _logger.LogInformation("Removed keyboard session for {sessionName}", kendName);
        }
        break;
    }
  }

  #region IObserver<IWoWChatEvent>
  void IObserver<IWoWChatEvent>.OnCompleted()
  {
    // Do Nothing
  }

  void IObserver<IWoWChatEvent>.OnError(Exception error)
  {
    // Do Nothing
  }

  void IObserver<IWoWChatEvent>.OnNext(IWoWChatEvent value)
  {
    switch (value)
    {
      case GameChatMessageEvent messageEvent when
        messageEvent.ChatMessage.Language == Language.Addon &&
        messageEvent.ChatMessage.AddonName == _options.AddonName &&
        messageEvent.ChatMessage.MessageType == ChatMessageType.Whisper &&
        _gameNameLookup.TryGetName(messageEvent.ChatMessage.SenderId, out var sender) &&
        sender?.Name.ToLower() == _options.AllowedCharacter.ToLower():
        ProcessPlayerCommand(sender, messageEvent.ChatMessage).Forget();
        break;
    }
  }
  #endregion
}
