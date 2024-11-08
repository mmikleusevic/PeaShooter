using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Pool;

public class HealthBarPoolManager : MonoBehaviour
{
    private EntityManager entityManager;

    private ObjectPool<GameObject> healthBarPool;
    private EntityQuery hpBarEntityQuery;
    private GameObject hpBarPrefab;
    public static HealthBarPoolManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        hpBarEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<Prefabs>()
            .Build(entityManager);

        healthBarPool = new ObjectPool<GameObject>(InstantiateObject,
            bar => bar.SetActive(true),
            bar => bar.SetActive(false),
            Destroy
        );
    }

    private GameObject InstantiateObject()
    {
        if (!hpBarPrefab) hpBarPrefab = hpBarEntityQuery.GetSingleton<Prefabs>().hpBar;

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