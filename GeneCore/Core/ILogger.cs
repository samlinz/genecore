using System;

namespace GeneCore.Core {
    /// <summary>
    /// Interface for a generic logger that is used to log debug
    /// information from the inner workings.
    /// </summary>
    public interface ILogger {
        void Info(String msg);

        void Warning(String msg);

        void Error(String msg);

        void Exception(String msg, Exception exception);
    }

    /// <summary>
    /// Default logger which logs the messages into STDOUT and STDERR.
    /// </summary>
    public class SimpleConsoleLogger : ILogger {
        private readonly String _prefix;
        private static readonly String _prefixLevelPlaceholder = "$LEVEL";

        public SimpleConsoleLogger(Boolean usePrefix = true) {
            _prefix = usePrefix ? $"{_prefixLevelPlaceholder}: " : "";
        }

        private String GetPrefix(String level) => _prefix?.Replace(_prefixLevelPlaceholder, level);

        public void Info(String msg) {
            Console.WriteLine($"{GetPrefix("INFO")}{msg}");
        }

        public void Warning(String msg) {
            Console.WriteLine($"{GetPrefix("WARN")}{msg}");
        }

        public void Error(String msg) {
            Console.Error.WriteLine($"{GetPrefix("ERR")}{msg}");
        }

        public void Exception(String msg, Exception exception) {
            Console.Error.WriteLine($"{GetPrefix("ERR")}{msg}");

            if (!String.IsNullOrEmpty(exception?.Message)) {
                Console.Error.WriteLine($"{GetPrefix("EXCEPTION")}{exception?.Message}");
            }

            if (!String.IsNullOrEmpty(exception?.StackTrace)) {
                Console.Error.WriteLine($"{GetPrefix("STACKTRACE")}{exception?.StackTrace}");
            }
        }
    }

    [Flags]
    public enum LogLevels {
        Nothing = 0x00,
        DebugInformation = 0x01 << 1,
    }
}