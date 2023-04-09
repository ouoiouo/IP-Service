using MyService.Frp;
using MyService.Frp.Services;
using Serilog;

IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog((hostingContext, loggerConfiguration) =>
    {
        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration).WriteTo.File(path: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "log-.txt"), rollingInterval: RollingInterval.Day);
    })
    .ConfigureServices(services =>
    {
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddSerilog();
        });
        services.AddWindowsService(options =>
        {
            options.ServiceName = "AA My Frp Service";
        });
        services.AddHostedService<Worker>();
        services.AddScoped<WorkerService>();
        services.AddScoped<DDNSService>();
    })
    .Build();

await host.RunAsync();
