using System.Diagnostics;
using FluentScheduler;
using GitOut.Utility;
using System.IO;
using System.Linq;

namespace GitOut.Services
{
  public class FeatureBranch : Registry
  {
    private static string FeboHost = @"http://feebl.azurewebsites.net/Service.asmx";
    public FeatureBranch()
    {
      Schedule(Process).NonReentrant().ToRunNow().AndEvery(10).Minutes();
    }

    public void Process()
    {
      // for now, we use the FeatureBranch.data configuration to determine which branches to pull into which other branch

      // in the future, i would like guidelines to define branch names, so this can be done automatically based on the branch name
      // for instance, FOO should always be pulled into any FOO#BAR branch unconditionally

      var lines = File.ReadAllLines("Services/FeatureBranch.data")
          .Where(x => !string.IsNullOrWhiteSpace(x) && !x.StartsWith("//"))
          .Select(x => x.Trim().Split(';'))
          .ToList();
          lines.ForEach(c => GitHub.Post($"repos/{GitHub.Main}/{c[0]}/pulls", $"{{\"title\": \"{c[1]} → {c[2]}\", \"body\": \"Automatic PR for feature branch\", \"head\": \"{c[1]}\", \"base\": \"{c[2]}\"}}", out var _));
      Feebl.Client.Service.Update(out var _1, FeboHost, "GitOut", "Diract", "AutoPull, branches", lines.Count);
    }
  }
}