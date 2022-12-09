using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct WorldBoundsSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SquareWorldBounds>();
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var bounds = SystemAPI.GetSingleton<SquareWorldBounds>();
        
        new BoundariesCollisionJob
        {
            bounds = bounds.value

        }.ScheduleParallel();
    }

    [BurstCompile]
    partial struct BoundariesCollisionJob : IJobEntity
    {
        public float2 bounds;

        public void Execute(ref LocalTransform transform, ref Velocity velocity)
        {
            if (transform.Position.x >= bounds.x)
            {
                transform.Position.x = bounds.x;
                velocity.value.x *= -1;
            }
            else if (transform.Position.x <= -bounds.x)
            {
                transform.Position.x = -bounds.x;
                velocity.value.x *= -1;
            }

            if (transform.Position.y >= bounds.y)
            {
                transform.Position.y = bounds.y;
                velocity.value.y *= -1;
            }

            if (transform.Position.y <= -bounds.y)
            {
                transform.Position.y = -bounds.y;
                velocity.value.y *= -1;
            }
        }
    }
}