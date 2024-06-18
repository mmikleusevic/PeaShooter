using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct PlayerMovementSystem : ISystem
{
    private float planeSize;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        CheckIfPlaneSet(ref state);

        MovePlayer(ref state);
    }

    private void CheckIfPlaneSet(ref SystemState state)
    {
        if (Mathf.Approximately(planeSize, 0))
        {
            if (!SystemAPI.TryGetSingletonEntity<PlaneComponent>(out Entity planeEntity)) return;

            RefRO<PlaneComponent> entitySpawner = SystemAPI.GetComponentRO<PlaneComponent>(planeEntity);

            planeSize = entitySpawner.ValueRO.planeSize;
        }
    }

    private void MovePlayer(ref SystemState state)
    {
        foreach (var (data, input, transform) in SystemAPI.Query<RefRO<PlayerControllerComponent>, RefRO<InputComponent>, RefRW<LocalTransform>>())
        {
            float x = input.ValueRO.move.x * data.ValueRO.speed * SystemAPI.Time.DeltaTime;
            float y = input.ValueRO.move.y * data.ValueRO.speed * SystemAPI.Time.DeltaTime;
            float z = 0;

            transform.ValueRW.Position += new float3(x, y, z);

            float3 currentPosition = transform.ValueRO.Position;

            currentPosition.x = Mathf.Clamp(currentPosition.x, -planeSize, planeSize);
            currentPosition.y = Mathf.Clamp(currentPosition.y, -planeSize, planeSize);

            transform.ValueRW.Position = currentPosition;
        }
    }
}
