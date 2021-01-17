using Exceptionless;
using Exceptionless.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CH.Project.Commont.LogCommont
{
    public class ExceptionlessActionExtention : SingleCommont<ExceptionlessActionExtention>, ILogHelper
    {
        public ExceptionlessActionExtention()
        {
            ExceptionlessClient.Default.Configuration.ApiKey = "lJSj6SwpmA9Wih3AFkhq7AoIchtPZyZEimNQ3eCL";
            ExceptionlessClient.Default.Configuration.ServerUrl = "http://localhost:50001";
            ExceptionlessClient.Default.Startup();
        }

        public void Trace(string strMsg)
        {
            ExceptionlessClient.Default.SubmitLog(strMsg, LogLevel.Trace);
        }

        public void Debug(string strMsg)
        {
            ExceptionlessClient.Default.SubmitLog(strMsg, LogLevel.Debug);
        }

        public void Info(string strMsg)
        {
            ExceptionlessClient.Default.SubmitLog(strMsg, LogLevel.Info);
        }

        public void Warning(string strMsg)
        {
            ExceptionlessClient.Default.SubmitLog(strMsg, LogLevel.Warn);
        }

        public void Error(string strMsg)
        {
            ExceptionlessClient.Default.SubmitLog(strMsg, LogLevel.Error);
        }

        public void Fatal(string strMsg)
        {
            ExceptionlessClient.Default.SubmitLog(strMsg, LogLevel.Fatal);
        }
    }
}
