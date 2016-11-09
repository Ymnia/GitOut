using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace GitOut.Utility
{
  public class GitHub
  {
    public static string Main = "diract-it";
    public static string[] Sources = {"purr"};
    //public static string[] Sources = {"ceyenne-wms", "purr", "purr-apps"};
    private const string Token = "bf375eb2985e1a516d3c2fd8a3d3a19649f130af";

    public static dynamic Get(string uri)
    {
      string error;
      return Get(uri, out error);
    }

    public static dynamic Get(string uri, out string error)
    {
      try
      {
        var req = (HttpWebRequest) WebRequest.Create($"https://api.github.com/{uri}");

        req.UserAgent = "GitOut";
        req.Headers.Add("Authorization", $"Basic {Token}");

        using (var res = (HttpWebResponse) req.GetResponse())
        {
          using (var s = res.GetResponseStream())
          {
            if (s == null) throw new Exception("Github down? Wut?");
            using (var r = new StreamReader(s))
            {
              error = null;
              return JsonConvert.DeserializeObject<dynamic>(r.ReadToEnd());
            }
          }
        }
      }
      catch (Exception ex)
      {
        error = ex.ToString();
        return null;
      }
    }
  }
}