#region

using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

#endregion

namespace Jobs
{
    [BurstCompile]
    public partial struct PlaneSpawnJob : IJobEntity
    {
        public EntityCommandBuffer ecb;

        private void Execute(in PlaneSpawnerComponent planeSpawnerComponent, in Entity spawnerEntity)
        {
            Entity spawnedEntity = ecb.Instantiate(planeSpawnerComponent.prefab);

            ecb.SetName(spawnedEntity, "Plane");

            ecb.SetComponent(spawnedEntity, new LocalTransform
            {
                Position = planeSpawnerComponent.position,
                Rotation = planeSpawnerComponent.rotation,
                Scale = 1f
            });

            ecb.AddComponent(spawnedEntity, new PlaneComponent());

            ecb.DestroyEntity(spawnerEntity);
        }
    }
}