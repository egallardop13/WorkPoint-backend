using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", (corsbuilder) =>
        {
            corsbuilder.WithOrigins("http://localhost:3000", "http://localhost:4200",   "http://localhost:8000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
        });
          options.AddPolicy("ProdCors", (corsbuilder) =>
        {
            corsbuilder.WithOrigins("http://localhost:3000", "http://localhost:4200",   "http://localhost:8000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
        });
});

string? tokenKeyString = builder.Configuration.GetSection("AppSettings:TokenKey").Value;

            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(tokenKeyString != null ? tokenKeyString : ""
                    )
                );

TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
{
    IssuerSigningKey = tokenKey,
    ValidateIssuerSigningKey = true,   //false
    ValidateIssuer = false,
    ValidateAudience = false
};

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = tokenValidationParameters;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors");
    app.UseSwagger();
    app.UseSwaggerUI();
}else{
    app.UseCors("ProdCors");
app.UseHttpsRedirection();

}



// app.MapGet("/weatherforecast", () =>
// {
    
// })
// .WithName("GetWeatherForecast")
// .WithOpenApi();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


app.Run();


