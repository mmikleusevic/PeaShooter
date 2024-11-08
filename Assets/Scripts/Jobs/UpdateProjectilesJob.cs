using Components;
using Unity.Burst;
using Unity.Entities;

namespace Jobs
{
    [BurstCompile]
    public partial struct UpdateProjectilesJob : IJobEntity
    {
        public UpdateProjectilesComponent updateProjectiles;

        public void Execute(ref ProjectileAbilityComponent projectileAbility, in Entity entity)
        {
            if (projectileAbility.parentEntity == updateProjectiles.oldAbilityEntity)
            {
                projectileAbility.parentEntity = updateProjectiles.newAbilityEntity;
            }
        }
    }
}