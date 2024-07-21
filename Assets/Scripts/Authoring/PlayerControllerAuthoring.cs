using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerControllerAuthoring : MonoBehaviour
{
    [SerializeField] private float speed = 100f;
    [SerializeField] private float rotationSpeed = 5f;

    public class PlayerControllerBaker : Baker<PlayerControllerAuthoring>
    {
        public override void Bake(PlayerControllerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new PlayerComponent
            {
                moveSpeed = authoring.speed,
                rotationSpeed = authoring.rotationSpeed,
            });

            AddComponent(entity, new InputComponent
            {
                move = new float2(0, 0)
            });
        }
    }
}