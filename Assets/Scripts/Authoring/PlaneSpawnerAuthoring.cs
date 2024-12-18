using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class PlaneSpawnerAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;

        public class PlaneSpawnerBaker : Baker<PlaneSpawnerAuthoring>
        {
            public override void Bake(PlaneSpawnerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                DependsOn(authoring.prefab);

                AddComponent(entity, new PlaneSpawnerComponent
                {
                    prefab = GetEntity(authoring.prefab, TransformUsageFlags.None),
                    position = authoring.prefab.transform.position,
                    rotation = authoring.prefab.transform.rotation
                });

                AddComponent(entity, new DebugNameComponent
                {
                    entityName = "PlaneSpawner"
                });
            }
        }
    }
}