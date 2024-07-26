using Unity.Entities;
using UnityEngine;

public class ObstacleSpawnerAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int numberToSpawn;

    public class ObstacleSpawnerBaker : Baker<ObstacleSpawnerAuthoring>
    {
        public override void Bake(ObstacleSpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new ObstacleSpawnerComponent
            {
                prefab = GetEntity(authoring.prefab, TransformUsageFlags.None),
                numberToSpawn = authoring.numberToSpawn,
                scale = authoring.prefab.transform.localScale.x
            });
        }
    }
}
