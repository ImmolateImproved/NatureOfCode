using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct SteeringForce : IComponentData
{
    public float3 value;
}

public struct SteeringDNA
{
    public float predictionAmount;
    public float maxForce;
    public float slowRadius;
    public float seekOrFlee;
}

public struct SteeringData : IBufferElementData
{
    public Entity target;
    public SteeringDNA DNA;
}

public readonly partial struct SteeringAgentAspect : IAspect
{
    readonly RefRW<SteeringForce> steeringForce;
    readonly RefRO<Translation> translation;
    readonly RefRO<Velocity> velocity;
    readonly RefRO<PhysicsData> physicsData;

    public void Steer(in SteeringDNA steeringDNA, float3 targetPosition)
    {
        var force = targetPosition - translation.ValueRO.Value;

        var distance = math.length(force);

        var maxSpeed = physicsData.ValueRO.maxSpeed;

        var desiredSpeed = distance < steeringDNA.slowRadius
           ? math.remap(steeringDNA.slowRadius, 0, maxSpeed, 0, distance)
           : maxSpeed;

        force = MathUtils.SetMagnitude(force, desiredSpeed);

        force -= velocity.ValueRO.value;

        force = MathUtils.ClampMagnitude(force, steeringDNA.maxForce);

        steeringForce.ValueRW.value += force * steeringDNA.seekOrFlee;
    }

    public void SteerAhead(in SteeringDNA steeringDNA, float3 targetPosition, float3 targetDirection)
    {
        targetPosition += targetDirection * steeringDNA.predictionAmount;
        Steer(steeringDNA, targetPosition);
    }
}