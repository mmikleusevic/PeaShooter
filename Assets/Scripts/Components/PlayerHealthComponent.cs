using Unity.Entities;
using Unity.Mathematics;

public struct PlayerHealthComponent : IComponentData
{
    public bool isDead;
    private float hitPoints;

    public float HitPoints
    {
        get => hitPoints;
        set => hitPoints = math.max(0, value);
    }
}