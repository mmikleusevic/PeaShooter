using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class AbilityPickerRNG : MonoBehaviour
{
    private EntityQuery playerEntityQuery;
    private EntityQuery prefabsQuery;
    private EntityManager entityManager;
    private PlayerExperienceSystem playerExperienceSystem;

    // TODO: Remove later
    private bool isExecuted = false;

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        playerEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<PlayerComponent>()
            .Build(entityManager);

        prefabsQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<Prefabs>()
            .Build(entityManager);

        playerExperienceSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerExperienceSystem>();

        if (playerExperienceSystem != null) playerExperienceSystem.OnLevelUp += OnLevelUp;
    }

    private void Update()
    {
        // TODO: Remove later
        if (isExecuted) return;
        AbilityChosen();
    }

    private void OnDestroy()
    {
        if (playerExperienceSystem != null) playerExperienceSystem.OnLevelUp -= OnLevelUp;
    }

    private void OnLevelUp()
    {
        // Instantiate 3-4 Random UI Cards of abilities so that player can pick a new ability or upgrade his old one
        // Make it scriptable objects probably with data to use for cards
        // After he picks invoke OnAbilityChosen to resume the game
    }

    private void AbilityChosen()
    {
        // Important that this can work like this, will fix later
        // Get the one actually picked later
        // TODO: Remove later check
        if (!prefabsQuery.HasSingleton<Prefabs>()) return;

        GameObject vfxPrefab = prefabsQuery.GetSingleton<Prefabs>().fireflyBoltsVFX;
        GameObject newVfx = Instantiate(vfxPrefab, vfxPrefab.transform.position, vfxPrefab.transform.rotation);

        if (!playerEntityQuery.HasSingleton<PlayerComponent>()) return;

        isExecuted = true;

        Entity entity = playerEntityQuery.GetSingletonEntity();

        ParticleReference particleReference = new ParticleReference
        {
            value = newVfx,
            updateTransform = false
        };

        entityManager.AddComponentData(entity, particleReference);

        GameStateManager.Instance.OnAbilityChosen();
    }
}