using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class HexGridManager : MonoBehaviour {

    public List<GameObject> BallPrefab;
    public List<GameObject> BallPool;
    public int XGridSize = 10;
    public int YGridSize = 10;
    public float HexSize = 1f;
    public int BallSpawnChance;
    public float gap = 0.1f;
    public List<Ball> ConnectedBalls;
    public GameObject[,] HexGrid;

    private float hexWidth;
    private float hexHeight;
    private BallManager ballManager;
    private GameManager gameManager;

    [SerializeField] private float xOffset = 0.882f;
    [SerializeField] private float yOffset = 0.764f;
    


    void OnEnable() {
        SingletonManager.Register(this);
    }

    private void OnDisable() {
        SingletonManager.Remove<HexGridManager>();
    }

    void Start() {
        //InitializeGrid();
        ballManager = SingletonManager.Get<BallManager>();
        gameManager = SingletonManager.Get<GameManager>();
        HexGrid = new GameObject[XGridSize, YGridSize];
        GenerateGrid();
        //BallPool = new List<GameObject>(BallPrefab);
    }
    

    private void GenerateGrid() {
        //create grid based on given size and spawn balls halfway;
        for (int x = 0; x < XGridSize; x++) {
            for (int y = 0; y < YGridSize; y++) {
                float xPos = x * xOffset;

                if (y % 2 == 1) {
                    xPos += xOffset / 2f;
                }

                float yPos = y * yOffset;

                if (y <= YGridSize / 2) {
                    List<GameObject> ballPrefab = ballManager.BallPrefab;
                    int randomBallIndex = Random.Range(0, ballPrefab.Count);
                    GameObject ball = Instantiate(ballPrefab[randomBallIndex], new Vector3(xPos, -yPos, 0),
                        Quaternion.identity);
                    ball.transform.localScale = Vector3.one * HexSize;
                    Ball spawnedBall = ball.GetComponent<Ball>();
                    

                    ballManager.AddBall(spawnedBall);
                    spawnedBall.SetBallPosition(x, y);
                    HexGrid[x, y] = ball;
                    if (y >= 1 && IsBallAt(x, y - 1)) 
                        spawnedBall.TopBall = (GetBallAt(x, y - 1));
                }
            }
        }
    }
   

    public Ball GetBallAt(int x, int y) {
        if (x >= 0 && x < XGridSize && y >= 0 && y < YGridSize) {
            return HexGrid[x, y].GetComponent<Ball>();
        }
        return null;
    }

    public bool IsBallAt(int x, int y) {
        if ((x >= 0 && x <= XGridSize && y >= 0 && y <= YGridSize)) {
            if(HexGrid[x,y] != null) return true;
        }

        return false;
    }
    
    private void FloodFill(int x, int y) {
        //if (!IsBallAt(x, y)) return;
        Ball.BallColor color = GetBallAt(x, y).GetComponent<Ball>().Color;
        List<Ball> tempBalls = new List<Ball>();
        
        if(x < 0 || x >= XGridSize) return;
        if(y < 0 || y >= YGridSize) return;

        //check top left of origin
        if (IsBallAt(x - 1, y)) {
            if (GetBallAt(x - 1, y).Color == color) {
                tempBalls.Add(GetBallAt(x + 1, y));
                FloodFill(x - 1, y);
            }
        }

        //check on top
        else if (IsBallAt(x, y + 1)) {
            if (GetBallAt(x, y + 1).Color == color) {
                tempBalls.Add(GetBallAt(x, y + 1));
                FloodFill(x, y + 1);
            }
        }

        // check top right of origin
        else if (IsBallAt(x + 1, y + 1)) {
            if (GetBallAt(x + 1, y + 1).Color == color) {
                tempBalls.Add(GetBallAt(x + 1, y + 1));
                FloodFill(x + 1, y + 1);
            }
        }

        else if (IsBallAt(x + 1, y)) {
            if (GetBallAt(x + 1, y).Color == color) {
                tempBalls.Add(GetBallAt(x + 1, y));
                FloodFill(x + 1, y);
            }
        }

        else if (IsBallAt(x, y - 1)) {
            if (GetBallAt(x, y - 1).Color == color) {
                tempBalls.Add(GetBallAt(x, y - 1));
                FloodFill(x, y - 1);
            }
        }

        else if (IsBallAt(x - 1, y - 1)) {
            if (GetBallAt(x - 1, y - 1).Color == color) {
                tempBalls.Add(GetBallAt(x - 1, y - 1));
                FloodFill(x - 1, y - 1);
            }
        }

        ConnectedBalls = tempBalls;
    }
    
    public void SnapToGrid(Ball ball, Collision ballCol) {
        //get position on point of contact;
        Vector3 collisionPos = ballCol.contacts[0].point;
        //GameObject newBall = Instantiate(ball);
        //Ball spawnedBall = newBall.GetComponent<Ball>();
        ballManager.AddBall(ball); ;

        //look for the nearest coordinate on the grid 
        int row = Mathf.RoundToInt(collisionPos.x / xOffset);
        int col = Mathf.Abs(Mathf.RoundToInt(collisionPos.y / yOffset));
        
        ball.SetBallPosition(row ,col);

        HexGrid[row, col] = ball.gameObject;
        if (col >= 1) {
            if (IsBallAt(row, col - 1))
                ball.TopBall = (GetBallAt(row, col - 1));
        }
        
        
        gameManager.CheckForLoseCondition(ball);
    }
}
