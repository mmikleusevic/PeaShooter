using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Scenes.SceneSystem;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private EntitySceneReference[] entitySubsceneReferences;

    private Entity currentSubsceneEntity = Entity.Null;
    private int subsceneIndex;

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

    public void LoadGameScene()
    {
        subsceneIndex = 0;

        UnloadSubScene();
        LoadScene(SceneEnums.Game);
        LoadSubScene();
    }

    public void LoadMainMenu()
    {
        UnloadSubScene();
        LoadScene(SceneEnums.MainMenu);
    }

    public void LoadNewSubScene()
    {
        subsceneIndex++;

        UnloadSubScene();
        LoadSubScene();
    }

    private void LoadScene(SceneEnums sceneEnum)
    {
        SceneManager.LoadScene(sceneEnum.ToString(), LoadSceneMode.Single);
    }

    private void UnloadSubScene()
    {
        if (currentSubsceneEntity != Entity.Null && IsSceneLoaded(World.DefaultGameObjectInjectionWorld.Unmanaged, currentSubsceneEntity))
        {
            UnloadScene(World.DefaultGameObjectInjectionWorld.Unmanaged, currentSubsceneEntity, UnloadParameters.DestroyMetaEntities);

            currentSubsceneEntity = Entity.Null;
        }
    }

    private void LoadSubScene()
    {
        if (subsceneIndex >= entitySubsceneReferences.Length) subsceneIndex = 0;

        currentSubsceneEntity = LoadSceneAsync(World.DefaultGameObjectInjectionWorld.Unmanaged, entitySubsceneReferences[subsceneIndex], new LoadParameters
        {
            Flags = SceneLoadFlags.BlockOnImport
        });
    }
}