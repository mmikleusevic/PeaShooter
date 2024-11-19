using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class EnemyDamageAuthoring : MonoBehaviour
    {
        [SerializeField] private float damage;

        public class EnemyDamageBaker : Baker<EnemyDamageAuthoring>
        {
            public override void Bake(EnemyDamageAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new EnemyDamageComponent
                {
                    damagePerSecond = authoring.damage
                });
            }
        }
    }
}