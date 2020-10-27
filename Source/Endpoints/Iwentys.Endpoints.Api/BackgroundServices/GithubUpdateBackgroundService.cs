﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Iwentys.Core;
using Iwentys.Core.Services;
using Iwentys.Database.Repositories;
using Iwentys.Models.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Iwentys.Endpoints.Api.BackgroundServices
{
    public class GithubUpdateBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger _logger;

        public GithubUpdateBackgroundService(ILoggerFactory loggerFactory, IServiceProvider sp)
        {
            _sp = sp;
            _logger = loggerFactory.CreateLogger("GithubUpdateBackgroundService");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("GithubUpdateBackgroundService start");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using IServiceScope scope = _sp.CreateScope();
                    _logger.LogInformation("Execute GithubUpdateBackgroundService update");

                    var studentRepository = scope.ServiceProvider.GetRequiredService<StudentRepository>();
                    var githubUserDataService = scope.ServiceProvider.GetRequiredService<GithubUserDataService>();
                    foreach (StudentEntity student in studentRepository.Read().Where(s => s.GithubUsername != null))
                    {
                        githubUserDataService.CreateOrUpdate(student.Id);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Fail to perform GithubUpdateBackgroundService update");
                }

                await Task.Delay(ApplicationOptions.DaemonUpdateInterval, stoppingToken).ConfigureAwait(false);
            }
        }
    }
}