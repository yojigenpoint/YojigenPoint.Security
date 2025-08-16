using System.Security.Cryptography;
using System.Text;
using YojigenPoint.Security.Abstractions;

namespace YojigenPoint.Security.Services;

public class AesEncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    private const int SaltSize = 16;
    private const int IvSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName _hashAlgorithm = HashAlgorithmName.SHA256;

    public AesEncryptionService(string masterKey)
    {
        if (string.IsNullOrWhiteSpace(masterKey))
        {
            throw new ArgumentException("Master key cannot be null or empty.", nameof(masterKey));
        }
        _key = DeriveKeyFromMasterKey(masterKey);
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrWhiteSpace(plainText)) return string.Empty;

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        using Aes aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        using MemoryStream memoryStream = new();
        memoryStream.Write(salt, 0, salt.Length);
        memoryStream.Write(aes.IV, 0, aes.IV.Length);

        using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
        }
        return Convert.ToBase64String(memoryStream.ToArray());
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrWhiteSpace(cipherText)) return string.Empty;

        byte[] cipherData = Convert.FromBase64String(cipherText);
        if (cipherData.Length < SaltSize + IvSize)
        {
            throw new ArgumentException("Invalid cipher text: data is too short.", nameof(cipherText));
        }

        try
        {
            byte[] salt = new byte[SaltSize];
            Array.Copy(cipherData, 0, salt, 0, SaltSize);
            byte[] iv = new byte[IvSize];
            Array.Copy(cipherData, SaltSize, iv, 0, IvSize);

            using Aes aes = Aes.Create();
            aes.Key = _key;
            aes.IV = iv;

            using MemoryStream memoryStream = new(cipherData, SaltSize + IvSize, cipherData.Length - (SaltSize + IvSize));
            using var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var streamReader = new StreamReader(cryptoStream);

            return streamReader.ReadToEnd();
        }
        catch (CryptographicException ex)
        {
            throw new CryptographicException("Decryption failed. The data is corrupted or the key is incorrect.", ex);
        }
    }

    private static byte[] DeriveKeyFromMasterKey(string masterKey)
    {
        var salt = Encoding.UTF8.GetBytes("YojigenPoint.Static.Salt.For.Key.Derivation");
        return Rfc2898DeriveBytes.Pbkdf2(masterKey, salt, Iterations, _hashAlgorithm, KeySize);
    }
}