using System.Text;
using System.Threading.RateLimiting;
using DotnetAPI.Data;
using DotnetAPI.Middleware;
using DotnetAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDataContextDapper, DataContextDapper>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ISalaryService, SalaryService>();

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddApplicationInsightsTelemetry();
    builder.Services.AddHealthChecks()
        .AddSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection") ?? "",
            name: "sqlserver",
            tags: new[] { "db", "sql" });
}
else
{
    builder.Services.AddHealthChecks();
}

var allowedOrigins =
    builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:3000" };

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "DevCors",
        (corsbuilder) =>
        {
            corsbuilder
                .WithOrigins(
                    "http://localhost:3000",
                    "http://localhost:4200",
                    "http://localhost:8000"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
    );
    options.AddPolicy(
        "ProdCors",
        (corsbuilder) =>
        {
            corsbuilder
                .WithOrigins(allowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
    );
});

string? tokenKeyString = builder.Configuration.GetSection("AppSettings:TokenKey").Value;
string issuer =
    builder.Configuration.GetValue<string>("AppSettings:Issuer") ?? "WorkPointAPI";
string audience =
    builder.Configuration.GetValue<string>("AppSettings:Audience") ?? "WorkPointClient";

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(tokenKeyString != null ? tokenKeyString : "")
            ),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
        };
    });

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(
        "auth",
        limiterOptions =>
        {
            limiterOptions.PermitLimit = 5;
            limiterOptions.Window = TimeSpan.FromMinutes(1);
            limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            limiterOptions.QueueLimit = 0;
        }
    );
    options.RejectionStatusCode = 429;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseCors("ProdCors");
    app.UseHttpsRedirection();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            }),
            totalDuration = report.TotalDuration
        };
        await context.Response.WriteAsJsonAsync(result);
    }
}).AllowAnonymous();

app.MapGet("/ping", () => Results.Ok("pong")).AllowAnonymous();

app.Run();

public partial class Program { }
