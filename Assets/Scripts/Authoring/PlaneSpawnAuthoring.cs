using Unity.Entities;
using UnityEngine;

public class PlaneSpawnerAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector3 position;
    [SerializeField] private Quaternion rotation;

    public class PlaneSpawnerAuthoringBaker : Baker<PlaneSpawnerAuthoring>
    {
        public override void Bake(PlaneSpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new PlaneSpawnerComponent
            {
                prefab = GetEntity(authoring.prefab, TransformUsageFlags.None),
                position = authoring.position,
                rotation = authoring.rotation,
            });
        }
    }
}