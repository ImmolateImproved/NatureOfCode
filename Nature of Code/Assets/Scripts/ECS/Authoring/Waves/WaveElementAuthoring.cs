using Unity.Entities;
using UnityEngine;

public class WaveElementAuthoring : MonoBehaviour
{
    class WaveElementBaker : Baker<WaveElementAuthoring>
    {
        public override void Bake(WaveElementAuthoring authoring)
        {
            AddComponent<WaveElementTag>();
        }
    }
}
