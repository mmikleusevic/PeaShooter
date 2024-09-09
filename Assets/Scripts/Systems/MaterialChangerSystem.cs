using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;
using UnityEngine.Rendering;
using Material = UnityEngine.Material;

[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
public partial class MaterialChangerSystem : SystemBase
{
    private Dictionary<Material, BatchMaterialID> materialMapping;
    private EntitiesGraphicsSystem hybridRendererSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        materialMapping = new Dictionary<Material, BatchMaterialID>();
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        hybridRendererSystem = World.GetOrCreateSystemManaged<EntitiesGraphicsSystem>();
    }

    protected override void OnStopRunning()
    {
        base.OnStopRunning();

        UnregisterMaterials();
    }

    protected override void OnUpdate()
    {
        if (SystemAPI.HasSingleton<PlayerDeadComponent>()) return;

        BeginSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(World.Unmanaged);

        Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .WithNone<MaterialChangedComponent>()
            .ForEach((MaterialChangerComponent changer, ref MaterialMeshInfo materialMeshInfo, in EnemyComponent enemyComponent, in Entity entity) =>
            {
                if (enemyComponent.moveTimer > enemyComponent.moveTimerTarget)
                {
                    Material material = changer.material;

                    RegisterMaterial(changer.material);

                    materialMeshInfo.MaterialID = materialMapping[material];

                    ecb.AddComponent(entity, new MaterialChangedComponent());
                    ecb.AddComponent(entity, new ActiveForCollisionComponent());
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