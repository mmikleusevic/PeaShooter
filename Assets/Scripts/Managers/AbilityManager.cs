using System.Collections.Generic;
using System.Linq;
using Components;
using Game;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class AbilityManager : MonoBehaviour
    {
        [SerializeField] private List<AbilityData> allAbilities;

        private readonly List<AbilityData> ownedAbilities = new List<AbilityData>();
        private EntityQuery abilityEntityQuery;
        private EntityManager entityManager;
        private EntityQuery playerEntityQuery;
        public static AbilityManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            //Adding default ability to ownedAbilities and omitting it from allAbilities
            ownedAbilities.Add(allAbilities[0]);
            allAbilities.RemoveAt(0);

            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            playerEntityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<PlayerAliveComponent, LocalTransform>()
                .Build(entityManager);

            abilityEntityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<AbilityComponent>()
                .Build(entityManager);
        }

        public List<AbilityData> GetPossibleRandomAbilityChoices()
        {
            List<AbilityData> possibleAbilities = new List<AbilityData>();

            while (possibleAbilities.Count < 3 && allAbilities.Count > 0)
            {
                IGrouping<Abilities, AbilityData>[] groupedPossibleAbilities = allAbilities
                    .Where(a => !possibleAbilities.Select(pa => pa.Ability).Contains(a.Ability))
                    .OrderBy(a => a.Level)
                    .GroupBy(a => a.Ability)
                    .ToArray();

                if (groupedPossibleAbilities.Length == 0) break;

                int randomIndex = Random.Range(0, groupedPossibleAbilities.Length);
                IGrouping<Abilities, AbilityData> randomAbilityGroup = groupedPossibleAbilities[randomIndex];

                AbilityData randomAbilityData = randomAbilityGroup.FirstOrDefault();

                possibleAbilities.Add(randomAbilityData);
                allAbilities.Remove(randomAbilityData);
            }

            return ReturnAndResetPossibleAbilities(possibleAbilities);
        }

        private List<AbilityData> ReturnAndResetPossibleAbilities(List<AbilityData> choices)
        {
            allAbilities.AddRange(choices);
            return choices;
        }

        public void AcquireAbility(AbilityData selectedAbility)
        {
            AbilityData ownedAbility = ownedAbilities.FirstOrDefault(a => a.Ability == selectedAbility.Ability);

            if (ownedAbility)
            {
                if (selectedAbility.Level > ownedAbility.Level)
                {
                    ownedAbilities.Remove(ownedAbility);
                    ownedAbilities.Add(selectedAbility);
                }
            }
            else
            {
                ownedAbilities.Add(selectedAbility);
            }

            AddNewAbilityToPlayerEntity(selectedAbility);

            allAbilities.Remove(selectedAbility);
        }

        private void AddNewAbilityToPlayerEntity(AbilityData selectedAbility)
        {
            NativeArray<Entity> abilityEntities = abilityEntityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<AbilityComponent> playerAbilities =
                abilityEntityQuery.ToComponentDataArray<AbilityComponent>(Allocator.Temp);

            AbilityComponent oldAbility = default;
            Entity oldAbilityEntity = default;
            for (int i = 0; i < playerAbilities.Length; i++)
            {
                if (playerAbilities[i].ability == selectedAbility.Ability)
                {
                    oldAbility = playerAbilities[i];
                    oldAbilityEntity = abilityEntities[i];
                    break;
                }
            }

            abilityEntities.Dispose();
            playerAbilities.Dispose();

            Entity newAbilityEntity = entityManager.CreateEntity();

            LocalTransform playerLocalTransform = playerEntityQuery.GetSingleton<LocalTransform>();

            entityManager.AddComponentData(newAbilityEntity, new LocalTransform
            {
                Position = playerLocalTransform.Position,
                Rotation = playerLocalTransform.Rotation,
                Scale = 0
            });

            entityManager.AddComponentData(newAbilityEntity, new AbilityComponent
            {
                ability = selectedAbility.Ability,
                cooldown = selectedAbility.cooldown,
                damage = selectedAbility.damage,
                level = selectedAbility.Level,
                range = selectedAbility.range,
                speed = selectedAbility.speed,
                cooldownRemaining = selectedAbility.cooldownRemaining,
                scale = selectedAbility.scale,
                hasProjectile = (byte)(selectedAbility.hasProjectile ? 1 : 0),
                abilityEntity = oldAbility.abilityEntity
            });

            if (selectedAbility.hasProjectile)
            {
                Entity projectileUpdateEntity = entityManager.CreateEntity();
                entityManager.AddComponentData(projectileUpdateEntity, new ProjectilesUpdateComponent
                {
                    oldAbilityEntity = oldAbilityEntity,
                    newAbilityEntity = newAbilityEntity,
                    projectileUpdateEntity = projectileUpdateEntity
                });
            }
            else
            {
                GameObject particleSystemGameObject = Instantiate(selectedAbility.abilityPrefab,
                    new Vector3(playerLocalTransform.Position.x, 0.5f, playerLocalTransform.Position.z),
                    selectedAbility.abilityPrefab.transform.rotation);

                entityManager.AddComponentData(newAbilityEntity, new ParticleObjectReferenceComponent
                {
                    updateTransform = (byte)(selectedAbility.updatePosition ? 1 : 0),
                    gameObject = particleSystemGameObject
                });

                if (oldAbilityEntity == default) return;

                entityManager.AddComponentData(oldAbilityEntity, new AbilityRemoveComponent());
            }
        }
    }
}