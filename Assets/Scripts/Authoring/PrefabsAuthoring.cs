using Unity.Entities;
using UnityEngine;

public class PrefabsAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject hpBarPrefab;
    [SerializeField] private GameObject fireflyBoltsVFXPrefab;
    [SerializeField] private GameObject veggieMaelstromVFXPrefab;

    public class PrefabsBaker : Baker<PrefabsAuthoring>
    {
        public override void Bake(PrefabsAuthoring authoring)
        {
            Entity prefabContainerEntity = GetEntity(TransformUsageFlags.None);

            DependsOn(authoring.hpBarPrefab);
            DependsOn(authoring.fireflyBoltsVFXPrefab);
            DependsOn(authoring.veggieMaelstromVFXPrefab);

            AddComponentObject(prefabContainerEntity, new Prefabs
            {
                hpBar = authoring.hpBarPrefab,
                fireflyBoltsVFX = authoring.fireflyBoltsVFXPrefab,
                veggieMaelstromVFX = authoring.veggieMaelstromVFXPrefab
            });
        }
    }
}