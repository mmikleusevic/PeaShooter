using Unity.Entities;
using UnityEngine;

public class PrefabsAuthoring : MonoBehaviour
{
    public GameObject hpBarPrefab;

    public class PrefabsBaker : Baker<PrefabsAuthoring>
    {
        public override void Bake(PrefabsAuthoring authoring)
        {
            Entity prefabContainerEntity = GetEntity(TransformUsageFlags.None);

            AddComponentObject(prefabContainerEntity, new UIPrefabs
            {
                hpBar = authoring.hpBarPrefab
            });
        }
    }
}