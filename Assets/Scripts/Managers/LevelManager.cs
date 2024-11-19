using System;
using System.Collections.Generic;
using Interfaces;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Scenes.SceneSystem;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private EntitySceneReference[] entitySubsceneReferences;

        private readonly List<Entity> currentSubsceneEntities = new();
        private IMemoryCleaner[] memoryCleaners;

        private int subsceneIndex;
        public static LevelManager Instance { get; private set; }

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            memoryCleaners = GetComponentsInChildren<IMemoryCleaner>();
        }

        public event Action<int> OnSubSceneLoaded;

        public void LoadGameScene()
        {
            subsceneIndex = 0;

            CleanupSubscene();
            UnloadSubScene();
            LoadScene(SceneEnums.Game);
            LoadSubScene();
        }

        public void LoadMainMenu()
        {
            CleanupSubscene();
            UnloadSubScene();
            LoadScene(SceneEnums.MainMenu);
        }

        public void LoadNewSubScene()
        {
            subsceneIndex++;

            LoadSubScene();
        }

        private void LoadScene(SceneEnums sceneEnum)
        {
            SceneManager.LoadScene(sceneEnum.ToString(), LoadSceneMode.Single);
        }

        private void UnloadSubScene()
        {
            if (currentSubsceneEntities.Count > 0)
            {
                foreach (Entity currentSubsceneEntity in currentSubsceneEntities)
                    UnloadScene(World.DefaultGameObjectInjectionWorld.Unmanaged, currentSubsceneEntity,
                        UnloadParameters.DestroyMetaEntities);

                currentSubsceneEntities.Clear();
            }
        }

        private void LoadSubScene()
        {
            if (subsceneIndex >= entitySubsceneReferences.Length) return;

            currentSubsceneEntities.Add(LoadSceneAsync(World.DefaultGameObjectInjectionWorld.Unmanaged,
                entitySubsceneReferences[subsceneIndex]));

            OnSubSceneLoaded?.Invoke(subsceneIndex);
        }

        private void CleanupSubscene()
        {
            World.DefaultGameObjectInjectionWorld.EntityManager.CompleteAllTrackedJobs();

            foreach (IMemoryCleaner cleaner in memoryCleaners) cleaner.Cleanup();
        }
    }
}