using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct BarrierComponent : IComponentData
    {
        public float maxBarrierValue;

        private float barrierValue;

        public float BarrierValue
        {
            readonly get => barrierValue;
            set => barrierValue = math.clamp(value, 0f, maxBarrierValue);
        }
    }
}