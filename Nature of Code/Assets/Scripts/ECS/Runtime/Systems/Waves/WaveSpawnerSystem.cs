using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(WaveDataInitializationAndCleanupSystem))]
public partial class WaveSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity e, ref WaveSpawner waveSpawner) =>
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                EntityManager.AddComponent<WaveSpawnRequest>(e);

                if (EntityManager.HasComponent<CombinedWaveTag>(e))
                {
                    EntityManager.RemoveComponent<CombinedWaveTag>(e);
                }
                else
                {
                    EntityManager.AddComponent<CombinedWaveTag>(e);
                }
            }

        }).WithStructuralChanges().Run();

        var destroyWaveElementECB = new EntityCommandBuffer(Allocator.Temp);

        Entities.WithAll<WaveSpawnRequest>()
            .ForEach((ref DynamicBuffer<WaveView> waveViews, ref DynamicBuffer<WaveElementPosition> waveElementPositions) =>
            {
                for (int i = 0; i < waveViews.Length; i++)
                {
                    destroyWaveElementECB.DestroyEntity(waveViews[i].waveElements.AsArray());
                    waveViews[i].waveElements.Clear();
                    waveElementPositions[i].waveElementPositions.Clear();
                }

            }).Run();

        destroyWaveElementECB.Playback(EntityManager);

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.WithAll<CombinedWaveTag>().WithAll<WaveSpawnRequest>()
            .ForEach((Entity e, ref DynamicBuffer<WaveElementPosition> waveElementPositions, ref DynamicBuffer<WaveView> waveElementEntities, in WaveSpawner spawner) =>
            {
                var waveElements = EntityManager.Instantiate(spawner.waveElementPrefab, spawner.elementsPerWave, Allocator.Temp);

                for (int j = 0; j < spawner.elementsPerWave; j++)
                {
                    var posX = spawner.xStartPos + j;

                    var waveElement = waveElements[j];
                    EntityManager.SetComponentData(waveElement, LocalTransform.FromPosition(new float3(posX, 0, 0)));

                    waveElementPositions[0].waveElementPositions.Add(new float2(posX, 0));
                    waveElementEntities[0].waveElements.Add(waveElement);
                }

                ecb.RemoveComponent<WaveSpawnRequest>(e);

            }).WithStructuralChanges().Run();

        Entities.WithNone<CombinedWaveTag>().WithAll<WaveSpawnRequest>()
            .ForEach((Entity e, ref DynamicBuffer<WaveElementPosition> waveElementPositions, ref DynamicBuffer<WaveView> waveElementEntities, in DynamicBuffer<WaveData> wavesData, in WaveSpawner spawner) =>
            {
                for (int i = 0; i < wavesData.Length; i++)
                {
                    var waveElements = EntityManager.Instantiate(spawner.waveElementPrefab, spawner.elementsPerWave, Allocator.Temp);

                    for (int j = 0; j < spawner.elementsPerWave; j++)
                    {
                        var posX = spawner.xStartPos + j;

                        var waveElement = waveElements[j];
                        EntityManager.SetComponentData(waveElement, LocalTransform.FromPosition(new float3(posX, 0, 0)));

                        waveElementPositions[i].waveElementPositions.Add(new float2(posX, 0));
                        waveElementEntities[i].waveElements.Add(waveElement);
                    }
                }

                ecb.RemoveComponent<WaveSpawnRequest>(e);

            }).WithStructuralChanges().Run();

        ecb.Playback(EntityManager);
    }
}