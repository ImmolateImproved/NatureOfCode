using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateBefore(typeof(SteeringSystem))]
[UpdateAfter(typeof(GridSpawnerSystem))]
public partial struct FindTargetSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var targetsQuery = SystemAPI.QueryBuilder().WithAll<Translation, TargetType>().Build();

        var targetEntities = targetsQuery.ToEntityArray(Allocator.TempJob);
        var targetPositions = targetsQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var targetTypes = targetsQuery.ToComponentDataArray<TargetType>(Allocator.TempJob);

        new FindTargetJob
        {
            targetEntities = targetEntities,
            targetPositions = targetPositions,
            targetTypes = targetTypes

        }.Run();

        targetEntities.Dispose();
        targetPositions.Dispose();
    }

    [BurstCompile]
    partial struct FindTargetJob : IJobEntity
    {
        [ReadOnly]
        public NativeArray<Entity> targetEntities;

        [ReadOnly]
        public NativeArray<Translation> targetPositions;

        [ReadOnly]
        public NativeArray<TargetType> targetTypes;

        public void Execute(in TransformAspect transform, in DynamicBuffer<TargetSeeker> seekerBuffer, DynamicBuffer<SteeringData> steeringDataBuffer)
        {
            var distances = new NativeArray<float>(steeringDataBuffer.Length, Allocator.Temp);

            for (int i = 0; i < distances.Length; i++)
            {
                distances[i] = math.INFINITY;
            }

            var newTargetEntity = new NativeArray<Entity>(steeringDataBuffer.Length, Allocator.Temp);

            for (int i = 0; i < targetEntities.Length; i++)
            {
                var targetTypeIndex = (int)targetTypes[i].value;

                var newDistance = math.distance(transform.Position, targetPositions[i].Value);

                if (newDistance < seekerBuffer[targetTypeIndex].searchRadius && newDistance < distances[targetTypeIndex])
                {
                    distances[targetTypeIndex] = newDistance;

                    newTargetEntity[targetTypeIndex] = targetEntities[i];
                }
            }

            for (int i = 0; i < steeringDataBuffer.Length; i++)
            {
                steeringDataBuffer.ElementAt(i).target = newTargetEntity[i];
            }
        }
    }
}