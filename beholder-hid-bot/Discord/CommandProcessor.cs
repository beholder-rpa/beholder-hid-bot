namespace beholder_hid_bot.Discord
{
  using global::Discord.Commands;
  using global::Discord.WebSocket;
  using System.Reflection;
  using System.Text.RegularExpressions;
  using System.Threading.Tasks;

  public class CommandProcessor
  {
    private readonly IServiceProvider _services;
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;

    private readonly Regex RelayMessagePattern = new(@"^\[(?<TimeStamp>\d+:\d+:\d+:\d+)\]\s+\[(?<Channel>.*?)\]\s+\[(?<User>.*?)\]:\s+%(?<Command>.*?)\s+?(?<Args>.*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // Retrieve client and CommandService instance via ctor
    public CommandProcessor(IServiceProvider services, DiscordSocketClient client, CommandService commands)
    {
      _services = services ?? throw new ArgumentNullException(nameof(services));
      _commands = commands ?? throw new ArgumentNullException(nameof(commands));
      _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task Initialize()
    {
      // Hook the MessageReceived event into our command handler
      _client.MessageReceived += ProcessCommandAsync;

      // Here we discover all of the command modules in the entry 
      // assembly and load them. Starting from Discord.NET 2.0, a
      // service provider is required to be passed into the
      // module registration method to inject the 
      // required dependencies.
      //
      // If you do not use Dependency Injection, pass null.
      // See Dependency Injection guide for more information.
      await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                      services: _services);
    }

    private async Task ProcessCommandAsync(SocketMessage messageParam)
    {
      // Don't process the command if it was a system message
      if (messageParam is not SocketUserMessage message) return;

      // Create a number to track where the prefix ends and the command begins
      int argPos = 0;

      SocketCommandContext? context = null;
      if (RelayMessagePattern.IsMatch(message.Content))
      {
        var match = RelayMessagePattern.Match(message.Content);
        var ctx = new RelaySocketCommandContext(_client, message)
        {
          TimeStamp = match.Groups["TimeStamp"].Value,
          RelayChannel = match.Groups["Channel"].Value,
          RelayUser = match.Groups["User"].Value
        };
        argPos = message.Content.IndexOf('%') + 1;
        context = ctx;
      }
      else if (message.HasCharPrefix('%', ref argPos))
      {
        // Create a WebSocket-based command context based on the message
        context = new SocketCommandContext(_client, message);
      }

      if (context == null)
      {
        return;
      }

      // Execute the command with the command context we just
      // created, along with the service provider for precondition checks.
      await _commands.ExecuteAsync(
          context: context,
          argPos: argPos,
          services: _services);
    }
  }
}
