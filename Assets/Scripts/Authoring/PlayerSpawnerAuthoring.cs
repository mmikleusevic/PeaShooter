using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class PlayerSpawnerAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private GameObject startingAbilityPrefab;

        public class PlayerSpawnerBaker : Baker<PlayerSpawnerAuthoring>
        {
            public override void Bake(PlayerSpawnerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                DependsOn(authoring.prefab);

                AddComponent(entity, new PlayerSpawnerComponent
                {
                    prefabEntity = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                    startingAbilityPrefabEntity =
                        GetEntity(authoring.startingAbilityPrefab, TransformUsageFlags.Renderable),
                    position = authoring.prefab.transform.position,
                    rotation = authoring.prefab.transform.rotation,
                    scale = authoring.prefab.transform.localScale.x
                });

                AddComponent(entity, new DebugNameComponent
                {
                    entityName = "PlayerSpawner"
                });
            }
        }
    }
}