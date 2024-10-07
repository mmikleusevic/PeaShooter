using Unity.Entities;
using Unity.Mathematics;

public struct HealthComponent : IComponentData
{
    public float maxHitPoints;
    private float hitPoints;
    private bool isDead;
    public float HitPoints
    {
        readonly get => hitPoints;
        set
        {
            hitPoints = math.max(0, value);

            if (hitPoints == 0) isDead = true;
        }
    }

    public bool IsDead => isDead;
}