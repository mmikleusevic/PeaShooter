using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class GridMemoryCleaner : MonoBehaviour, IMemoryCleaner
{
    private EntityManager entityManager;
    private EntityQuery gridEntityQuery;

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        gridEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<GridComponent>()
            .Build(entityManager);
    }

    public void Cleanup()
    {
        if (gridEntityQuery.CalculateEntityCount() != 0 && gridEntityQuery.HasSingleton<GridComponent>())
        {
            GridComponent gridComponent = gridEntityQuery.GetSingleton<GridComponent>();

            gridComponent.gridNodes.Dispose();

            foreach (KVPair<int2, NativeList<Entity>> enemyPosition in gridComponent.enemyPositions)
                enemyPosition.Value.Dispose();

            gridComponent.enemyPositions.Dispose();

            entityManager.DestroyEntity(gridEntityQuery);
        }
    }
}