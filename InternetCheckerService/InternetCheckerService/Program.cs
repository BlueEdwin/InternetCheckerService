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
        services.AddHostedService<TimedInternetChecker>(); //Add the Timed based service
    })
    .Build();

await host.RunAsync();
