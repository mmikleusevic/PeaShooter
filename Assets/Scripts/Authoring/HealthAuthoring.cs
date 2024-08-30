using Unity.Entities;
using UnityEngine;

public class HealthAuthoring : MonoBehaviour
{
    [SerializeField] private float hitPoints;

    public class HealthBaker : Baker<HealthAuthoring>
    {
        public override void Bake(HealthAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new HealthComponent
            {
                HitPoints = authoring.hitPoints,
            });
        }
    }
}
