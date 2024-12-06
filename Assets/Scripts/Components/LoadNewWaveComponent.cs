#region

using Unity.Entities;

#endregion

namespace Components
{
    public struct LoadNewWaveComponent : IComponentData
    {
        public float loadTimerTarget;
    }
}