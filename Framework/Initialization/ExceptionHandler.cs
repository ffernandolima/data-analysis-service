using DataAnalysis.Framework.Logs;
using System;

namespace DataAnalysis.Framework.Initialization
{
    public static class ExceptionHandler
    {
        public static void Initialize()
        {
            var appDomain = AppDomain.CurrentDomain;
            appDomain.UnhandledException += AppDomainException;
        }

        static void AppDomainException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var exception = (Exception)e.ExceptionObject;
                Log4NetHelper.All(x => x.Error("AppDomain Unhandled exception", exception));
            }
            catch
            {
                // ignored
            }
        }
    }
}
