using Unity.Entities;
using UnityEngine;

public class PlaneSpawnerAuthoring : MonoBehaviour
{
    public GameObject prefab;
    public Vector3 position;
    public Quaternion rotation;
    public float planeSize;
}

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
            planeSize = authoring.planeSize,
        });
    }
}
