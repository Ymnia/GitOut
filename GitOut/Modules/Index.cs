using Nancy;

namespace GitOut.Modules
{
  public class Index : NancyModule
  {
    public Index()
    {
      Get["/"] = p => View["index.html"];
    }
  }
}