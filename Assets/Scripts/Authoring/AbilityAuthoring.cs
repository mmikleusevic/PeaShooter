using Unity.Entities;
using UnityEngine;

public class AbilityAuthoring : MonoBehaviour
{
    [SerializeField] private float cooldown;
    [SerializeField] private float range;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private bool hasProjectile;
    [SerializeField] private float projectileScale;

    public class AbilityBaker : Baker<AbilityAuthoring>
    {
        public override void Bake(AbilityAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new AbilityComponent
            {
                cooldown = authoring.cooldown,
                cooldownRemaining = 0f,
                range = authoring.range,
                projectileEntity = GetEntity(authoring.projectilePrefab, TransformUsageFlags.None),
                hasProjectile = authoring.hasProjectile,
                projectileScale = authoring.projectileScale
            });
        }
    }
}
