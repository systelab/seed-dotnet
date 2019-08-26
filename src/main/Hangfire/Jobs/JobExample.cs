using Hangfire;
using main.Hangfire.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace main.Hangfire.Jobs
{
    public class JobExample : IJobExample
    {
        public Task JobExample1(DateTime now)
        {
            //Do something
            throw new NotImplementedException();
        }

        public Task JobExample2(DateTime now)
        {
            //Do something
            throw new NotImplementedException();
        }

        public async Task JobExample1(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await JobExample1(DateTime.Now).ConfigureAwait(false);
        }

        public async Task JobExample2(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await JobExample2(DateTime.Now).ConfigureAwait(false);
        }
    }
}
