using UnityEngine;

public class Config : MonoBehaviour
{
    public static Config Instance { get; private set; }

    [SerializeField] private float planeSize;

    private void Start()
    {
        Instance = this;
    }

    public float GetPlaneSize()
    {
        return planeSize;
    }
}
