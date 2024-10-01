using Unity.Entities;
using UnityEngine;

public class ProjectileAuthoring : MonoBehaviour
{
    [SerializeField] private float maxLifetime;
    [SerializeField] private float speed;
    [SerializeField] private float damage;

    public class ProjectileBaker : Baker<ProjectileAuthoring>
    {
        public override void Bake(ProjectileAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new ProjectileComponent
            {
                hasCollided = 0,
                lifetime = authoring.maxLifetime,
                maxLifetime = authoring.maxLifetime,
                speed = authoring.speed,
                damage = authoring.damage
            });
        }
    }
}
