using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace main.Hangfire.Contracts
{
    public interface IJobExample
    {
        Task JobExample1(DateTime now);
        Task JobExample2(DateTime now);
    }
}
