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
    [BurstCompile]
    partial struct SteeringJob : IJobEntity
    {
        [ReadOnly]
        public ComponentLookup<Translation> targetTranslations;

        [ReadOnly]
        public ComponentLookup<Rotation> targetRotations;

        public void Execute(SteeringAgentAspect steeringAgent, in DynamicBuffer<SteeringData> steeringDatas)
        {
            var steeringDataArray = steeringDatas.AsNativeArray();

            for (int i = 0; i < steeringDataArray.Length; i++)
            {
                var steeringData = steeringDataArray[i];

                if (!targetTranslations.HasComponent(steeringData.target))
                    continue;

                var targetDirection = math.mul(targetRotations[steeringData.target].Value, new float3(1, 0, 0));
                var targetPos = targetTranslations[steeringData.target].Value;

                steeringAgent.SteerAhead(steeringData.DNA, targetPos, targetDirection);
            }
        }
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
    public void OnUpdate(ref SystemState state)
    {
        var translationLookup = SystemAPI.GetComponentLookup<Translation>(true);
        var rotationLookup = SystemAPI.GetComponentLookup<Rotation>(true);

        new SteeringJob
        {
            targetTranslations = translationLookup,
            targetRotations = rotationLookup

        }.ScheduleParallel();

        new ApplySteeringForceJob
        {

        }.ScheduleParallel();
    }

    public void OnCreate(ref SystemState state)
    {

    }

    public void OnDestroy(ref SystemState state)
    {

    }
}