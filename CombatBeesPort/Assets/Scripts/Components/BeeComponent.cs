using Unity.Entities;
using Unity.Mathematics;

namespace CombatBees
{
    public struct BeeComponent : IComponentData
    {
        public float3 velocity;
    }
}

