namespace MessManagement.Interfaces;

/// <summary>
/// Singleton service interface for application-wide settings.
/// Demonstrates Singleton DI lifetime - one instance for entire application.
/// </summary>
public interface IAppSettingsService
{
    string ApplicationName { get; }
    string Version { get; }
    DateTime ApplicationStartTime { get; }
    Guid InstanceId { get; }
    int GetRequestCount();
    void IncrementRequestCount();
}

/// <summary>
/// Transient service interface for generating unique identifiers.
/// Demonstrates Transient DI lifetime - new instance every time.
/// </summary>
public interface IGuidGeneratorService
{
    Guid InstanceId { get; }
    Guid GenerateNewGuid();
    string GenerateShortId();
    string GenerateTransactionId(string prefix = "TXN");
}
