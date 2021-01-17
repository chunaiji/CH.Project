using CH.Project.Commont.ConfigCommont;
using System;
using System.Collections.Generic;
using System.Text;

namespace CH.Project.Commont.LogCommont
{
    public class LogBuilder : SingleCommont<LogBuilder>
    {
        public static ILogHelper logHelper;

        public LogBuilder()
        {
            var logType = "1";
            logType = ConfigActionCommont.CreateInstance().GetValue("LogSetting:LogType");
            if (logType.ToLower() == "1")
            {
                logHelper = SerilogActionExtention.CreateInstance();
            }
            else if (logType.ToLower() == "2")
            {
                logHelper = NlogActionExtention.CreateInstance();
            }
            else if (logType.ToLower() == "3")
            {
                logHelper = ExceptionlessActionExtention.CreateInstance();
            }
        }

        public void Info(string data)
        {
            logHelper.Info(data);
        }
        public void Error(string data)
        {
            logHelper.Error(data);
        }
        public void Debug(string data)
        {
            logHelper.Debug(data);
        }
        public void Warning(string data)
        {
            logHelper.Warning(data);
        }

        public void Fatal(string data)
        {
            logHelper.Fatal(data);
        }

        public void Trace(string data)
        {
            logHelper.Trace(data);
        }
    }
}
