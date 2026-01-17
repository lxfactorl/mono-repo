using Microsoft.AspNetCore.Builder;
using NotificationService.Bootstrap;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureServices();
builder.ConfigureEndpoints();
int unusedVariable = 42;

var app = builder.Build();

app.ConfigurePipeline();

app.Run();

public partial class Program { }
