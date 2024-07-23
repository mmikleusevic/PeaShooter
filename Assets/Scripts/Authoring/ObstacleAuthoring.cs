using Unity.Entities;
using UnityEngine;

public class ObstacleAuthoring : MonoBehaviour
{
    [SerializeField] private ObstacleShape shape;

    public class ObstacleBaker : Baker<ObstacleAuthoring>
    {
        public override void Bake(ObstacleAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new ObstacleComponent
            {
                position = default,
                shape = authoring.shape,
                size = new Unity.Mathematics.float2(authoring.transform.localScale.x, authoring.transform.localScale.z)
            });
        }
    }
}
