using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class RandomDataAuthoring : MonoBehaviour
{
    public float3 minimumPosition;
    public float3 maximumPosition;

    public class RandomDataBaker : Baker<RandomDataAuthoring>
    {
        public override void Bake(RandomDataAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new RandomDataComponent
            {
                value = new Random((uint)UnityEngine.Random.Range(1, uint.MaxValue)),
                maximumPosition = authoring.maximumPosition,
                minimumPosition = authoring.minimumPosition
            });
        }
    }
}