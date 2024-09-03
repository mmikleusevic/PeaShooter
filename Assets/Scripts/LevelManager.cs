using System.Collections.Generic;
using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private List<EntitySceneReference> entitySceneReferences;

    private Entity currentSubSceneEntity = Entity.Null;

    private int currentSubSceneIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadStartingScene()
    {
        currentSubSceneIndex = 0;

        UnloadSubScene();
        SceneManager.LoadScene(SceneEnums.Game.ToString(), LoadSceneMode.Single);
        LoadSubScene();
    }

    public void LoadWave()
    {
        UnloadSubScene();
        LoadSubScene();
    }

    public void LoadMainMenu()
    {
        UnloadSubScene();
        SceneManager.LoadScene(SceneEnums.MainMenu.ToString(), LoadSceneMode.Single);

        currentSubSceneEntity = Entity.Null;
        currentSubSceneIndex = 0;
    }

    public void LoadSubScene()
    {
        if (currentSubSceneIndex < entitySceneReferences.Count)
        {
            currentSubSceneEntity = SceneSystem.LoadSceneAsync(World.DefaultGameObjectInjectionWorld.Unmanaged, entitySceneReferences[currentSubSceneIndex]);
        }
    }

    private void UnloadSubScene()
    {
        if (currentSubSceneEntity == Entity.Null) return;

        SceneSystem.UnloadScene(World.DefaultGameObjectInjectionWorld.Unmanaged, currentSubSceneEntity, SceneSystem.UnloadParameters.DestroyMetaEntities);
    }
}