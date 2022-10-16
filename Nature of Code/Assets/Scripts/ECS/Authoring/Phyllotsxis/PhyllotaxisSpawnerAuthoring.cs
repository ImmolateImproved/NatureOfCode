using Unity.Entities;
using UnityEngine;

public class PhyllotaxisSpawnerAuthoring : MonoBehaviour
{
    public GameObject prefab;
    public float angle;

    public float c;

    public float spawnDelay;

    class PhyllotaxisSpawnerBaker : Baker<PhyllotaxisSpawnerAuthoring>
    {
        public override void Bake(PhyllotaxisSpawnerAuthoring authoring)
        {
            AddComponent(new PhyllotaxisComponents
            {
                prefab = GetEntity(authoring.prefab),
                angle = authoring.angle,
                c = authoring.c,
                spawnDelay = authoring.spawnDelay
            });
        }
    }
}