using Microsoft.EntityFrameworkCore;
using FluentValidation;
using AutoMapper;
using TaskPlatformBE.Data;
using TaskPlatformBE.Services;
using TaskPlatformBE.Models;

var builder = WebApplication.CreateBuilder(args);

try
{
    Console.WriteLine("Starting TaskPlatform application");

    // Configure strongly-typed settings
    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
    var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();

    // Add Entity Framework with configuration
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(
            appSettings?.Database.ConnectionString ?? builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.CommandTimeout(appSettings?.Database.CommandTimeout ?? 30)
        );
        if (appSettings?.Database.EnableSensitiveDataLogging == true)
        {
            options.EnableSensitiveDataLogging();
        }
    });

    // Add AutoMapper
    builder.Services.AddAutoMapper(typeof(Program));

    // Add FluentValidation
    builder.Services.AddValidatorsFromAssemblyContaining<NewTaskRequestValidator>();

    // Add services
    builder.Services.AddScoped<ITaskService, TaskService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IMappingService, MappingService>();

    // Add CORS with configuration
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngular", policy =>
        {
            var origins = appSettings?.Cors.AllowedOrigins ?? new[] { "http://localhost:4200" };
            var methods = appSettings?.Cors.AllowedMethods ?? new[] { "GET", "POST", "PUT", "DELETE" };
            var headers = appSettings?.Cors.AllowedHeaders ?? new[] { "*" };

            policy.WithOrigins(origins)
                  .WithMethods(methods)
                  .WithHeaders(headers);
        });
    });

    // Add controllers with JSON configuration
    builder.Services.AddControllersWithViews()
        .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
        });

    // Add Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "TaskPlatform API", Version = "v1" });
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(Program).Assembly.GetName().Name}.xml"));
    });

    // Add authorization
    builder.Services.AddAuthorization();

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskPlatform API v1"));
    }
    else
    {
        app.UseHsts();
    }

    app.UseCors("AllowAngular");
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthorization();

    // Map endpoints
    app.MapControllers();
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=MvcTasks}/{action=Index}/{id?}");

    // Ensure database is created and seeded
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        try
        {
            context.Database.EnsureCreated();
            SeedData.Seed(context);
            Console.WriteLine("Database initialized successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while initializing the database: {ex.Message}");
            throw;
        }
    }

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Application terminated unexpectedly: {ex.Message}");
}
