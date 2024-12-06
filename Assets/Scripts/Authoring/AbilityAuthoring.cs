using Components;
using Game;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class AbilityAuthoring : MonoBehaviour
    {
        [SerializeField] private AbilityData abilityData;

        public class AbilityBaker : Baker<AbilityAuthoring>
        {
            public override void Bake(AbilityAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new AbilityComponent
                {
                    ability = authoring.abilityData.Ability,
                    level = authoring.abilityData.Level,
                    cooldown = authoring.abilityData.cooldown,
                    cooldownRemaining = authoring.abilityData.cooldownRemaining,
                    range = authoring.abilityData.range,
                    speed = authoring.abilityData.speed,
                    damage = authoring.abilityData.damage,
                    abilityEntity = GetEntity(authoring.abilityData.abilityPrefab, TransformUsageFlags.None),
                    scale = authoring.abilityData.scale,
                    hasProjectile = (byte)(authoring.abilityData.hasProjectile ? 1 : 0)
                });
            }
        }
    }
}