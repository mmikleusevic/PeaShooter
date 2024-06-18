using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateAfter(typeof(PlaneSpawnerSystem))]
public partial struct PlayerMovementSystem : ISystem
{
    private const float epsilon = math.EPSILON;
    private float planeSize;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        planeSize = 0f;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        bool planeSet = CheckIfPlaneSet(ref state);

        if (!planeSet) return;

        MovePlayer(ref state);
    }

    [BurstCompile]
    private bool CheckIfPlaneSet(ref SystemState state)
    {
        bool approximatelyZero = MathExtensions.Approximately(planeSize, 0f);

        if (approximatelyZero)
        {
            if (!SystemAPI.TryGetSingletonEntity<PlaneComponent>(out Entity planeEntity)) return false;

            RefRO<PlaneComponent> entitySpawner = SystemAPI.GetComponentRO<PlaneComponent>(planeEntity);

            planeSize = entitySpawner.ValueRO.planeSize;

            return true;
        }

        return true;
    }

    [BurstCompile]
    private void MovePlayer(ref SystemState state)
    {
        foreach (var (data, input, transform) in SystemAPI.Query<RefRO<PlayerControllerComponent>, RefRO<InputComponent>, RefRW<LocalTransform>>())
        {
            float x = input.ValueRO.move.x * data.ValueRO.speed * SystemAPI.Time.DeltaTime;
            float y = input.ValueRO.move.y * data.ValueRO.speed * SystemAPI.Time.DeltaTime;
            float z = 0;

            transform.ValueRW.Position += new float3(x, y, z);

            float3 currentPosition = transform.ValueRO.Position;

            currentPosition.x = math.clamp(currentPosition.x, -planeSize, planeSize);
            currentPosition.y = math.clamp(currentPosition.y, -planeSize, planeSize);

            transform.ValueRW.Position = currentPosition;
        }
    }
}
