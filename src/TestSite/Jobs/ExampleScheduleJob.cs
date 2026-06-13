using Blend.Optimizely.ScheduledJobs;
using EPiServer.Scheduler;

namespace TestSite.Jobs;

[ScheduledJob(
    DisplayName = "Example Job",
    Description = "Tests that the BlendJobBase can be instantiated correctly",
    GUID = "D1C94B3E-A5F2-4789-B0C3-E17D6A4F9021")]
public class ExampleScheduleJob : BlendJobBase
{
    public override string Execute()
    {
        for (var i = 0; i < 10; i++)
        {
            Increment("Iterations");
            Thread.Sleep(5000);

            OnStatusChanged(CounterReport());
        }

        return CounterReport();
    }
}
