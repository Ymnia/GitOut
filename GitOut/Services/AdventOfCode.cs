using FluentScheduler;
using GitOut.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace GitOut.Services
{
  public class AdventOfCode : Registry
  {
    public bool Initializing = true;
    public Dictionary<string, List<int>> Stars = new Dictionary<string, List<int>>();

    public AdventOfCode()
    {
      Schedule(Process).ToRunNow().AndEvery(30).Minutes();
    }

    public void Process()
    {
      try
      {
        var stats = Get();
        if (stats == null) return;

        foreach (var x in stats.members)
        {
          foreach (var m in x)
          {
            if (!Stars.ContainsKey(m.name.ToString()))
            {
              Stars.Add(m.name.ToString(), new List<int>());
              if (!Initializing) Slack.SendMessage($"{m.name} joined the Advent of Code challenge! Welcome to the team!", "#aoc", ":aoc:", "Advent of Code");
            }

            foreach (var s in m.completion_day_level)
            {
              foreach (var y in s)
              {
                foreach (var p in y)
                {
                  var star = int.Parse(s.Name) * 100 + int.Parse(p.Name);
                  if (Stars[m.name.ToString()].Contains(star)) continue;

                  Stars[m.name.ToString()].Add(star);
                  if (!Initializing) Slack.SendMessage($"{m.name} solved part {p.Name} of day {s.Name}.", "#aoc", ":aoc:", "Advent of Code");
                }
              }
            }
          }
        }

        Initializing = false;
      }
      catch
      {
        // ignored
      }
    }

    public static dynamic Get()
    {
      try
      {
        var uri = new Uri("http://adventofcode.com/2017/leaderboard/private/view/59887.json");
        var req = (HttpWebRequest) WebRequest.Create(uri);

        req.UserAgent = "GitOut";
        req.CookieContainer = new CookieContainer();
        req.CookieContainer.Add(new Cookie("session", "53616c7465645f5f8cb973a4eda9a58e7259d719088e2b92f6c92eb471849bc9689ba22180596aafb03ccd99a56ac8f2") {Domain = uri.Host});

        using (var res = (HttpWebResponse) req.GetResponse())
        {
          using (var s = res.GetResponseStream())
          {
            if (s == null) throw new Exception("Github down? Wut?");
            using (var r = new StreamReader(s))
            {
              return JsonConvert.DeserializeObject<dynamic>(r.ReadToEnd());
            }
          }
        }
      }
      catch
      {
        return null;
      }
    }
  }
}