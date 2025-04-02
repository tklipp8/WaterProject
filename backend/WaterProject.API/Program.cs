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
        
        // Log current directory and its contents
        var currentDir = Directory.GetCurrentDirectory();
        Console.WriteLine($"Current directory: {currentDir}");
        Console.WriteLine("Directory contents:");
        foreach (var file in Directory.GetFiles(currentDir))
        {
            Console.WriteLine($"  - {file}");
        }
        
        // Check if the database file exists in the current directory
        var dbPath = Path.Combine(currentDir, "WaterProject.sqlite");
        Console.WriteLine($"Database path: {dbPath}");
        Console.WriteLine($"Database exists: {File.Exists(dbPath)}");
        
        // If the database doesn't exist, try to copy it from the project directory
        if (!File.Exists(dbPath))
        {
            Console.WriteLine("Database file not found in current directory. Attempting to copy from project directory...");
            
            // Try to find the database file in the project directory
            var projectDbPath = Path.Combine(currentDir, "..", "WaterProject.sqlite");
            if (File.Exists(projectDbPath))
            {
                Console.WriteLine($"Found database at {projectDbPath}. Copying to {dbPath}...");
                File.Copy(projectDbPath, dbPath);
                Console.WriteLine("Database copied successfully.");
            }
            else
            {
                Console.WriteLine($"Database not found at {projectDbPath} either.");
            }
        }
        
        // Ensure the database directory exists
        var dbDirectory = Path.GetDirectoryName(dbPath);
        if (!Directory.Exists(dbDirectory))
        {
            Console.WriteLine($"Creating directory: {dbDirectory}");
            Directory.CreateDirectory(dbDirectory);
        }
        
        // Ensure the database is created
        Console.WriteLine("Ensuring database is created...");
        context.Database.EnsureCreated();
        
        // Log the number of projects in the database
        var projectCount = context.Projects.Count();
        Console.WriteLine($"Number of projects in database: {projectCount}");
        
        // If there are no projects, try to seed the database
        if (projectCount == 0)
        {
            Console.WriteLine("No projects found in database. Attempting to seed...");
            // Add your seeding logic here if needed
        }
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
