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
        [SerializeField] private EntitySceneReference[] entitySubSceneReferences;

        private readonly List<Entity> currentSubSceneEntities = new List<Entity>();
        private IMemoryCleaner[] memoryCleaners;

        private int subSceneIndex;
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
            subSceneIndex = 0;

            CleanupSubScene();
            UnloadSubScene();
            LoadScene(Scenes.Game);
            LoadSubScene();
        }

        public void LoadMainMenu()
        {
            CleanupSubScene();
            UnloadSubScene();
            LoadScene(Scenes.MainMenu);
        }

        public void LoadNewSubScene()
        {
            subSceneIndex++;

            LoadSubScene();
        }

        private void LoadScene(Scenes scene)
        {
            SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);
        }

        private void UnloadSubScene()
        {
            if (currentSubSceneEntities.Count == 0) return;

            foreach (Entity currentSubSceneEntity in currentSubSceneEntities)
                UnloadScene(World.DefaultGameObjectInjectionWorld.Unmanaged, currentSubSceneEntity,
                    UnloadParameters.DestroyMetaEntities);

            currentSubSceneEntities.Clear();
        }

        private void LoadSubScene()
        {
            if (subSceneIndex >= entitySubSceneReferences.Length) return;

            currentSubSceneEntities.Add(LoadSceneAsync(World.DefaultGameObjectInjectionWorld.Unmanaged,
                entitySubSceneReferences[subSceneIndex]));

            OnSubSceneLoaded?.Invoke(subSceneIndex);
        }

        private void CleanupSubScene()
        {
            World.DefaultGameObjectInjectionWorld.EntityManager.CompleteAllTrackedJobs();

            foreach (IMemoryCleaner cleaner in memoryCleaners) cleaner.Cleanup();
        }
    }
}