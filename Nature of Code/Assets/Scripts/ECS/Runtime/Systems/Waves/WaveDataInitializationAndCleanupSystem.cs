using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class WaveDataInitializationAndCleanupSystem : SystemBase
{
    struct WaveDataCleanup : ICleanupBufferElementData
    {
        public NativeList<float2> waveElementPositions;
        public NativeList<Entity> waveElementEntities;
    }

    protected override void OnUpdate()
    {
        Entities.WithNone<WaveElementPosition, WaveView>()
            .ForEach((Entity e, in DynamicBuffer<WaveData> waveDatas) =>
            {
                var wavesCount = waveDatas.Length;

                var waveElementPositions = EntityManager.AddBuffer<WaveElementPosition>(e);
                waveElementPositions.Length = wavesCount;

                var waveElementEntities = EntityManager.AddBuffer<WaveView>(e);
                waveElementEntities.Length = wavesCount;

            }).WithStructuralChanges().Run();

        Entities.WithNone<WaveDataCleanup>()
            .ForEach((Entity e, ref DynamicBuffer<WaveElementPosition> waveElementPositions, ref DynamicBuffer<WaveView> waveElementEntities, in WaveSpawner waveSpawner) =>
            {
                var cleanupList = new NativeList<WaveDataCleanup>(waveElementPositions.Length, Allocator.Temp);

                for (int i = 0; i < waveElementPositions.Length; i++)
                {
                    var waveElementPoitionsList = new NativeList<float2>(waveSpawner.elementsPerWave, Allocator.Persistent);
                    var waveElementEntitiesList = new NativeList<Entity>(waveSpawner.elementsPerWave, Allocator.Persistent);

                    cleanupList.Add(new WaveDataCleanup
                    {
                        waveElementPositions = waveElementPoitionsList,
                        waveElementEntities = waveElementEntitiesList
                    });

                    waveElementPositions[i] = new WaveElementPosition
                    {
                        waveElementPositions = waveElementPoitionsList
                    };

                    waveElementEntities[i] = new WaveView
                    {
                        waveElements = waveElementEntitiesList
                    };
                }

                var cleanupBuffer = EntityManager.AddBuffer<WaveDataCleanup>(e);
                cleanupBuffer.AddRange(cleanupList.AsArray());

            }).WithStructuralChanges().Run();

        Entities.WithNone<WaveElementPosition>()
            .ForEach((Entity e, ref DynamicBuffer<WaveDataCleanup> waveDataCleanups) =>
            {
                for (int i = 0; i < waveDataCleanups.Length; i++)
                {
                    waveDataCleanups[i].waveElementPositions.Dispose();
                    waveDataCleanups[i].waveElementEntities.Dispose();
                }

                EntityManager.RemoveComponent<WaveDataCleanup>(e);

            }).WithStructuralChanges().Run();
    }
}