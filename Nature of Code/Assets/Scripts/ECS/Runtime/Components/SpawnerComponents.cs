using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct GridSpawner : IComponentData
{
    public float3 minPosition;
    public float3 maxPosition;
    public float3 offset;

    public Random random;

    public float3 GetNextPosition()
    {
        return random.NextFloat3(minPosition, maxPosition) + offset;
    }
}

public struct SpawnRequest : IBufferElementData
{
    public Entity prefab;

    public int count;
}

public readonly partial struct GridSpawnerAspect : IAspect
{
    readonly RefRW<GridSpawner> spawner;

    public void Init(int index)
    {
        spawner.ValueRW.random = Random.CreateFromIndex((uint)index);
    }

    public NativeArray<Entity> Spawn(ref SystemState state, in SpawnRequest spawnRequest)
    {
        var entities = state.EntityManager.Instantiate(spawnRequest.prefab, spawnRequest.count, Allocator.Temp);

        for (int i = 0; i < entities.Length; i++)
        {
            var randomPosition = spawner.ValueRW.GetNextPosition();
            state.EntityManager.SetComponentData(entities[i], new Translation { Value = randomPosition });
        }

        return entities;
    }
}