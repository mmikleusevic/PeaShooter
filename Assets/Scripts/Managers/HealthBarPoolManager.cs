using UnityEngine;
using UnityEngine.Pool;

namespace Managers
{
    public class HealthBarPoolManager : MonoBehaviour
    {
        [SerializeField] private GameObject hpBarPrefab;

        private ObjectPool<GameObject> healthBarPool;

        public static HealthBarPoolManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            healthBarPool = new ObjectPool<GameObject>(InstantiateObject,
                bar => bar.SetActive(true),
                bar => bar.SetActive(false),
                Destroy
            );
        }

        private GameObject InstantiateObject()
        {
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
}