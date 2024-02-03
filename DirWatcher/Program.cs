using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DirWatcher.Services;
using DirWatcher.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<DirWatcherService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var dirWatcherService = app.Services.GetRequiredService<DirWatcherService>();
// Pass the necessary configuration parameters to the StartBackgroundTask method
var watchRequest = new WatchRequest
{
    DirectoryPath = "your_directory_path",
    Interval = TimeSpan.FromSeconds(10), // Example interval
    MagicString = "your_magic_string"
};
dirWatcherService.StartBackgroundTask(watchRequest);

app.Run();
