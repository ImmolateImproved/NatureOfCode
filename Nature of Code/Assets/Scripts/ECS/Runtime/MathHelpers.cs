using Unity.Mathematics;

public static class MathHelpers
{
    public static float3 ClampMagnitude(float3 vector, float maxLength)
    {
        if (math.lengthsq(vector) > maxLength * maxLength)
            return math.normalizesafe(vector) * maxLength;

        return vector;
    }
}