using Unity.Entities;

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