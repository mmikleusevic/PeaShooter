using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class PickUpSpawnerAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject pickUpPrefab;

        private class PickUpSpawnerBaker : Baker<PickUpSpawnerAuthoring>
        {
            public override void Bake(PickUpSpawnerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                DependsOn(authoring.pickUpPrefab);

                AddComponent(entity, new PickUpSpawnerComponent
                {
                    prefabEntity = GetEntity(authoring.pickUpPrefab, TransformUsageFlags.Dynamic)
                });

                AddComponent(entity, new DebugNameComponent
                {
                    entityName = "PickUpSpawner"
                });
            }
        }
    }
}