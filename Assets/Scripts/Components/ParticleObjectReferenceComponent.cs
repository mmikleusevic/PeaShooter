using Unity.Entities;
using UnityEngine;

public class ParticleObjectReferenceComponent : ICleanupComponentData
{
    public GameObject gameObject;
    public byte updateTransform;
}