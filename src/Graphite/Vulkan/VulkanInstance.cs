global using VkInstance = Silk.NET.Vulkan.Instance;
using Graphite.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanInstance : Instance
{
    private readonly Vk _vk;
    private readonly VkInstance _instance;
    
    public VulkanInstance(ref readonly InstanceInfo info)
    {
        _vk = Vk.GetApi();

        using Utf8String pAppName = info.AppName;
        using Utf8String pEngine = "Graphite";

        ApplicationInfo appInfo = new()
        {
            SType = StructureType.ApplicationInfo,
            PApplicationName = pAppName,
            PEngineName = pEngine,
            ApiVersion = Vk.Version13
        };

        List<string> extensions = [KhrSurface.ExtensionName];

        uint numExtensions;
        _vk.EnumerateInstanceExtensionProperties((byte*) null, &numExtensions, null);
        ExtensionProperties* properties = stackalloc ExtensionProperties[(int) numExtensions];
        _vk.EnumerateInstanceExtensionProperties((byte*) null, &numExtensions, properties);

        for (uint i = 0; i < numExtensions; i++)
        {
            string name = new string((sbyte*) properties[i].ExtensionName);

            switch (name)
            {
                case KhrWin32Surface.ExtensionName:
                case KhrXlibSurface.ExtensionName:
                case KhrXcbSurface.ExtensionName:
                case KhrWaylandSurface.ExtensionName:
                    extensions.Add(name);
                    break;
            }
        }
        
        GraphiteLog.Log($"Using instance extensions: [{string.Join(", ", extensions)}]");

        using Utf8StringArray pExtensions = new Utf8StringArray(extensions);

        InstanceCreateInfo instanceInfo = new()
        {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &appInfo,

            EnabledExtensionCount = pExtensions.Length,
            PpEnabledExtensionNames = pExtensions
        };

        GraphiteLog.Log("Creating instance.");
        _vk.CreateInstance(&instanceInfo, null, out _instance).Check("Create instance");
    }

    public override Adapter[] EnumerateAdapters()
    {
        List<Adapter> adapters = [];
        
        uint numDevices;
        _vk.EnumeratePhysicalDevices(_instance, &numDevices, null);
        PhysicalDevice* devices = stackalloc PhysicalDevice[(int) numDevices];
        _vk.EnumeratePhysicalDevices(_instance, &numDevices, devices);

        for (uint i = 0; i < numDevices; i++)
        {
            PhysicalDevice device = devices[i];
            
            PhysicalDeviceProperties props;
            _vk.GetPhysicalDeviceProperties(device, &props);
            
            // Must support Vulkan 1.3 or above.
            if (props.ApiVersion < Vk.Version13)
                continue;

            string name = new string((sbyte*) props.DeviceName);
            
            adapters.Add(new Adapter(device.Handle, i, name));
        }

        return adapters.ToArray();
    }

    public override void Dispose()
    {
        GraphiteLog.Log("Destroying instance.");
        _vk.DestroyInstance(_instance, null);
        _vk.Dispose();
    }
}