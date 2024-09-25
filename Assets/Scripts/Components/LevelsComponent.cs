using Unity.Entities;

public struct LevelDataBlob
{
    public BlobArray<byte> levels;
    public BlobArray<uint> experience;
}

public struct LevelsComponent : IComponentData
{
    public BlobAssetReference<LevelDataBlob> levels;
}