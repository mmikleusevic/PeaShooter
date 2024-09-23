using Unity.Entities;

public struct ExperienceComponent : IComponentData
{
    public byte isGranted;
    public uint value;
}