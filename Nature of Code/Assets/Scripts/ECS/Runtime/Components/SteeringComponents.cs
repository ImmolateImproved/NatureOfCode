using Unity.Entities;
using Unity.Mathematics;

public struct SteeringForce : IComponentData
{
    public float3 value;
}

public struct SteeringData : IBufferElementData
{
    public Entity target;
    public float maxForce;
    public float predictionAmount;
    public float slowRadius;
    public float seekOrFlee;
}

public struct TargetSeeker : IBufferElementData
{
    public float searchRadius;
}

public enum TargetTypeEnum
{
    Food, Poison
}

public struct TargetType : IComponentData
{
    public TargetTypeEnum value;
}