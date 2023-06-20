using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace InternetCheckerService
{
    /// <summary>
    /// Timed Internet Checker(System.Timers.Timer)
    /// </summary>
    public class TimedInternetChecker2 : IHostedService, IDisposable
    {
        #region Fields

        private int executionCount = 0;
        private StreamWriter checkerLogger = null!;
        private readonly ILogger<TimedInternetChecker> _logger;
        private readonly string logPath;
        private System.Timers.Timer? _timer = null;

        #endregion

        #region Constructor

        public TimedInternetChecker2(ILogger<TimedInternetChecker> logger, IConfiguration config)
        {
            _logger = logger;
            logPath = Path.Combine(
                config.GetValue<string>("LogPath") ??
                System.AppContext.BaseDirectory!,
                "log2.txt");
        }

        #endregion

        #region Methods

        public Task StartAsync(CancellationToken stoppingToken)
        {
            //Start Logging
            checkerLogger = new StreamWriter(logPath, true);  //Initialize logger
            Log("Timed Internet Checker Service start running");
            _logger.LogInformation("Timed Internet Checker Service running");

            //Initialize timer
            _timer = new System.Timers.Timer(30000);
            _timer.Elapsed += DoWork;
            _timer.AutoReset = true;
            _timer.Enabled = true;

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Internet Checker Service is stoping");

            _timer?.Stop();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        /// <summary>
        /// Work to do
        /// </summary>
        /// <param name="state"></param>
        private void DoWork(object? state, ElapsedEventArgs e)
        {
            string count = Interlocked.Increment(ref executionCount).ToString();
            string signalTime = e.SignalTime.ToString("yyyy-MM-dd hh:mm:ss.fff");
            _logger.LogInformation("Checker is working. Count: {count}.  Time: {Time}", count, signalTime);

            Log("Checker is working. Count: {Count}" + count.ToString() + "Time:" + signalTime);

            bool pingResult = IsConnectedToInternet();

            if (pingResult)
            {
                _logger.LogInformation($"Target host internet connected!" + DateTime.Now.ToString());
            }
            else
            {
                _logger.LogInformation("Internet connection failure!" + DateTime.Now.ToString());
            }

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
                Log(" Address:" + reply.Address.ToString() + " RoundTrip Time:" + reply.RoundtripTime + " Time to live:" + reply.Options.Ttl.ToString() + " Don't fragment:" + reply.Options.DontFragment + " Buffer size:" + reply.Buffer.Length);
                _logger.LogInformation(" Address:" + reply.Address.ToString() + " RoundTrip Time:" + reply.RoundtripTime + " Time to live:" + reply.Options.Ttl.ToString() + " Don't fragment:" + reply.Options.DontFragment + " Buffer size:" + reply.Buffer.Length);
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
