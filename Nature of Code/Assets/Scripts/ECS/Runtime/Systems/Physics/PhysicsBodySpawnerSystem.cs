using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class PhysicsBodySpawnerSystem : SystemBase
{
    private Random random;

    protected override void OnCreate()
    {
        random = new Random(1235);
    }

    protected override void OnUpdate()
    {
        var ecb = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();

        var rng = random;

        Entities.ForEach((int entityInQueryIndex, ref PhysicsBodySpawner spawner) =>
        {
            for (int i = 0; i < spawner.count; i++)
            {
                var direction = rng.NextFloat2Direction();
                var length = rng.NextFloat(spawner.minRadius, spawner.maxRadius);

                var position = direction * length;
                var position3D = new float3(position, 0);

                var velocity = SystemAPI.GetComponent<Velocity>(spawner.prefab);
                //physicsData.mass = UnityEngine.Random.Range(0, 8);
                velocity.value = math.cross(math.normalize(position3D), new float3(0, 0, 1)) * spawner.startSpeed;

                var e = ecb.Instantiate(spawner.prefab);
                ecb.SetComponent(e, LocalTransform.FromPosition(position3D));
                ecb.SetComponent(e, velocity);
            }

            spawner.count = 0;

        }).Run();

        random = rng;
    }
}
