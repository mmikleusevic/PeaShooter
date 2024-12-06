#region

using Unity.Entities;
using Unity.Mathematics;

#endregion

namespace Components
{
    public struct PositionsComponent : IComponentData
    {
        public BlobArray<int2> positions;
    }
}