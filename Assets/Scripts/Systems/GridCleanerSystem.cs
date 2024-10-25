using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
public partial class GridCleanerSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
    }

    protected override void OnUpdate() { }

    protected override void OnDestroy()
    {
        base.OnStopRunning();

        Cleanup();

#if UNITY_EDITOR
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
    }

    private void Cleanup()
    {
        if (SystemAPI.TryGetSingleton(out GridComponent gridComponent) && gridComponent.gridNodes.IsCreated)
        {
            Dependency.Complete();

            gridComponent.gridNodes.Dispose();

            foreach (KVPair<int2, NativeList<Entity>> enemyPosition in gridComponent.enemyPositions)
            {
                enemyPosition.Value.Dispose();
            }

            gridComponent.enemyPositions.Dispose();

            Entity entity = SystemAPI.GetSingletonEntity<GridComponent>();

            EntityManager.DestroyEntity(entity);
        }
    }

#if UNITY_EDITOR
    private void OnPlayModeStateChanged(PlayModeStateChange stateChange)
    {
        if (stateChange == PlayModeStateChange.ExitingPlayMode)
        {
            Cleanup();
        }
    }
#endif
}