using Unity.Entities;

public struct PlayerExperienceComponent : IComponentData
{
    public uint points;
    public int currentLevel;
}