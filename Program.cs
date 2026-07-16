using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<NeoSyncService>();
builder.Services.AddDbContext<NeoContext>(options => options.UseInMemoryDatabase("NeoWatcher"));
builder.Services.AddHostedService<NeoSyncJob>();
var app = builder.Build();

app.Run();
