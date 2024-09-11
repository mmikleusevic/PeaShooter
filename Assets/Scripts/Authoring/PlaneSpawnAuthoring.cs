using Unity.Entities;
using UnityEngine;

public class PlaneSpawnerAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    public class PlaneSpawnerBaker : Baker<PlaneSpawnerAuthoring>
    {
        public override void Bake(PlaneSpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new PlaneSpawnerComponent
            {
                prefab = GetEntity(authoring.prefab, TransformUsageFlags.None),
                position = authoring.prefab.transform.position,
                rotation = authoring.prefab.transform.rotation,
            });
        }
    }
}