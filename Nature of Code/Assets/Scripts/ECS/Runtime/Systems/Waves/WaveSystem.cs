using Unity.Entities;
using Unity.Mathematics;

[UpdateAfter(typeof(WaveSpawnerSystem))]
public partial class WaveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var dt = SystemAPI.Time.DeltaTime;

        Entities.WithAll<CombinedWaveTag>()
            .ForEach((in DynamicBuffer<WaveData> waveDatas, in DynamicBuffer<WaveElementPosition> waveElementPositions, in CombinedWaveSpeed combinedWave) =>
            {
                var waveDataArray = waveDatas.AsNativeArray();
                var waveElementPositionsArray = waveElementPositions.AsNativeArray();

                for (int i = 0; i < waveDataArray.Length; i++)
                {
                    var waveElementPosList = waveElementPositionsArray[i].waveElementPositions;

                    for (int k = 0; k < waveElementPosList.Length; k++)
                    {
                        var waveElementPos = waveElementPosList[k];

                        var y = 0f;

                        for (int j = 0; j < waveDataArray.Length; j++)
                        {
                            var wave = waveDataArray[j];

                            var sinArg = (wave.phase + 2 * math.PI * waveElementPos.x / wave.period);

                            y += math.sin(sinArg) * wave.amplitude;
                        }

                        waveElementPos.y = y;
                        waveElementPosList[k] = waveElementPos;
                    }
                }

                for (int j = 0; j < waveDataArray.Length; j++)
                {
                    var wave = waveDataArray[j];

                    wave.phase += combinedWave.phaseSpeed * dt;

                    waveDataArray[j] = wave;
                }

            }).Run();

        Entities.WithNone<CombinedWaveTag>()
            .ForEach((ref DynamicBuffer<WaveData> waveDatas, in DynamicBuffer<WaveElementPosition> waveElementPositions) =>
            {
                var waveDataArray = waveDatas.AsNativeArray();
                var waveElementPositionsArray = waveElementPositions.AsNativeArray();

                for (int i = 0; i < waveDataArray.Length; i++)
                {
                    var waveElementPosArray = waveElementPositionsArray[i].waveElementPositions;

                    var wave = waveDataArray[i];

                    for (int j = 0; j < waveElementPosArray.Length; j++)
                    {
                        var waveElementPos = waveElementPosArray[j];

                        var sinArg = (wave.phase + 2 * math.PI * waveElementPos.x / wave.period);
                        waveElementPos.y = math.sin(sinArg) * wave.amplitude;

                        waveElementPosArray[j] = waveElementPos;
                    }
                }

                for (int j = 0; j < waveDataArray.Length; j++)
                {
                    var wave = waveDataArray[j];

                    wave.phase += wave.phaseSpeed * dt;

                    waveDataArray[j] = wave;
                }

            }).Run();
    }
}