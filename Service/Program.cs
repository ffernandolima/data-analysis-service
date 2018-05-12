using DataAnalysis.Framework.Initialization;
using log4net.Config;
using System.ServiceProcess;
using System.Threading;

namespace DataAnalysis.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            ApplicationInitialize();

#if DEBUG
            var service = new DataAnalysisService();

            service.DebugRun(new string[] { });

            Thread.Sleep(Timeout.Infinite);
#endif
            var servicesToRun = new ServiceBase[] 
            {
                new DataAnalysisService() 
            };

            ServiceBase.Run(servicesToRun);
        }

        private static void ApplicationInitialize()
        {
            // log4net
            XmlConfigurator.Configure();

            // Appdomain
            ExceptionHandler.Initialize();

            // Invariantculture
            CultureConfigurator.Configure();
        }
    }
}
