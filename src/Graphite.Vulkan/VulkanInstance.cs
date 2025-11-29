using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Graphite.Vulkan;

public sealed unsafe class VulkanInstance : Instance
{
    private static readonly Version32 Version = Vk.Version13;
    
    /// <inheritdoc />
    public override bool IsDisposed { get; protected set; }

    private readonly Vk _vk;
    private readonly VkInstance _instance;

    public VulkanInstance(in InstanceInfo info)
    {
        _vk = Vk.GetApi();
        Instance.Log($"InstanceInfo: {info}");

        uint availableVersionInt;
        _vk.EnumerateInstanceVersion(&availableVersionInt).Check("Enumerate instance version");
        Version32 availableVersion = (Version32) availableVersionInt;
        Instance.Log($"Available Vulkan version: {availableVersion.Major}.{availableVersion.Minor}");
        
        if (availableVersion < Version)
        {
            throw new PlatformNotSupportedException(
                $"Vulkan 1.3 not supported. (Available version: {availableVersion.Major}.{availableVersion.Minor})");
        }

        nint pAppName = SilkMarshal.StringToPtr(info.AppName);
        nint pEngineName = SilkMarshal.StringToPtr("Graphite");

        ApplicationInfo appInfo = new()
        {
            SType = StructureType.ApplicationInfo,
            PApplicationName = (byte*) pAppName,
            PEngineName = (byte*) pEngineName,
            ApiVersion = Version
        };

        // KHR_surface is basically guaranteed to be supported by all Vulkan implementations.
        // If it isn't supported for some reason, there's absolutely nothing we can do anyway.
        List<string> instanceExtensions = [KhrSurface.ExtensionName];

        uint numExtensionsProps;
        _vk.EnumerateInstanceExtensionProperties((byte*) null, &numExtensionsProps, null);
        ExtensionProperties* extensionProps = stackalloc ExtensionProperties[(int) numExtensionsProps];
        _vk.EnumerateInstanceExtensionProperties((byte*) null, &numExtensionsProps, extensionProps);

        for (uint i = 0; i < numExtensionsProps; i++)
        {
            string extName = new string((sbyte*) extensionProps[i].ExtensionName);

            switch (extName)
            {
                case KhrWin32Surface.ExtensionName:
                case KhrWaylandSurface.ExtensionName:
                case KhrXlibSurface.ExtensionName:
                case KhrXcbSurface.ExtensionName:
                    instanceExtensions.Add(extName);
                    break;
            }
        }
        
        Instance.Log($"Instance extensions: [{string.Join(", ", instanceExtensions)}]");
        nint pInstanceExtensions = SilkMarshal.StringArrayToPtr(instanceExtensions);

        InstanceCreateInfo instanceInfo = new()
        {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &appInfo,
            
            EnabledExtensionCount = (uint) instanceExtensions.Count,
            PpEnabledExtensionNames = (byte**) pInstanceExtensions
        };
        
        Instance.Log("Creating instance.");
        _vk.CreateInstance(&instanceInfo, null, out _instance).Check("Create instance");

        SilkMarshal.Free(pInstanceExtensions);
        SilkMarshal.FreeString(pEngineName);
        SilkMarshal.FreeString(pAppName);
    }

    public override Adapter[] EnumerateAdapters()
    {
        uint numPhysicalDevices;
        _vk.EnumeratePhysicalDevices(_instance, &numPhysicalDevices, null);
        PhysicalDevice* physicalDevices = stackalloc PhysicalDevice[(int) numPhysicalDevices];
        _vk.EnumeratePhysicalDevices(_instance, &numPhysicalDevices, physicalDevices);

        List<Adapter> adapters = [];
        for (uint i = 0; i < numPhysicalDevices; i++)
        {
            PhysicalDevice device = physicalDevices[i];
            
            PhysicalDeviceProperties properties;
            _vk.GetPhysicalDeviceProperties(device, &properties);
            
            if (properties.ApiVersion < Version)
                continue;

            string name = new string((sbyte*) properties.DeviceName);
            
            adapters.Add(new Adapter(device.Handle, i, name));
        }

        return adapters.ToArray();
    }

    public override Device CreateDevice(Surface surface, Adapter? adapter = null)
    {
        if (adapter is not { } adapterToUse)
        {
            Adapter[] adapters = EnumerateAdapters();

            if (adapters.Length == 0)
                throw new PlatformNotSupportedException("No supported adapters.");
            
            adapterToUse = adapters[0];
        }
        
        Instance.Log(LogSeverity.Info, $"Using adapter: {adapterToUse}");

        PhysicalDevice physicalDevice = new PhysicalDevice(adapterToUse.Handle);
        return new VulkanDevice(_vk, _instance, (VulkanSurface) surface, physicalDevice);
    }

    /// <inheritdoc />
    public override Surface CreateSurface(in SurfaceInfo info)
    {
        return new VulkanSurface(_vk, _instance, in info);
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        
        _vk.DestroyInstance(_instance, null);
        _vk.Dispose();
    }
}