using Unity.Entities;
using UnityEditor;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
public partial class GridCleanerSystem : SystemBase
{
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
        if (SystemAPI.TryGetSingleton(out GridComponent gridComponent))
        {
            Dependency.Complete();

            if (gridComponent.enemyPositions.IsCreated)
            {
                gridComponent.enemyPositions.Dispose();
            }

            if (gridComponent.gridNodes.IsCreated)
            {
                gridComponent.gridNodes.Dispose();
            }

            Entity entity = SystemAPI.GetSingletonEntity<GridComponent>();

            EntityManager.DestroyEntity(entity);
        }
    }

#if UNITY_EDITOR
    private void OnPlayModeStateChanged(PlayModeStateChange stateChange)
    {
        if (stateChange == PlayModeStateChange.ExitingPlayMode) Cleanup();
    }
#endif
}