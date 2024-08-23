using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Rendering;
using UnityEngine.Rendering;
using Material = UnityEngine.Material;

[RequireMatchingQueriesForUpdate]
public partial class MaterialChangerSystem : SystemBase
{
    private Dictionary<Material, BatchMaterialID> materialMapping;
    private EntitiesGraphicsSystem hybridRendererSystem;

    private void RegisterMaterial(Material material)
    {
        if (!materialMapping.ContainsKey(material))
            materialMapping[material] = hybridRendererSystem.RegisterMaterial(material);
    }

    protected override void OnStartRunning()
    {
        materialMapping = new Dictionary<Material, BatchMaterialID>();
        hybridRendererSystem = World.GetExistingSystemManaged<EntitiesGraphicsSystem>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        Entities
            .WithoutBurst()           
            .WithNone<MaterialChangedComponent>()
            .ForEach((MaterialChangerComponent changer, ref MaterialMeshInfo materialMeshInfo, ref PhysicsCollider physicsCollider, in EnemyComponent enemyComponent, in Entity entity) =>
            {
                if (enemyComponent.moveTimer > enemyComponent.moveTimerTarget)
                {
                    Material material = changer.material;

                    RegisterMaterial(changer.material);

                    materialMeshInfo.MaterialID = materialMapping[material];

                    ecb.AddComponent(entity, typeof(MaterialChangedComponent));
                    ecb.AddComponent(entity, typeof(ActiveForCollisionComponent));
                }
            })
            .Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}