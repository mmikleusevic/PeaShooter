using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct ObstacleSpawnJob : IJobFor
{
    public EntityCommandBuffer ecb;
    public RandomDataComponent randomData;
    public NativeList<float3> positionsOccupied;

    [ReadOnly] public float distance;
    [ReadOnly] public Entity prefabToSpawn;

    public void Execute(int index)
    {
        Entity spawnedEntity = ecb.Instantiate(prefabToSpawn);
        float3 newPosition = GetValidPosition();

        ecb.SetComponent(spawnedEntity, new LocalTransform
        {
            Position = newPosition,
            Rotation = quaternion.identity,
            Scale = 1f,
        });

        positionsOccupied.Add(newPosition);
    }

    private float3 GetValidPosition()
    {
        if (positionsOccupied.IsEmpty)
        {
            return randomData.nextPosition;
        }

        while (true)
        {
            float3 candidatePosition = randomData.nextPosition;

            if (IsPositionValid(candidatePosition))
            {
                return candidatePosition;
            }
        }
    }

    private bool IsPositionValid(float3 candidatePosition)
    {
        foreach (float3 occupiedPosition in positionsOccupied)
        {
            if (MathExtensions.AreTooClose(occupiedPosition, candidatePosition, distance))
            {
                return false;
            }
        }

        return true;
    }
}