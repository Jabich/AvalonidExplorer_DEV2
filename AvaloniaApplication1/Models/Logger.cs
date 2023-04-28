using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Targets;
using NLog.Config;
using System.IO;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace AvaloniaApplication1.Models
{
    public class Logger
    {
        private LoggingConfiguration config = new LoggingConfiguration();
        private LoggingRule rule = new LoggingRule("*", LogLevel.Debug, fileTarget);
        public static NLog.Logger logger = LogManager.GetCurrentClassLogger();
        private static int countLogFiles = 0;

        static FileTarget fileTarget = new FileTarget("logfile")
        {
            FileName = "${basedir}/logs/${shortdate}.log",
            Layout = "${longdate} ${level} ${message} ${exception:format=ToString}",
            ArchiveAboveSize = 1073741824L,
            MaxArchiveDays = 7
        };
        public Logger()
        {
            config.AddTarget(fileTarget);
            config.AddRule(rule);
            LogManager.Configuration = config;
        }
    }
}
