using MessManagement.Interfaces;

namespace MessManagement.Services;

/// <summary>
/// Singleton service implementation - ONE instance shared across the entire application.
/// Use for: Configuration, Caching, Application state, Counters
/// 
/// DI Lifetime: SINGLETON
/// - Created once when first requested
/// - Same instance returned for all subsequent requests
/// - Lives for the entire application lifetime
/// </summary>
public class AppSettingsService : IAppSettingsService
{
    private int _requestCount = 0;
    private readonly object _lock = new();

    public string ApplicationName { get; } = "DineSync - Mess Management System";
    public string Version { get; } = "1.0.0";
    public DateTime ApplicationStartTime { get; }
    public Guid InstanceId { get; }

    public AppSettingsService()
    {
        ApplicationStartTime = DateTime.UtcNow;
        InstanceId = Guid.NewGuid();
        
        // This constructor runs ONCE for the entire application
        Console.WriteLine($"[SINGLETON] AppSettingsService created with InstanceId: {InstanceId}");
    }

    public int GetRequestCount()
    {
        lock (_lock)
        {
            return _requestCount;
        }
    }

    public void IncrementRequestCount()
    {
        lock (_lock)
        {
            _requestCount++;
        }
    }
}

/// <summary>
/// Transient service implementation - NEW instance created every time it's requested.
/// Use for: Lightweight, stateless operations, Utilities
/// 
/// DI Lifetime: TRANSIENT
/// - New instance created every time it's injected
/// - Different InstanceId each time
/// - Good for stateless operations
/// </summary>
public class GuidGeneratorService : IGuidGeneratorService
{
    public Guid InstanceId { get; }

    public GuidGeneratorService()
    {
        InstanceId = Guid.NewGuid();
        
        // This constructor runs EVERY TIME the service is injected
        Console.WriteLine($"[TRANSIENT] GuidGeneratorService created with InstanceId: {InstanceId}");
    }

    public Guid GenerateNewGuid()
    {
        return Guid.NewGuid();
    }

    public string GenerateShortId()
    {
        return Guid.NewGuid().ToString("N")[..8].ToUpper();
    }

    public string GenerateTransactionId(string prefix = "TXN")
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = GenerateShortId()[..4];
        return $"{prefix}-{timestamp}-{random}";
    }
}
