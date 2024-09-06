using System;
using System.Collections;
using Unity.Collections;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Scenes.SceneSystem;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public event Action OnLoaded;
    public SubScene[] subscenes;

    private EntityManager entityManager;
    private EntityQuery subSceneEntitiesQuery;
    private Entity currentSceneEntity = Entity.Null;
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

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        subSceneEntitiesQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<EnemyComponent, ProjectileComponent, GridComponent>()
            .Build(entityManager);
    }

    public IEnumerator LoadGameScene()
    {
        subsceneIndex = 0;

        entityManager.DestroyEntity(subSceneEntitiesQuery.ToEntityArray(Allocator.Temp));

        UnloadSubScene();
        yield return StartCoroutine(LoadSubScene());
        yield return StartCoroutine(LoadScene(SceneEnums.Game));
    }

    public IEnumerator LoadMainMenu()
    {
        UnloadSubScene();
        yield return StartCoroutine(LoadScene(SceneEnums.MainMenu));
    }

    private IEnumerator LoadScene(SceneEnums sceneEnum)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneEnum.ToString(), LoadSceneMode.Single);

        while (!loadOperation.isDone)
        {
            yield return null;
        }

        OnLoaded?.Invoke();
    }

    private void UnloadSubScene()
    {
        UnloadScene(World.DefaultGameObjectInjectionWorld.Unmanaged, currentSceneEntity, UnloadParameters.DestroyMetaEntities);

        currentSceneEntity = Entity.Null;
    }

    private IEnumerator LoadSubScene()
    {
        yield return currentSceneEntity = LoadSceneAsync(World.DefaultGameObjectInjectionWorld.Unmanaged, subscenes[subsceneIndex].SceneGUID, new LoadParameters
        {
            Flags = SceneLoadFlags.BlockOnImport | SceneLoadFlags.BlockOnStreamIn
        });
    }
}