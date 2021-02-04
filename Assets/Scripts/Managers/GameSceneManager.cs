using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scene
{
    Menu,
    Gameplay,
    Gameover
}

public class GameSceneManager : MonoBehaviour
{
    public static Action<Scene> onSceneLoaded;

    [SerializeField]
    [Tooltip("Gameplay Scene Build Index")]
    private int menuSceneIndex;

    [SerializeField]
    [Tooltip("Gameplay Scene Build Index")]
    private int gameplaySceneIndex;

    [SerializeField]
    [Tooltip("GameOver Scene Build Index")]
    private int gameOverSceneIndex;

    private void OnEnable()
    {
        GameplayManager.onGameOver += GameOver;
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    

    private void OnDisable()
    {
        GameplayManager.onGameOver -= GameOver;
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    public void Menu()
    {
        StartCoroutine(LoadScene(menuSceneIndex));
    }

    public void Play()
    {
        StartCoroutine(LoadScene(gameplaySceneIndex));
    }

    public void GameOver()
    {
        StartCoroutine(LoadScene(gameOverSceneIndex));
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator LoadScene(int index)
    {
        int lastScene = SceneManager.GetActiveScene().buildIndex;

        AsyncOperation load = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
        yield return load;
    }

    private void SceneManager_activeSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
    {
        Scene result;

        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        if (buildIndex == gameplaySceneIndex)
            result = Scene.Gameplay;
        else if (buildIndex == gameOverSceneIndex)
            result = Scene.Gameover;
        else
            result = Scene.Menu;

        Debug.LogWarning("GameSceneManager: Loaded scene " + result);
        onSceneLoaded?.Invoke(result);
    }
}
