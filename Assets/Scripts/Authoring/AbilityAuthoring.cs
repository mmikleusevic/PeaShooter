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
                hasProjectile = (byte)(authoring.projectilePrefab != null ? 1 : 0),
                projectileScale = authoring.projectileScale
            });
        }
    }
}
