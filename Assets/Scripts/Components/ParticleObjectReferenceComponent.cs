using Unity.Entities;
using UnityEngine;

namespace Components
{
    public class ParticleObjectReferenceComponent : ICleanupComponentData
    {
        public GameObject gameObject;
        public byte updateTransform;
    }
}