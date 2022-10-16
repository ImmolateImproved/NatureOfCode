using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(ApplyForceSystem))]
public partial class ForcesSystem : SystemBase
{
    [BurstCompile]
    [WithAll(typeof(ApplyNaturalForces))]
    private partial struct AddForcesJob : IJobEntity
    {
        public GlobalForceSettings forceSettings;
        public WorldBoundaries worldBoundaries;
        public float2 mousePosition;
        public bool mousePressed;

        public void Execute(PhysicsBodyAspect physicsBody, in Translation translation, in NonUniformScale scale)
        {
            if (mousePressed)
            {
                var mousePos3D = new float3(mousePosition, 0);

                forceSettings.wind = mousePos3D * forceSettings.windMultiplayer;

                physicsBody.ResultantForce += forceSettings.wind;
            }

            physicsBody.AddGravity(forceSettings);
            physicsBody.AddFriction(translation, scale, worldBoundaries);
            physicsBody.AddDrag();
        }
    }

    protected override void OnCreate()
    {
        RequireForUpdate<WorldBoundaries>();
        RequireForUpdate<GlobalForceSettings>();
        RequireForUpdate<MousePosition>();
    }

    protected override void OnUpdate()
    {
        new AddForcesJob
        {
            forceSettings = GetSingleton<GlobalForceSettings>(),
            worldBoundaries = GetSingleton<WorldBoundaries>(),
            mousePosition = GetSingleton<MousePosition>().value,
            mousePressed = Input.GetMouseButton(0)

        }.Run();
    }
}