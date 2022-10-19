using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[System.Serializable]
public struct SteeringAgentData
{
    public GameObject target;
    public float maxForce;

    public float predictionAmount;
    public float slowRadius;
    public float seekOrFlee;

    public float searchRadius;

    public PhysicsCategoryTags belongsTo;
    public PhysicsCategoryTags collidesWith;
}

public class SteeringAuthoring : MonoBehaviour
{
    public SteeringAgentData[] seekerDatas;

    class SteeringBaker : Baker<SteeringAuthoring>
    {
        public override void Bake(SteeringAuthoring authoring)
        {
            AddComponent(new SteeringForce());
            AddComponent(new TargetInRangeTag());
            var seekerDatas = AddBuffer<TargetSeeker>();

            var steeringDatas = AddBuffer<SteeringData>();

            for (int i = 0; i < authoring.seekerDatas.Length; i++)
            {
                var steeringData = authoring.seekerDatas[i];

                var steeringDNA = new SteeringDNA
                {
                    maxForce = steeringData.maxForce,
                    predictionAmount = steeringData.predictionAmount,
                    slowRadius = steeringData.slowRadius,
                    seekOrFlee = steeringData.seekOrFlee
                };

                var seeker = new SteeringData
                {
                    DNA = steeringDNA
                };

                steeringDatas.Add(seeker);

                seekerDatas.Add(new TargetSeeker
                {
                    target = GetEntity(steeringData.target),
                    searchRadius = authoring.seekerDatas[i].searchRadius,
                    layers = new Unity.Physics.CollisionFilter
                    {
                        BelongsTo = authoring.seekerDatas[i].belongsTo.Value,
                        CollidesWith = authoring.seekerDatas[i].collidesWith.Value
                    }

                });
            }
        }
    }
}