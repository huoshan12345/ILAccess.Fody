using System;

namespace ILAccess.Tests.AssemblyToProcess;

public static class RandomExtensions
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public static char NextChar(this Random random)
    {
        var index = random.Next(0, Chars.Length);
        return Chars[index];
    }

    public static string NextString(this Random random, int length)
    {
        var stringChars = new char[length];
        for (var i = 0; i < stringChars.Length; ++i)
        {
            stringChars[i] = random.NextChar();
        }
        return new string(stringChars);
    }
}