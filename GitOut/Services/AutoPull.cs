using FluentScheduler;
using GitOut.Utility;
using System;
using System.Configuration;
using System.Linq;

namespace GitOut.Services
{
  public class AutoPull : Registry
  {
    public AutoPull()
    {
      Schedule(Process).ToRunNow().AndEvery(15).Minutes();
    }

    public void Process()
    {
      // for now, we use the AutoPull configuration to determine which branches to pull into which other branch
      // in the future, i would like guidelines to define branch names, so this can be done automatically based on the branch name
      // for instance, FOO should always be pulled into any FOO#BAR branch unconditionally

      var config = ConfigurationManager.AppSettings["AutoPull"].Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
                                                               .Where(x => !string.IsNullOrWhiteSpace(x))
                                                               .Select(x => x.Trim().Split(';'));

      foreach (var c in config)
      {
        string error;
        var repo = c[0];
        var from = c[1];
        var to = c[2];

        GitHub.Post($"repos/{GitHub.Main}/{repo}/pulls",
          $@"{{
            ""title"": ""{from} --> {to} (auto)"",
            ""body"": ""Automatic PR for feature branch"",
            ""head"": ""{from}"",
            ""base"": ""{to}""
          }}", out error);
      }
    }
  }
}