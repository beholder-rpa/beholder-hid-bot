namespace beholder_hid_bot
{
  using beholder_hid_bot.Models;
  using global::Discord.WebSocket;
  using Microsoft.Extensions.Logging;
  using Microsoft.Extensions.Options;
  using System;
  using System.Text;
  using System.Threading.Tasks;

  public class MouseObserver : IObserver<MouseEvent>
  {
    private readonly BeholderHidBotOptions _botOptions;
    private readonly ILogger<MouseObserver> _logger;
    private readonly DiscordSocketClient _discordClient;

    public MouseObserver(IOptions<BeholderHidBotOptions> botOptions, DiscordSocketClient discordClient, ILogger<MouseObserver> logger)
    {
      _botOptions = botOptions?.Value ?? throw new ArgumentNullException(nameof(botOptions));
      _discordClient = discordClient ?? throw new ArgumentNullException(nameof(discordClient));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnCompleted()
    {
      // Do Nothing
    }

    public void OnError(Exception error)
    {
      // Do Nothing
    }

    public void OnNext(MouseEvent mouseEvent)
    {
      switch (mouseEvent)
      {
        case MouseResolutionChangedEvent mouseResolutionChanged:
          HandleMouseResolutionChanged(mouseResolutionChanged).Forget();
          break;
        case MouseMoveToPointsEvent mouseMoveToPoints:
          HandleMouseMoveToPoints(mouseMoveToPoints).Forget();
          break;
        case MovedMouseEvent movedMouse:
          HandleMouseMoved(movedMouse).Forget();
          break;
        default:
          _logger.LogWarning($"Unhandled or unknown MouseEvent: {mouseEvent}");
          break;
      }
    }

    private Task HandleMouseResolutionChanged(MouseResolutionChangedEvent mouseResolutionChanged)
    {
      //var mouseUpdatesChannel = _discordClient.GroupChannels.FirstOrDefault(n => n.Name == _botOptions.MouseUpdatesChannel);

      //if (mouseUpdatesChannel == null)
      //{
      //  // create the channel
      //  _discordClient.Guilds.First().CreateTextChannelAsync
      //  var newChannel = await _discordClient.guildCreateTextChannelAsync(_botOptions.MouseUpdatesChannel);

      //  // If you need the newly created channels id
      //  var newChannelId = newChannel.Id;
      //}
        
      _logger.LogTrace($"Published mouse resolution changed");
      return Task.CompletedTask;
    }

    private Task HandleMouseMoveToPoints(MouseMoveToPointsEvent mouseMoveToPoints)
    {
      var sb = new StringBuilder();
      sb.AppendLine($"x,y,");
      foreach (var point in mouseMoveToPoints.Points)
      {
        sb.AppendLine($"{point.X},{point.Y},");
      }

      //await _beholderClient
      //  .PublishEventAsync(
      //    mouseMoveToPoints.Topic,
      //    sb.ToString()
      //  );
      _logger.LogTrace($"Published mouse move to points");
      return Task.CompletedTask;
    }

    private Task HandleMouseMoved(MovedMouseEvent mouseMoved)
    {
      //await _beholderClient
      //  .PublishEventAsync(
      //    "beholder/stalk/{HOSTNAME}/status/mouse/moved",
      //    mouseMoved
      //  );
      _logger.LogTrace($"Published mouse moved");
      return Task.CompletedTask;
    }
  }
}