using System;
using System.IO;
using System.Security.Cryptography;
using Dorbit.Framework.Extensions;

namespace Dorbit.Framework.Utils.Cryptography;

public class RsaHelper : IDisposable
{
    private RSA _rsa;

    private RSA GetRsa()
    {
        return _rsa ?? throw new NullReferenceException("First load rsa.");
    }
    
    public void LoadPem(string pem)
    {
        _rsa = RSA.Create();
        _rsa.ImportFromPem(pem);
    }
    
    public void LoadPemFile(string path)
    {
        var privateKeyPem = File.ReadAllText(path);
        _rsa = RSA.Create();
        _rsa.ImportFromPem(privateKeyPem);
    }
    
    public byte[] Decrypt(byte[] encryptedBytes, RSAEncryptionPadding padding)
    {
        return GetRsa().Decrypt(encryptedBytes, padding);
    }

    public byte[] DecryptFromHex(string encryptedHex, RSAEncryptionPadding padding)
    {
        var encryptedBytes = encryptedHex.HexToByteArray();
        return GetRsa().Decrypt(encryptedBytes, padding);
    }

    public byte[] DecryptFromBase64(string encryptedBase64, RSAEncryptionPadding padding)
    {
        var encryptedBytes = Convert.FromBase64String(encryptedBase64);
        return GetRsa().Decrypt(encryptedBytes, padding);
    }

    public void Dispose()
    {
        _rsa?.Dispose();
    }
}