using Unity.Entities;
using UnityEngine;

public class ParticleReference : ICleanupComponentData
{
    public float damage;
    public byte updateTransform;
    public GameObject value;
}