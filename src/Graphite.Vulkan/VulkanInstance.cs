using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Graphite.Vulkan;

public unsafe class VulkanInstance : Instance
{
    /// <inheritdoc />
    public override bool IsDisposed { get; protected set; }

    private readonly Vk _vk;
    private readonly VkInstance _instance;

    public VulkanInstance(in InstanceInfo info)
    {
        _vk = Vk.GetApi();
        Instance.Log($"InstanceInfo: {info}");

        uint availableVersion;
        _vk.EnumerateInstanceVersion(&availableVersion).Check("Enumerate instance version");

        if (availableVersion < Vk.Version13)
        {
            Version32 version = (Version32) availableVersion;
            throw new PlatformNotSupportedException(
                $"Vulkan 1.3 not supported. (Available version: {version.Major}.{version.Minor}.)");
        }

        nint pAppName = SilkMarshal.StringToPtr(info.AppName);
        nint pEngineName = SilkMarshal.StringToPtr("Graphite");

        ApplicationInfo appInfo = new()
        {
            SType = StructureType.ApplicationInfo,
            PApplicationName = (byte*) pAppName,
            PEngineName = (byte*) pEngineName,
            ApiVersion = Vk.Version13
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