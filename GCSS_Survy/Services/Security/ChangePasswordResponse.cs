namespace GCSS_Survy.Services.Security
{
    public sealed record ChangePasswordResponse
    {
        public string Message { get; init; } = "Password changed successfully";
    }
}

