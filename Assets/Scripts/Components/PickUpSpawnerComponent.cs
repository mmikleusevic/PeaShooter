using Unity.Entities;

namespace Components
{
    public struct PickUpSpawnerComponent : IComponentData
    {
        public Entity prefabEntity;
    }
}