using Microsoft.EntityFrameworkCore;
using VT.Context;
using VT.Interface;
using VT.Repository;
using VT.Services;
using Azure.Storage.Blobs;
using VT.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();    

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    
// Dependency Injection
builder.Services.AddScoped<ITrainingVideoRepository, TrainingVideoRepository>();
builder.Services.AddScoped<ITrainingVideoService, TrainingVideoService>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();