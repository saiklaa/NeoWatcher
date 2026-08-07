using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient<NeoSyncService>();
builder.Services.AddScoped<NeoStatsCalculator>();
if (builder.Environment.IsDevelopment())
{
	builder.Services.AddDbContext<NeoContext>(options => options.UseInMemoryDatabase("NeoWatcherInMemory"));
}
else
{
	builder.Services.AddDbContext<NeoContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("NeoDb")));
}
builder.Services.AddHostedService<NeoSyncJob>();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
	errorApp.Run(async context =>
	{
		var feature = context.Features.Get<IExceptionHandlerPathFeature>();
		var detail = app.Environment.IsDevelopment()
			? feature?.Error.Message
			: "An unexpected error occurred.";

		await Results.Problem(
			title: "Unexpected error",
			detail: detail,
			statusCode: StatusCodes.Status500InternalServerError)
			.ExecuteAsync(context);
	});
});

app.MapControllers();
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapOpenApi();

await using (var scope = app.Services.CreateAsyncScope())
{
	var db = scope.ServiceProvider.GetRequiredService<NeoContext>();
	try
	{
		await db.Database.MigrateAsync();
	}
	catch (Exception ex)
	{
		var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
		logger.LogWarning(ex, "Database migration failed — continuing without migration.");
		if (!app.Environment.IsDevelopment())
		{
			// in production we should not silently continue when migrations fail
			logger.LogError("Migrations failed in non-development environment, aborting startup.");
			throw;
		}
	}
}

app.Run();

public partial class Program { }
