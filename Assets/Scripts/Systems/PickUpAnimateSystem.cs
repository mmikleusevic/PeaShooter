using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [RequireMatchingQueriesForUpdate]
    [BurstCompile]
    public partial struct PickUpAnimateSystem : ISystem
    {
        private EntityQuery pickUpEntityQuery;
        private float rotationSpeed;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            pickUpEntityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAny<HealthPickUpComponent, BarrierPickUpComponent>()
                .WithAll<LocalTransform>()
                .Build(ref state);

            rotationSpeed = math.radians(45f);

            state.RequireForUpdate(pickUpEntityQuery);
            state.RequireForUpdate<PlayerAliveComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;

            NativeArray<LocalTransform> localTransforms =
                pickUpEntityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            NativeArray<Entity> entities = pickUpEntityQuery.ToEntityArray(Allocator.Temp);

            for (int i = 0; i < localTransforms.Length; i++)
            {
                var localTransform = localTransforms[i];
                quaternion currentRotation = localTransform.Rotation;

                quaternion rotationIncrement = quaternion.RotateY(rotationSpeed * deltaTime);
                localTransform.Rotation = math.mul(currentRotation, rotationIncrement);

                state.EntityManager.SetComponentData(entities[i], localTransform);
            }

            localTransforms.Dispose();
            entities.Dispose();
        }
    }
}