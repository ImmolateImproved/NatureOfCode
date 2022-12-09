using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(ApplyForceSystem))]
public partial class AttractorSystem : SystemBase
{
    private EntityQuery attractorsQuery;

    protected override void OnCreate()
    {
        attractorsQuery = GetEntityQuery(typeof(Attractor), typeof(LocalTransform), typeof(PhysicsData));

        RequireAnyForUpdate(attractorsQuery);
        RequireForUpdate<GlobalForceSettings>();
    }

    protected override void OnUpdate()
    {
        var attractorEntities = attractorsQuery.ToEntityArray(Allocator.Temp);
        var attractorPositions = attractorsQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
        var attractorDatas = attractorsQuery.ToComponentDataArray<PhysicsData>(Allocator.Temp);

        var physicsSettings = SystemAPI.GetSingleton<GlobalForceSettings>();

        Entities.WithAll<ApplyNaturalForces>()
            .ForEach((Entity e, ref ResultantForce resultantForce, in PhysicsData physicsData, in LocalTransform transform) =>
            {
                for (int i = 0; i < attractorEntities.Length; i++)
                {
                    if (e == attractorEntities[i])
                        continue;

                    var attractorPos = attractorPositions[i];
                    var attractorData = attractorDatas[i];

                    var force = (attractorPos.Position - transform.Position);
                    var dist = math.lengthsq(force);

                    dist = math.clamp(dist, physicsSettings.minAttractionDistance, physicsSettings.maxAttractionDistance);

                    var strength = physicsSettings.G * (attractorData.mass * physicsData.mass) / dist;

                    force = math.normalizesafe(force) * strength;

                    resultantForce.value += force;
                }

            }).Run();
    }
}