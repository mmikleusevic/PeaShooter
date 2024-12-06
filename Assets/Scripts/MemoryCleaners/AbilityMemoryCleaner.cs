using Components;
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
            NativeArray<AbilityComponent> abilities =
                abilityEntityQuery.ToComponentDataArray<AbilityComponent>(Allocator.Temp);

            for (int i = 0; i < abilities.Length; i++)
            {
                AbilityComponent abilityComponent = abilities[i];

                abilityComponent.Dispose();

                abilities[i] = abilityComponent;
            }

            abilityEntityQuery.CopyFromComponentDataArray(abilities);
            abilities.Dispose();
            entityManager.DestroyEntity(abilityEntityQuery);
        }
    }
}