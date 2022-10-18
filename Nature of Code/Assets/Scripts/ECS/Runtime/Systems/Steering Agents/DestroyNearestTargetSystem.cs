using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateAfter(typeof(FindTargetSystem))]
public partial struct DestroyNearestTargetSystem : ISystem
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
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var targetsQuery = SystemAPI.QueryBuilder().WithAll<Translation, TargetType>().Build();

        var targetEntities = targetsQuery.ToEntityArray(Allocator.TempJob);
        var targetPositions = targetsQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

        state.Dependency = new DestroyNearestTargetJob
        {
            targetEntities = targetEntities,
            targetPositions = targetPositions,
            ecb = ecb.AsParallelWriter()

        }.ScheduleParallel(state.Dependency);

        targetEntities.Dispose(state.Dependency);
        targetPositions.Dispose(state.Dependency);
    }

    [BurstCompile]
    [WithAll(typeof(TargetSeeker))]
    partial struct DestroyNearestTargetJob : IJobEntity
    {
        [ReadOnly]
        public NativeArray<Entity> targetEntities;

        [ReadOnly]
        public NativeArray<Translation> targetPositions;

        public EntityCommandBuffer.ParallelWriter ecb;

        public void Execute([ChunkIndexInQuery] int chunkIndex, in Translation translation)
        {
            for (int i = 0; i < targetEntities.Length; i++)
            {
                var dist = math.distance(translation.Value, targetPositions[i].Value);

                if (dist < 1)
                {
                    ecb.DestroyEntity(chunkIndex, targetEntities[i]);
                }
            }
        }
    }
}