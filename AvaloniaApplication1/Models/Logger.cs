using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Targets;
using NLog.Config;

namespace AvaloniaApplication1.Models
{
    public class Logger
    {
        private LoggingConfiguration config = new LoggingConfiguration();
        private LoggingRule rule = new LoggingRule("*", LogLevel.Debug, fileTarget);
        public static NLog.Logger logger = LogManager.GetCurrentClassLogger();

        static FileTarget fileTarget = new FileTarget("logfile")
        {
            FileName = "${basedir}/logs/${shortdate}.log",
            Layout = "${longdate} ${level} ${message} ${exception:format=ToString}"
            //target name = "file" xsi:type = "File" fileName = "${basedir}/logs/${shortdate}.log"
            //    layout = "${longdate} ${uppercase:${level}} ${message} ${exception:format=ToString}"
        };
        public Logger()
        {
            config.AddTarget(fileTarget);
            config.AddRule(rule);
            LogManager.Configuration = config;
        }
    }
}
