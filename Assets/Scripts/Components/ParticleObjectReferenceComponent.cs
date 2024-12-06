#region

using Unity.Entities;
using UnityEngine;

#endregion

namespace Components
{
    public class ParticleObjectReferenceComponent : ICleanupComponentData
    {
        public GameObject gameObject;
        public byte updateTransform;
    }
}