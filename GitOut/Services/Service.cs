using FluentScheduler;
using GitOut.Utility;
using Nancy.Hosting.Self;
using System;
using System.Configuration;

namespace GitOut.Services
{
  public class Service
  {
    private NancyHost _host;
    private readonly Uri _uri = new Uri($"http://{ConfigurationManager.AppSettings["HostAddress"]}");

    public void Start()
    {
      (_host = new NancyHost(_uri, new Bootstrapper(), new HostConfiguration
      {
        UrlReservations = new UrlReservations {CreateAutomatically = true}
      })).Start();

      JobManager.Initialize(new AutoPull());
      JobManager.Initialize(new AdventOfCode());
    }

    public void Stop()
    {
      _host.Stop();
      JobManager.Stop();
    }
  }
}