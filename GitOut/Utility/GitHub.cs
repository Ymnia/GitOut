using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace GitOut.Utility
{
  public partial class GitHub
  {
    public static string Main = "diract-it";
    public static string[] Sources = {"ceyenne-wms", "purr", "purr-apps"};

    // first time; create a file Token.cs in Utility (it's missing from solution)
    // and create a public partial class GitHub with a mere public static string Token = "<your_GitHub_access_token_here>"

    // .gitignore will prevent you from ever pushing this file, you should always keep it safe!

    public static dynamic Get(string uri, out string error)
    {
      try
      {
        var req = (HttpWebRequest) WebRequest.Create($"https://api.github.com/{uri}");

        req.UserAgent = "GitOut";
        req.Headers.Add("Authorization", $"Token {Token}");

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

    public static dynamic Post(string uri, string json, out string error, string method = "POST")
    {
      try
      {
        var req = (HttpWebRequest) WebRequest.Create($"https://api.github.com/{uri}");

        req.Method = method;
        req.UserAgent = "GitOut";
        req.Headers.Add("Authorization", $"Token {Token}");

        using (var writer = new StreamWriter(req.GetRequestStream()))
        {
          writer.Write(json);

          writer.Flush();
          writer.Close();
        }

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