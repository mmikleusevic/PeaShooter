using Unity.Entities;
using Unity.Mathematics;

namespace Game
{
    public struct PositionsComponent : IComponentData
    {
        public BlobArray<int2> positions;
    }
}