namespace InternetCheckerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly string logPath;
        private StreamWriter checkerLogger = null!;

        public Worker(ILogger<Worker> logger, IConfiguration config)
        {
            _logger = logger;
            logPath = Path.Combine(
                config.GetValue<string>("LogPath") ??
                System.AppContext.BaseDirectory!,
                "log.txt");
        }

        public void Log(string message)
        {
            if (checkerLogger == null) return;

            checkerLogger.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}");
            checkerLogger.Flush();
        }

        public override async Task StartAsync(CancellationToken stoppingToken)
        {
            checkerLogger = new StreamWriter(logPath, true);

            _logger.LogInformation("Service started");
            Log("Service started");

            // �����O BackgroundService �b StartAsync() �I�s ExecuteAsync�B
            // �b StopAsync() �ɩI�s stoppingToken.Cancel() �u������
            await base.StartAsync(stoppingToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                Log($"Worker running at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

                await Task.Delay(10000, stoppingToken);
            }
        }

        // �A�Ȱ����
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service stopped");
            Log("Service stopped");
            checkerLogger.Dispose();
            checkerLogger = null!;
            await base.StopAsync(stoppingToken);
        }
    }
}