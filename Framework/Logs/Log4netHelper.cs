using log4net;
using System;
using System.IO;
using System.Linq;

namespace DataAnalysis.Framework.Logs
{
    public static class Log4NetHelper
    {
        private static string _loggerPrefix;

        public static string LoggerPrefix
        {
            set
            {
                _loggerPrefix = value;
            }
            get
            {
                if (string.IsNullOrWhiteSpace(_loggerPrefix))
                {
                    _loggerPrefix = DefaultPrefix();
                }

                return _loggerPrefix;
            }
        }

        public static ILog EventLogger
        {
            get
            {
                return LogManager.GetLogger(LoggerPrefix + ".EventLogger");
            }
        }

        public static ILog RollingFileLogger
        {
            get
            {
                return LogManager.GetLogger(LoggerPrefix + ".RollingFileLogger");
            }
        }

        public static void All(Action<ILog> logAction)
        {
            var list = new[] { EventLogger, RollingFileLogger }.AsParallel().Where(x => x != null);
            list.ForAll(logAction);
        }

        private static string DefaultPrefix()
        {
            var appName = AppDomain.CurrentDomain.FriendlyName;
            var domainName = Path.GetFileNameWithoutExtension(appName);
#if DEBUG
            domainName = domainName.Replace(".vshost", string.Empty);
#endif
            return domainName;
        }
    }
}
