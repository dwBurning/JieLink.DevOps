using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
namespace PartialViewInterface.Utils
{
    public class LogHelper
    {
        public static readonly NLogWrapper CommLogger = new NLogWrapper("CommLogger");
        public static readonly NLogWrapper XmppLogger = new NLogWrapper("XmppLogger");

    }
    public class NLogWrapper
    {
        static NLogWrapper()
        {
            LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nLog-config.xml"));
        }
        private Logger logger = null;
        public NLogWrapper(string name)
        {
            logger = LogManager.GetLogger(name);
        }
        public void Info(string msg)
        {
            logger.Info(msg);
        }
        public void Info(string msg, string args)
        {
            logger.Info(msg, args);
        }
        public void Debug(string msg)
        {
            logger.Debug(msg);
        }
        public void Debug(string msg, string args)
        {
            logger.Debug(msg, args);
        }
        public void Error(string msg)
        {
            logger.Error(msg);
        }
        public void Error(string msg, string args)
        {
            logger.Error(msg, args);
        }
        public void Error(Exception exception, string message)
        {
            logger.Error(exception, message);
        }
    }
}
