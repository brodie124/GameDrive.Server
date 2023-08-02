using System.Security.Cryptography;
using System.Text;

namespace GameDrive.Server.Utilities;

public static class HashHelper
{
    public static byte[] Sha1(byte[] inputBytes)
    {
        using var sha1Hash = SHA1.Create();
        var hashBytes = sha1Hash.ComputeHash(inputBytes);
        return hashBytes;
    }

    public static string Sha1String(string input)
    {
        using var sha1Hash = SHA1.Create();
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = sha1Hash.ComputeHash(inputBytes);
        var hashString = Convert.ToBase64String(hashBytes);
        return hashString;
    }
    
    public static string Sha1String(byte[] input)
    {
        var hashBytes = Sha1(input);
        var hashString = Convert.ToBase64String(hashBytes);
        return hashString;
    }

    public static string Md5String(string input)
    {
        using var md5 = MD5.Create();
        var inputBytes = Encoding.ASCII.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);
        return Convert.ToHexString(hashBytes);
    }
}