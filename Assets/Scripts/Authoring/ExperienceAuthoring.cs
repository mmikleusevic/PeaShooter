using Unity.Entities;
using UnityEngine;

public class ExperienceAuthoring : MonoBehaviour
{
    [SerializeField][Range(0, 1)] private byte cooldown;
    [SerializeField] private uint value;

    public class ExperienceBaker : Baker<ExperienceAuthoring>
    {
        public override void Bake(ExperienceAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new ExperienceComponent
            {
                isGranted = authoring.cooldown,
                value = authoring.value,
            });
        }
    }
}
