using System.Collections.Generic;
using System.Linq;
using Components;
using Game;
using Unity.Collections;
using Unity.Entities;
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
        private EntityQuery projectileAbilityEntityQuery;
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

            abilityEntityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<AbilityComponent>()
                .Build(entityManager);

            projectileAbilityEntityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithPresentRW<ProjectileAbilityComponent>()
                .Build(entityManager);
        }

        public List<AbilityData> GetRandomAbilityChoices()
        {
            List<AbilityData> choices = new List<AbilityData>();

            while (choices.Count < 3)
            {
                if (allAbilities.Count == 0)
                {
                    return ReturnAndResetChosenAbilities(choices);
                }

                AbilityData randomAbility = allAbilities[Random.Range(0, allAbilities.Count)];

                if (choices.Contains(randomAbility)) continue;

                AbilityData ownedAbility = ownedAbilities.FirstOrDefault(a => a.Ability == randomAbility.Ability);
                AbilityData ownedAbilityLeveledUp = allAbilities.FirstOrDefault(a =>
                    a.Ability == ownedAbility?.Ability && a.Level == ownedAbility.Level + 1);

                if (randomAbility != ownedAbilityLeveledUp)
                {
                    choices.Add(randomAbility);
                    allAbilities.Remove(randomAbility);
                }
                else
                {
                    choices.Add(ownedAbilityLeveledUp);
                    allAbilities.Remove(ownedAbilityLeveledUp);
                }
            }

            return ReturnAndResetChosenAbilities(choices);
        }

        private List<AbilityData> ReturnAndResetChosenAbilities(List<AbilityData> choices)
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

            AbilityComponent lastAbility = default;
            Entity lastAbilityEntity = default;
            for (int i = 0; i < playerAbilities.Length; i++)
            {
                if (playerAbilities[i].ability == selectedAbility.Ability)
                {
                    lastAbility = playerAbilities[i];
                    lastAbilityEntity = abilityEntities[i];
                    break;
                }
            }

            abilityEntities.Dispose();
            playerAbilities.Dispose();

            Entity newAbilityEntity = entityManager.CreateEntity();

            entityManager.AddComponentData(newAbilityEntity, new AbilityComponent
            {
                ability = selectedAbility.Ability,
                cooldown = selectedAbility.cooldown,
                damage = selectedAbility.damage,
                level = selectedAbility.Level,
                range = selectedAbility.range,
                speed = selectedAbility.speed,
                cooldownRemaining = selectedAbility.cooldownRemaining,
                hasProjectile = (byte)(selectedAbility.HasProjectile ? 1 : 0),
                projectileEntity = lastAbility.projectileEntity,
                projectileScale = selectedAbility.projectileScale
            });

            if (selectedAbility.HasProjectile)
            {
                UpdateProjectiles(newAbilityEntity, lastAbilityEntity);
            }

            if (!lastAbilityEntity.Equals(default))
            {
                entityManager.AddComponent(lastAbilityEntity, typeof(RemoveAbilityComponent));
            }
        }

        private void UpdateProjectiles(Entity newAbilityEntity, Entity oldAbilityEntity)
        {
            NativeArray<Entity> projectileEntities = projectileAbilityEntityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<ProjectileAbilityComponent> projectiles =
                projectileAbilityEntityQuery.ToComponentDataArray<ProjectileAbilityComponent>(Allocator.Temp);

            for (int i = 0; i < projectiles.Length; i++)
            {
                ProjectileAbilityComponent projectile = projectiles[i];

                if (projectile.parentEntity == oldAbilityEntity)
                {
                    projectile.parentEntity = newAbilityEntity;

                    entityManager.SetComponentData(projectileEntities[i], projectile);
                }
            }

            projectileEntities.Dispose();
            projectiles.Dispose();
        }
    }
}