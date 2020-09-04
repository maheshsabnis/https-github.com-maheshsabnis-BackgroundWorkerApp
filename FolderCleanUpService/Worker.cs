using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace FolderCleanUpService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IServiceScopeFactory _serviceScopeFactory;

        private IList<string> _folderPaths;
        private int _numberOfDaysBeforeDelete;
        private int _runIntervallInHours;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var configuration = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IConfiguration>();
            _numberOfDaysBeforeDelete = int.Parse(configuration["App.Configurations:NumberOfDaysBeforeDelete"]);
            _runIntervallInHours = int.Parse(configuration["App.Configurations:RunIntervallInHours"]);
            _folderPaths = File.ReadAllLines(configuration["App.Configurations:ConfigurationFilePath"]).Select(x => x.Trim()).ToList();

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                foreach (var path in _folderPaths)
                {
                    if (!stoppingToken.IsCancellationRequested)
                    {
                        // Get old files
                        var files = Directory.GetFiles(path).Select(file => new FileInfo(file)).Where(file => file.LastWriteTime < DateTime.Now.AddDays(-1* _numberOfDaysBeforeDelete)).ToList();

                        // Delete found files
                        files.ForEach(file => file.Delete());
                    }
                }
                
                await Task.Delay(TimeSpan.FromHours(_runIntervallInHours), stoppingToken);
            }
        }
    }
}
