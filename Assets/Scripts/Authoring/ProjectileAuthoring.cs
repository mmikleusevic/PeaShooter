using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class ProjectileAuthoring : MonoBehaviour
    {
        [SerializeField] private float maxLifetime;

        public class ProjectileBaker : Baker<ProjectileAuthoring>
        {
            public override void Bake(ProjectileAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new ProjectileComponent
                {
                    hasCollided = 0,
                    lifetime = authoring.maxLifetime,
                    maxLifetime = authoring.maxLifetime
                });
            }
        }
    }
}