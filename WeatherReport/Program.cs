using Microsoft.EntityFrameworkCore;
using WeatherReport.Data;
using WeatherReport.Models;
using WeatherReport.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient(); // Register HttpClient without specifying a type
builder.Services.AddHttpClient<OpenWeatherApiService>();
builder.Services.AddSingleton<WeatherPredictionService>();

var conncectionString = builder.Configuration.GetConnectionString("Postgre");
builder.Services.AddDbContext<DataContext>(options =>
options.UseNpgsql(conncectionString));

builder.Services.Configure<WeatherApiSettings>(builder.Configuration.GetSection("WeatherApi"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<User>()
    .AddEntityFrameworkStores<DataContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.MapIdentityApi<User>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();