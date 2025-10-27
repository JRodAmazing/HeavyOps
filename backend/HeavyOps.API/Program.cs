using HeavyOps.Data.Models;
using HeavyOps.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add shared data storage
builder.Services.AddSingleton<List<Project>>();
builder.Services.AddSingleton<List<EquipmentAssignment>>();
builder.Services.AddSingleton<List<CostEntry>>();
builder.Services.AddSingleton<List<ServiceLog>>();
builder.Services.AddSingleton<ReportGeneratorService>();

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
