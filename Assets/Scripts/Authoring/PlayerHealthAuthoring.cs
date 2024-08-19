using Unity.Entities;
using UnityEngine;

public class PlayerHealthAuthoring : MonoBehaviour
{
    [SerializeField] private float hitPoints;

    public class PlayerHealthBaker : Baker<PlayerHealthAuthoring>
    {
        public override void Bake(PlayerHealthAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new PlayerHealthComponent
            {
                HitPoints = authoring.hitPoints,
            });
        }
    }
}
