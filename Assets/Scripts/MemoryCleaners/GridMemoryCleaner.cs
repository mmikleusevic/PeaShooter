using Unity.Collections;
using Unity.Entities;
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

            if (gridComponent.enemyPositions.IsCreated)
            {
                gridComponent.enemyPositions.Dispose();
            }

            if (gridComponent.gridNodes.IsCreated)
            {
                gridComponent.gridNodes.Dispose();
            }

            entityManager.DestroyEntity(gridEntityQuery);
        }
    }
}