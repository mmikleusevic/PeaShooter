using Unity.Entities;
using UnityEngine;

public class PlayerSpawnerAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    public class PlayerSpawnerBaker : Baker<PlayerSpawnerAuthoring>
    {
        public override void Bake(PlayerSpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            DependsOn(authoring.prefab);

            AddComponent(entity, new PlayerSpawnerComponent
            {
                prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                position = authoring.prefab.transform.position,
                rotation = authoring.prefab.transform.rotation,
                scale = authoring.prefab.transform.localScale.x
            });
        }
    }
}