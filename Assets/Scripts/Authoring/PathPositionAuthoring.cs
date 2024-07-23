using Unity.Entities;
using UnityEngine;

public class PathPositionAuthoring : MonoBehaviour
{
    public class PathPositionAuthoringBaker : Baker<PathPositionAuthoring>
    {
        public override void Bake(PathPositionAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new PathfindingRequestComponent
            {
                start = default,
                end = default,
            });
        }
    }
}