using GitOut.Models;
using GitOut.Utility;
using Nancy;
using System;

namespace GitOut.Modules
{
  public class Diffs : NancyModule
  {
    public Diffs()
    {
      Get["/diff/{repo}/{owner}/{branch}"] = p =>
      {
        p.repo = NancyUri.Decode(p.repo);
        p.owner = NancyUri.Decode(p.owner);
        p.branch = NancyUri.Decode(p.branch);

        string error;
        var json = GitHub.Get($"repos/{GitHub.Main}/{p.repo}/compare/{p.branch}...{p.owner}:{p.branch}", out error);
        if (json == null) return new Error {Message = error};

        return new Diff
        {
          Status = json.status,
          By = Math.Max((int) json.ahead_by, (int) json.behind_by),
          Link = $"https://github.com/{GitHub.Main}/{p.repo}/compare/{p.branch}...{p.owner}:{p.branch}?expand=1"
        };
      };
    }
  }
}