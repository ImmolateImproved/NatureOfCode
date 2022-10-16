using Unity.Entities;
using UnityEngine;

public class UnitDnaAuthoring : MonoBehaviour
{
    public float[] dna;

    class UnitDnaBaker : Baker<UnitDnaAuthoring>
    {
        public override void Bake(UnitDnaAuthoring authoring)
        {
    
        }
    }
}