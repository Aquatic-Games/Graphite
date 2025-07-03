using Graphite;
using Graphite.Core;

GraphiteLog.LogMessage += (severity, message, _, _) => Console.WriteLine($"{severity}: {message}"); 

Instance instance = Instance.Create(new InstanceInfo("Graphite.Tests", true));
Console.WriteLine($"Adapters: {string.Join(", ", instance.EnumerateAdapters())}");

instance.Dispose();