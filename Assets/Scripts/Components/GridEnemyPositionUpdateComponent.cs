using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct GridEnemyPositionUpdateComponent : IComponentData
    {
        public Entity enemyEntity;
        public int2 oldPosition;
        public int2 position;
        public UpdateStatus status;
    }

    public enum UpdateStatus
    {
        Add,
        Move,
        Remove
    }
}