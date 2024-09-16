using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class EventSystemManager : MonoBehaviour
{
    private void Awake()
    {
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            gameObject.AddComponent<EventSystem>();
            gameObject.AddComponent<InputSystemUIInputModule>();
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
}