using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct UnitInitialized : IComponentData
{

}

public struct MousePosition : IComponentData
{
    public float2 value;
}

public struct HoverColor : IComponentData
{
    public Color value;
}

public struct WorldBoundaries : IComponentData
{
    public float2 value;
}