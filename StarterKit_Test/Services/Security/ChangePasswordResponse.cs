namespace StarterKit_Test.Services.Security
{
    public sealed record ChangePasswordResponse
    {
        public string Message { get; init; } = "Password changed successfully";
    }
}

