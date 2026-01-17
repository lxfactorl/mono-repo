using Microsoft.AspNetCore.Builder;
using NotificationService.Bootstrap;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureServices();

var app = builder.Build();

app.ConfigurePipeline();

app.Run();

public partial class Program { }
