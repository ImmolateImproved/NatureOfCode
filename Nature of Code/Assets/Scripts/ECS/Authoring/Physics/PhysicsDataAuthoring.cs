using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PhysicsDataAuthoring : MonoBehaviour
{
    public float mass = 1;
    [Range(0f, 10)]
    public float coeffOfFriction;
    [Range(0f, 10)]
    public float coeffOfDrag;

    public float maxSpeed;

    public Vector2 initialVelocity;

    public bool isAttractor;

    public bool applyNaturalForces;

    class PhysicsDataBaker : Baker<PhysicsDataAuthoring>
    {
        public override void Bake(PhysicsDataAuthoring authoring)
        {
            if (authoring.isAttractor)
            {
                AddComponent(new Attractor { });
            }

            AddComponent(new PhysicsData
            {
                mass = authoring.mass,
                coeffOfFriction = authoring.coeffOfFriction,
                coeffOfDrag = authoring.coeffOfDrag
            });

            AddComponent(new ResultantForce { });

            AddComponent(new Velocity
            {
                value = new Vector3(authoring.initialVelocity.x, authoring.initialVelocity.y, 0),
                maxSpeed = authoring.maxSpeed
            });

            if (authoring.applyNaturalForces)
            {
                AddComponent<ApplyNaturalForces>();
            }
        }
    }

    private void OnValidate()
    {
        if (mass < 0.1f)
        {
            mass = 0.1f;
        }
    }
}