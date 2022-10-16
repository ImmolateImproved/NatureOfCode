using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateBefore(typeof(RotateTowardsVelocitySystem))]
[UpdateAfter(typeof(GridSpawnerSystem))]
public partial struct SteeringSystem : ISystem
{
    [BurstCompile]
    partial struct SteeringJob : IJobEntity
    {
        [ReadOnly]
        public ComponentLookup<Translation> targetTranslations;

        [ReadOnly]
        public ComponentLookup<Rotation> targetRotations;

        public void Execute(PhysicsBodyAspect physicsBody, ref SteeringForce steeringForce, in DynamicBuffer<SteeringData> steeringDatas, in Translation translation)
        {
            var steeringDataArray = steeringDatas.AsNativeArray();

            for (int i = 0; i < steeringDataArray.Length; i++)
            {
                var steeringData = steeringDataArray[i];

                if (!targetTranslations.HasComponent(steeringData.target))
                    continue;

                var targetDirection = math.mul(targetRotations[steeringData.target].Value, new float3(1, 0, 0));
                var targetPos = targetTranslations[steeringData.target].Value + targetDirection * steeringData.predictionAmount;

                var force = targetPos - translation.Value;

                var distance = math.length(force);

                var desiredSpeed = distance < steeringData.slowRadius
                   ? math.remap(steeringData.slowRadius, 0, physicsBody.PhysicsData.maxSpeed, 0, distance)
                   : physicsBody.PhysicsData.maxSpeed;

                force = math.normalizesafe(force) * desiredSpeed;

                force -= physicsBody.Velocity;

                force = MathHelpers.ClampMagnitude(force, steeringData.maxForce);

                steeringForce.value += force * steeringData.seekOrFlee;
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