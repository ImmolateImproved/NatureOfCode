using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct ApplyForceSystem : ISystem
{
    [BurstCompile]
    partial struct ApplyForceJob : IJobEntity
    {
        public float dt;

        public void Execute(ref Translation translation, PhysicsBodyAspect physicsBody)
        {
            physicsBody.ApplyVelocity(ref translation, dt);
        }
    }

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var dt = SystemAPI.Time.DeltaTime;

        new ApplyForceJob
        {
            dt = dt

        }.ScheduleParallel();
    }
}