using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

namespace Jobs
{
    [BurstCompile]
    public struct TriggerPickUpJob : ITriggerEventsJob
    {
        public EntityCommandBuffer ecb;
        public ComponentLookup<HealthComponent> healthComponentLookup;
        public ComponentLookup<BarrierComponent> barrierComponentLookup;
        public ComponentLookup<HealthPickUpComponent> healthPickUpComponentLookup;
        public ComponentLookup<BarrierPickUpComponent> barrierPickUpComponentLookup;

        [BurstCompile]
        private bool HasHealthPickUp(Entity entity)
        {
            return healthPickUpComponentLookup.HasComponent(entity);
        }

        [BurstCompile]
        private bool HasBarrierPickUp(Entity entity)
        {
            return barrierPickUpComponentLookup.HasComponent(entity);
        }

        [BurstCompile]
        private bool HasBarrier(Entity entity)
        {
            return barrierComponentLookup.HasComponent(entity);
        }

        [BurstCompile]
        private bool HasHealth(Entity entity)
        {
            return healthComponentLookup.HasComponent(entity);
        }

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            bool hasTriggerOccured = false;

            if (HasBarrierPickUp(entityA) && HasBarrier(entityB))
            {
                HandleBarrierPickUpTrigger(ref hasTriggerOccured, entityA, entityB);
            }
            else if (HasBarrierPickUp(entityB) && HasBarrier(entityA))
            {
                HandleBarrierPickUpTrigger(ref hasTriggerOccured, entityB, entityA);
            }

            if (hasTriggerOccured) return;

            if (HasHealthPickUp(entityA) && HasHealth(entityB))
            {
                HandleHealthPickUpTrigger(entityA, entityB);
            }
            else if (HasHealthPickUp(entityB) && HasHealth(entityA))
            {
                HandleHealthPickUpTrigger(entityB, entityA);
            }
        }

        [BurstCompile]
        private void HandleBarrierPickUpTrigger(ref bool hasTriggerOccured, Entity barrierPickUpEntity,
            Entity barrierEntity)
        {
            hasTriggerOccured = true;

            RefRW<BarrierPickUpComponent> barrierPickUpComponentRW =
                barrierPickUpComponentLookup.GetRefRW(barrierPickUpEntity);
            RefRW<BarrierComponent> barrierComponentRW = barrierComponentLookup.GetRefRW(barrierEntity);

            barrierComponentRW.ValueRW.BarrierValue += barrierPickUpComponentRW.ValueRO.value;
            barrierPickUpComponentRW.ValueRW.value = 0;

            ecb.AddComponent<DestroyComponent>(barrierPickUpEntity);
        }

        [BurstCompile]
        private void HandleHealthPickUpTrigger(Entity healthPickUpEntity, Entity healthEntity)
        {
            RefRW<HealthPickUpComponent> healthPickUpComponentRW =
                healthPickUpComponentLookup.GetRefRW(healthPickUpEntity);
            RefRW<HealthComponent> healthComponentRW = healthComponentLookup.GetRefRW(healthEntity);

            healthComponentRW.ValueRW.HitPoints += healthPickUpComponentRW.ValueRO.value;
            healthPickUpComponentRW.ValueRW.value = 0;

            ecb.AddComponent<DestroyComponent>(healthPickUpEntity);
        }
    }
}