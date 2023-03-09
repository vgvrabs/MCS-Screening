using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Update = Unity.VisualScripting.Update;

public class BallManager : MonoBehaviour {

    public int ScorePerBall;
    public List<GameObject> BallPrefab;
    public List<GameObject> BallPool;
    public List<Ball> activeBalls;
    public List<Ball> DisconnectedBalls = new List<Ball>();

    public int MinimumMatch;

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

    public void DestroyBalls(List<Ball> connectedBalls) {

        if (connectedBalls.Count >= MinimumMatch) {
            for (int i = connectedBalls.Count - 1; i >= 0; i--) {
                activeBalls.Remove(connectedBalls[i]);

                if (!hexGridManager.IsBallAt(connectedBalls[i].Row, connectedBalls[i].Col)) {
                    hexGridManager.HexGrid[connectedBalls[i].Row, connectedBalls[i].Col] = null;
                }
                
                if (DisconnectedBalls.Contains(connectedBalls[i])) {
                    DisconnectedBalls.Remove(connectedBalls[i]);
                }
                
                Destroy(connectedBalls[i].gameObject);
            }
            
            //CheckDisconnectedBalls();
            StartCoroutine(WaitAndCheck());
            
            gameManager.AddScore(ScorePerBall, connectedBalls.Count);
        }
        
        //gameManager.AddScore(ScorePerBall, connectedBalls.Count);
        gameManager.CheckForWinCondition();
    }
    
    public void CheckAvailableColors() {
        BallPool.Clear();
        List<Ball.BallColor> ballColors = new List<Ball.BallColor>();

        Ball.BallColor[] colorList = {
            Ball.BallColor.Brown, Ball.BallColor.White, 
            Ball.BallColor.Gold, Ball.BallColor.Red, Ball.BallColor.Blue
        };

        
        foreach (Ball b in activeBalls) {
            ballColors.Add(b.Color);
        }


        for (int i = 0; i < colorList.Length; i++) {
            if (ballColors.Contains(BallPrefab[i].GetComponent<Ball>().Color))
            {
                BallPool.Add(BallPrefab[i]);
            }
        }
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
        
        // gameManager.CheckForWinCondition();
        DisconnectedBalls.Clear();
    }
    
    public IEnumerator WaitAndCheck() {
        //yield return new WaitForSeconds(0.1f);
        yield return new WaitForEndOfFrame();

        CheckDisconnectedBalls();
    }
    
}
