using Newtonsoft.Json;

namespace VisionCareCore.User.Domain.Model.Aggregates;

public enum VisualImpairmentLevel
{
    Leve,
    Moderada,
    Grave,
    Ceguera
}

public class AuthUser
{
    public Guid Id { get; }

    public string Email { get; private set; }

    [JsonIgnore]
    public string PasswordHash { get; private set; }

    public string Name { get; private set; }

    public string LastName { get; private set; }

    public DateTime Birthday { get; private set; }

    public DateTime RegisteredAt { get; private set; }

    public VisualImpairmentLevel VisualImpairment { get; private set; }

    public bool IsActive { get; private set; } = true;

    public bool IsDeleted { get; private set; } = false;

    public List<AuthUserRefreshToken> RefreshTokens { get; private set; } = new();

    // Constructor principal
    public AuthUser(string email, string passwordHash, string name, string lastname, DateTime birthday, DateTime registeredAt, VisualImpairmentLevel visualImpairment)
    {
        Email = email;
        PasswordHash = passwordHash;
        Name = name;
        LastName = lastname;
        Birthday = birthday;
        RegisteredAt = registeredAt;
        VisualImpairment = visualImpairment;
    }

    // Constructor vacío
    public AuthUser()
        : this(string.Empty, string.Empty, string.Empty, string.Empty, DateTime.UtcNow, DateTime.UtcNow, VisualImpairmentLevel.Leve) { }

    public AuthUser UpdateEmail(string email)
    {
        Email = email;
        return this;
    }

    public AuthUser UpdatePassword(string password)
    {
        PasswordHash = password;
        return this;
    }

    public void SetPassword(string newPassword)
    {
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
    }

    public void Delete()
    {
        IsDeleted = true;
    }

    public void Restore()
    {
        IsDeleted = false;
    }
}
