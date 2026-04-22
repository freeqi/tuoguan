using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cqgis.ui.Core;
using Microsoft.Extensions.Logging;

namespace SimpleLogger.FileLog
{
    internal sealed class FileLoggerProvider : ILoggerProvider
    {
        private readonly string _basepath;
        private readonly Func<string, LogLevel, bool> _filter;

        private readonly DoubleBufferedQueue<LogIndex> _logWriter = new DoubleBufferedQueue<LogIndex>(3000); //三秒一写入

        public FileLoggerProvider(string basepath, Func<string, LogLevel, bool> filter = null)
        {
            _basepath = basepath;
            this._filter = filter;

            _logWriter.ConsumerAction = WriteToTxtFile;
        }

        private void WriteToTxtFile(Queue<LogIndex> logs)
        {
            var time = DateTime.Now;
            var folder = Path.Combine(_basepath, time.ToString("yyyy_MM_dd"));
            var path = Path.GetFullPath(folder);
            Directory.CreateDirectory(path);

            List<LogIndex> ls = new List<LogIndex>();
            while (logs.Count > 0)
            {
                var item = logs.Dequeue();
                ls.Add(item);
            }

            foreach (var g in ls.GroupBy(t => t.logLevel))
            {
                var filename = $"{g.Key}.txt";

                var txtfile = Path.Combine(folder, filename);

                using (FileStream fs = new FileStream(txtfile, FileMode.Append, FileAccess.Write,FileShare.ReadWrite))
                {
                    using (StreamWriter sr = new StreamWriter(fs))
                    {
                        foreach (var logIndex in g)
                        {
                            sr.WriteLine(logIndex.message);
                        }
                        sr.Flush();
                    }
                }
            }
        }

        public void Dispose()
        {
            _logWriter?.Dispose();
        }

        private void WriteLog(string message, LogLevel logLevel)
        {
            _logWriter.Equeue(new LogIndex()
            {
                logLevel = logLevel,
                message = message,
            });
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(_basepath, categoryName, WriteLog, _filter);
        }


        private class LogIndex
        {
            public LogLevel logLevel { get; set; }

            public string message { get; set; }
        }
    }
}
