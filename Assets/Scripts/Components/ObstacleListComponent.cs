using Unity.Collections;
using Unity.Entities;

public struct ObstacleListComponent : IComponentData
{
    public NativeList<ObstacleComponent> obstacles;
}