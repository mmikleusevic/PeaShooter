using Unity.Entities;
using UnityEngine;

public class ParticleReference : ICleanupComponentData
{
    public float damage;
    public bool updateTransform;
    public GameObject value;
}