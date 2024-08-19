using Unity.Entities;
using UnityEngine;

public class EnemyHealthAuthoring : MonoBehaviour
{
    [SerializeField] private float hitPoints;

    public class EnemyHealthBaker : Baker<EnemyHealthAuthoring>
    {
        public override void Bake(EnemyHealthAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new EnemyHealthComponent
            {
                HitPoints = authoring.hitPoints,
            });
        }
    }
}
