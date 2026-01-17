using Microsoft.AspNetCore.Builder;
using NotificationService.Bootstrap;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureServices();

var app = builder.Build();

app.ConfigurePipeline();

app.Run();

// TEST: This should cause build to fail and block merge
var undeclaredVariable = nonExistentMethod();

// INTENTIONAL SYNTAX ERROR TO TEST BUILD GATE
ThisWillNotCompile();

public partial class Program { }
