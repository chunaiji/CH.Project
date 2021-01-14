using System;
using System.Collections.Generic;
using System.Text;

namespace CH.Project.Commont.LogCommont
{
    public interface ILogHelper
    {
        void Info(string data);
        void Error(string data);
        void Debug(string data);
        void Warning(string data);
    }
}
