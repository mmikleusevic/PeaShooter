using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerControllerAuthoring : MonoBehaviour
{
    [SerializeField] private float speed = 100f;

    public class PlayerControllerBaker : Baker<PlayerControllerAuthoring>
    {
        public override void Bake(PlayerControllerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new PlayerControllerComponent
            {
                speed = authoring.speed
            });

            AddComponent(entity, new InputComponent
            {
                move = new float2(0, 0)
            });
        }
    }
}