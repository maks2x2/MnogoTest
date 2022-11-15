using Microsoft.Extensions.Hosting;
using System.ComponentModel;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace MessageProcessor
{
    public class MokMessageProcessor : IHostedService
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private readonly BackgroundWorker _backgroundWorker;
        private readonly ManualResetEvent _stopEvent;
        private readonly Random _randomizer;

        private readonly IMessageStorage _messageStorage;
        private readonly ILogger _logger;

        public MokMessageProcessor(IMessageStorage messageStorage, ILogger<MokMessageProcessor> logger)
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
            _logger.LogDebug("MokMessageProcessor is started");
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
                        _logger.LogDebug("MokMessageProcessor is stopped");
                        return;
                    }

                    var msg = _messageStorage.PullMessage();
                    if (msg != null)
                    {
                        PrintMessage(msg);
                    }

                    _stopEvent.WaitOne(1 * 1000); // simulate slow work
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "MokMessageProcessor is failed");
            }
        }

        private void PrintMessage(Message msg)
        {
            _logger.LogInformation($"Process message: {msg.Body} with priority: {msg.Priority}");
        }

    }

}