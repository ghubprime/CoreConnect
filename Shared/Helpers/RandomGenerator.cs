using System.Security.Cryptography;

namespace CoreConnect.Shared.Helpers;

public class RandomGenerator
{
    private const string AllowableCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string GenerateString(int length)
    {
        return RandomNumberGenerator.GetString(AllowableCharacters, length);
    }

    public static string GenerateAccessKey()
    {
        return GenerateString(64);
    }
}
