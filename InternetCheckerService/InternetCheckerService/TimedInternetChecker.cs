using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace InternetCheckerService
{
    /// <summary>
    /// Timed Internet Checker(System.Threadinig.Timer)
    /// </summary>
    public class TimedInternetChecker : IHostedService, IDisposable
    {
        #region FIelds

        private int executionCount = 0;
        private readonly ILogger<TimedInternetChecker> _logger;
        private Timer? _timer = null;
        private readonly string logPath;
        private StreamWriter checkerLogger = null!;

        #endregion

        #region Constructor

        public TimedInternetChecker(ILogger<TimedInternetChecker> logger, IConfiguration config)
        {
            _logger = logger;
            logPath = Path.Combine(
                config.GetValue<string>("LogPath") ??
                System.AppContext.BaseDirectory!,
                "log.txt");
        }

        #endregion

        #region Methods

        public Task StartAsync(CancellationToken stoppigToken)
        {
            //Start logging
            checkerLogger = new StreamWriter(logPath, true);  //Initialize logger
            Log("Timed Internet Checker Service start running");
            _logger.LogInformation("Timed Internet Checker Service running");

            //Initialize Timer
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Work to do
        /// </summary>
        /// <param name="state"></param>
        private void DoWork(object? state)
        {
            var count = Interlocked.Increment(ref executionCount);

            _logger.LogInformation("Timed Internet Checker Service is working. Count: {Count}", count);
            Log("Timed Internet Checker Service is working.");

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

        /// <summary>
        /// Check internet connection
        /// </summary>
        /// <returns></returns>
        public bool IsConnectedToInternet()
        {
            string host = "www.google.com.tw";

            Ping p = new Ping();

            PingReply reply = p.Send(host, 3000);

            if (reply.Status == IPStatus.Success)
            {
                Log("Target host is connected..." + DateTime.Now.ToString());
                return true;
            }
            else
            {
                Log("Target host disconnected..." + DateTime.Now.ToString());
                return false;
            }                
        }

        /// <summary>
        /// Logging and settings
        /// </summary>
        /// <param name="message"></param>
        public void Log(string message)
        {
            if (checkerLogger == null) return;

            checkerLogger.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}");
            checkerLogger.Flush();
        }

        #endregion
    }
}
