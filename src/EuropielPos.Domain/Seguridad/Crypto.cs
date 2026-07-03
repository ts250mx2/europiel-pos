using System.Security.Cryptography;
using System.Text;

namespace EuropielPos.Domain.Seguridad;

/// <summary>
/// Port de <c>Crypto.vb</c> — cifrado del cuerpo de las peticiones al
/// procesador de pagos: SHA-256 del cuerpo, cifrado AES-CBC/PKCS7 y
/// resultado como <c>{merchant}_{HEX}</c>.
/// </summary>
public static class Crypto
{
    public static string StartEncryption(string body, string keyHex, string ivHex, string merchant)
    {
        byte[] key = ConvertHexStringToByteArray(keyHex);
        byte[] iv = ConvertHexStringToByteArray(ivHex);

        return EncryptString(body, key, iv, merchant);
    }

    public static byte[] ConvertHexStringToByteArray(string hexString)
    {
        var bytes = new byte[hexString.Length / 2];

        for (int i = 0; i < hexString.Length; i += 2)
            bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);

        return bytes;
    }

    private static string EncryptString(string body, byte[] key, byte[] iv, string merchant)
    {
        string sha256 = GenerateSha256String(body);

        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = key;
        aes.IV = iv;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        byte[] plainBytes = Encoding.ASCII.GetBytes(sha256);
        byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        return merchant + "_" + Convert.ToHexString(cipherBytes);
    }

    public static string GenerateSha256String(string inputString)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(inputString));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
