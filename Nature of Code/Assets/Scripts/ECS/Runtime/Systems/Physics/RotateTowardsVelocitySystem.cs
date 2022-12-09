using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateAfter(typeof(ApplyForceSystem))]
public partial struct RotateTowardsVelocitySystem : ISystem
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
        new RotateTowardsVelocityJob
        {

        }.ScheduleParallel();
    }

    [BurstCompile]
    partial struct RotateTowardsVelocityJob : IJobEntity
    {
        public void Execute(TransformAspect transform, in Velocity velocity)
        {
            var lookDir = velocity.value;
            float angle = math.atan2(lookDir.y, lookDir.x);
            transform.LocalRotation = quaternion.Euler(0, 0, angle);
        }
    }
}
