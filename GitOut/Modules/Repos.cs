using GitOut.Models;
using GitOut.Utility;
using Nancy;
using System.Linq;

namespace GitOut.Modules
{
  public class Repos : NancyModule
  {
    public Repos()
    {
      Get["/repos"] = p =>
      {
        return GitHub.Sources.Select(r => new Repo {Name = r}).ToList();
      };
    }
  }
}