using Google.Protobuf.WellKnownTypes;
using Scalar.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder
    .AddProject<Projects.Chronique_Des_Mondes_ApiService>("apiservice");

// Add Scalar API Reference for all services
var scalar = builder.AddScalarApiReference(options =>
{
    // Configure global options. They will apply to all services
    options.WithTheme(ScalarTheme.Purple);
});

// Configure API References for specific services
scalar
    .WithApiReference(apiService);


builder.AddProject<Projects.Chronique_Des_Mondes_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
