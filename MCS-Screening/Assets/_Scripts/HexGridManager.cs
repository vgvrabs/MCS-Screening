using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridManager : MonoBehaviour {

    public List<GameObject> BallPrefab;
    public int XGridSize = 10;
    public int YGridSize = 10;
    public float HexSize = 1f;
    public int BallSpawnChance;
    public float gap = 0.1f;

    private float hexWidth;
    private float hexHeight;
    private Vector3[,] hexPositions;

    void Start() {
        InitializeGrid();
    }

    public void InitializeGrid() {
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
            for (int y = 0; y < YGridSize/1.5; y++) {
                
                int rand = Random.Range(0, 100);

                //if (rand <= BallSpawnChance) {
                    int randomBallIndex = Random.Range(0, BallPrefab.Count);

                    GameObject spawnedBall = Instantiate(BallPrefab[randomBallIndex]);
                    spawnedBall.transform.position = hexPositions[x, y];
                    spawnedBall.transform.parent = transform;
                //}
            }
        }
        
    }
}
