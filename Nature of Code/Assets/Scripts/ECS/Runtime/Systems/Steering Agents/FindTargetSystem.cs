using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
[UpdateBefore(typeof(SteeringSystem))]
public partial struct FindTargetSystem : ISystem
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
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        new FindTargetJob
        {
            physicsWorld = physicsWorld

        }.ScheduleParallel();
    }

    [BurstCompile]
    partial struct FindTargetJob : IJobEntity
    {
        [ReadOnly]
        public PhysicsWorldSingleton physicsWorld;

        public void Execute(in LocalTransform transform, ref DynamicBuffer<TargetSeeker> seeker)
        {
            for (int i = 0; i < seeker.Length; i++)
            {
                var input = new PointDistanceInput
                {
                    Filter = seeker[i].layers,
                    MaxDistance = seeker[i].searchRadius,
                    Position = transform.Position
                };

                if (physicsWorld.CalculateDistance(input, out var closet))
                {
                    seeker.ElementAt(i).target = closet.Entity;
                }
            }
        }
    }
}