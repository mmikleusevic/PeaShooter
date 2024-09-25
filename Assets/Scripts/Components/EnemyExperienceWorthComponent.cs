using Unity.Entities;

public struct EnemyExperienceWorthComponent : IComponentData
{
    public byte isGranted;
    public uint value;
}