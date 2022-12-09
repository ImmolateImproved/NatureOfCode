using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateBefore(typeof(RotateTowardsVelocitySystem))]
public partial struct SteeringSystem : ISystem
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
        var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true);

        new SteeringJob
        {
            transformLookup = transformLookup

        }.ScheduleParallel();

        new ApplySteeringForceJob
        {

        }.ScheduleParallel();
    }

    [BurstCompile]
    partial struct ApplySteeringForceJob : IJobEntity
    {
        public void Execute(PhysicsBodyAspect physicsBody, ref SteeringForce steeringForce)
        {
            physicsBody.ResultantForce += steeringForce.value;
            steeringForce.value = 0;
        }
    }

    [BurstCompile]
    partial struct SteeringJob : IJobEntity
    {
        [ReadOnly]
        public ComponentLookup<LocalTransform> transformLookup;

        public void Execute(SteeringAgentAspect steeringAgent, in DynamicBuffer<TargetSeeker> seeker, in DynamicBuffer<SteeringData> steeringDatas)
        {
            var seekerArray = seeker.AsNativeArray();

            for (int i = 0; i < seekerArray.Length; i++)
            {
                var seekerData = seekerArray[i];

                if (!transformLookup.HasComponent(seekerData.target))
                    continue;

                var targetDirection = transformLookup[seekerData.target].Forward();
                var targetPos = transformLookup[seekerData.target].Position;

                steeringAgent.SteerAhead(steeringDatas[i].DNA, targetPos, targetDirection);
            }
        }
    }
}