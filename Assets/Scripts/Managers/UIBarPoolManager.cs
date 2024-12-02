using UnityEngine;
using UnityEngine.Pool;

namespace Managers
{
    public class UIBarPoolManager : MonoBehaviour
    {
        [SerializeField] private GameObject uiBarPrefab;

        private ObjectPool<GameObject> uiBarObjectPool;
        public static UIBarPoolManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            uiBarObjectPool = new ObjectPool<GameObject>(InstantiateObject,
                bar => bar.SetActive(true),
                bar => bar.SetActive(false),
                Destroy
            );
        }

        private GameObject InstantiateObject()
        {
            return Instantiate(uiBarPrefab, transform);
        }

        public GameObject GetUIBar(Vector3 spawnPosition)
        {
            GameObject uiBarGameObject = uiBarObjectPool.Get();
            uiBarGameObject.transform.position = spawnPosition;
            uiBarGameObject.transform.rotation = uiBarGameObject.transform.rotation;
            return uiBarGameObject;
        }

        public void ReturnUIBar(GameObject healthBar)
        {
            uiBarObjectPool.Release(healthBar);
        }
    }
}