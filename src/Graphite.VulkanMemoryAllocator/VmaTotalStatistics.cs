using Silk.NET.Vulkan;
using System;
using System.Runtime.InteropServices;

namespace Graphite.VulkanMemoryAllocator;

public partial struct VmaTotalStatistics
{
    [NativeTypeName("VmaDetailedStatistics[32]")]
    public _memoryType_e__FixedBuffer memoryType;

    [NativeTypeName("VmaDetailedStatistics[16]")]
    public _memoryHeap_e__FixedBuffer memoryHeap;

    public VmaDetailedStatistics total;

    public partial struct _memoryType_e__FixedBuffer
    {
        public VmaDetailedStatistics e0;
        public VmaDetailedStatistics e1;
        public VmaDetailedStatistics e2;
        public VmaDetailedStatistics e3;
        public VmaDetailedStatistics e4;
        public VmaDetailedStatistics e5;
        public VmaDetailedStatistics e6;
        public VmaDetailedStatistics e7;
        public VmaDetailedStatistics e8;
        public VmaDetailedStatistics e9;
        public VmaDetailedStatistics e10;
        public VmaDetailedStatistics e11;
        public VmaDetailedStatistics e12;
        public VmaDetailedStatistics e13;
        public VmaDetailedStatistics e14;
        public VmaDetailedStatistics e15;
        public VmaDetailedStatistics e16;
        public VmaDetailedStatistics e17;
        public VmaDetailedStatistics e18;
        public VmaDetailedStatistics e19;
        public VmaDetailedStatistics e20;
        public VmaDetailedStatistics e21;
        public VmaDetailedStatistics e22;
        public VmaDetailedStatistics e23;
        public VmaDetailedStatistics e24;
        public VmaDetailedStatistics e25;
        public VmaDetailedStatistics e26;
        public VmaDetailedStatistics e27;
        public VmaDetailedStatistics e28;
        public VmaDetailedStatistics e29;
        public VmaDetailedStatistics e30;
        public VmaDetailedStatistics e31;

        public ref VmaDetailedStatistics this[int index]
        {
            get
            {
                return ref AsSpan()[index];
            }
        }

        public Span<VmaDetailedStatistics> AsSpan() => MemoryMarshal.CreateSpan(ref e0, 32);
    }

    public partial struct _memoryHeap_e__FixedBuffer
    {
        public VmaDetailedStatistics e0;
        public VmaDetailedStatistics e1;
        public VmaDetailedStatistics e2;
        public VmaDetailedStatistics e3;
        public VmaDetailedStatistics e4;
        public VmaDetailedStatistics e5;
        public VmaDetailedStatistics e6;
        public VmaDetailedStatistics e7;
        public VmaDetailedStatistics e8;
        public VmaDetailedStatistics e9;
        public VmaDetailedStatistics e10;
        public VmaDetailedStatistics e11;
        public VmaDetailedStatistics e12;
        public VmaDetailedStatistics e13;
        public VmaDetailedStatistics e14;
        public VmaDetailedStatistics e15;

        public ref VmaDetailedStatistics this[int index]
        {
            get
            {
                return ref AsSpan()[index];
            }
        }

        public Span<VmaDetailedStatistics> AsSpan() => MemoryMarshal.CreateSpan(ref e0, 16);
    }
}
