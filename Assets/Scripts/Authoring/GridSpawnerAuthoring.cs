using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring
{
    public class GridSpawnerAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int2 size;

        public class GridSpawnerBaker : Baker<GridSpawnerAuthoring>
        {
            public override void Bake(GridSpawnerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                DependsOn(authoring.prefab);

                AddComponent(entity, new GridSpawnerComponent
                {
                    prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                    size = authoring.size
                });
            }
        }
    }
}