using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {
    
    [SerializeField] private TextMeshProUGUI gameScoreText;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private TextMeshProUGUI gameWinScoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameWinPanel;

    private GameManager gameManager;
    private void OnEnable() {
        SingletonManager.Register(this);
    }

    private void OnDisable() {
        SingletonManager.Remove<UIManager>();
    }

    void Start() {
        if(gameWinPanel) gameWinPanel.SetActive(false);
        if(gameOverPanel)gameOverPanel.SetActive(false);
        
        gameScoreText.text = 0.ToString();
        gameManager = SingletonManager.Get<GameManager>();
    }

    public void SetScoreText(int score) {
        if (score <= 0) return;
        
        gameScoreText.text = score.ToString();
    }

    public void OnGameOver() {
        if (!gameOverPanel) return;
        
        gameOverPanel.SetActive(true);
        gameOverScoreText.text = "Score: " + gameManager.PlayerScore.ToString();
    }

    public void OnGameWin() {
        if (!gameWinPanel) return;

        gameWinPanel.SetActive(true);
        gameWinScoreText.text = "Score: " + gameManager.PlayerScore.ToString();
    }

    public void OnClickGameOverReturnButton() {
        print("restart");
        SingletonManager.Get<SceneMngr>().LoadGameScene();
    }

    public void OnClickGameWinReturnButton() {
        SingletonManager.Get<SceneMngr>().LoadMainMenu();
    }
}
