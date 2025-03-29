using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Configure authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
    {
        builder.Configuration.Bind("AzureAd", options);
        options.TokenValidationParameters.NameClaimType = "name";
		options.TokenValidationParameters.ValidateIssuer = true;
		options.TokenValidationParameters.ValidIssuer = "https://login.microsoftonline.com/bfbbe59c-bfe0-4cf0-937c-4d100883a491/v2.0";
		options.TokenValidationParameters.ValidateAudience = true;
		options.TokenValidationParameters.ValidAudience = "da690e01-d4e2-4d14-b633-ded29a7e7a7d";
	}, options => { builder.Configuration.Bind("AzureAd", options); });

builder.Services.AddCors(options => {
	options.AddPolicy("AllowAll", builder => {
		builder.AllowAnyOrigin()
			   .AllowAnyMethod()
			   .AllowAnyHeader();
	});
});

// Configure authorization
builder.Services.AddAuthorization(config =>
{
config.AddPolicy("AuthZPolicy", policy =>
    policy.RequireRole("Forecast.Read"));
});

if (Debugger.IsAttached) IdentityModelEventSource.ShowPII = true;

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("weatherForecast")
.RequireAuthorization("AuthZPolicy"); // Protect this endpoint with the AuthZPolicy

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}