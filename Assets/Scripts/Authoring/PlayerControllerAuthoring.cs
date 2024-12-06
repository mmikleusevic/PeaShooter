using Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring
{
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
                    gridPosition = default
                });

                AddComponent(entity, new InputComponent
                {
                    moveInput = new float2(0, 0)
                });
            }
        }
    }
}