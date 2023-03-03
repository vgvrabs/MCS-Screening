using System;
using System.Collections;
using System.Collections.Generic;
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

    private float hexWidth;
    private float hexHeight;
    private GameObject[,] hexGrid;
    private BallManager ballManager;

    [SerializeField] private float xOffset = 0.882f;
    [SerializeField] private float yOffset = 0.764f;
    [SerializeField] private List<GameObject> connectedBalls;


    void OnEnable() {
        SingletonManager.Register(this);
    }

    private void OnDisable() {
        SingletonManager.Remove<HexGridManager>();
    }

    void Start() {
        //InitializeGrid();
        ballManager = SingletonManager.Get<BallManager>();
        hexGrid = new GameObject[XGridSize, YGridSize];
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
                    GameObject ball = Instantiate(ballPrefab[randomBallIndex], new Vector3(xPos, yPos, 0),
                        Quaternion.identity);
                    ball.transform.localScale = Vector3.one * HexSize;
                    Ball spawnedBall = ball.GetComponent<Ball>();

                    ballManager.AddBall(spawnedBall);
                    spawnedBall.SetBallPosition(x, y);
                    hexGrid[x, y] = ball;
                }
            }
        }
    }
    /*public void SnapToGrid(GameObject ball, Collision ballCol) {
        Vector3 collisionPosition = ballCol.contacts[0].point;
        GameObject spawnedBall = Instantiate(ball);
        //collisionPosition = new Vector3(collisionPosition.x + offset, collisionPosition.y + offset, collisionPosition.z);
        spawnedBall.transform.position = collisionPosition;
        
        //CheckConnectedBalls();
    }*/

    public List<GameObject> GetConnectedBalls(GameObject ball, GameObject spawnedBall) {
        List<GameObject> connectedBalls = new List<GameObject>();
        Ball.BallColor color = ball.GetComponent<Ball>().Color;
        
        //find all connected bubbles of the same color

        Queue<GameObject> floodFillQueue = new Queue<GameObject>();
        HashSet<GameObject> visited = new HashSet<GameObject>();
        floodFillQueue.Enqueue(ball);
        visited.Add(ball);

        while (floodFillQueue.Count > 0) {
            GameObject currentBall = floodFillQueue.Dequeue();
            connectedBalls.Add(currentBall);
            Ball newBall = currentBall.GetComponent<Ball>();
            List<GameObject> neighbors = GetNeighboringBalls(newBall.Row, newBall.Col);

            foreach (GameObject neighbor in neighbors) {
                if (!visited.Contains(neighbor) && neighbor.GetComponent<Ball>().Color == color) {
                    floodFillQueue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }
        
        //connectedBalls.Add(spawnedBall);
        return connectedBalls;
    }

    public GameObject GetBallAt(int x, int y) {
        if (x >= 0 && x < XGridSize && y >= 0 && y < YGridSize) {
            return hexGrid[x, y];
        }
        return null;
    }

    private List<GameObject> GetNeighboringBalls(int row, int col) {
        List<GameObject> neighbors = new List<GameObject>();
        
        if (col > 0) {
            //checking left neighbor
            GameObject leftNeighbor = GetBallAt(row, col - 1); 
            leftNeighbor = GetBallAt(row, col - 1);
            
            if(leftNeighbor != null) neighbors.Add(leftNeighbor);
        }

        if (col < YGridSize - 1) {
            //checking right neighbor
            GameObject rightNeigbor = GetBallAt(row, col + 1);
            
            if(rightNeigbor != null) neighbors.Add(rightNeigbor);
        }
        
        //check neighboring row above
        bool isRowAboveOdd = (row % 2 != 0);

        if (row > 0 && col >= 0 && col < YGridSize) {
            //check the neighbor on the upper left
            if (isRowAboveOdd && col > 0) {
                GameObject upperLeftNeighbor = GetBallAt(row - 1, col - 1);
                if(upperLeftNeighbor != null) neighbors.Add(upperLeftNeighbor);
            }
            
            else if (!isRowAboveOdd && col < YGridSize - 1) {
                //check upper right neighbor
                GameObject upperRightNeighbor = GetBallAt(row - 1, col + 1);
                if(upperRightNeighbor != null) neighbors.Add(upperRightNeighbor);
            }

            GameObject upperNeighbor = GetBallAt(row + 1, col);
            if(upperNeighbor != null) neighbors.Add(upperNeighbor);
        }

        bool isRowBelowOdd = (row % 2 == 0);

        if (row < XGridSize - 1 && col >= 0 && col < YGridSize) {
            if (isRowBelowOdd && col > 0) {
                GameObject lowerLeftNeighbor = GetBallAt(row + 1, col - 1);
                
                if(lowerLeftNeighbor != null) neighbors.Add(lowerLeftNeighbor);
            }
            
            else if (!isRowBelowOdd && col < YGridSize - 1) {
                GameObject lowerRightNeigbor = GetBallAt(row + 1, col + 1);
                if(lowerRightNeigbor != null) neighbors.Add(lowerRightNeigbor);
            }

            GameObject lowerNeighbor = GetBallAt(row + 1, col);
            if(lowerNeighbor != null) neighbors.Add(lowerNeighbor);
        }
        return neighbors;
    }

    private int GetRowFromPosition(Vector3 position) {
        float rowHeight = XGridSize;
        float y = position.y;

        int row = Mathf.RoundToInt(y / rowHeight);
        return row;
    }

    private int GetColFromPosition(Vector3 position) {
        float colWidth = yOffset;
        float x = position.x;
        int row = GetRowFromPosition(position);
        int col;

        if (row % 2 == 0) {
            col = Mathf.RoundToInt((x + colWidth / 2f) / colWidth);
        }

        else {
            col = Mathf.RoundToInt(x / colWidth);
        }

        return col;

    }

    public void SnapToGrid(GameObject ball, Collision ballCol) {
        //get position on point of contact;
        Vector3 collisionPos = ball.GetComponent<Ball>().GetCollisionPosition();
        GameObject newBall = Instantiate(ball);
        Ball spawnedBall = newBall.GetComponent<Ball>();
        ballManager.AddBall(spawnedBall);
        //activeBalls.Add(spawnedBall);

        //look for the nearest coordinate on the grid 
        int row = Mathf.RoundToInt(collisionPos.x / xOffset);
        //int row = GetRowFromPosition(ball.transform.position);
        //int col = GetColFromPosition(ball.transform.position);
        int col = Mathf.Abs(Mathf.RoundToInt(collisionPos.y / yOffset));
        
        spawnedBall.SetBallPosition(row ,col);
        //spawnedBall.Evt_OnHit.RemoveAllListeners();
        print(row.ToString() + col.ToString());
        hexGrid[row, col] = ball;
        
        
        //CheckForMatches(ballCol.gameObject.GetComponent<Ball>());
        connectedBalls = GetConnectedBalls(ball, newBall);
        
        /*foreach (GameObject b in connectedBalls) {
            if (b != null) {
                b.SetActive(false);
                Destroy(b, 1f);
            }
        }
        connectedBalls.Clear();*/
    }

    public void CheckForMatches(Ball collidedBall) {
        //creates a dictionary to iterate and collate all balls of the same color
        Dictionary<Ball.BallColor, List <Ball>> colorGroups =
            new Dictionary<Ball.BallColor, List<Ball>>();

        //iterates all balls attached to the board
        foreach (Ball ball in ballManager.GetActiveBalls()) {
            //checks if the ball is near the collided ball
            if (ball.IsConnectedTo(collidedBall)) {
                Ball.BallColor color = ball.Color;
                
                if (!colorGroups.ContainsKey(color)) {
                    colorGroups[color] = new List<Ball>();
                }

                colorGroups[color].Add(ball);
            }
        }

        foreach (List<Ball> ballGroup in colorGroups.Values) {
            if (ballGroup.Count >= 3) {
                foreach (Ball ball in ballGroup) {
                    ballManager.RemoveBall(ball);
                }
            }
        }
        //DropBalls();
    }

    private void DropBalls(){
        for (int x = 0; x < XGridSize; x++) {
            for (int y = 0; y < YGridSize; y++) {
                Vector2 position = GetWorldPosition(x, y);

                Ball ball = GetBallAtPosition(position);

                if (ball != null) {
                    if (ball.transform.position.y > position.y) {
                        print(ball.name);
                    }
                }

                else {
                    for (int i = y + 1; i < YGridSize; i++) {
                        Vector2 abovePos = GetWorldPosition(x, i);
                        Ball ballAbove = GetBallAtPosition(abovePos);

                        if (ballAbove != null) {
                            print(ballAbove.name);
                        }
                    }
                }
            }
        }
    }

    private Vector2 GetWorldPosition(int x, int y) {

        float xPos = x * xOffset;

        if (y % 2 == 1) {
            xPos += xOffset / 2f;
        }

        return new Vector2(xPos, y);
    }

    private Ball GetBallAtPosition(Vector2 position) {
        int xPos = Mathf.RoundToInt(position.x);
        int yPos = Mathf.RoundToInt(position.y);

        return hexGrid[xPos, yPos].GetComponent<Ball>();
    }
    
    private void FloodFill(GameObject ball, List<GameObject> connectedBalls, bool[,] checkedPosition) {
        int xPos = Mathf.RoundToInt(ball.transform.position.x / hexWidth);
        int yPos = Mathf.RoundToInt(ball.transform.position.y / (hexHeight * 0.75f));

        if (xPos < 0 || xPos >= XGridSize || yPos < 0 || yPos >= YGridSize || checkedPosition[xPos, yPos]) return;

        checkedPosition[xPos, yPos] = true;

        GameObject checkedBall = hexGrid[xPos, yPos];
        connectedBalls.Add(checkedBall);
    }
    
}
