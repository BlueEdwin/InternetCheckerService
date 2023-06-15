using InternetCheckerService;

IHost host = Host.CreateDefaultBuilder(args)
    //Sets service name
    .UseWindowsService(options =>
    {
        options.ServiceName = ".NET Internet Checker Service";
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
