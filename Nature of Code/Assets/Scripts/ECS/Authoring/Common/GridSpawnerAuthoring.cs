using Unity.Entities;
using UnityEngine;

[System.Serializable]
public struct SpawnRequestData
{
    public GameObject prefab;
    public int count;
}

public class GridSpawnerAuthoring : MonoBehaviour
{
    public Vector3 minPosition;
    public Vector3 maxPosition;
    public Vector3 offset;

    public SpawnRequestData[] spawnRequests;

    class GridSpawnerBaker : Baker<GridSpawnerAuthoring>
    {
        public override void Bake(GridSpawnerAuthoring authoring)
        {
            AddComponent(new GridSpawner
            {
                minPosition = authoring.minPosition,
                maxPosition = authoring.maxPosition,
                offset = authoring.offset,
                random = new Unity.Mathematics.Random((uint)System.DateTime.Now.Millisecond)
            });

            var requests = AddBuffer<SpawnRequest>();

            for (int i = 0; i < authoring.spawnRequests.Length; i++)
            {
                requests.Add(new SpawnRequest
                {
                    count = authoring.spawnRequests[i].count,
                    prefab = GetEntity(authoring.spawnRequests[i].prefab)
                });
            }
        }
    }
}