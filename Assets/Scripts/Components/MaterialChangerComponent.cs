#region

using Unity.Entities;
using UnityEngine;

#endregion

namespace Components
{
    public class MaterialChangerComponent : IComponentData
    {
        public Material material;
    }
}