using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<NeoSyncService>();
builder.Services.AddDbContext<NeoContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("NeoDb")));
builder.Services.AddHostedService<NeoSyncJob>();
var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
	var db = scope.ServiceProvider.GetRequiredService<NeoContext>();
	await db.Database.MigrateAsync();
}

app.Run();
