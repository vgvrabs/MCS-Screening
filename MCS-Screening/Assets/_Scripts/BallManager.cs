using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour {
    
    public List<GameObject> BallPrefab;
    public List<GameObject> BallPool;

    [SerializeField] private List<Ball> activeBalls;
    private void OnEnable() {
        SingletonManager.Register(this);
    }

    private void OnDisable() {
        SingletonManager.Remove<BallManager>();
    }

    void Start() {
        BallPool = new List<GameObject>(BallPrefab);
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
}
