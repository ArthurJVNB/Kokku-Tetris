using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private string scoreTextMeshProTag = "Score";

    private TextMeshProUGUI scoreHUD;
    private GameSceneManager sceneManager;

    private void Awake()
    {
        sceneManager = GetComponent<GameSceneManager>();
    }

    private void OnEnable()
    {
        GameplayManager.onScored += UpdateScore;
        GameSceneManager.onSceneLoaded += SetupUI;
    }

    private void OnDisable()
    {
        GameplayManager.onScored -= UpdateScore;
        GameSceneManager.onSceneLoaded -= SetupUI;
    }

    private void SetupUI(Scene scene)
    {
        Debug.LogWarning(scene + " loaded!");
        switch (scene)
        {
            case Scene.Menu:
                SetPlayButton();
                SetExitButton();
                break;
            case Scene.Gameplay:
                scoreHUD = GetScoreHUD();
                SetMenuButton();
                break;
            case Scene.Gameover:
                scoreHUD = GetScoreHUD();
                UpdateScore(GameplayManager.Score);

                SetPlayButton();
                SetExitButton();
                break;
            default:
                break;
        }
    }

    private void SetMenuButton()
    {
        GameObject.FindGameObjectWithTag("Menu Button").GetComponent<Button>().onClick.AddListener(sceneManager.Menu);
    }

    private void SetPlayButton()
    {
        GameObject.FindGameObjectWithTag("Play Button").GetComponent<Button>().onClick.AddListener(sceneManager.Play);
    }

    private void SetExitButton()
    {
        GameObject.FindGameObjectWithTag("Exit Button").GetComponent<Button>().onClick.AddListener(sceneManager.Exit);
    }

    private TextMeshProUGUI GetScoreHUD()
    {
        return GameObject.FindGameObjectWithTag(scoreTextMeshProTag).GetComponent<TextMeshProUGUI>();
    }

    private void UpdateScore(int score)
    {
        scoreHUD.text = score.ToString();
    }
}
