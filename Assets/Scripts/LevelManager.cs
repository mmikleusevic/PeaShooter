using System.Collections;
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

    public IEnumerator LoadGameScene()
    {
        subsceneIndex = 0;

        yield return StartCoroutine(UnloadSubScene());
        StartCoroutine(LoadSubScene());
        yield return StartCoroutine(LoadScene(SceneEnums.Game));
    }

    public IEnumerator LoadMainMenu()
    {
        StartCoroutine(UnloadSubScene());
        yield return StartCoroutine(LoadScene(SceneEnums.MainMenu));
    }

    public void LoadNewSubScene()
    {
        subsceneIndex++;

        StartCoroutine(UnloadSubScene());
        StartCoroutine(LoadSubScene());
    }

    private IEnumerator LoadScene(SceneEnums sceneEnum)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneEnum.ToString(), LoadSceneMode.Single);

        while (!loadOperation.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator UnloadSubScene()
    {
        if (currentSubsceneEntity != Entity.Null && IsSceneLoaded(World.DefaultGameObjectInjectionWorld.Unmanaged, currentSubsceneEntity))
        {
            UnloadScene(World.DefaultGameObjectInjectionWorld.Unmanaged, currentSubsceneEntity, UnloadParameters.DestroyMetaEntities);

            yield return new WaitUntil(() => !IsSceneLoaded(World.DefaultGameObjectInjectionWorld.Unmanaged, currentSubsceneEntity));

            currentSubsceneEntity = Entity.Null;
        }
    }

    private IEnumerator LoadSubScene()
    {
        if (subsceneIndex >= entitySubsceneReferences.Length) subsceneIndex = 0;

        yield return currentSubsceneEntity = LoadSceneAsync(World.DefaultGameObjectInjectionWorld.Unmanaged, entitySubsceneReferences[subsceneIndex], new LoadParameters
        {
            Flags = SceneLoadFlags.BlockOnStreamIn | SceneLoadFlags.BlockOnImport | SceneLoadFlags.NewInstance
        });
    }
}