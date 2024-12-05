using Unity.Collections;
using Unity.Entities;

namespace Components
{
    public struct DebugNameComponent : IComponentData
    {
        public FixedString64Bytes entityName;
    }
}