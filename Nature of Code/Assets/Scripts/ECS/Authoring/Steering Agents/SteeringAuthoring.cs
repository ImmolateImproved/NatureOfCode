using Unity.Entities;
using UnityEngine;

[System.Serializable]
public struct SteeringAgentData
{
    public GameObject target;
    public float maxForce;

    public float predictionAmount;
    public float slowRadius;
    public float seekOrFlee;

    public float searchRadius;
}

public class SteeringAuthoring : MonoBehaviour
{
    public SteeringAgentData[] seekerDatas;

    class SteeringBaker : Baker<SteeringAuthoring>
    {
        public override void Bake(SteeringAuthoring authoring)
        {
            AddComponent(new SteeringForce());
            var seekerDatas = AddBuffer<TargetSeeker>();

            var steeringDatas = AddBuffer<SteeringData>();

            for (int i = 0; i < authoring.seekerDatas.Length; i++)
            {
                var steeringData = authoring.seekerDatas[i];

                var seeker = new SteeringData
                {
                    target = GetEntity(steeringData.target),
                    maxForce = steeringData.maxForce,
                    predictionAmount = steeringData.predictionAmount,
                    slowRadius = steeringData.slowRadius,
                    seekOrFlee = steeringData.seekOrFlee
                };

                steeringDatas.Add(seeker);

                seekerDatas.Add(new TargetSeeker
                {
                    searchRadius = authoring.seekerDatas[i].searchRadius

                });
            }
        }
    }
}