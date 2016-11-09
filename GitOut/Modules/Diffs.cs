using GitOut.Models;
using GitOut.Utility;
using Nancy;

namespace GitOut.Modules
{
  public class Diffs : NancyModule
  {
    public Diffs()
    {
      Get["/diff/{repo}/{owner}/{branch}"] = p =>
      {
        string error;
        var json = GitHub.Get($"repos/{GitHub.Main}/{p.repo}/compare/{p.branch}...{p.owner}:{p.branch}", out error);

        return json == null
          ? new Diff {Error = error}
          : new Diff
          {
            Status = json.status,
            By = json.ahead_by == 0 ? json.behind_by : json.ahead_by,
            Link = json.status == "ahead" || json.status == "diverged"
              ? $"https://github.com/{GitHub.Main}/{p.repo}/compare/{p.branch}...{p.owner}:{p.branch}?expand=1"
              : null
          };
      };
    }
  }
}