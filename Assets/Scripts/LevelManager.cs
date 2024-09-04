using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public event Action OnLoad;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator Load(SceneEnums sceneEnum)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneEnum.ToString(), LoadSceneMode.Single);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        OnLoad?.Invoke();
    }
}