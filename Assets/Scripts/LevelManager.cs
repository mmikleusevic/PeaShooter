using System;
using System.Collections;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public event Action<SubScene> OnLoad;

    public SubScene[] subscenes;
    private int subsceneIndex;

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

    public IEnumerator LoadGameScene()
    {
        subsceneIndex = 0;

        Scene currentScene = SceneManager.GetActiveScene();

        OnLoad?.Invoke(subscenes[subsceneIndex]);

        yield return StartCoroutine(UnloadLoad(currentScene, SceneEnums.Game));
    }

    public IEnumerator LoadMainMenu()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        OnLoad?.Invoke(null);

        yield return StartCoroutine(UnloadLoad(currentScene, SceneEnums.MainMenu));
    }

    private IEnumerator UnloadLoad(Scene currentScene, SceneEnums sceneEnum)
    {
        yield return StartCoroutine(LoadScene(sceneEnum));
        yield return StartCoroutine(UnloadScene(currentScene));
    }

    private IEnumerator UnloadScene(Scene currentScene)
    {
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentScene);
        while (!unloadOperation.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator LoadScene(SceneEnums sceneEnum)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneEnum.ToString(), LoadSceneMode.Additive);
        while (!loadOperation.isDone)
        {
            yield return null;
        }
    }
}