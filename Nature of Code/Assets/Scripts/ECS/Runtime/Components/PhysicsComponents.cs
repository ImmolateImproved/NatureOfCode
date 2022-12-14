using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct Attractor : IComponentData
{

}

public struct ApplyNaturalForces : IComponentData
{

}

public struct GlobalForceSettings : IComponentData
{
    public float3 gravity;
    public float3 wind;

    public float3 windMultiplayer;

    public float G;

    public float minAttractionDistance;
    public float maxAttractionDistance;
}

public struct PhysicsData : IComponentData
{
    public float mass;
    public float coeffOfFriction;
    public float coeffOfDrag;
}

public struct ResultantForce : IComponentData
{
    public float3 value;
}

public struct Velocity : IComponentData
{
    public float3 value;
    public float maxSpeed;
}

public readonly partial struct PhysicsBodyAspect : IAspect
{
    readonly RefRW<ResultantForce> resultantForce;
    readonly RefRW<Velocity> velocity;
    readonly RefRO<PhysicsData> physicsData;

    public float3 ResultantForce
    {
        get => resultantForce.ValueRO.value;
        set => resultantForce.ValueRW.value = value;
    }

    public float3 Velocity
    {
        get => velocity.ValueRO.value;
        set => velocity.ValueRW.value = value;
    }

    public float MaxSpeed
    {
        get => velocity.ValueRO.maxSpeed;
        set => velocity.ValueRW.maxSpeed = value;
    }

    public PhysicsData PhysicsData
    {
        get => physicsData.ValueRO;
    }

    public void ApplyVelocity(ref LocalTransform transform, float deltaTime)
    {
        var acceleration = ResultantForce / PhysicsData.mass;

        Velocity += acceleration;// * deltaTime;
        Velocity = MathUtils.ClampMagnitude(Velocity, MaxSpeed);

        transform.Position += Velocity * deltaTime;

        ResultantForce = 0;
    }

    public void AddGravity(in GlobalForceSettings forceSettings)
    {
        var weight = forceSettings.gravity * PhysicsData.mass;

        ResultantForce += weight;
    }

    public void AddFriction(in LocalTransform localTransform, in SquareWorldBounds worldBoundaries)
    {
        var diff = (localTransform.Position.y - localTransform.Scale / 2) + worldBoundaries.value.y;

        if (diff < 1)
        {
            var friction = math.normalizesafe(Velocity);
            var mu = PhysicsData.coeffOfFriction;
            var normal = PhysicsData.mass;

            friction *= -1 * mu * normal;

            ResultantForce += friction;
        }
    }

    public void AddDrag()
    {
        var drag = -math.normalizesafe(Velocity);

        var c = PhysicsData.coeffOfDrag;
        var speed = math.lengthsq(Velocity);

        drag = drag * c * speed;
        ResultantForce += drag;
    }
}