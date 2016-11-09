using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace GitOut.Utility
{
  public class Bootstrapper : DefaultNancyBootstrapper
  {
    protected override void RequestStartup(TinyIoCContainer container, IPipelines p, NancyContext context)
    {
      p.AfterRequest.AddItemToEndOfPipeline((ctx) =>
      {
        ctx.Response.WithHeader("Access-Control-Allow-Origin", "*")
           .WithHeader("Access-Control-Allow-Methods", "POST,GET")
           .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type, Authorization");
      });
    }
  }
}