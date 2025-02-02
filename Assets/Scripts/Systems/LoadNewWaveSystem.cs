using Components;
using Managers;
using Unity.Collections;
using Unity.Entities;

namespace Systems
{
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
            RequireForUpdate<PlayerAliveComponent>();
            RequireForUpdate<PickUpSpawnerComponent>();
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            if (LevelManager.Instance) LevelManager.Instance.OnSubSceneLoaded += OnSubSceneLoaded;
        }

        protected override void OnStopRunning()
        {
            base.OnStopRunning();

            if (LevelManager.Instance) LevelManager.Instance.OnSubSceneLoaded -= OnSubSceneLoaded;
        }

        private void OnSubSceneLoaded(int obj)
        {
            timer = 0;
        }

        protected override void OnUpdate()
        {
            LoadNewWaveComponent loadNewWaveComponent = loadNewWaveEntityQuery.GetSingleton<LoadNewWaveComponent>();

            timer += SystemAPI.Time.DeltaTime;

            if (timer < loadNewWaveComponent.loadTimerTarget || !LevelManager.Instance) return;

            LevelManager.Instance.LoadNewSubScene();
        }
    }
}