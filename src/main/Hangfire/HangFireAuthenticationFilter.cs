namespace main.Hangfire
{
    using global::Hangfire.Dashboard;

    public class HangFireAuthenticationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            //TODO Implement a secure authentication
            // var httpContext = context.GetHttpContext();

            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            return true;
        }
    }
}