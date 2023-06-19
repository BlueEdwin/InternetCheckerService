using InternetCheckerService;

IHost host = Host.CreateDefaultBuilder(args)
    //Sets service name
    .UseWindowsService(options =>
    {
        options.ServiceName = ".NET Internet Checker Service";
    })
    .ConfigureServices(services =>
    {
        //services.AddHostedService<Worker>();
        //services.AddHostedService<TimedInternetChecker>(); //Add the Timed based service(System.Threading.Timer)
        services.AddHostedService<TimedInternetChecker2>(); //Add the Timed based service(System.Timers.Timer)
    })
    .Build();

await host.RunAsync();
