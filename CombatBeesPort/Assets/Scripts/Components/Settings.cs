using Unity.Entities;
using Unity.Mathematics;

namespace CombatBees
{
    [GenerateAuthoringComponent]
    public struct SettingsSingleton : IComponentData
    {
        // Bee settings
        public float maxSpawnSpeed;
        public float damping;
        public float aggression;
        public int jitter;
        public int teamAttraction;
        public int teamRepulsion;

        // Global settings
        public short initialResourceCount;
        public short initialBeeCount;
        public short fieldX;
        public short fieldZ;
    }
}
