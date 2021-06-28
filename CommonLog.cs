using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NLog;

namespace TcpChecker
{
    public class CommonLog
    {
        static Logger m_Logger = null;

        public static void LoadLogger()
        {
            if (m_Logger == null) m_Logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
        }

        public static void Info(string msg)
        {
            if (m_Logger != null) m_Logger.Info(msg);
        }

        public static void Debug(string msg)
        {
            if (m_Logger != null) m_Logger.Debug(msg);
        }

        public static void Warn(string msg)
        {
            if (m_Logger != null) m_Logger.Warn(msg);
        }

        public static void Error(string msg)
        {
            if (m_Logger != null) m_Logger.Error(msg);
        }
    }
}
