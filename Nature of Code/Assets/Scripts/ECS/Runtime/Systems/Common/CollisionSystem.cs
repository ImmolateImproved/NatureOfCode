using Unity.Entities;
using Unity.Transforms;

public partial class CollisionSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<WorldBoundaries>();
    }

    protected override void OnUpdate()
    {
        var bounds = GetSingleton<WorldBoundaries>().value;

        Entities.ForEach((ref Translation translation, ref Velocity velocity, in NonUniformScale scale) =>
        {
            var radius = scale.Value.x / 2;

            if (translation.Value.x >= bounds.x - radius)
            {
                translation.Value.x = bounds.x - radius;
                velocity.value.x *= -1;
            }
            else if (translation.Value.x <= -bounds.x + radius)
            {
                translation.Value.x = -bounds.x + radius;
                velocity.value.x *= -1;
            }

            if (translation.Value.y >= bounds.y + radius)
            {
                translation.Value.y = bounds.y + radius;
                velocity.value.y *= -1;
            }

            if (translation.Value.y <= -bounds.y + radius)
            {
                translation.Value.y = -bounds.y + radius;
                velocity.value.y *= -1;
            }

        }).Run();
    }
}