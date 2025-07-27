using System.Drawing;

namespace Graphite.Core;

public struct ColorF
{
    public float R;

    public float G;

    public float B;

    public float A;

    public ColorF(float r, float g, float b, float a = 1.0f)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public ColorF(Color color)
    {
        R = color.R / (float) byte.MaxValue;
        G = color.G / (float) byte.MaxValue;
        B = color.B / (float) byte.MaxValue;
        A = color.A / (float) byte.MaxValue;
    }
}