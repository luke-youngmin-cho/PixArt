using System;
using Unity.Mathematics;

internal class flaot3
{
    private float x;
    private float y;
    private float z;

    public flaot3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static implicit operator float3(flaot3 v)
    {
        throw new NotImplementedException();
    }
}