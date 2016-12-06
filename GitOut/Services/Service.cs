using FluentScheduler;
using GitOut.Utility;
using Nancy.Hosting.Self;
using System;
using System.Configuration;

namespace GitOut.Services
{
  public class Service : Registry
  {
    private NancyHost _host;
    private readonly Uri _uri = new Uri($"http://{ConfigurationManager.AppSettings["HostAddress"]}");

    public void Start()
    {
      (_host = new NancyHost(_uri, new Bootstrapper(), new HostConfiguration
      {
        UrlReservations = new UrlReservations {CreateAutomatically = true}
      })).Start();

      Schedule(Process).ToRunNow().AndEvery(1).Hours();
      JobManager.Initialize(this);
    }

    public void Process()
    {
      string error, repo, from, to;

      repo = "ceyenne-wms";
      from = "1.8.0e";
      to = "1.8.0e#smph";

      GitHub.Post($"repos/{GitHub.Main}/{repo}/pulls",
        $@"{{
            ""title"": ""Automatic PR for feature branch"",
            ""body"": ""From {from} into {to}"",
            ""head"": ""{from}"",
            ""base"": ""{to}""
          }}", out error);

      repo = "purr";
      from = "master";
      to = "master#smph";

      GitHub.Post($"repos/{GitHub.Main}/{repo}/pulls",
        $@"{{
            ""title"": ""Automatic PR for feature branch"",
            ""body"": ""From {from} into {to}"",
            ""head"": ""{from}"",
            ""base"": ""{to}""
          }}", out error);
    }

    public void Stop()
    {
      _host.Stop();
      JobManager.Stop();
    }
  }
}