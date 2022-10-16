using Unity.Entities;
using UnityEngine;

public class PhysicsBodySpawnerAuthoring : MonoBehaviour
{
    public GameObject prefab;
    public int count;

    public float startSpeed;
    public float minRadius;
    public float maxRadius;

    class PhysicsBodyBaker : Baker<PhysicsBodySpawnerAuthoring>
    {
        public override void Bake(PhysicsBodySpawnerAuthoring authoring)
        {
            AddComponent(new PhysicsBodySpawner
            {
                prefab = GetEntity(authoring.prefab),
                count = authoring.count,
                startSpeed = authoring.startSpeed,
                minRadius = authoring.minRadius,
                maxRadius = authoring.maxRadius

            });
        }
    }
}