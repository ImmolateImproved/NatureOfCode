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

        var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true);
        var targetInRangeLookup = SystemAPI.GetComponentLookup<TargetInRangeTag>();

        state.Dependency = new DestroyNearestTargetJob
        {
            ecb = ecb.AsParallelWriter(),
            transformLookup = transformLookup,
            targetInRangeLookup = targetInRangeLookup

        }.ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    [WithAll(typeof(TargetSeeker))]
    partial struct DestroyNearestTargetJob : IJobEntity
    {
        [ReadOnly]
        public ComponentLookup<LocalTransform> transformLookup;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<TargetInRangeTag> targetInRangeLookup;

        public EntityCommandBuffer.ParallelWriter ecb;

        public void Execute(Entity e, [ChunkIndexInQuery] int chunkIndex, in LocalTransform transform, in DynamicBuffer<TargetSeeker> targetSeeker)
        {
            for (int i = 0; i < targetSeeker.Length; i++)
            {
                var target = targetSeeker[i].target;

                if (!transformLookup.HasComponent(target))
                    continue;

                var dist = math.distance(transform.Position, transformLookup[target].Position);

                if (dist < 1)
                {
                    targetInRangeLookup.SetComponentEnabled(e, true);
                    targetInRangeLookup[e] = new TargetInRangeTag
                    {
                        targetType = (TargetTypeEnum)i 
                    };

                    ecb.DestroyEntity(chunkIndex, target);
                }
            }
        }
    }
}