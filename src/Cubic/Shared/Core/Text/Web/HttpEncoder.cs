﻿using Cubic.Core.Diagnostics;

namespace Cubic.Core.Text.Web
{
  public class HttpEncoder
  {
    public static int HexToInt(char h)
    {
      return (h >= '0' && h <= '9') ? h - '0' :
        (h >= 'a' && h <= 'f') ? h - 'a' + 10 :
        (h >= 'A' && h <= 'F') ? h - 'A' + 10 :
        -1;
    }

    public static char IntToHex(int n)
    {
      Guard.EnsuresArgument( ()=> n < 0x10, nameof(n));
      //ga.Assert(n < 0x10);

      if (n <= 9)
        return (char)(n + (int)'0');
      else
        return (char)(n - 10 + (int)'a');
    }

    // Set of safe chars, from RFC 1738.4 minus '+'
    public static bool IsUrlSafeChar(char ch)
    {
      if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9'))
        return true;

      switch (ch)
      {
        case '-':
        case '_':
        case '.':
        case '!':
        case '*':
        case '(':
        case ')':
          return true;
      }

      return false;
    }

    //  Helper to encode spaces only
    public static string UrlEncodeSpaces(string str)
    {
      if (str != null && str.IndexOf(' ') >= 0)
        str = str.Replace(" ", "%20");
      return str;
    }
  }
}