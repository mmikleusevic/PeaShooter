using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class ObstacleSpawnerAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int numberToSpawn;

        public class ObstacleSpawnerBaker : Baker<ObstacleSpawnerAuthoring>
        {
            public override void Bake(ObstacleSpawnerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                DependsOn(authoring.prefab);

                AddComponent(entity, new ObstacleSpawnerComponent
                {
                    prefabEntity = GetEntity(authoring.prefab, TransformUsageFlags.None),
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