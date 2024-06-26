using Cinemachine;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CameraDOTSFollow : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private EntityManager entityManager;
    private Entity playerEntity;

    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
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

            if (cameraTargetTransform == null)
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
        var entityQuery = entityManager.CreateEntityQuery(typeof(PlayerComponent));

        if (entityQuery.CalculateEntityCount() > 0)
        {
            return entityQuery.GetSingletonEntity();
        }

        return Entity.Null;
    }
}
