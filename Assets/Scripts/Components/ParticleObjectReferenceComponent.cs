using Unity.Entities;
using UnityEngine;

public class ParticleObjectReferenceComponent : ICleanupComponentData
{
    public byte updateTransform;
    public GameObject value;
}