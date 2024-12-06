#region

using System;
using Unity.Entities;

#endregion

namespace Components
{
    public struct LevelDataBlob
    {
        public BlobArray<byte> levels;
        public BlobArray<uint> experience;
    }

    public struct LevelsComponent : IComponentData, IDisposable
    {
        public BlobAssetReference<LevelDataBlob> levels;

        public void Dispose()
        {
            levels.Dispose();
        }
    }
}