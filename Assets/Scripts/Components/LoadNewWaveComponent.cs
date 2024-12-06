using Unity.Entities;

namespace Components
{
    public struct LoadNewWaveComponent : IComponentData
    {
        public float loadTimerTarget;
    }
}