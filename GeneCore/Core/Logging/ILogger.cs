using System;

namespace GeneCore.Core.Logging
{
    /// <summary>
    ///     Interface for a generic logger that is used to log debug
    ///     information from the inner workings.
    /// </summary>
    public interface ILogger
    {
        void Debug(String msg);
        void Info(String msg);

        void Warning(String msg);

        void Error(String msg);

        void Exception(String msg, Exception exception);
    }

    [Flags]
    public enum LogLevels
    {
        Nothing = 0x00
        , DebugInformation = 0x01 << 1
        , FitnessValues = 0x01 << 1
    }
}