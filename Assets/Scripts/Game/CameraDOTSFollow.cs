using Unity.Cinemachine;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Game
{
    public class CameraDOTSFollow : MonoBehaviour
    {
        private EntityManager entityManager;
        private Entity playerEntity;
        private EntityQuery playerEntityQuery;
        private CinemachineCamera virtualCamera;

        private void Start()
        {
            virtualCamera = GetComponent<CinemachineCamera>();

            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            playerEntityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<PlayerComponent>()
                .Build(entityManager);

            playerEntity = GetPlayerEntity();
        }

        private void LateUpdate()
        {
            if (playerEntity == Entity.Null)
            {
                playerEntity = GetPlayerEntity();

                if (playerEntity == Entity.Null) return;
            }

            if (entityManager.HasComponent<PlayerComponent>(playerEntity))
            {
                LocalTransform playerTranslation = entityManager.GetComponentData<LocalTransform>(playerEntity);
                Transform cameraTargetTransform = virtualCamera.Follow;

                if (!cameraTargetTransform)
                {
                    GameObject cameraTarget = new GameObject("CameraTarget");
                    cameraTargetTransform = cameraTarget.transform;
                    virtualCamera.Follow = cameraTargetTransform;
                    virtualCamera.LookAt = cameraTargetTransform;
                }

                cameraTargetTransform.position = playerTranslation.Position;
            }
        }

        private Entity GetPlayerEntity()
        {
            if (playerEntityQuery.CalculateEntityCount() > 0) return playerEntityQuery.GetSingletonEntity();

            return Entity.Null;
        }
    }
}