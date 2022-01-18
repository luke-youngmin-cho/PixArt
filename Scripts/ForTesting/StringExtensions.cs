using System;
using System.Collections.Generic;

public static class StringExtensions 
{
    public static string Filter(this string str, List<char> charsToRemove)
    {
        foreach (char c in charsToRemove)
        {
            str = str.Replace(c.ToString(), String.Empty);
        }

        return str;
    }
}
