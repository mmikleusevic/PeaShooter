using Unity.Entities;
using Unity.Mathematics;

public struct EnemyHealthComponent : IComponentData
{
    private float hitPoints;

    public float HitPoints
    {
        get => hitPoints;
        set => hitPoints = math.max(0, value);
    }
}