using Unity.Entities;
using UnityEngine;

public class WaveSpawnerAuthoring : MonoBehaviour
{
    public bool combineWave;

    public GameObject waveElementPrefab;
    public int elementsPerWave;
    public int xStartPos;

    public float combinePhaseSpeed;

    public WaveData[] waves;

    class WaveSpawnerBaker : Baker<WaveSpawnerAuthoring>
    {
        public override void Bake(WaveSpawnerAuthoring authoring)
        {
            if (authoring.combineWave)
            {
                AddComponent(new CombinedWaveTag());
            }

            AddComponent(new CombinedWaveSpeed { phaseSpeed = authoring.combinePhaseSpeed });

            AddComponent(new WaveSpawnRequest());

            AddComponent(new WaveSpawner
            {
                waveElementPrefab = GetEntity(authoring.waveElementPrefab),
                elementsPerWave = authoring.elementsPerWave,
                xStartPos = authoring.xStartPos
            });

            var waveDatas = AddBuffer<WaveData>();

            for (int i = 0; i < authoring.waves.Length; i++)
            {
                waveDatas.Add(authoring.waves[i]);
            }
        }
    }
}