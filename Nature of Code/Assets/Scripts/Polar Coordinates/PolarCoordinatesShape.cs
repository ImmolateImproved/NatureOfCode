using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using Random = Unity.Mathematics.Random;

public class PolarCoordinatesShape : MonoBehaviour
{
    public LineRenderer line;

    public Vector2 center;
    public float r;
    [Range(1, 180)]
    public float angleBetweenPoints;

    public float noise;

    private Random random;

    private void Awake()
    {
        random = new Random(12341);
    }

    void Update()
    {
        //if (!Input.GetKeyDown(KeyCode.Space))
        //    return;

        DrawCircle();
    }

    private void DrawCircle()
    {
        var vertexCount = (int)(360 / angleBetweenPoints);

        var vertices = new NativeArray<Vector3>(vertexCount, Allocator.TempJob);

        var randomRef = new NativeReference<Random>(random, Allocator.TempJob);

        var circleVertexJob = new CircleVertexJob
        {
            center = center,
            r = r,
            angleBetweenPoints = angleBetweenPoints,
            noise = noise,
            vertices = vertices,
            randomRef = randomRef

        };

        circleVertexJob.Run();

        line.positionCount = vertexCount;
        line.SetPositions(vertices);
        vertices.Dispose();

        random = circleVertexJob.randomRef.Value;
        randomRef.Dispose();
    }

    [BurstCompile]
    private struct CircleVertexJob : IJob
    {
        public NativeArray<Vector3> vertices;

        public float2 center;
        public float r;
        public float angleBetweenPoints;

        public float noise;

        public NativeReference<Random> randomRef;

        public void Execute()
        {
            angleBetweenPoints = math.radians(angleBetweenPoints);

            for (int i = 0; i < vertices.Length; i++)
            {
                var angle = angleBetweenPoints * i;

                var rng = randomRef.Value;
                var r1 = r + rng.NextFloat(-noise, noise);
                randomRef.Value = rng;

                math.sincos(angle, out var sin, out var cos);

                var point = new float2(cos, sin) * r1 + center;

                vertices[i] = new float3(point, 0);
            }
        }
    }
}