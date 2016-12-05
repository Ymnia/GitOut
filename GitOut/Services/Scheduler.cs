using FluentScheduler;
using GitOut.Utility;
using System;

namespace GitOut.Services
{
  public class Scheduler
  {
    public class JobRegistery : Registry
    {
      public JobRegistery()
      {
        //Schedule(Process).ToRunNow();
        Schedule(Process).ToRunEvery(1).Days().At(12, 00);
      }

      public void Process()
      {
        string error;

        const string @from = "1.8.0e";
        const string to = "1.8.0e#bf";

        GitHub.Post($"repos/{GitHub.Main}/ceyenne-wms/pulls",
          $@"{{
            ""title"": ""Automatic PR for feature branch"",
            ""body"": ""From {from} into {to}"",
            ""head"": ""{from}"",
            ""base"": ""{to}""
          }}", out error);
      }
    }

    public void Start()
    {
      Console.WriteLine("Service started");
      JobManager.Initialize(new JobRegistery());
    }

    public void Stop()
    {
      Console.WriteLine("Service stopped");
    }
  }
}