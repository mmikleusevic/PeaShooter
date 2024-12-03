using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class HealthPickUpAuthoring : MonoBehaviour
    {
        [SerializeField] private float value;

        private class HealthPickUpAuthoringBaker : Baker<HealthPickUpAuthoring>
        {
            public override void Bake(HealthPickUpAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new HealthPickUpComponent
                {
                    value = authoring.value
                });
            }
        }
    }
}