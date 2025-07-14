namespace TaskPlatformBE.Models;

/// <summary>
/// Application configuration settings
/// </summary>
public class AppSettings
{
    public DatabaseSettings Database { get; set; } = new();
    public CorsSettings Cors { get; set; } = new();
    public LoggingSettings Logging { get; set; } = new();
    public WorkflowSettings Workflow { get; set; } = new();
}

/// <summary>
/// Database configuration settings
/// </summary>
public class DatabaseSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public int CommandTimeout { get; set; } = 30;
    public bool EnableSensitiveDataLogging { get; set; } = false;
}

/// <summary>
/// CORS configuration settings
/// </summary>
public class CorsSettings
{
    public string[] AllowedOrigins { get; set; } = { "http://localhost:4200" };
    public string[] AllowedMethods { get; set; } = { "GET", "POST", "PUT", "DELETE" };
    public string[] AllowedHeaders { get; set; } = { "*" };
}

/// <summary>
/// Logging configuration settings
/// </summary>
public class LoggingSettings
{
    public string LogLevel { get; set; } = "Information";
    public string LogFilePath { get; set; } = "logs/taskplatform-{Date}.log";
    public bool EnableConsoleLogging { get; set; } = true;
    public bool EnableFileLogging { get; set; } = true;
}

/// <summary>
/// Workflow configuration settings
/// </summary>
public class WorkflowSettings
{
    public int MaxRequirementLength { get; set; } = 500;
    public int MaxTaskTypes { get; set; } = 10;
    public int MaxStatusesPerType { get; set; } = 10;
    public bool EnableStrictValidation { get; set; } = true;
} 