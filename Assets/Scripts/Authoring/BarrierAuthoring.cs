using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class BarrierAuthoring : MonoBehaviour
    {
        [SerializeField] private float barrierValue;
        [SerializeField] private float maxBarrierValue;

        private class BarrierBaker : Baker<BarrierAuthoring>
        {
            public override void Bake(BarrierAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new BarrierComponent
                {
                    maxBarrierValue = authoring.maxBarrierValue,
                    BarrierValue = authoring.barrierValue
                });
            }
        }
    }
}