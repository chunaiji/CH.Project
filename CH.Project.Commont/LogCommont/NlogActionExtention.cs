using System;
using System.Collections.Generic;
using System.Text;

namespace CH.Project.Commont.LogCommont
{
    public class NlogActionExtention : SingleCommont<NlogActionExtention>, ILogHelper
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public void Trace(string strMsg)
        {
            _logger.Trace(strMsg);
        }

        public void Debug(string strMsg)
        {
            _logger.Debug(strMsg);
        }

        public void Info(string strMsg)
        {
            _logger.Info(strMsg);
        }

        public void Warning(string strMsg)
        {
            _logger.Warn(strMsg);
        }

        public void Error(string strMsg)
        {
            _logger.Error(strMsg);
        }

        public void Fatal(string strMsg)
        {
            _logger.Fatal(strMsg);
        }
    }
}
