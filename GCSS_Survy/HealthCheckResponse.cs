namespace GCSS_Survy
{
    public sealed record HealthCheckResponse
    {
        public string Status { get; init; } = "healthy";
        public string Database { get; init; } = "connected";
        public int UserCount { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}

