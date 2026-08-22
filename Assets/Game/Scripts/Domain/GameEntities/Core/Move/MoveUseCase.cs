using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Scripts.Domain.GameEntities.Core.Move
{
    public static class MoveUseCase
    {
        public static void MoveStep(
            ref LocalTransform transform,
            in float3 direction,
            in MoveSpeed speed,
            float deltaTime
        )
        {
            if (!math.all(direction == float3.zero)) 
                transform.Position += direction * (speed.Value * deltaTime);
        }
    }
}