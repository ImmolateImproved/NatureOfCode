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

public struct SquareWorldBounds : IComponentData
{
    public float2 value;
}

public struct OutOfBoundSteering : IComponentData
{
    public float3 center;
    public float radiusSq;

    public float steeringForce;
}