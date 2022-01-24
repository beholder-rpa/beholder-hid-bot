namespace beholder_hid_bot.Discord
{
  using global::Discord.Commands;
  using global::Discord.WebSocket;

  public class RelaySocketCommandContext : SocketCommandContext
  {
    public RelaySocketCommandContext(DiscordSocketClient client, SocketUserMessage msg) : base(client, msg)
    {
    }

    public string? TimeStamp { get; set; }

    public string? RelayChannel { get; set; }

    public string? RelayUser { get; set; }
  }
}
