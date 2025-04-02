using Microsoft.EntityFrameworkCore;
using WaterProject.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Log the connection string
var connectionString = builder.Configuration.GetConnectionString("WaterConnection");
Console.WriteLine($"Connection string: {connectionString}");

builder.Services.AddDbContext<WaterDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Ensure database is created and log the result
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<WaterDbContext>();
        var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "WaterProject.sqlite");
        Console.WriteLine($"Database path: {dbPath}");
        Console.WriteLine($"Database exists: {File.Exists(dbPath)}");
        
        // Ensure the database directory exists
        var dbDirectory = Path.GetDirectoryName(dbPath);
        if (!Directory.Exists(dbDirectory))
        {
            Directory.CreateDirectory(dbDirectory);
        }
        
        context.Database.EnsureCreated();
        
        // Log the number of projects in the database
        var projectCount = context.Projects.Count();
        Console.WriteLine($"Number of projects in database: {projectCount}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database error: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
