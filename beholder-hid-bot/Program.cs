using beholder_hid_bot;
using beholder_hid_bot.Discord;
using beholder_hid_bot.HardwareInterfaceDevices;
using beholder_hid_bot.Models;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Concurrent;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
      var config = context.Configuration;

      var hidBotOptions = config.GetSection("beholder_hid_bot").Get<BeholderHidBotOptions>();
      services.Configure<BeholderHidBotOptions>(config.GetSection("beholder_hid_bot"));

      services.AddLogging(options =>
      {
        options.AddSimpleConsole(c =>
        {
          c.TimestampFormat = "[yyyy-MM-dd HH:mm:ss:fff] ";
        });
      });

      services.AddSingleton(sp =>
       {
         var client = new DiscordSocketClient();
         var logger = sp.GetRequiredService<ILogger<DiscordSocketClient>>();
         client.Log += (msg) =>
         {
           var logLevel = LogLevel.Debug;
           switch (msg.Severity)
           {
             case Discord.LogSeverity.Warning:
               logLevel = LogLevel.Warning;
               break;
             case Discord.LogSeverity.Error:
               logLevel = LogLevel.Error;
               break;
             case Discord.LogSeverity.Info:
               logLevel = LogLevel.Information;
               break;
             case Discord.LogSeverity.Debug:
               logLevel = LogLevel.Debug;
               break;
             case Discord.LogSeverity.Verbose:
               logLevel = LogLevel.Trace;
               break;
             case Discord.LogSeverity.Critical:
               logLevel = LogLevel.Critical;
               break;
           }
#pragma warning disable CA2254 // Template should be a static expression
           logger.Log(logLevel, msg.Exception, msg.Message);
#pragma warning restore CA2254 // Template should be a static expression
           return Task.CompletedTask;
         };

         return client;
       });

      services.AddSingleton<Keyboard>();

      services.AddSingleton<KeyboardSessionWorker>();

      services.AddSingleton<Mouse>();
      services.AddSingleton<MouseObserver>();
      services.AddSingleton<Joystick>();

      services.AddSingleton<CommandService>();
      services.AddSingleton<CommandProcessor>();

      services.AddHostedService<Worker>();
    })
    .Build();


await host.RunAsync();
