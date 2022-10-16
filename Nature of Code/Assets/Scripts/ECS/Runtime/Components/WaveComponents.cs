using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct WaveElementTag : IComponentData
{

}

public struct CombinedWaveTag : IComponentData
{

}

public struct CombinedWaveSpeed : IComponentData
{
    public float phaseSpeed;
}

public struct WaveSpawnRequest : IComponentData
{

}

public struct WaveSpawner : IComponentData
{
    public Entity waveElementPrefab;
    public int elementsPerWave;
    public float xStartPos;
}

[System.Serializable]
public struct WaveData : IBufferElementData
{
    public float amplitude;
    public float period;
    public float phase;

    public float phaseSpeed;
}

public struct WaveView : IBufferElementData
{
    public NativeList<Entity> waveElements;
}

public struct WaveElementPosition : IBufferElementData
{
    public NativeList<float2> waveElementPositions;
}