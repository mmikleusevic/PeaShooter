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
                .WithAll<AbilityComponent>()
                .Build(entityManager);
        }

        public void Cleanup()
        {
            entityManager.DestroyEntity(abilityEntityQuery);
        }
    }
}