using System;

namespace GitOut.Utility
{
  public static class NancyUri
  {
    public static string Decode(string s)
    {
      return Uri.EscapeDataString(s.Replace("x_____x", "/").Replace("x____x", "\\").Replace("x___x", "#"));
    }
  }
}