# Graphite Framework
The Graphite Framework, a modern, cross-platform .NET 9 framework for graphics and compute.

## Graphite RHI
An expandable Render Hardware Interface (RHI), supporting the following APIs:

* Vulkan 1.3
* ~~DirectX 12~~ (Planned, not yet supported)
* DirectX 11.1
* ~~OpenGL 4.3/ES 3.0~~ (Planned, not yet supported)

#### Vulkan Support
Graphite uses many features of Vulkan 1.3 and therefore requires it to be supported. It also requires the following
extensions:

* `VK_KHR_swapchain`
* `VK_KHR_push_descriptor` (This will be made optional and can be checked through the `AdapterSupports` structure.)