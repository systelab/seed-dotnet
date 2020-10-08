namespace Main.Hangfire.Jobs
{
    using System;
    using System.Threading.Tasks;

    using global::Hangfire;

    using Main.Hangfire.Contracts;

    public class JobExample : IJobExample
    {
        public Task JobExample1(DateTime now)
        {
            //Do something
            throw new NotImplementedException();
        }

        public async Task JobExample1(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await this.JobExample1(DateTime.Now).ConfigureAwait(false);
        }

        public Task JobExample2(DateTime now)
        {
            //Do something
            throw new NotImplementedException();
        }

        public async Task JobExample2(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await this.JobExample2(DateTime.Now).ConfigureAwait(false);
        }
    }
}