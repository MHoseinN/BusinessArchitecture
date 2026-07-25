namespace BusinessTemplate.Modules.Identity.Domain;

public sealed partial class User
{
    public string PasswordHash { get; private set; } = string.Empty;

    public void SetPasswordHash(string hash)
    {
        PasswordHash = hash;
    }
}
