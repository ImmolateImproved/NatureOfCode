using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public partial class PhyllotaxisSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = World.GetOrCreateSystemManaged<BeginSimulationEntityCommandBufferSystem>().CreateCommandBuffer();

        var dt = SystemAPI.Time.DeltaTime;

        Entities.ForEach((ref PhyllotaxisComponents spawner) =>
        {
            if (spawner.timer >= spawner.spawnDelay)
            {
                var a = spawner.n * spawner.angle;
                var r = spawner.c * math.sqrt(spawner.n);

                var radA = math.radians(a);
                var x = r * math.cos(radA);
                var y = r * math.sin(radA);

                var color = new URPMaterialPropertyBaseColor
                {
                    Value = (Vector4)Color.HSVToRGB((a - r) % 1, 1, 1)//(Vector4)Color.LerpUnclamped(Color.cyan, Color.green, spawner.n / 150f)
                };

                var e = ecb.Instantiate(spawner.prefab);
                ecb.SetComponent(e, new Translation { Value = new float3(x, y, 0) });
                ecb.SetComponent(e, color);

                spawner.n++;
            }

            spawner.timer += dt;

        }).Run();
    }
}