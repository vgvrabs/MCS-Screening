using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using Unity.VisualScripting;
using UnityEngine;

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
        activeBalls.Remove(ball);
        Destroy(ball.gameObject);
    }

    public void DestroyBalls(List<Ball> connectedBalls) {

        if (connectedBalls.Count >= 3) {
            for (int i = connectedBalls.Count - 1; i >= 0; i--) {
                activeBalls.Remove(connectedBalls[i]);
                hexGridManager.HexGrid[connectedBalls[i].Row, connectedBalls[i].Col] = null;
                if (DisconnectedBalls.Contains(connectedBalls[i])) {
                    DisconnectedBalls.Remove(connectedBalls[i]);
                }
                
                Destroy(connectedBalls[i].gameObject);
            }
        }
        
        gameManager.CheckForWinCondition();
        //UpdateBallPool();
        //CheckDisconnectedBalls();
        //(activeBalls[0]);
        //sRemoveDisconnectedBalls();
        //CheckIsConnectedToTop();
    }
    
    private void CheckIsConnectedToTop() {
        foreach (Ball ball in activeBalls) {
            if (ball.Col >= 1) {
                if (!ball.IsConnectedTo()) {
                    if (!activeBalls.Contains(ball) || ball == null) return;
                    //activeBalls.Remove(ball);
                    //Destroy(ball.gameObject);
                    //DropDisconnectedBalls(ball);
                    DropDisconnectedBalls(ball);
                }
            }
        }
    }

    public void UpdateBallPool() {
        BallPool.Clear();
        Ball.BallColor[] ballColors = new[] {
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
        }
        /*foreach (Ball ball in activeBalls) {
            for (int i = 0; i < 4; i++) {
                
            }
        }*/
    }

    /*public Ball CheckDisconnectedBalls(Ball firstBall) {
        List<Collider> colliders = Physics.OverlapSphere(firstBall.transform.position, firstBall.Radius).ToList();
        colliders.Remove(firstBall.Collider);

        foreach (Collider col in colliders) {
            Ball ball = col.GetComponent<Ball>();

            if (ball) {
                if(ball.TopBall == null){
                    if (!DisconnectedBalls.Contains(ball)) {
                        DisconnectedBalls.Add(ball);
                       //if (ball.Col < 1) DisconnectedBalls.Remove(ball);
                        CheckDisconnectedBalls(ball);
                    }
                }
            }
        }
        
        return null;
    }*/

    public void CheckDisconnectedBalls() {
        List<Ball> disconnectedBalls = new List<Ball>();

        foreach (Ball ball in activeBalls) {
            if (ball.Col <= 0) continue;
            
            if (ball.TopBall == null) {
                if (!disconnectedBalls.Contains(ball)) {
                    disconnectedBalls.Add(ball);
                }
            }
        }

        foreach (Ball ball in disconnectedBalls) {
            for (int y = 0; y < hexGridManager.YGridSize; y++) {
                
                if(ball == null) continue;
                
                if (hexGridManager.IsBallAt(ball.Row, y)) {
                    print("Disconnected here");
                    if (!disconnectedBalls.Contains(ball)) {
                        disconnectedBalls.Add(hexGridManager.GetBallAt(ball.Row, y));
                    }
                }
            }
        }

        DisconnectedBalls = disconnectedBalls;
        
        foreach (Ball ball in DisconnectedBalls) {
            if (DisconnectedBalls.Contains(ball)) {
                if (ball == null) continue;
                
                hexGridManager.HexGrid[ball.Row, ball.Col] = null;
                Destroy(ball.gameObject);
            }
        }
        
        DisconnectedBalls.Clear();
    }

    public void RemoveDisconnectedBalls() {
        if (DisconnectedBalls.Count <= 0) return;

        foreach (Ball ball in DisconnectedBalls) {
            if (ball) {
                if (ball.Col <= 0) return;

                Destroy(ball.gameObject);
            }
        }
    }

    public IEnumerator WaitAndCheck() {
        yield return new WaitForSeconds(1f);
        
        //CheckDisconnectedBalls(firstBall);
        CheckDisconnectedBalls();
    }

    private void DropDisconnectedBalls(Ball ball) {
        int columnSize = hexGridManager.YGridSize;
        //List<Ball> disconnectedBalls = new List<Ball>();

        for (int y = ball.Col; y < columnSize; y++) {
            if (hexGridManager.IsBallAt(ball.Row, y)) {
                DisconnectedBalls.Add(hexGridManager.GetBallAt(ball.Row, y));
                //disconnectedBalls.Add(ball);
            }
        }

        /*for (int i = disconnectedBalls.Count - 1; i >= 0; i--) {
            activeBalls.Remove(disconnectedBalls[i]);
            Destroy(disconnectedBalls[i].gameObject);
        }*/
    }
}
