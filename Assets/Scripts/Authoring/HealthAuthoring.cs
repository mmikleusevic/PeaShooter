using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class HealthAuthoring : MonoBehaviour
    {
        [SerializeField] private float hitPoints;
        [SerializeField] private Vector3 healthBarOffset;

        public class HealthBaker : Baker<HealthAuthoring>
        {
            public override void Bake(HealthAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new HealthComponent
                {
                    HitPoints = authoring.hitPoints,
                    maxHitPoints = authoring.hitPoints
                });

                AddComponent(entity, new HealthBarOffset
                {
                    offset = authoring.healthBarOffset
                });
            }
        }
    }
}