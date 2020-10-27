﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Iwentys.Endpoints.Api.Tools;
using Iwentys.Endpoints.ClientBot;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Tef.BotFramework.Core;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Iwentys.Endpoints.Api.BackgroundServices
{
    public class BotBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;

        public BotBackgroundService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("BotBackgroundService");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("IwentysBackgroundService start");

            try
            {
                Bot bot = BotProvider.Init(new TelegramDebugSettings(), Log.Logger);
                bot.Start();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Bot run failed");
            }

            return Task.CompletedTask;
        }
    }
}