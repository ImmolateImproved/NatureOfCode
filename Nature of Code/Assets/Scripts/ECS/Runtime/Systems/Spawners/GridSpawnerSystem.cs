using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct GridSpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {

    }

    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (spawner, spawnRequests) in SystemAPI.Query<GridSpawnerAspect, DynamicBuffer<SpawnRequest>>())
        {
            for (int i = 0; i < spawnRequests.Length; i++)
            {
                spawner.Spawn(ref state, spawnRequests[i]);
            }

            spawnRequests.Clear();
        }
    }
}
