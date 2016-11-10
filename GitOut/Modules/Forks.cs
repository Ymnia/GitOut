using GitOut.Models;
using GitOut.Utility;
using Nancy;
using System.Collections.Generic;

namespace GitOut.Modules
{
  public class Forks : NancyModule
  {
    public Forks()
    {
      Get["/forks/{repo}"] = p =>
      {
        p.repo = NancyUri.Decode(p.repo);

        string error;
        var json = GitHub.Get($"repos/{GitHub.Main}/{p.repo}/forks?per_page=100", out error);
        if (json == null) return new Error {Message = error};

        var forks = new List<Fork>();
        foreach (var f in json) forks.Add(new Fork {Owner = f.owner.login});
        return new Repo {Name = p.repo, Owner = GitHub.Main, Forks = forks};
      };
    }
  }
}