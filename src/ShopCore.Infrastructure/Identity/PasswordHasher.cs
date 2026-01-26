using BCrypt.Net;
using ShopCore.Application.Common.Interfaces;

namespace ShopCore.Infrastructure.Identity;

public class PasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;
    private const int MinWorkFactor = 10;

    public string Hash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public bool Verify(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        if (string.IsNullOrWhiteSpace(passwordHash))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        catch (SaltParseException)
        {
            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Check if password hash needs to be upgraded to current work factor
    /// </summary>
    public bool NeedsRehash(string passwordHash)
    {
        try
        {
            // Extract work factor from existing hash
            // BCrypt hash format: $2a$12$... where 12 is the work factor
            var parts = passwordHash.Split('$');
            if (parts.Length < 4)
                return true;

            if (int.TryParse(parts[2], out var existingWorkFactor))
            {
                return existingWorkFactor < WorkFactor;
            }

            return true;
        }
        catch
        {
            return true;
        }
    }
}