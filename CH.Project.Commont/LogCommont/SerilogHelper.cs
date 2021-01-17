using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace CH.Project.Commont.LogCommont
{
    public class SerilogHelper : SingleCommont<SerilogHelper>, ILogHelper
    {
        private static Logger LoggerInstantiation { get; set; }

        public SerilogHelper()
        {
            if (LoggerInstantiation == null)
            {
                var fileSizeLimitBytes = 100 * 1024 * 1024;// 1024;// 100 * 1024 * 1024;//单个文件大小
                var retainedFileCountLimit = 100;
                LoggerInstantiation = new LoggerConfiguration().Enrich.FromLogContext()
.WriteTo.Console()
.MinimumLevel.Debug() // 所有Sink的最小记录级别
.WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Debug).WriteTo.File(LogFilePath("Debug"), rollingInterval: RollingInterval.Day, outputTemplate: SerilogOutputTemplate, rollOnFileSizeLimit: true, fileSizeLimitBytes: fileSizeLimitBytes, retainedFileCountLimit: retainedFileCountLimit))
.WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Information).WriteTo.File(LogFilePath("Information"), rollingInterval: RollingInterval.Day, outputTemplate: SerilogOutputTemplate, rollOnFileSizeLimit: true, fileSizeLimitBytes: fileSizeLimitBytes, retainedFileCountLimit: retainedFileCountLimit))
.WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Warning).WriteTo.File(LogFilePath("Warning"), rollingInterval: RollingInterval.Day, outputTemplate: SerilogOutputTemplate, rollOnFileSizeLimit: true, fileSizeLimitBytes: fileSizeLimitBytes, retainedFileCountLimit: retainedFileCountLimit))
.WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Error).WriteTo.File(LogFilePath("Error"), rollingInterval: RollingInterval.Day, outputTemplate: SerilogOutputTemplate, rollOnFileSizeLimit: true, fileSizeLimitBytes: fileSizeLimitBytes, retainedFileCountLimit: retainedFileCountLimit))
.WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Fatal).WriteTo.File(LogFilePath("Fatal"), rollingInterval: RollingInterval.Day, outputTemplate: SerilogOutputTemplate, rollOnFileSizeLimit: true, fileSizeLimitBytes: fileSizeLimitBytes, retainedFileCountLimit: retainedFileCountLimit))
.CreateLogger();
            }
        }

        private string LogFilePath(string LogEvent)
        {
            return $@"{AppContext.BaseDirectory}Logs\{DateTime.Now.ToString("yyyy-MM-dd")}\{LogEvent}\log.log";
        }

        private string SerilogOutputTemplate = "{NewLine}{NewLine}【时间】：{Timestamp:yyyy-MM-dd HH:mm:ss.fff}【线程】{ThreadId}{NewLine}【日志等级】：{Level}{NewLine}Message：{Message}{NewLine}{Exception}"
            + new string('-', 50);

        public void Info(string data)
        {
            LoggerInstantiation.Information(data);
        }

        public void Error(string data)
        {
            LoggerInstantiation.Error(data);
        }

        public void Debug(string data)
        {
            LoggerInstantiation.Debug(data);
        }

        public void Warning(string data)
        {
            LoggerInstantiation.Warning(data);
        }
    }
}
