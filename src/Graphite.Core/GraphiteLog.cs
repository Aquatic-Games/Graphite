using System.Runtime.CompilerServices;
// ReSharper disable ExplicitCallerInfoArgument

namespace Graphite.Core;

public static class GraphiteLog
{
    public static event OnLogMessage LogMessage = delegate { };

    public static void Log(Severity severity, Type type, string message, [CallerLineNumber] int line = 0,
        [CallerFilePath] string file = "")
    {
        LogMessage(severity, type, message, line, file);
    }

    public static void Log(string message, [CallerLineNumber] int line = 0, [CallerFilePath] string file = "")
        => Log(Severity.Verbose, Type.General, message, line, file);

    public enum Severity
    {
        Verbose,
        Info,
        Warning,
        Error
    }

    public enum Type
    {
        Other,
        General,
        Performance,
        Validation
    }

    public delegate void OnLogMessage(Severity severity, Type type, string message, int line, string file);
}