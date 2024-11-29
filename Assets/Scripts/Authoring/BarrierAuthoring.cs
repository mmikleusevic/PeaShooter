using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class BarrierAuthoring : MonoBehaviour
    {
        [SerializeField] private float barrierValue;

        private class BarrierAuthoringBaker : Baker<BarrierAuthoring>
        {
            public override void Bake(BarrierAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new BarrierComponent
                {
                    maxBarrierValue = 100f,
                    BarrierValue = authoring.barrierValue
                });
            }
        }
    }
}