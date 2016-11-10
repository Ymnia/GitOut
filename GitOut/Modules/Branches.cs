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
        p.repo = NancyUri.Decode(p.repo);
        p.owner = NancyUri.Decode(p.owner);

        string error;
        var json = GitHub.Get($"repos/{p.owner}/{p.repo}/branches?per_page=100", out error);
        if (json == null) return new Error {Message = error};

        var branches = new List<Branch>();
        foreach (var b in json) branches.Add(new Branch {Name = b.name});
        return new Repo {Name = p.repo, Owner = p.owner, Branches = branches};
      };
    }
  }
}