namespace beholder_hid_bot;

using Discord;
using Models;
using global::Discord;
using global::Discord.WebSocket;
using Microsoft.Extensions.Options;

public class Worker : BackgroundService
{
  private readonly IConfiguration _configuration;
  private readonly BeholderHidBotOptions _botOptions;
  private readonly DiscordSocketClient _discordClient;
  private readonly CommandProcessor _commandProcessor;
  private readonly ILogger<Worker> _logger;
  private readonly IHostApplicationLifetime _hostApplicationLifetime;

  public Worker(
    IConfiguration configuration,
    IOptions<BeholderHidBotOptions> botOptions,
    DiscordSocketClient discordClient,
    CommandProcessor commandProcessor,
    ILogger<Worker> logger,
    IHostApplicationLifetime hostApplicationLifetime)
  {
    _configuration = configuration ?? throw new ArgumentNullException(nameof(botOptions));
    _botOptions = botOptions?.Value ?? throw new ArgumentNullException(nameof(botOptions));
    _discordClient = discordClient ?? throw new ArgumentNullException(nameof(discordClient));
    _commandProcessor = commandProcessor ?? throw new ArgumentNullException(nameof(commandProcessor));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _hostApplicationLifetime =
      hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    var discordToken = _botOptions.DiscordToken;

    if (string.IsNullOrWhiteSpace(discordToken))
    {
      _logger.LogInformation("A discord token was not found as part of configuration. Falling back to the DISCORD_TOKEN environment variable.");
      discordToken = _configuration["DISCORD_TOKEN"];
      foreach(var c in _configuration.GetChildren())
      {
        _logger.LogInformation($"{c.Key}: {c.Value}");
      }
    }

    if (string.IsNullOrWhiteSpace(discordToken))
    {
      _logger.LogCritical("A Discord Token could not be located in configuration. Exiting");
      _hostApplicationLifetime.StopApplication();
      return;
    }
    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

    _logger.LogInformation("Logging into Discord...");
    await _discordClient.LoginAsync(TokenType.Bot, discordToken);
    await _discordClient.StartAsync();

    await _commandProcessor.Initialize();

    // Block this task until the program is closed.
    await Task.Delay(-1, stoppingToken);
  }
}