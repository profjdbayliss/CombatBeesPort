using Unity.Entities;


namespace CombatBees
{
    [GenerateAuthoringComponent]
    public struct ModelInstanceComponent : IComponentData
    {
        public Entity Child;
        public bool isLaunched;
        public float timer;  
        public bool playBouncingAudio;
        public bool hasPlayedBouncingAudio;
    }
}
