using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using log4net.Config;

namespace ksWebSocketServer.Utils
{
    public class Logger
    {
        public static string GetCodeBasePath(Type type)
        {
            string codebase = type.Assembly.CodeBase;
            int endIndex = codebase.LastIndexOf("/", StringComparison.Ordinal);
            int startIndex = codebase.LastIndexOf(":", StringComparison.Ordinal);
            string tempPath = codebase.Substring(startIndex - 1, endIndex - startIndex + 1);
            string path = tempPath.Replace("/", "\\") + "\\";
            return path;
        }

        #region Location Information

        private class LocationInfo
        {
            private readonly string _filename;
            private readonly string _methodName;
            private readonly int _lineNumber;

            public LocationInfo(string filename, int lineNumber, string methodName)
            {
                _filename = filename;
                _lineNumber = lineNumber;
                _methodName = methodName;
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.AppendFormat("file: {0}, line: {1}, func: {2}", _filename, _lineNumber, _methodName);
                return sb.ToString();
            }
        }

        #endregion

        private static Logger _logger;
        public static Logger Instance()
        {
            if (_logger == null)
            {
                _logger = new Logger();

                var sb = new StringBuilder();
                sb.AppendFormat("{0}\\{1}", GetCodeBasePath(typeof(Logger)), "log4netConfig.xml");
                _logger.SetConfigFilePath(sb.ToString());
            }

            return _logger;
        }

        private Logger()
        {
        }

        private readonly log4net.ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// set logConfigPath 
        /// </summary>
        /// <param name="path"></param>
        public void SetConfigFilePath(string path)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo(path));
        }

        /// <summary>
        /// out put debug level log
        /// </summary>
        /// <param name="message"></param>
        public void Debug(object message)
        {
#if DEBUG
            DebugWithFormat("{0}\r\nMsg：{1}", GetLocationInfo(), message);
#endif
        }

        /// <summary>
        /// out put debug level log with format
        /// </summary>
        /// <param name="format">format string</param>
        /// <param name="args">object array</param>
        public void DebugWithFormat(string format, params object[] args)
        {
#if DEBUG
            var info = args[0] as LocationInfo;
            if (info != null)
            {
                _log.DebugFormat(format, args);
            }
            else
            {
                _log.DebugFormat(GetLocationInfo() + "\r\nMsg: " + format, args);
            }
#endif
        }

        /// <summary>
        /// out put info level log
        /// </summary>
        /// <param name="message"></param>
        public void Info(object message)
        {
            InfoWithFormat("{0}\r\nMsg: {1}", GetLocationInfo(), message);
        }

        /// <summary>
        /// out put info level log with format
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void InfoWithFormat(string format, params object[] args)
        {
            var info = args[0] as LocationInfo;
            if (info != null)
            {
                _log.InfoFormat(format, args);
            }
            else
            {
                _log.InfoFormat(GetLocationInfo() + "\r\nMsg: " + format, args);
            }
        }

        public void Warn(object message)
        {
            WarnWithFormat("{0}\r\nMsg: {1}", GetLocationInfo(), message);
        }

        public void WarnWithFormat(string format, params object[] args)
        {
            var info = args[0] as LocationInfo;
            if (info != null)
            {
                _log.WarnFormat(format, args);
            }
            else
            {
                _log.WarnFormat(GetLocationInfo() + "\r\nMsg: " + format, args);
            }
        }

        public void Error(object message)
        {
            ErrorWithFormat("{0}\r\nMsg: {1}", GetLocationInfo(), message);
        }

        public void ErrorWithFormat(string format, params object[] args)
        {
            var info = args[0] as LocationInfo;
            if (info != null)
            {
                _log.ErrorFormat(format, args);
            }
            else
            {
                _log.ErrorFormat(GetLocationInfo() + "\r\nMsg: " + format, args);
            }
        }

        public void Fatal(object message)
        {
            FatalWithFormat("{0}\r\nMsg: {1}", GetLocationInfo(), message);
        }

        public void FatalWithFormat(string format, params object[] args)
        {
            var info = args[0] as LocationInfo;
            if (info != null)
            {
                _log.FatalFormat(format, args);
            }
            else
            {
                _log.FatalFormat(GetLocationInfo() + "\r\nMsg: " + format, args);
            }
        }

        #region Inner Method
        private LocationInfo GetLocationInfo()
        {
            //debug and release version's StackTrace is difference.
#if DEBUG
            StackFrame sf = (new StackTrace(true)).GetFrame(3);
#else
            StackFrame sf = (new System.Diagnostics.StackTrace(true)).GetFrame(2);
#endif
            return new LocationInfo(Path.GetFileName(sf.GetFileName()), sf.GetFileLineNumber(), sf.GetMethod().Name);
        }

        #endregion

    }
}