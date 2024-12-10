using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class ObstacleSpawnerAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject prefab1;
        [SerializeField] private GameObject prefab2;
        [SerializeField] private int numberToSpawn;

        public class ObstacleSpawnerBaker : Baker<ObstacleSpawnerAuthoring>
        {
            public override void Bake(ObstacleSpawnerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                DependsOn(authoring.prefab1);
                DependsOn(authoring.prefab2);

                AddComponent(entity, new ObstacleSpawnerComponent
                {
                    prefab1Entity = GetEntity(authoring.prefab1, TransformUsageFlags.None),
                    prefab2Entity = GetEntity(authoring.prefab2, TransformUsageFlags.None),
                    numberToSpawn = authoring.numberToSpawn
                });

                AddComponent(entity, new DebugNameComponent
                {
                    entityName = "ObstacleSpawner"
                });
            }
        }
    }
}