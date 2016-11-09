using GitOut.Models;
using GitOut.Utility;
using Nancy;
using System.Collections.Generic;

namespace GitOut.Modules
{
  public class Branches : NancyModule
  {
    public Branches()
    {
      Get["/branches/{repo}/{owner}"] = p =>
      {
        var branches = new List<Branch>();
        var json = GitHub.Get($"repos/{p.owner}/{p.repo}/branches?per_page=100");

        foreach (var b in json) branches.Add(new Branch {Name = b.name});
        return new Repo {Name = p.repo, Owner = p.owner, Branches = branches};
      };
    }
  }
}