using System.Security.Cryptography;

namespace WpfApp2.Security;

/// <summary>Versioned PBKDF2 password storage. Existing plaintext passwords are upgraded on successful login.</summary>
public static class PasswordHasher
{
    private const int Iterations = 210_000;
    private const int SaltLength = 16;
    private const int KeyLength = 32;
    private const string Prefix = "PBKDF2-SHA256";

    public static string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);
        byte[] salt = RandomNumberGenerator.GetBytes(SaltLength);
        byte[] key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeyLength);
        return $"{Prefix}${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
    }

    public static bool Verify(string enteredPassword, string? storedPassword, out bool needsUpgrade)
    {
        needsUpgrade = false;
        if (string.IsNullOrEmpty(enteredPassword) || string.IsNullOrEmpty(storedPassword)) return false;
        if (!storedPassword.StartsWith(Prefix + "$", StringComparison.Ordinal))
        {
            // Backwards compatibility with the original Final 3 database; rehash after login.
            needsUpgrade = true;
            return CryptographicOperations.FixedTimeEquals(
                System.Text.Encoding.UTF8.GetBytes(enteredPassword),
                System.Text.Encoding.UTF8.GetBytes(storedPassword));
        }

        string[] parts = storedPassword.Split('$');
        if (parts.Length != 4 || !int.TryParse(parts[1], out int rounds) || rounds < 100_000 || rounds > 2_000_000)
            return false;
        try
        {
            byte[] salt = Convert.FromBase64String(parts[2]);
            byte[] expected = Convert.FromBase64String(parts[3]);
            if (salt.Length != SaltLength || expected.Length != KeyLength) return false;
            byte[] actual = Rfc2898DeriveBytes.Pbkdf2(enteredPassword, salt, rounds, HashAlgorithmName.SHA256, expected.Length);
            needsUpgrade = rounds < Iterations;
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException) { return false; }
    }
}
