using Domain.Interfaces;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace MessageGenerator
{
    public class MokMessageGenerator : IHostedService
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private readonly BackgroundWorker _backgroundWorker;
        private readonly ManualResetEvent _stopEvent;
        private readonly Random _randomizer;

        private readonly IMessageStorage _messageStorage;
        private readonly ILogger _logger;

        public MokMessageGenerator(IMessageStorage messageStorage, ILogger<MokMessageGenerator> logger)
        {
            _messageStorage = messageStorage;
            _logger = logger;

            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.WorkerSupportsCancellation = true;
            _stopEvent = new ManualResetEvent(false);
            _randomizer = new Random();

            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_backgroundWorker.IsBusy)
                _backgroundWorker.RunWorkerAsync();
            _logger.LogDebug("MokMessageGenerator is started");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _backgroundWorker.CancelAsync(); // set CancellationPending
            _stopEvent.Set(); // abort sleep
        }

        private void _backgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            try
            {
                while (true)
                {
                    // check a stop of service
                    if (_backgroundWorker.CancellationPending)
                    {
                        _logger.LogDebug("MokMessageGenerator is stopped");
                        return;
                    }

                    var countMsg = _randomizer.Next(10, 50);
                    _logger.LogInformation($"Generated {countMsg} messages");
                    for (int i = 0; i < countMsg; i++)
                    {
                        var msg = GenerateMessage();
                        _messageStorage.PushMessage(msg);
                    }

                    var sleepSpan = _randomizer.Next(10, 2 * 60); // in seconds
                    _stopEvent.WaitOne(sleepSpan * 1000);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "MokMessageGenerator is failed");
            }
        }

        private Message GenerateMessage()
        {
            return new Message(
                GetRandomString(),
                _randomizer.Next(0, 100)
            );
        }

        public string GetRandomString()
        {
            var length = _randomizer.Next(5, 20);
            var randomCharts = Enumerable.Repeat(chars, length)
                                         .Select(s => s[_randomizer.Next(s.Length)]).ToArray();
            return new string(randomCharts);
        }

    }

}