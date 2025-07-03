global using VkInstance = Silk.NET.Vulkan.Instance;
using Graphite.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Graphite.Vulkan;

internal sealed unsafe class VulkanInstance : Instance
{
    private readonly Vk _vk;
    private readonly VkInstance _instance;

    private readonly ExtDebugUtils? _debugUtilsExt;
    private readonly DebugUtilsMessengerEXT _debugMessenger;
    
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
        List<string> layers = [];

        bool debug = false;

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
                
                // Only enable debugging if the debug extension is present.
                // TODO: Throw custom exception if debug layers not found instead of ignoring.
                case ExtDebugUtils.ExtensionName when info.Debug:
                    extensions.Add(ExtDebugUtils.ExtensionName);
                    debug = true;
                    layers.Add("VK_LAYER_KHRONOS_validation");
                    break;
            }
        }
        
        GraphiteLog.Log($"Using instance extensions: [{string.Join(", ", extensions)}]");
        GraphiteLog.Log($"Using layers: [{string.Join(", ", layers)}]");

        using Utf8StringArray pExtensions = new Utf8StringArray(extensions);
        using Utf8StringArray pLayers = new Utf8StringArray(layers);

        InstanceCreateInfo instanceInfo = new()
        {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &appInfo,

            EnabledExtensionCount = pExtensions.Length,
            PpEnabledExtensionNames = pExtensions,
            
            EnabledLayerCount = pLayers.Length,
            PpEnabledLayerNames = pLayers
        };

        GraphiteLog.Log("Creating instance.");
        _vk.CreateInstance(&instanceInfo, null, out _instance).Check("Create instance");

        if (debug)
        {
            if (!_vk.TryGetInstanceExtension(_instance, out _debugUtilsExt))
                throw new Exception("Debug utils extension not found.");

            DebugUtilsMessengerCreateInfoEXT messengerInfo = new()
            {
                SType = StructureType.DebugUtilsMessengerCreateInfoExt,
                MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt |
                                  DebugUtilsMessageSeverityFlagsEXT.InfoBitExt |
                                  DebugUtilsMessageSeverityFlagsEXT.WarningBitExt |
                                  DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt,
                MessageType = DebugUtilsMessageTypeFlagsEXT.GeneralBitExt |
                              DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt |
                              DebugUtilsMessageTypeFlagsEXT.ValidationBitExt,
                PfnUserCallback = new PfnDebugUtilsMessengerCallbackEXT(DebugMessage)
            };

            _debugUtilsExt!.CreateDebugUtilsMessenger(_instance, &messengerInfo, null, out _debugMessenger)
                .Check("Create debug messenger");
        }
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
        if (_debugUtilsExt != null)
        {
            GraphiteLog.Log("Destroying debug messenger.");
            _debugUtilsExt.DestroyDebugUtilsMessenger(_instance, _debugMessenger, null);
            _debugUtilsExt.Dispose();
        }
        
        GraphiteLog.Log("Destroying instance.");
        _vk.DestroyInstance(_instance, null);
        _vk.Dispose();
    }
    
    private static uint DebugMessage(DebugUtilsMessageSeverityFlagsEXT messageSeverity,
        DebugUtilsMessageTypeFlagsEXT messageTypes, DebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData)
    {
        string message = new string((sbyte*) pCallbackData->PMessage);

        GraphiteLog.Severity severity = messageSeverity switch
        {
            DebugUtilsMessageSeverityFlagsEXT.None => GraphiteLog.Severity.Verbose,
            DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt => GraphiteLog.Severity.Verbose,
            DebugUtilsMessageSeverityFlagsEXT.InfoBitExt => GraphiteLog.Severity.Info,
            DebugUtilsMessageSeverityFlagsEXT.WarningBitExt => GraphiteLog.Severity.Warning,
            DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt => GraphiteLog.Severity.Error,
            _ => throw new ArgumentOutOfRangeException(nameof(messageSeverity), messageSeverity, null)
        };

        GraphiteLog.Type type = messageTypes switch
        {
            DebugUtilsMessageTypeFlagsEXT.None => GraphiteLog.Type.Other,
            DebugUtilsMessageTypeFlagsEXT.GeneralBitExt => GraphiteLog.Type.General,
            DebugUtilsMessageTypeFlagsEXT.ValidationBitExt => GraphiteLog.Type.Validation,
            DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt => GraphiteLog.Type.Performance,
            DebugUtilsMessageTypeFlagsEXT.DeviceAddressBindingBitExt => GraphiteLog.Type.Other,
            _ => throw new ArgumentOutOfRangeException(nameof(messageTypes), messageTypes, null)
        };

        GraphiteLog.Log(severity, type, message);
        
        return Vk.True;
    }
}