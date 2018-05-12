using System.ComponentModel;
using System.ServiceProcess;

namespace DataAnalysis.Service.Installer
{
    [RunInstaller(true)]
    public partial class DataAnalysisServiceInstaller : System.Configuration.Install.Installer
    {
        public DataAnalysisServiceInstaller()
        {
            this.InitializeComponent();

            var serviceProcessInstaller = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem
            };

            var serviceInstaller = new ServiceInstaller
            {
                StartType = ServiceStartMode.Manual,
                ServiceName = "DataAnalysisService",
                DisplayName = "DataAnalysisService"
            };

            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}
