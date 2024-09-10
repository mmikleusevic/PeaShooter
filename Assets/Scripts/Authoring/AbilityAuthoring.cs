using Unity.Entities;
using UnityEngine;

public class AbilityAuthoring : MonoBehaviour
{
    [SerializeField] private float cooldown;
    [SerializeField] private float range;
    [SerializeField] private GameObject projectilePrefab = null;
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
                projectileEntity = GetEntity(authoring.projectilePrefab, TransformUsageFlags.Dynamic),
                hasProjectile = authoring.projectilePrefab != null ? true : false,
                projectileScale = authoring.projectileScale
            });
        }
    }
}
