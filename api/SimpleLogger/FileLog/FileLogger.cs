using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace SimpleLogger.FileLog
{
    internal sealed class FileLogger : ILogger
    {
        private readonly string _name ;
       
        private readonly Action<string,LogLevel> _writeline;
        private readonly Func<string, LogLevel, bool> _filter ;
         

        public FileLogger(string basepath, string name, Action<string,LogLevel> writeline, Func<string, LogLevel, bool> filter = null )
        {
            _name = string.IsNullOrEmpty(name) ? "default" : name;
           
            _writeline = writeline;
            _filter = filter;

         
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            var time = DateTime.Now; 

            message = $"{time:yyyy-MM-dd HH:mm:ss fff}\t {_name}\t {message}";
            if (logLevel > LogLevel.Warning)
                message += "\r\n==============================================================";
            _writeline?.Invoke(message,logLevel);

        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (_filter == null || _filter(_name, logLevel));
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new NoopDisposable();
        }
 

        private class NoopDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
