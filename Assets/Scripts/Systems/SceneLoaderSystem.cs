using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Scenes;

public partial class SceneLoaderSystem : SystemBase
{
    private EntityQuery sceneEntitiesQuery;
    private Entity currentSceneEntity;
    private int currentSceneIndex;

    protected override void OnCreate()
    {
        sceneEntitiesQuery = GetEntityQuery(typeof(SceneLoader));
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLoad += OnLoad;
        }

        currentSceneEntity = Entity.Null;
        currentSceneIndex = 0;
    }

    protected override void OnStopRunning()
    {
        base.OnStopRunning();

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLoad -= OnLoad;
        }
    }

    protected override void OnUpdate() { }  

    private void UnloadSubScene()
    {
        if (currentSceneEntity != Entity.Null)
        {
            SceneSystem.UnloadScene(World.Unmanaged, currentSceneEntity, SceneSystem.UnloadParameters.DestroyMetaEntities);

            currentSceneEntity = Entity.Null;
        }
    }
    
    public void LoadSubScene()
    {
        NativeArray<SceneLoader> sceneEntities = sceneEntitiesQuery.ToComponentDataArray<SceneLoader>(Allocator.Temp);

        if (currentSceneIndex < sceneEntities.Length)
        {
            EntitySceneReference entitySceneReference = sceneEntities[currentSceneIndex].SceneReference;

            currentSceneEntity = SceneSystem.LoadSceneAsync(World.Unmanaged, entitySceneReference, new SceneSystem.LoadParameters
            {
                Flags = SceneLoadFlags.BlockOnImport
            });

            currentSceneIndex++;
        }
        else
        {
            LevelManager.Instance.Load(SceneEnums.Game);

            currentSceneIndex = 0;
        }

        sceneEntities.Dispose();
    }

    private void OnLoad()
    {
        UnloadSubScene();
        LoadSubScene();
    }
}