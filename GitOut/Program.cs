using GitOut.Utility;
using Nancy.Hosting.Self;
using System;
using System.Configuration;
using Topshelf;

namespace GitOut
{
  public class Program
  {
    public static void Main(string[] args)
    {
      HostFactory.Run(x =>
      {
        x.Service<NancySelfHost>(y =>
        {
          y.ConstructUsing(z => new NancySelfHost());
          y.WhenStarted(z => z.Start());
          y.WhenStopped(z => z.Stop());
        });

        x.RunAsLocalSystem();

        x.SetDescription("GitOut");
        x.SetDisplayName("GitOut");
        x.SetServiceName("GitOut");
      });
    }

    public class NancySelfHost
    {
      private NancyHost _host;
      private readonly Uri _uri = new Uri($"http://{ConfigurationManager.AppSettings["HostAddress"]}");

      public void Start()
      {
        (_host = new NancyHost(_uri, new Bootstrapper(), new HostConfiguration
        {
          UrlReservations = new UrlReservations {CreateAutomatically = true}
        })).Start();
      }

      public void Stop()
      {
        _host.Stop();
      }
    }
  }
}