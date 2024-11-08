using Unity.Entities;
using Unity.Mathematics;

namespace Game
{
    public struct PositionsBlob
    {
        public BlobArray<int2> positions;
    }
}