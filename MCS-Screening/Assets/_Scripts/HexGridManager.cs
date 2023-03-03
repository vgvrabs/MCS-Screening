using System;
using System.Collections;
using System.Collections.Generic;
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
    private Vector3[,] hexPositions;
    private GameObject[,] hexGrid;

    [SerializeField] private float xOffset = 0.882f;
    [SerializeField] private float yOffset = 0.764f;
    [SerializeField] private List<Ball> activeBalls;


    void OnEnable() {
        SingletonManager.Register(this);
    }

    private void OnDisable() {
        SingletonManager.Remove<HexGridManager>();
    }

    void Start() {
        //InitializeGrid();
        GenerateGrid();
        BallPool = new List<GameObject>(BallPrefab);
    }
    
    /*public void InitializeGrid() {
        hexWidth = HexSize * Mathf.Sqrt(3f);
        hexHeight = HexSize * 1.5f;
        
        // Initialize positions
        hexPositions = new Vector3[XGridSize, YGridSize];

        for (int x = 0; x < XGridSize; x++) {
            for (int y = 0; y < YGridSize; y++) {
                float xPos = x * hexWidth + (y % 2 == 0 ? 0 : hexWidth / 2f);
                float yPos = y * hexHeight * 0.75f;
                hexPositions[x, y] = new Vector3(xPos, -yPos, 0);
            }
        }

        
        
        // Spawn balls
        for (int x = 0; x < XGridSize; x++) {
            for (int y = 0; y < YGridSize/2; y++) {
                
                int rand = Random.Range(0, 100);

                if (rand <= BallSpawnChance) {
                    int randomBallIndex = Random.Range(0, BallPrefab.Count);

                    GameObject spawnedBall = Instantiate(BallPrefab[randomBallIndex]);
                    spawnedBall.transform.position = hexPositions[x, y];
                    spawnedBall.transform.parent = transform;
                }
            }
        }
        
    }*/

    private void GenerateGrid() {
        
        //create grid based on given size and spawn balls halfway;
        for (int x = 0; x < XGridSize; x++) {
            for (int y = 0; y < YGridSize/2; y++) {
                float xPos = x * xOffset;

                if (y % 2 == 1) {
                    xPos += xOffset / 2f;
                }
                
                int rand = Random.Range(0, 100);

                if (rand <= BallSpawnChance) {
                    int randomBallIndex = Random.Range(0, BallPrefab.Count);
                    GameObject ball = Instantiate(BallPrefab[randomBallIndex], new Vector3(xPos, -(y * yOffset), 0),
                        Quaternion.identity);
                    Ball spawnedBall = ball.GetComponent<Ball>();

                    activeBalls.Add(spawnedBall);

                    spawnedBall.Row = x;
                    spawnedBall.Col = y;
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

    public void SnapToGrid(GameObject ball, Collision ballCol) {
        //get position on point of contact;
        Vector3 collisionPos = ball.GetComponent<Ball>().GetCollisionPosition();
        GameObject newBall = Instantiate(ball);
        Ball spawnedBall = newBall.GetComponent<Ball>();
        activeBalls.Add(spawnedBall);

        //look for the nearest coordinate on the grid 
        int row = Mathf.RoundToInt(collisionPos.x / xOffset);
        int col = Mathf.RoundToInt(collisionPos.y / yOffset);

        spawnedBall.Row = row;
        spawnedBall.Col = col;
        
        CheckForMatches(ballCol.gameObject.GetComponent<Ball>());
    }

    public void CheckForMatches(Ball collidedBall) {
        //creates a dictionary to iterate and collate all balls of the same color
        Dictionary<Ball.BallColor, List <Ball>> colorGroups =
            new Dictionary<Ball.BallColor, List<Ball>>();

        //iterates all balls attached to the board
        foreach (Ball ball in activeBalls) {
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
                    activeBalls.Remove(ball);
                    Destroy(ball.gameObject);
                }
            }
        }
    }

    private void DropBalls(){
        for (int x = 0; x < XGridSize; x++) {
            for (int y = 0; y < YGridSize; y++) {
                //Vector2 position = GetWorldPosition
            }
        }
    }

    private Vector2 GetWorldPosition(int x, int y) {
        
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
