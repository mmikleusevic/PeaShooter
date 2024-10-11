using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine.Rendering;
using Material = UnityEngine.Material;

[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(PresentationSystemGroup), OrderFirst = true)]
public partial class MaterialChangerSystem : SystemBase
{
    private Dictionary<Material, BatchMaterialID> materialMapping;
    private EntitiesGraphicsSystem hybridRendererSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        materialMapping = new Dictionary<Material, BatchMaterialID>();
        hybridRendererSystem = World.GetOrCreateSystemManaged<EntitiesGraphicsSystem>();

        RequireForUpdate<PlayerAliveComponent>();
        RequireForUpdate<BeginPresentationEntityCommandBufferSystem.Singleton>();
        RequireForUpdate<EnemyComponent>();
    }
    protected override void OnUpdate()
    {
        BeginPresentationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<BeginPresentationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(World.Unmanaged);

        foreach (var (changer, linkedEntityGroup, enemyComponent, entity) in SystemAPI.Query<MaterialChangerComponent, DynamicBuffer<LinkedEntityGroup>, RefRO<EnemyComponent>>()
            .WithEntityAccess()
            .WithNone<MaterialChangedComponent>())
        {
            if (enemyComponent.ValueRO.moveTimer >= enemyComponent.ValueRO.moveTimerTarget)
            {
                Entity materialEntity = linkedEntityGroup[1].Value;
                MaterialMeshInfo materialMeshInfo = SystemAPI.GetComponent<MaterialMeshInfo>(materialEntity);

                Material material = changer.material;

                RegisterMaterial(changer.material);

                materialMeshInfo.MaterialID = materialMapping[material];

                ecb.AddComponent(entity, typeof(MaterialChangedComponent));
                ecb.AddComponent(entity, typeof(ActiveForCollisionComponent));
                ecb.AddComponent(entity, new GridEnemyPositionUpdateComponent
                {
                    entity = entity,
                    position = enemyComponent.ValueRO.gridPosition,
                    status = UpdateStatus.Add
                });
                ecb.SetComponent(materialEntity, materialMeshInfo);
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        UnregisterMaterials();
    }

    private void RegisterMaterial(Material material)
    {
        if (!materialMapping.ContainsKey(material))
        {
            materialMapping[material] = hybridRendererSystem.RegisterMaterial(material);
        }
    }

    private void UnregisterMaterials()
    {
        if (hybridRendererSystem == null) return;

        foreach (KeyValuePair<Material, BatchMaterialID> materialMap in materialMapping)
        {
            hybridRendererSystem.UnregisterMaterial(materialMap.Value);
        }

        materialMapping.Clear();
    }
}