using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private BallManager ballManager;
    private HexGridManager hexGridManager;
    
    private void OnEnable() {
        SingletonManager.Register(this);
    }

    private void OnDisable() {
        SingletonManager.Remove<GameManager>();
    }

    void Start() {
        ballManager = SingletonManager.Get<BallManager>();
        hexGridManager = SingletonManager.Get<HexGridManager>();
    }

    public void CheckForWinCondition() {
        if (ballManager.activeBalls.Count <= 0) {
           print("You win");
           Time.timeScale = 0;
        }
    }

    public void CheckForLoseCondition(Ball ball) {
        //change the number to a threshold variable
        if (ball.Col >= 8) {
            print("You Lose");
            Time.timeScale = 0;
        }
        
    }
}
