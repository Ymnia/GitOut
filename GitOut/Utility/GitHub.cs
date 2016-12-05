﻿using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace GitOut.Utility
{
  public partial class GitHub
  {
    public static string Main = "diract-it";
    public static string[] Sources = {"ceyenne-wms", "purr", "purr-apps"};

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

    public static dynamic Post(string uri, string json, out string error)
    {
      try
      {
        var req = (HttpWebRequest) WebRequest.Create($"https://api.github.com/{uri}");

        req.Method = "POST";
        req.UserAgent = "GitOut";
        req.Headers.Add("Authorization", $"Basic {Token}");

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