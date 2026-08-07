using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient<NeoSyncService>();
builder.Services.AddDbContext<NeoContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("NeoDb")));
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
app.MapOpenApi();

await using (var scope = app.Services.CreateAsyncScope())
{
	var db = scope.ServiceProvider.GetRequiredService<NeoContext>();
	await db.Database.MigrateAsync();
}

app.Run();
