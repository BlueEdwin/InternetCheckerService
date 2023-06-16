using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace InternetCheckerService
{
    public class TimedInternetChecker : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<TimedInternetChecker> _logger;
        private Timer? _timer = null;

        public TimedInternetChecker(ILogger<TimedInternetChecker> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppigToken)
        {
            _logger.LogInformation("Timed Internet Checker Service running");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            var count = Interlocked.Increment(ref executionCount);

            _logger.LogInformation("Timed Internet Checker Service is working. Count: {Count}", count);

            bool pingResult = IsConnectedToInternet();

            if (pingResult)
            {
                _logger.LogInformation($"Target host internet connected!");
            }
            else
            {
                _logger.LogInformation("Internet connection failure!");
            }

        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Internet Checker Service is stoping");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public bool IsConnectedToInternet()
        {
            string host = "www.google.com.tw";

            Ping p = new Ping();

            PingReply reply = p.Send(host, 3000);

            if (reply.Status == IPStatus.Success)
                return true;
            else
                return false;
        }
    }
}
