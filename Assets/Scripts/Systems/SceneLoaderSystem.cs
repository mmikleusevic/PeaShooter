using Unity.Entities;
using Unity.Scenes;
using static Unity.Scenes.SceneSystem;

public partial class SceneLoaderSystem : SystemBase
{
    private SubScene subscene;
    private Entity currentSceneEntity;
    private bool load;
    private bool unload;
    private bool isUnloading;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLoad += OnLoad;
        }

        subscene = null;
        currentSceneEntity = Entity.Null;
        load = false;
        unload = false;
        isUnloading = false;
    }

    protected override void OnStopRunning()
    {
        base.OnStopRunning();

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLoad -= OnLoad;
        }
    }

    protected override void OnUpdate()
    {
        if (unload)
        {
            if (IsSceneLoaded(World.Unmanaged, currentSceneEntity))
            {
                if (isUnloading) return;

                UnloadScene(World.Unmanaged, currentSceneEntity, UnloadParameters.DestroyMetaEntities);

                isUnloading = true;
            }
            else
            {
                currentSceneEntity = Entity.Null;
                load = true;
                unload = false;
                isUnloading = false;
            }
        }
        if (load)
        {
            load = false;

            if (subscene == null) return;

            subscene.AutoLoadScene = true;

            currentSceneEntity = LoadSceneAsync(World.Unmanaged, subscene.SceneGUID, new LoadParameters
            {
                Flags = SceneLoadFlags.BlockOnImport | SceneLoadFlags.BlockOnStreamIn | SceneLoadFlags.NewInstance
            });
        }
    }

    private void OnLoad(SubScene subscene)
    {
        this.subscene = subscene;
        unload = true;
    }
}