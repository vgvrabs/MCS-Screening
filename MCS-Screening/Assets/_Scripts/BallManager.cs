using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class BallManager : MonoBehaviour {
    
    public List<GameObject> BallPrefab;
    public List<GameObject> BallPool;
    public List<Ball> activeBalls;
    public List<Ball> DisconnectedBalls = new List<Ball>();

    private GameManager gameManager;
    private HexGridManager hexGridManager;
    private void OnEnable() {
        SingletonManager.Register(this);
    }

    private void OnDisable() {
        SingletonManager.Remove<BallManager>();
    }

    void Start() {
        BallPool = new List<GameObject>(BallPrefab);
        gameManager = SingletonManager.Get<GameManager>();
        hexGridManager = SingletonManager.Get<HexGridManager>();
    }

    public List<Ball> GetActiveBalls() {
        return activeBalls;
    }

    public void AddBall(Ball ball) {
        activeBalls.Add(ball);
    }

    public void RemoveBall(Ball ball) {

        if (activeBalls.Contains(ball)) {
            activeBalls.Remove(ball);
        }
        
        gameManager.CheckForWinCondition();
    }

    public void DestroyBalls(List<Ball> connectedBalls, int threshold) {

        if (connectedBalls.Count >= 3) {
            for (int i = connectedBalls.Count - 1; i >= 0; i--) {
                activeBalls.Remove(connectedBalls[i]);
                hexGridManager.HexGrid[connectedBalls[i].Row, connectedBalls[i].Col] = null;
                if (DisconnectedBalls.Contains(connectedBalls[i])) {
                    DisconnectedBalls.Remove(connectedBalls[i]);
                }
                
                Destroy(connectedBalls[i].gameObject);
            }
            
            StartCoroutine(WaitAndCheck());
        }
        
        gameManager.CheckForWinCondition();
    }
    
    
    public void UpdateBallPool() {
        //BallPool.Clear();

        //BallPool = (gameObject) activeBalls;
        /*Ball.BallColor[] ballColors = new[] {
            Ball.BallColor.Brown, Ball.BallColor.White,
            Ball.BallColor.Gold, Ball.BallColor.Red, Ball.BallColor.Blue
        };

        for (int i = 0; i < 5; i++) {
            foreach (Ball ball in activeBalls) {
                if(ball.Color != ballColors[i]) continue;

                if (ball) {
                    BallPool.Add(ball.gameObject);
                    break;
                }
            }
        }*/
        /*foreach (Ball ball in activeBalls) {
            for (int i = 0; i < 4; i++) {
                
            }
        }*/
    }
    
    public void CheckDisconnectedBalls() {
        List<Ball> disconnectedBalls = new List<Ball>();
        List<Ball> tempBalls = new List<Ball>();

        foreach (Ball ball in activeBalls) {
            if (ball.Col <= 0) continue;
            
            if (ball.TopBall == null) {
                if (!disconnectedBalls.Contains(ball)) {
                    disconnectedBalls.Add(ball);
                }
            }
        }

        foreach (Ball ball in disconnectedBalls) {
            ball.GetBallBelow(ball, tempBalls);
        }
        
        
        disconnectedBalls.AddRange(tempBalls);
        DisconnectedBalls = disconnectedBalls;
        
        
        
        //DestroyBalls(DisconnectedBalls, 0);
        
        foreach (Ball ball in DisconnectedBalls) {
            if (DisconnectedBalls.Contains(ball)) {
                if (ball == null) continue;

                if (activeBalls.Contains(ball)) {
                    activeBalls.Remove(ball);
                }
                
                hexGridManager.HexGrid[ball.Row, ball.Col] = null;
                ball.DropBall();
            }
        }
        
        
        gameManager.CheckForWinCondition();
        UpdateBallPool();
        //DisconnectedBalls.Clear();
    }
    
    public IEnumerator WaitAndCheck() {
        yield return new WaitForSeconds(0.5f);
        
        CheckDisconnectedBalls();
    }
    
}
