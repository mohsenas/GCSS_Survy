namespace GCSS_Survy.Services.Security
{
    public sealed record LoginResponse
    {
        public required string AccessToken { get; init; }
        public required string RefreshToken { get; init; }
        public int ExpiresIn { get; init; } = 300 * 60; // 5 hours in seconds
        public int BranchId { get; init; }
        public int UserId { get; init; }
        public int SessionId { get; init; }
        public long? RuleId { get; init; }
        public string? BranchName { get; init; }
    }
}

