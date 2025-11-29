using Graphite;
using Graphite.Vulkan;

Instance.LogMessage += (severity, message, line, path) => Console.WriteLine($"[{severity}] {message}"); 

InstanceInfo instanceInfo = new InstanceInfo("Graphite.Vulkan.Tests");
Instance instance = new VulkanInstance(in instanceInfo);

Adapter[] adapters = instance.EnumerateAdapters();
foreach (Adapter adapter in adapters)
    Console.WriteLine(adapter);

instance.Dispose();