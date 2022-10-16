using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ForceSettingsAuthoring : MonoBehaviour
{
    public float gravity;
    public Vector3 windMultiplayer;
    public float G;

    public float minAttractionDistance;
    public float maxAttractionDistance;

    class ForceSettingsBaker : Baker<ForceSettingsAuthoring>
    {
        public override void Bake(ForceSettingsAuthoring authoring)
        {
            AddComponent(new GlobalForceSettings
            {
                gravity = new float3(0, authoring.gravity, 0),
                windMultiplayer = authoring.windMultiplayer,
                G = authoring.G,
                minAttractionDistance = authoring.minAttractionDistance,
                maxAttractionDistance = authoring.maxAttractionDistance

            });
        }
    }
}