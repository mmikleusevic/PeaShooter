using UnityEngine;
using UnityEngine.Pool;

namespace Managers
{
    public class HealthBarPoolManager : MonoBehaviour
    {
        [SerializeField] private GameObject hpBarPrefab;

        private ObjectPool<GameObject> healthBarObjectPool;
        public static HealthBarPoolManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            healthBarObjectPool = new ObjectPool<GameObject>(InstantiateObject,
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
            GameObject healthBarGameObject = healthBarObjectPool.Get();
            healthBarGameObject.transform.position = spawnPosition;
            healthBarGameObject.transform.rotation = healthBarGameObject.transform.rotation;
            return healthBarGameObject;
        }

        public void ReturnHealthBar(GameObject healthBar)
        {
            healthBarObjectPool.Release(healthBar);
        }
    }
}