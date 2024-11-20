using Unity.Entities;
using Unity.Mathematics;

public struct HealthComponent : IComponentData
{
    public float maxHitPoints;

    private float hitPoints;

    public float HitPoints
    {
        readonly get => hitPoints;
        set
        {
            hitPoints = math.max(0, value);

            if (hitPoints == 0) IsDead = true;
        }
    }

    public bool IsDead { get; private set; }
}