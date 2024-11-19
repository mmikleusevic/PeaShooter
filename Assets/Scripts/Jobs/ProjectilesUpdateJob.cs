using Components;
using Unity.Burst;
using Unity.Entities;

namespace Jobs
{
    [BurstCompile]
    public partial struct ProjectilesUpdateJob : IJobEntity
    {
        public ProjectilesUpdateComponent ProjectilesUpdate;

        public void Execute(ref ProjectileAbilityComponent projectileAbility, in Entity entity)
        {
            if (projectileAbility.parentEntity == ProjectilesUpdate.oldAbilityEntity)
            {
                projectileAbility.parentEntity = ProjectilesUpdate.newAbilityEntity;
            }
        }
    }
}