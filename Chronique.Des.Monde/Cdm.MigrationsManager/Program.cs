using Cdm.Migrations;
using Cdm.MigrationsManager;
using Cmd.ServiceDefaults;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddSqlServerDbContext<MigrationContext>("DefaultConnection");

builder.Services.AddHostedService<Worker>();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

var host = builder.Build();
host.Run();
