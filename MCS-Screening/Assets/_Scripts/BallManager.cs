using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BallManager : MonoBehaviour {
    
    public List<GameObject> BallPrefab;
    public List<GameObject> BallPool;
    public List<Ball> activeBalls;

    private GameManager gameManager;
    private void OnEnable() {
        SingletonManager.Register(this);
    }

    private void OnDisable() {
        SingletonManager.Remove<BallManager>();
    }

    void Start() {
        BallPool = new List<GameObject>(BallPrefab);
        gameManager = SingletonManager.Get<GameManager>();
    }

    public List<Ball> GetActiveBalls() {
        return activeBalls;
    }

    public void AddBall(Ball ball) {
        activeBalls.Add(ball);
    }

    public void RemoveBall(Ball ball) {
        activeBalls.Remove(ball);
        Destroy(ball.gameObject);
    }

    public void DestroyBalls(List<Ball> connectedBalls) {

        if (connectedBalls.Count >= 3) {

            for (int i = connectedBalls.Count - 1; i >= 0; i--) {
                activeBalls.Remove(connectedBalls[i]);
                Destroy(connectedBalls[i].gameObject);
            }
        }
        
        gameManager.CheckForWinCondition();
    }
}
