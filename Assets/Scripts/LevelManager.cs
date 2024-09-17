using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Scenes.SceneSystem;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private EntitySceneReference[] entitySubsceneReferences;

    private EntityManager entityManager;
    private EntityQuery gridEntityQuery;
    private EntityQuery linkedEntityQuery;
    private EntityQuery sceneEntityQuery;
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

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        gridEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<GridComponent>()
            .Build(entityManager);

        linkedEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAny<LinkedEntityGroup>()
            .WithNone<Prefab>()
            .Build(entityManager);

        sceneEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAny<SceneTag>()
            .WithNone<Prefab>()
            .Build(entityManager);
    }

    public void LoadGameScene()
    {
        subsceneIndex = 0;

        CleanupScene();
        UnloadSubScene();
        LoadScene(SceneEnums.Game);
        LoadSubScene();
    }

    public void LoadMainMenu()
    {
        CleanupScene();
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

    private void CleanupScene()
    {
        entityManager.CompleteAllTrackedJobs();

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        if (gridEntityQuery.HasSingleton<GridComponent>())
        {
            GridComponent gridComponent = gridEntityQuery.GetSingleton<GridComponent>();
            gridComponent.gridNodes.Dispose();

            ecb.DestroyEntity(gridEntityQuery, EntityQueryCaptureMode.AtRecord);
        }

        NativeArray<Entity> entities = linkedEntityQuery.ToEntityArray(Allocator.Temp);

        foreach (Entity entity in entities)
        {
            if (entityManager.HasComponent<LinkedEntityGroup>(entity))
            {
                DynamicBuffer<LinkedEntityGroup> linkedEntities = entityManager.GetBuffer<LinkedEntityGroup>(entity);

                foreach (var linkedEntity in linkedEntities)
                {
                    if (entityManager.Exists(linkedEntity.Value))
                    {
                        ecb.DestroyEntity(linkedEntity.Value);
                    }
                }
            }

            if (entityManager.Exists(entity))
            {
                ecb.DestroyEntity(entity);
            }
        }

        ecb.DestroyEntity(sceneEntityQuery, EntityQueryCaptureMode.AtRecord);

        ecb.Playback(entityManager);

        entities.Dispose();
        ecb.Dispose();
    }
}