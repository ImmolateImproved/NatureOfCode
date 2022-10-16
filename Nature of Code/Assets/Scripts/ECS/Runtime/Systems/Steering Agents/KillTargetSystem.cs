using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateAfter(typeof(FindTargetSystem))]
public partial struct KillTargetSystem : ISystem
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
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var translation in SystemAPI.Query<Translation>().WithAll<TargetSeeker>())
        {
            foreach (var (targetPos, targetEntity) in SystemAPI.Query<Translation>().WithAll<TargetType>().WithEntityAccess())
            {
                var dist = math.distance(translation.Value, targetPos.Value);

                if (dist < 1)
                {
                    ecb.DestroyEntity(targetEntity);
                }
            }
        }

        ecb.Playback(state.EntityManager);
    }
}