using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    private Vector2 cameraBounds;

    public void SetPerspectiveCameraBounds(float distanceFromCamera)
    {
        Camera mainCamera = Camera.main;

        float verticalFOV = mainCamera.fieldOfView;
        float horizontalFOV = 2f * Mathf.Atan(Mathf.Tan(verticalFOV * Mathf.Deg2Rad / 2f) * mainCamera.aspect) * Mathf.Rad2Deg;

        float cameraHeight = 2f * distanceFromCamera * Mathf.Tan(verticalFOV * Mathf.Deg2Rad / 2f);
        float cameraWidth = 2f * distanceFromCamera * Mathf.Tan(horizontalFOV * Mathf.Deg2Rad / 2f);

        cameraBounds = new Vector2(cameraWidth / 2f, cameraHeight / 2f);
    }

    public Vector2 GetPerspectiveCameraBounds()
    {
        return cameraBounds;
    }
}
