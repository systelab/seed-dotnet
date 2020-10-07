namespace main.Hangfire
{
    using System;

    using global::Hangfire;

    using main.Hangfire.Jobs;

    public class HangfireJobScheduler
    {
        public static void ScheduleRecurringJobs()
        {
            //Job Executions
            RecurringJob.RemoveIfExists("Job Example 1");
            RecurringJob.AddOrUpdate<JobExample>("Job Example 1", job => job.JobExample1(JobCancellationToken.Null), Cron.Daily, TimeZoneInfo.Local);

            RecurringJob.RemoveIfExists("Job Example 2");
            RecurringJob.AddOrUpdate<JobExample>("Job Example 2", job => job.JobExample2(JobCancellationToken.Null), Cron.Daily, TimeZoneInfo.Local);
        }
    }
}