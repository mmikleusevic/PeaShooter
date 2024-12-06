#region

using Unity.Entities;
using Unity.Mathematics;

#endregion

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