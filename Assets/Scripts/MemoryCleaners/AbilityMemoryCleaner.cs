using Interfaces;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace MemoryCleaners
{
    public class AbilityMemoryCleaner : MonoBehaviour, IMemoryCleaner
    {
        private EntityQuery abilityEntityQuery;
        private EntityManager entityManager;

        private void Start()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            abilityEntityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<AbilityComponent>()
                .Build(entityManager);
        }

        public void Cleanup()
        {
            NativeArray<Entity> abilityEntities = abilityEntityQuery.ToEntityArray(Allocator.Temp);

            foreach (Entity abilityEntity in abilityEntities)
            {
                AbilityComponent abilityComponent = entityManager.GetComponentData<AbilityComponent>(abilityEntity);

                abilityComponent.Dispose();

                entityManager.SetComponentData(abilityEntity, abilityComponent);
            }

            abilityEntities.Dispose();

            entityManager.DestroyEntity(abilityEntityQuery);
        }
    }
}