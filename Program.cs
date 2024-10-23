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
app.UseAuthorization();
app.MapControllers();


app.Run();


