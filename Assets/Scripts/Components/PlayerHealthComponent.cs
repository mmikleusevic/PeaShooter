using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PlayerHealthComponent : IComponentData
{
    private bool isDead;
    private float hitPoints;

    public float HitPoints
    {
        readonly get => hitPoints;
        set
        {
            hitPoints = math.max(0, value);
            if (hitPoints == 0)
            {
                isDead = true;
                Debug.Log("Player has died");
            }
        }
    }

    public bool IsDead => isDead;
}