﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cubic.Core.Runtime
{
  public static class WindowsFoundationTools
  {
    private const int EscapeDataStringCharactersMaxLength = 10000;

    public static IDictionary<string, string> GetParameters(this Uri uri)
    {
      return uri
        .Query
        .Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(p => p.Split(new[] { '=' }))
        .Where(parts => parts.Count() > 1)
        .ToDictionary(parts => parts[0], parts => string.Join("=", parts.Skip(1)));
    }

    /// <summary>
    /// Converts a string to its escaped representation.
    /// This extension bypasses the Uri.EscapeDataString characters limit.
    /// </summary>
    /// Source: http://stackoverflow.com/questions/6695208/uri-escapedatastring-invalid-uri-the-uri-string-is-too-long
    public static string EscapeDataString(string value)
    {
      var sb = new StringBuilder();
      var loops = value.Length / EscapeDataStringCharactersMaxLength;

      for (var i = 0; i <= loops; i++)
      {
        if (i < loops)
        {
          sb.Append(Uri.EscapeDataString(value.Substring(EscapeDataStringCharactersMaxLength * i, EscapeDataStringCharactersMaxLength)));
        }
        else
        {
          sb.Append(Uri.EscapeDataString(value.Substring(EscapeDataStringCharactersMaxLength * i)));
        }
      }

      return sb.ToString();
    }

    /// <summary>
    /// Get extension of the traget file of the uri.
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    public static string GetExtension(this Uri uri)
    {
      var url = uri.OriginalString;

      try
      {
        //Path.GetExtension can throw if invalid path characters are present in the Uri (some of these characters are valid in Uri)
        return System.IO.Path.GetExtension(url);
      }
      catch (ArgumentException)
      {
        //Suppress the exception (Could cause crash in some cases, e.g. SetImageSource from OnApplyTemplate).

        var lastIndex = url.LastIndexOf('.');

        if (lastIndex != -1)
        {
          return url.Substring(lastIndex);
        }
        //try to manually extract extension from string. This solution is less efficient than Path.GetExtension, but 
        //will do the job for the vast majority of paths. Moreso, it will not cause any exceptions of its own.
      }

      return string.Empty;
    }

    //public static bool IsLocalResource(this Uri uri)
    //{
    //  return uri.Scheme.Equals("ms-appx", StringComparison.OrdinalIgnoreCase)
    //         || uri.Scheme.Equals("ms-appdata", StringComparison.OrdinalIgnoreCase);
    //}
  }
}