using System;

namespace Lib
{
    public class Logger
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Logger));

        static Logger()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        public static void LogInfo(string info)
        {
            logger.Info(info);
        }
        public static void LogError(string error, Exception ex)
        {
            logger.Error(error, ex);
        }
    }
}
