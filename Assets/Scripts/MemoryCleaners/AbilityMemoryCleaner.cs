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
            NativeArray<Entity> entities = abilityEntityQuery.ToEntityArray(Allocator.Temp);

            foreach (Entity entity in entities)
            {
                AbilityComponent ability = entityManager.GetComponentData<AbilityComponent>(entity);

                ability.Dispose();

                entityManager.SetComponentData(entity, ability);
            }

            entities.Dispose();

            entityManager.DestroyEntity(abilityEntityQuery);
        }
    }
}