namespace GCSS_Survy.Services.Security
{
    public sealed record LogoutResponse
    {
        public string Message { get; init; } = "Logged out successfully";
    }
}

