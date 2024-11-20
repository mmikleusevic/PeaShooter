using Components;
using Unity.Burst;
using Unity.Entities;

namespace Jobs
{
    [BurstCompile]
    public partial struct ProjectilesUpdateJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        public ProjectilesUpdateComponent projectilesUpdateComponent;

        private void Execute([ChunkIndexInQuery] int sortKey, ref ProjectileAbilityComponent projectileAbilityComponent,
            in Entity projectileEntity)
        {
            if (projectileAbilityComponent.parentEntity != projectilesUpdateComponent.oldAbilityEntity) return;

            projectileAbilityComponent.parentEntity = projectilesUpdateComponent.newAbilityEntity;

            ecb.AddComponent<AbilityRemoveComponent>(sortKey, projectilesUpdateComponent.oldAbilityEntity);
            ecb.DestroyEntity(sortKey, projectilesUpdateComponent.projectileUpdateEntity);
        }
    }
}