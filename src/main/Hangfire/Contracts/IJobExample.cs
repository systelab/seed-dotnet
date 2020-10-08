namespace Main.Hangfire.Contracts
{
    using System;
    using System.Threading.Tasks;

    public interface IJobExample
    {
        Task JobExample1(DateTime now);

        Task JobExample2(DateTime now);
    }
}