using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
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
        RequireForUpdate<EnemyComponent>();
    }

    protected override void OnStopRunning()
    {
        base.OnStopRunning();

        UnregisterMaterials();
    }

    protected override void OnUpdate()
    {
        if (SystemAPI.HasSingleton<PlayerDeadComponent>()) return;

        BeginPresentationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<BeginPresentationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(World.Unmanaged);

        Entities
            .WithoutBurst()
            .WithNone<MaterialChangedComponent>()
            .ForEach((MaterialChangerComponent changer, in DynamicBuffer<LinkedEntityGroup> linkedEntityGroup, in EnemyComponent enemyComponent, in Entity entity) =>
            {
                if (enemyComponent.moveTimer > enemyComponent.moveTimerTarget)
                {
                    Entity materialEntity = linkedEntityGroup[1].Value;
                    MaterialMeshInfo materialMeshInfo = SystemAPI.GetComponent<MaterialMeshInfo>(materialEntity);

                    Material material = changer.material;

                    RegisterMaterial(changer.material);

                    materialMeshInfo.MaterialID = materialMapping[material];

                    ecb.AddComponent(entity, typeof(MaterialChangedComponent));
                    ecb.AddComponent(entity, typeof(ActiveForCollisionComponent));
                    ecb.SetComponent(materialEntity, materialMeshInfo);
                }
            })
            .Run();
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