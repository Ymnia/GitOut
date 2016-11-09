using System.Collections.Generic;

namespace GitOut.Models
{
  public class Repo
  {
    public string Name;
    public string Owner;
    public List<Branch> Branches;
    public List<Fork> Forks;
  }
}