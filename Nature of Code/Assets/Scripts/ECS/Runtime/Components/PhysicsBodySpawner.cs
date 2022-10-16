using Unity.Entities;

public struct PhysicsBodySpawner : IComponentData
{
    public Entity prefab;
    public int count;

    public float startSpeed;
    public float minRadius;
    public float maxRadius;
}