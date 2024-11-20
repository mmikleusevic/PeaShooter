using System.Collections.Generic;
using Components;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine.Rendering;
using Material = UnityEngine.Material;

namespace Systems
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(PresentationSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(BeginPresentationEntityCommandBufferSystem))]
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

            foreach ((MaterialChangerComponent changer, RefRW<MaterialMeshInfo> materialMeshInfoRW,
                         RefRO<EnemyComponent> enemyComponentRO, Entity enemyEntity) in SystemAPI
                         .Query<MaterialChangerComponent, RefRW<MaterialMeshInfo>, RefRO<EnemyComponent>>()
                         .WithEntityAccess()
                         .WithNone<MaterialChangedComponent>())
                if (enemyComponentRO.ValueRO.moveTimer >= enemyComponentRO.ValueRO.moveTimerTarget)
                {
                    Material material = changer.material;

                    RegisterMaterial(changer.material);

                    materialMeshInfoRW.ValueRW.MaterialID = materialMapping[material];

                    ecb.AddComponent(enemyEntity, typeof(MaterialChangedComponent));
                    ecb.AddComponent(enemyEntity, typeof(CollisionActiveComponent));
                    ecb.AddComponent(enemyEntity, new GridEnemyPositionUpdateComponent
                    {
                        enemyEntity = enemyEntity,
                        position = enemyComponentRO.ValueRO.gridPosition,
                        oldPosition = enemyComponentRO.ValueRO.gridPosition,
                        status = UpdateStatus.Add
                    });
                    ecb.AddComponent(enemyEntity, typeof(PositionChangedComponent));
                }
        }

        private void RegisterMaterial(Material material)
        {
            if (!materialMapping.ContainsKey(material))
                materialMapping[material] = hybridRendererSystem.RegisterMaterial(material);
        }
    }
}