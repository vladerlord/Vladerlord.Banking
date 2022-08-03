using System.Security.Cryptography;
using System.Text;

namespace Service.Identity.Services;

public class EncryptionService
{
    private readonly byte[] _secretKey;

    public EncryptionService(string secretKey)
    {
        _secretKey = Convert.FromBase64String(secretKey);
    }

    public static string GenerateIV()
    {
        using var aes = Aes.Create();

        aes.GenerateIV();

        return Convert.ToBase64String(aes.IV);
    }

    public string Encrypt(string input, string iv)
    {
        var inputArray = Encoding.Unicode.GetBytes(input);
        var encrypted = Encrypt(inputArray, Convert.FromBase64String(iv));

        return Convert.ToBase64String(encrypted);
    }

    public string Decrypt(string input, string iv)
    {
        var inputArray = Convert.FromBase64String(input);
        var decrypted = Decrypt(inputArray, Convert.FromBase64String(iv));

        return Encoding.Unicode.GetString(decrypted);
    }

    public static string Hash(string input)
    {
        return BCrypt.Net.BCrypt.HashPassword(input, 12);
    }

    public bool VerifyHash(string input, string hashed)
    {
        return BCrypt.Net.BCrypt.Verify(input, hashed);
    }

    private byte[] Encrypt(byte[] input, byte[] iv)
    {
        using var aes = Aes.Create();

        aes.Key = _secretKey;
        aes.IV = iv;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        return PerformCryptography(input, encryptor);
    }

    private byte[] Decrypt(byte[] input, byte[] iv)
    {
        using var aes = Aes.Create();

        aes.Key = _secretKey;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        return PerformCryptography(input, decryptor);
    }

    private static byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
    {
        using var ms = new MemoryStream();
        using var cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write);

        cryptoStream.Write(data, 0, data.Length);
        cryptoStream.FlushFinalBlock();

        return ms.ToArray();
    }
}