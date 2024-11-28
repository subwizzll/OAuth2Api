using OAuth2Api;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureEnvironment();
builder.ConfigureServices();

var app = builder.Build();

app.ConfigureMiddleware();

await app.RunAsync();
