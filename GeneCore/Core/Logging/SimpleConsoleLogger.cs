using System;

namespace GeneCore.Core.Logging
{
    /// <summary>
    ///     Default logger which logs the messages into STDOUT and STDERR.
    /// </summary>
    public class SimpleConsoleLogger : ILogger
    {
        private static readonly String _prefixLevelPlaceholder = "$LEVEL";
        private readonly Boolean _logDebug;
        private readonly String _prefix;

        public SimpleConsoleLogger(Boolean usePrefix = true, Boolean logDebug = false)
        {
            _logDebug = logDebug;
            _prefix = usePrefix ? $"{_prefixLevelPlaceholder}: " : "";
        }

        public void Debug(String msg)
        {
            if (_logDebug) Console.WriteLine($"{GetPrefix("DEBUG")}{msg}");
        }

        public void Info(String msg)
        {
            Console.WriteLine($"{GetPrefix("INFO")}{msg}");
        }

        public void Warning(String msg)
        {
            Console.WriteLine($"{GetPrefix("WARN")}{msg}");
        }

        public void Error(String msg)
        {
            Console.Error.WriteLine($"{GetPrefix("ERR")}{msg}");
        }

        public void Exception(String msg, Exception exception)
        {
            Console.Error.WriteLine($"{GetPrefix("ERR")}{msg}");

            if (!string.IsNullOrEmpty(exception?.Message))
                Console.Error.WriteLine($"{GetPrefix("EXCEPTION")}{exception?.Message}");

            if (!string.IsNullOrEmpty(exception?.StackTrace))
                Console.Error.WriteLine($"{GetPrefix("STACKTRACE")}{exception?.StackTrace}");
        }

        private String GetPrefix(String level)
        {
            return _prefix?.Replace(_prefixLevelPlaceholder, level);
        }
    }
}