using Unity.Collections;
using Unity.Entities;
using UnityEditor;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
//This system is made because there are leaks when game is stopped from editor
//because  it doesn't close properly things in order
public partial class EditorCleanerSystem : SystemBase
{
    private EntityQuery abilityEntityQuery;
    private EntityQuery gridEntityQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        abilityEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<AbilityComponent>()
            .Build(EntityManager);

        gridEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<GridComponent>()
            .Build(EntityManager);
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
    }

    protected override void OnUpdate()
    {
    }

    protected override void OnStopRunning()
    {
        base.OnStopRunning();

        Cleanup();

#if UNITY_EDITOR
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
    }

    private void Cleanup()
    {
        Dependency.Complete();

        if (gridEntityQuery.CalculateEntityCount() > 0)
        {
            GridComponent gridComponent = gridEntityQuery.GetSingleton<GridComponent>();

            gridComponent.Dispose();

            EntityManager.DestroyEntity(gridEntityQuery);
        }

        if (abilityEntityQuery.CalculateEntityCount() > 0)
        {
            NativeArray<Entity> entities = abilityEntityQuery.ToEntityArray(Allocator.Temp);

            foreach (Entity entity in entities)
            {
                AbilityComponent ability = EntityManager.GetComponentData<AbilityComponent>(entity);

                ability.Dispose();

                EntityManager.SetComponentData(entity, ability);
            }

            entities.Dispose();

            EntityManager.DestroyEntity(abilityEntityQuery);
        }
    }

#if UNITY_EDITOR
    private void OnPlayModeStateChanged(PlayModeStateChange stateChange)
    {
        if (stateChange == PlayModeStateChange.ExitingPlayMode) Cleanup();
    }
#endif
}