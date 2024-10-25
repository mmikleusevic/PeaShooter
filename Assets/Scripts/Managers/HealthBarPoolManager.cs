using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Pool;

public class HealthBarPoolManager : MonoBehaviour
{
    public static HealthBarPoolManager Instance { get; private set; }

    private ObjectPool<GameObject> healthBarPool;
    private GameObject hpBarPrefab;
    private EntityManager entityManager;
    private EntityQuery hpBarEntityQuery;

    private void Awake()
    {
        Instance = this;

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        hpBarEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<Prefabs>()
            .Build(entityManager);

        healthBarPool = new ObjectPool<GameObject>(
            createFunc: () => InstantiateObject(),
            actionOnGet: bar => bar.SetActive(true),
            actionOnRelease: bar => bar.SetActive(false),
            actionOnDestroy: Destroy
        );
    }

    private GameObject InstantiateObject()
    {
        if (!hpBarPrefab)
        {
            hpBarPrefab = hpBarEntityQuery.GetSingleton<Prefabs>().hpBar;
        }

        return Instantiate(hpBarPrefab, transform);
    }

    public GameObject GetHealthBar(Vector3 spawnPosition)
    {
        GameObject healthBar = healthBarPool.Get();
        healthBar.transform.position = spawnPosition;
        healthBar.transform.rotation = healthBar.transform.rotation;
        return healthBar;
    }

    public void ReturnHealthBar(GameObject healthBar)
    {
        healthBarPool.Release(healthBar);
    }
}
