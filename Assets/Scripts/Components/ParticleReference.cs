using Unity.Entities;
using UnityEngine;

public class ParticleReference : ICleanupComponentData
{
    public GameObject value;
    public bool updateTransform;
    public float damage;
}