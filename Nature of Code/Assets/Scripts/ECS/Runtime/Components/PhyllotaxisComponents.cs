using Unity.Entities;

public struct PhyllotaxisComponents : IComponentData
{
    public Entity prefab;
    public float angle;

    public int n;
    public float c;

    public float spawnDelay;
    public float timer;
}