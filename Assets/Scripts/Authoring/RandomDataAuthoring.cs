#region

using Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

#endregion

namespace Authoring
{
    public class RandomDataAuthoring : MonoBehaviour
    {
        public int2 minimumPosition;
        public int2 maximumPosition;

        public class RandomDataBaker : Baker<RandomDataAuthoring>
        {
            public override void Bake(RandomDataAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new RandomDataComponent
                {
                    seed = new Random((uint)UnityEngine.Random.Range(1, uint.MaxValue)),
                    maximumPosition = authoring.maximumPosition,
                    minimumPosition = authoring.minimumPosition
                });
            }
        }
    }
}