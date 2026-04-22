using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace SimpleLogger.FileLog
{
    public static class FileLoggerExtensions
    {
        public static ILoggerFactory AddFileLogger(this ILoggerFactory factory, string basepath, Func<string, LogLevel, bool> filter)
        {
            factory.AddProvider(new FileLoggerProvider(basepath, filter));
            return factory;
        }

        public static ILoggerFactory AddFileLogger(this ILoggerFactory factory, string basepath, LogLevel minLevel = LogLevel.Information)
        {
            return AddFileLogger(
               factory,
               basepath,
               (_, logLevel) => logLevel >= minLevel);
        }
    }
}
