using Unity.Entities;
using Unity.Mathematics;

public struct HealthComponent : IComponentData
{
    private float hitPoints;

    public float HitPoints
    {
        readonly get => hitPoints;
        set
        {
            hitPoints = math.max(0, value);
        }
    }
}