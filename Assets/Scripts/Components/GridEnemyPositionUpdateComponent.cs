using Unity.Entities;
using Unity.Mathematics;

public struct GridEnemyPositionUpdateComponent : IComponentData
{
    public Entity entity;
    public int2 oldPosition;
    public int2 position;
    public UpdateStatus status;
}

public enum UpdateStatus
{
    Add,
    Move
}