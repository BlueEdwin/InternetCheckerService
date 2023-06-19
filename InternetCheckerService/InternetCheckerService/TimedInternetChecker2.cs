using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetCheckerService
{
    public class TimedInternetChecker2
    {
        #region Fields

        private StreamWriter checkerLogger = null!;
        private readonly ILogger<TimedInternetChecker> _logger;
        private readonly string logPath;

        #endregion

        #region Constructor

        public TimedInternetChecker2(ILogger<TimedInternetChecker> logger, IConfiguration config)
        {
            _logger = logger;
            logPath = Path.Combine(
                config.GetValue<string>("LogPath") ??
                System.AppContext.BaseDirectory!,
                "log.txt");
        }

        #endregion


        #region

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
