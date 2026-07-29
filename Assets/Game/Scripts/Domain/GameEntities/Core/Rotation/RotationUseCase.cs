using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

namespace SampleGame
{
    [BurstCompile]
    public static class RotationUseCase
    {
        [BurstCompile]
        public static void RotateStep(
            ref LocalTransform transform,
            in float3 direction,
            in RotationSpeed speed,
            float deltaTime)
        {
            if (math.lengthsq(direction) <= 0f)
                return;

            quaternion target = quaternion.LookRotationSafe(direction, math.up());

            float dot = math.abs(math.dot(transform.Rotation.value, target.value));
            dot = math.min(dot, 1f);

            float angle = 2f * math.acos(dot);
            float maxStep = math.radians(speed.value) * deltaTime;

            float t = angle < 1e-5f ? 1f : math.min(1f, maxStep / angle);
            transform.Rotation = math.slerp(transform.Rotation, target, t);
        }
    }
}