using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour {

    public PlayerController Player;
    public int PlayerScore;
    public int YThreshold;
    public int[] XThreshold;

    public UnityEvent<int> Evt_OnScoreChanged = new();
    public UnityEvent Evt_WinGame = new UnityEvent();
    public UnityEvent Evt_LoseGame = new UnityEvent();
    private BallManager ballManager;
    private HexGridManager hexGridManager;
    private UIManager uiManager;
    
    private void OnEnable() {
        SingletonManager.Register(this);
    }

    private void OnDisable() {
        SingletonManager.Remove<GameManager>();
    }

    void Start() {
        ballManager = SingletonManager.Get<BallManager>();
        hexGridManager = SingletonManager.Get<HexGridManager>();
        uiManager = SingletonManager.Get<UIManager>();
        
        Evt_OnScoreChanged.AddListener(uiManager.SetScoreText);
        Evt_WinGame.AddListener(uiManager.OnGameWin);
        Evt_LoseGame.AddListener(uiManager.OnGameOver);
        PlayerScore = 0;
    }

    private void OnDestroy() {
        Evt_WinGame.RemoveAllListeners();
        Evt_LoseGame.RemoveAllListeners();
        Evt_OnScoreChanged.RemoveAllListeners();
    }

    public void CheckForWinCondition() {
        if (ballManager.activeBalls.Count <= 0) {
           print("You win");
           //Time.timeScale = 0;
           Player.enabled = false;
           Evt_WinGame?.Invoke();
        }
    }

    public void CheckForLoseCondition(Ball ball) {
        //checking middle coordinates and if a ball snaps there the game is triggered to be over
        if (ball.Col >= YThreshold && ball.Row ==  XThreshold[0] || 
            ball.Col >= YThreshold && ball.Row == XThreshold[1] ||
            ball.Col >= YThreshold && ball.Row == XThreshold[2]) {
            
            //print("You Lose");
            //Time.timeScale = 0;
            Evt_LoseGame?.Invoke();
            Player.enabled = false;
        }
    }

    public void AddScore(int score, int multiplier = 1) {
        PlayerScore += (score * multiplier);
        PlayerScore = Mathf.Clamp(PlayerScore, 0, 100000);
        Evt_OnScoreChanged?.Invoke(PlayerScore);
    }
}
