using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication("CookieAuthentication").AddCookie("CookieAuthentication");
builder.Services.AddAuthorization();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication(); // Enable authentication middleware
app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("api/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapPost("api/create-user", async (HttpContext context, [FromBody] User user) =>
{
    if (user.Email == "josephibochi@gmail.com" && user.Password == "joseph@1111")
    {
        // Claims
        List<Claim> claims = [
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name)
        ];

        // Identity
        var claimsIdentity = new ClaimsIdentity(claims, "CookieAuthentication");

        // Principal
        ClaimsPrincipal claimsPrincipal = new(claimsIdentity);
        await context.SignInAsync("CookieAuthentication", claimsPrincipal);

        return Results.Created();
    }
    return Results.NotFound();
})
.WithName("CreateUser")
.Produces(StatusCodes.Status201Created)
.Accepts<User>("application/json")
;

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

class User
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}