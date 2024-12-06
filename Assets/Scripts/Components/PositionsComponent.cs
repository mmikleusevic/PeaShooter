using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct PositionsComponent : IComponentData
    {
        public BlobArray<int2> positions;
    }
}