using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine.Rendering;
using Material = UnityEngine.Material;

[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(PresentationSystemGroup), OrderFirst = true)]
public partial class MaterialChangerSystem : SystemBase
{
    private EntitiesGraphicsSystem hybridRendererSystem;
    private Dictionary<Material, BatchMaterialID> materialMapping;

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
        BeginPresentationEntityCommandBufferSystem.Singleton ecbSingleton =
            SystemAPI.GetSingleton<BeginPresentationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(World.Unmanaged);

        foreach ((MaterialChangerComponent changer, RefRW<MaterialMeshInfo> materialMeshInfo,
                     RefRO<EnemyComponent> enemyComponent, Entity entity) in SystemAPI
                     .Query<MaterialChangerComponent, RefRW<MaterialMeshInfo>, RefRO<EnemyComponent>>()
                     .WithEntityAccess()
                     .WithNone<MaterialChangedComponent>())
            if (enemyComponent.ValueRO.moveTimer >= enemyComponent.ValueRO.moveTimerTarget)
            {
                Material material = changer.material;

                RegisterMaterial(changer.material);

                materialMeshInfo.ValueRW.MaterialID = materialMapping[material];

                ecb.AddComponent(entity, typeof(MaterialChangedComponent));
                ecb.AddComponent(entity, typeof(ActiveForCollisionComponent));
                ecb.AddComponent(entity, new GridEnemyPositionUpdateComponent
                {
                    entity = entity,
                    position = enemyComponent.ValueRO.gridPosition,
                    status = UpdateStatus.Add
                });
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
            materialMapping[material] = hybridRendererSystem.RegisterMaterial(material);
    }

    private void UnregisterMaterials()
    {
        if (hybridRendererSystem == null) return;

        foreach (KeyValuePair<Material, BatchMaterialID> materialMap in materialMapping)
            hybridRendererSystem.UnregisterMaterial(materialMap.Value);

        materialMapping.Clear();
    }
}