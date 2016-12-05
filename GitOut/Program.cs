using GitOut.Services;
using Topshelf;

namespace GitOut
{
  public class Program
  {
    public static void Main(string[] args)
    {
      HostFactory.Run(x =>
      {
        x.Service<Service>(y =>
        {
          y.ConstructUsing(z => new Service());
          y.WhenStarted(z => z.Start());
          y.WhenStopped(z => z.Stop());
        });

        x.Service<Scheduler>(s =>
        {
          s.ConstructUsing(z => new Scheduler());
          s.WhenStarted(z => z.Start());
          s.WhenStopped(z => z.Stop());
        });

        x.RunAsLocalSystem();

        x.SetDescription("GitOut");
        x.SetDisplayName("GitOut");
        x.SetServiceName("GitOut");

        x.StartAutomatically();
      });
    }
  }
}