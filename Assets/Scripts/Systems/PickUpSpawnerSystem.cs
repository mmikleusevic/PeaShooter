using Components;
using Managers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(BeginInitializationEntityCommandBufferSystem))]
    [BurstCompile]
    public partial class PickUpSpawnerSystem : SystemBase
    {
        private EntityQuery gridEntityQuery;
        private float timer;
        private float timerTarget;

        protected override void OnCreate()
        {
            base.OnCreate();

            timerTarget = 60f;

            gridEntityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<GridComponent>()
                .Build(EntityManager);

            RequireForUpdate<PickUpSpawnerComponent>();
            RequireForUpdate<PlayerAliveComponent>();
            RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            if (LevelManager.Instance) LevelManager.Instance.OnGameLoaded += OnGameLoaded;
        }

        protected override void OnStopRunning()
        {
            base.OnStopRunning();

            if (LevelManager.Instance) LevelManager.Instance.OnGameLoaded -= OnGameLoaded;
        }

        private void OnGameLoaded()
        {
            timer = 0f;
        }

        protected override void OnUpdate()
        {
            timer += SystemAPI.Time.DeltaTime;

            if (timer < timerTarget) return;

            BeginInitializationEntityCommandBufferSystem.Singleton ecbSingleton =
                SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(World.Unmanaged);

            GridComponent gridComponent = gridEntityQuery.GetSingleton<GridComponent>();

            foreach (var (pickUpSpawnerComponentRO, randomDataComponetRW) in SystemAPI
                         .Query<RefRO<PickUpSpawnerComponent>, RefRW<RandomDataComponent>>())
            {
                uint randomSeed = math.max(1, (uint)math.floor(UnityEngine.Random.value * uint.MaxValue));

                randomDataComponetRW.ValueRW.seed = new Random(randomSeed);

                int2 randomGridPosition = randomDataComponetRW.ValueRW.GetRandomPosition(gridComponent.gridNodes);
                float3 randomPosition = new float3(randomGridPosition.x, 0.5f, randomGridPosition.y);

                Entity pickUpEntity = ecb.Instantiate(pickUpSpawnerComponentRO.ValueRO.prefabEntity);

                ecb.SetComponent(pickUpEntity, new LocalTransform
                {
                    Position = randomPosition,
                    Rotation = quaternion.identity,
                    Scale = 1
                });
            }

            timer = 0f;
        }
    }
}