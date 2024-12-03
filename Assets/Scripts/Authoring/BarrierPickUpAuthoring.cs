using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class BarrierPickUpAuthoring : MonoBehaviour
    {
        [SerializeField] private float value;

        private class BarrierPickUpAuthoringBaker : Baker<BarrierPickUpAuthoring>
        {
            public override void Bake(BarrierPickUpAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new BarrierPickUpComponent
                {
                    value = authoring.value
                });
            }
        }
    }
}