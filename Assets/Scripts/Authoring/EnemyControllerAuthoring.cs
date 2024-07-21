using Unity.Entities;
using UnityEngine;

public class EnemyControllerAuthoring : MonoBehaviour
{
    [SerializeField] private float speed = 100f;

    public class EnemyControllerBaker : Baker<EnemyControllerAuthoring>
    {
        public override void Bake(EnemyControllerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new EnemyComponent
            {
                moveSpeed = authoring.speed,
            });
        }
    }
}