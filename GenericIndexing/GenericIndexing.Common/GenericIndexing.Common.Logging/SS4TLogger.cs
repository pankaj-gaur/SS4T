using System;
using log4net;
using log4net.Config;
using System.Configuration;

namespace GenericIndexing.Common.Logging
{
    public enum ELogLevel
    {
        DEBUG = 1,
        ERROR,
        FATAL,
        INFO,
        WARN
    }
    public static class SS4TLogger
    {
        #region Members
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SS4TLogger));
        #endregion

        #region Constructors
        static SS4TLogger()
        {
            
            
            string LOGFILECONFIG = ConfigurationManager.AppSettings["LoggingConfigPath"];

            System.IO.FileInfo config = new System.IO.FileInfo(LOGFILECONFIG);
            XmlConfigurator.Configure(config);           
        }
        #endregion

        #region Methods
        public static void WriteLog(ELogLevel logLevel, String log)
        {

            if (SS4TLogger.Logger.IsDebugEnabled && logLevel.Equals(ELogLevel.DEBUG))
            {
                
                Logger.Debug(log);

            }

            else if (SS4TLogger.Logger.IsErrorEnabled && logLevel.Equals(ELogLevel.ERROR))
            {

                Logger.Error(log);

            }

            else if (SS4TLogger.Logger.IsFatalEnabled && logLevel.Equals(ELogLevel.FATAL))
            {

                Logger.Fatal(log);

            }

            else if (SS4TLogger.Logger.IsInfoEnabled && logLevel.Equals(ELogLevel.INFO))
            {

                Logger.Info(log);

            }

            else if (logLevel.Equals(ELogLevel.WARN))
            {
                Logger.Warn(log);
            }
        }

        #endregion
    } 
}
