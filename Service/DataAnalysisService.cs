using DataAnalysis.Framework.Initialization;
using DataAnalysis.Framework.Logs;
using DataAnalysis.Service.Configuration;
using System;
using System.ServiceProcess;
using System.Threading;

namespace DataAnalysis.Service
{
    partial class DataAnalysisService : ServiceBase
    {
        private readonly Settings _settings;
        private readonly ManualResetEvent _manualResetEvent;

        private Thread _thread;
        private DataAnalysis _dataAnalysis;

        public DataAnalysisService()
        {
            this.InitializeComponent();

            this._settings = Settings.Instance;
            this._manualResetEvent = new ManualResetEvent(false);
        }

        public void DebugRun(string[] args)
        {
            this.OnStart(args);
        }

        protected override void OnStart(string[] args)
        {
            Log4NetHelper.All(x => x.Info("DataAnalysisService.Start"));

            try
            {
                this._dataAnalysis = new DataAnalysis(this._settings);
                this._thread = new Thread(this.Run);
                this._thread.Start();

            }
            catch (Exception ex)
            {
                Log4NetHelper.All(x => x.Error("An error occurred while starting service.", ex));
            }
        }

        protected override void OnStop()
        {
            Log4NetHelper.All(x => x.Info("DataAnalysisService.Stop"));

            var sleepTime = this._settings.SleepTime;
            var extraTime = (long)(sleepTime.Ticks * 1.5); // Adds an extra time to ensure that the process will finish successfully
            var waitTimeout = sleepTime + new TimeSpan(extraTime);

            try
            {
                this._manualResetEvent.Set();
                this._thread.Join(waitTimeout);
            }
            catch (Exception ex)
            {
                Log4NetHelper.All(x => x.Error("An error occurred while stopping service.", ex));
            }
        }

        private void Run()
        {
            CultureConfigurator.Configure();

            bool signaled;
            var sleepTime = this._settings.SleepTime;

            do
            {
                var success = this._dataAnalysis.Process();

                if (!success)
                {
                    Thread.Sleep(sleepTime);
                }

                signaled = this._manualResetEvent.WaitOne(0);

            } while (!signaled);
        }
    }
}
