using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(WaveSystem))]
public partial class WaveViewSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var transfromLookup = GetComponentLookup<LocalTransform>();

        Entities.ForEach((in DynamicBuffer<WaveView> waveElements, in DynamicBuffer<WaveElementPosition> waveElementPositions) =>
        {
            var waveElementsArray = waveElements.AsNativeArray();
            var waveElementsPosArray = waveElementPositions.AsNativeArray();

            for (int i = 0; i < waveElementsArray.Length; i++)
            {
                var waveElementsList = waveElementsArray[i].waveElements;
                var waveElementsPosList = waveElementsPosArray[i].waveElementPositions;

                for (int j = 0; j < waveElementsList.Length; j++)
                {
                    ref var waveElementPos = ref transfromLookup.GetRefRW(waveElementsList[j], false).ValueRW.Position;

                    waveElementPos.y = waveElementsPosList[j].y;
                }
            }

        }).Run();

        Entities.WithAll<WaveElementTag>()
            .ForEach((ref URPMaterialPropertyBaseColor color, in LocalTransform transfrom) =>
            {
                var yPos = math.remap(-15, 15, 0, 1, transfrom.Position.y);

                color.Value = (Vector4)Color.HSVToRGB(yPos, 1, 1);

            }).Run();
    }
}