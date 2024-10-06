using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
public partial class LoadNewWaveSystem : SystemBase
{
    private EntityQuery loadNewWaveEntityQuery;

    private float timer;

    protected override void OnCreate()
    {
        base.OnCreate();

        loadNewWaveEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<LoadNewWaveComponent>()
            .Build(EntityManager);

        RequireForUpdate(loadNewWaveEntityQuery);
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        if (LevelManager.Instance != null) LevelManager.Instance.OnSubSceneLoaded += OnSubSceneLoaded;
    }

    protected override void OnStopRunning()
    {
        base.OnStopRunning();

        if (LevelManager.Instance != null) LevelManager.Instance.OnSubSceneLoaded -= OnSubSceneLoaded;
    }

    private void OnSubSceneLoaded(int obj)
    {
        timer = 0;
    }

    protected override void OnUpdate()
    {
        LoadNewWaveComponent loadNewWaveComponent = loadNewWaveEntityQuery.GetSingleton<LoadNewWaveComponent>();

        timer += SystemAPI.Time.DeltaTime;

        if (timer < loadNewWaveComponent.loadTimerTarget || LevelManager.Instance == null) return;

        LevelManager.Instance.LoadNewSubScene();
    }
}