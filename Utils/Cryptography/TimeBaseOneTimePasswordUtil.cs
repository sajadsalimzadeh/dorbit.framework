using System;
using System.Security.Cryptography;

namespace Dorbit.Framework.Utils.Cryptography;

public static class TimeBaseOneTimePasswordUtil
{
    public static string GenerateOtp(string secret, int timeOffset = 0)
    {
        return GenerateOtp(HashUtil.Base32Decode(secret), timeOffset);
    }
    
    public static string GenerateOtp(byte[] secret, int timeOffset = 0)
    {
        long timestep = 30;
        var unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var counter = unixTime / timestep + timeOffset;
        
        var counterBytes = BitConverter.GetBytes(counter);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(counterBytes);
        
        using var hmac = new HMACSHA1(secret);
        var hash = hmac.ComputeHash(counterBytes);
        var offset = hash[^1] & 0x0F;

        var binary =
            ((hash[offset] & 0x7F) << 24) |
            ((hash[offset + 1] & 0xFF) << 16) |
            ((hash[offset + 2] & 0xFF) << 8) |
            (hash[offset + 3] & 0xFF);
        
        int otp = binary % 1_000_000;

        return otp.ToString("D6");
    }

    public static bool Validate(string secret, string input, int window = 1)
    {
        for (var i = -window; i <= window; i++)
        {
            var code = GenerateOtp(secret, i);
            if (code == input)
                return true;
        }

        return false;
    }

    public static string GenerateUrl(string issuer, string username, string secret)
    {
        return $"otpauth://totp/{issuer}:{username}?secret={secret}&issuer={issuer}&digits=6&period=30";
    }
}