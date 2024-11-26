using System.Security.Cryptography;
using System.Text;

namespace SchoolStaffAPI.Services.Mixin;

public abstract class PasswordMixin
{
    protected static string HashPassword(string password)
    {
        var passwordBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(passwordBytes).ToLower();
    }

    protected static bool VerifyPassword(string password, string hashedPassword)
    {
        var computedHash = HashPassword(password);
        return computedHash == hashedPassword;
    }
}
