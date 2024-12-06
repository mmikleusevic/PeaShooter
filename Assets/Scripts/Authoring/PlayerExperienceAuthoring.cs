#region

using Components;
using Unity.Entities;
using UnityEngine;

#endregion

namespace Authoring
{
    public class PlayerExperienceAuthoring : MonoBehaviour
    {
        public class PlayerExperienceBaker : Baker<PlayerExperienceAuthoring>
        {
            public override void Bake(PlayerExperienceAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new PlayerExperienceComponent
                {
                    points = 0,
                    currentLevel = 1
                });
            }
        }
    }
}