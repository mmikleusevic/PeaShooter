using Unity.Entities;
using UnityEngine;

public class ExperienceWorthAuthoring : MonoBehaviour
{
    [SerializeField][Range(0, 1)] private byte cooldown;
    [SerializeField] private uint value;

    public class ExperienceWorthBaker : Baker<ExperienceWorthAuthoring>
    {
        public override void Bake(ExperienceWorthAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new EnemyExperienceWorthComponent
            {
                value = authoring.value,
            });
        }
    }
}
