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

        timer = 0;

        loadNewWaveEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<LoadNewWaveComponent>()
            .Build(EntityManager);

        RequireForUpdate(loadNewWaveEntityQuery);
    }

    protected override void OnUpdate()
    {
        LoadNewWaveComponent loadNewWaveComponent = loadNewWaveEntityQuery.GetSingleton<LoadNewWaveComponent>();

        timer += SystemAPI.Time.DeltaTime;

        if (timer >= loadNewWaveComponent.loadTimerTarget)
        {
            timer = 0;

            if (LevelManager.Instance == null) return;

            LevelManager.Instance.LoadNewSubScene();
        }
    }
}