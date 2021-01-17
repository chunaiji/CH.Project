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
            var logType = "Serilog";
            logType = ConfigActionCommont.CreateInstance().GetValue("LogSetting:LogType");
            if (logType.ToLower() == "Serilog".ToLower())
            {
                logHelper = SerilogActionExtention.CreateInstance();
            }
            if (logType.ToLower() == "nlog".ToLower())
            {
                logHelper = NlogActionExtention.CreateInstance();
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
