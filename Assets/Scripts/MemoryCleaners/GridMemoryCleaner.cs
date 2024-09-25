using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class GridMemoryCleaner : MonoBehaviour, IMemoryCleaner
{
    private EntityManager entityManager;
    private EntityQuery gridEntityQuery;

    private void OnEnable()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        gridEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<GridComponent>()
            .Build(entityManager);
    }

    public void Cleanup()
    {
        if (gridEntityQuery.CalculateEntityCount() == 0) return;

        if (gridEntityQuery.HasSingleton<GridComponent>())
        {
            GridComponent gridComponent = gridEntityQuery.GetSingleton<GridComponent>();
            gridComponent.gridNodes.Dispose();
            entityManager.DestroyEntity(gridEntityQuery);
        }
    }
}
