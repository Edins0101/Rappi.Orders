using Rappi.Orders.Application;
using Rappi.Orders.Application.Interfaces;
using Rappi.Orders.Infrastructure;
using Rappi.Orders.Worker.Common;
using Rappi.Orders.Worker.Jobs;

var builder = Host.CreateApplicationBuilder(args);

// Bind Worker options por entorno
builder.Services.Configure<WorkerOptions>(
    builder.Configuration.GetSection("Worker")
);

// Layers
builder.Services.AddInfrastructure(builder.Configuration);

// Worker services
builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
builder.Services.AddSingleton<ICurrentUserService, WorkerCurrentUserService>();

builder.Services.AddHostedService<OrderCleanupWorker>();

var host = builder.Build();
host.Run();