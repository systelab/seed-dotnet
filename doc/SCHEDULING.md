# Scheduling

## Hangfire

Hangfire is an easy way to perform background processing without any Windows process. https://www.hangfire.io/

Creation of a New Job:

Step 1: 

Include in the hangfire.contracts the interface of the new job

Step 2:

Include in Hangfire.jobs, a class to implement the interface of the new job.

To be able to execute the job include the following lines referring to the new job:

 public async Task NewJobExample(IJobCancellationToken token)
	{
		token.ThrowIfCancellationRequested();
		await NewJobExample(DateTime.Now);
	}

Step 3:
Include the job execution reference in the HangfireJobScheduler.

	RecurringJob.RemoveIfExists("New Job Name");
    RecurringJob.AddOrUpdate<NewJobExample>("New Job Name",
    job => job.NewJobExample(JobCancellationToken.Null),
    Cron.Daily, TimeZoneInfo.Local);

Step 4: 

Launch the application and access to https://{domain}/hangfire

For more information access to https://docs.hangfire.io/en/latest/


