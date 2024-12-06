#region

using Components;
using Unity.Entities;
using UnityEngine;

#endregion

namespace Authoring
{
    public class ExperienceWorthAuthoring : MonoBehaviour
    {
        [SerializeField] private uint experience;

        public class ExperienceWorthBaker : Baker<ExperienceWorthAuthoring>
        {
            public override void Bake(ExperienceWorthAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new EnemyExperienceWorthComponent
                {
                    experience = authoring.experience
                });
            }
        }
    }
}