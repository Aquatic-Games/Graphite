using Graphite;
using Graphite.Core;

GraphiteLog.LogMessage += (severity, type, message, _, _) =>
{
    if (severity == GraphiteLog.Severity.Error)
        throw new Exception(message);

    Console.WriteLine($"{severity} - {type}: {message}");
};

Instance instance = Instance.Create(new InstanceInfo("Graphite.Tests", true));
Console.WriteLine($"Adapters: {string.Join(", ", instance.EnumerateAdapters())}");

instance.Dispose();