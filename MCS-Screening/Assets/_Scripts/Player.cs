using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour {
    
    // used for rotating the shooter
    public float MaxZRot = 90f;
    public float MinZRot = -90f;
    public float ShotDelay = 1f;

    public GameObject FirePoint;
    public SpriteRenderer CurrentBallSprite;
    public GameObject CurrentBall;
    public SpriteRenderer NextBallSprite;
    public GameObject NextBall;

    private HexGridManager hexGridManager;
    
    [SerializeField] private bool canShoot;

    IEnumerator Start() {
        yield return new WaitForSeconds(1f);
        
        Initialize();
    }

    void Update() {
        LookAtMouse();
        
        if (Input.GetMouseButtonDown(0) && canShoot) {
            ShootBall(CurrentBall);
            GenerateBall();
        }
    }

    public void Initialize() {
        hexGridManager = SingletonManager.Get<HexGridManager>();
        GenerateInitialBall();
        GenerateBall();
        canShoot = true;
    }

    private void LookAtMouse() {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - 
            transform.position.y);

        transform.up = direction;

        Vector3 playerAngles = transform.rotation.eulerAngles;
        playerAngles.z = (playerAngles.z > 180) ? playerAngles.z - 360 : playerAngles.z;
        playerAngles.z = Mathf.Clamp(playerAngles.z, MinZRot, MaxZRot);
        transform.rotation = Quaternion.Euler(playerAngles);
    }

    public void GenerateInitialBall() {
        print("generated Initial Ball");
        int ballCount = hexGridManager.BallPool.Count;
        
        
        NextBall = hexGridManager.BallPool[Random.Range(0, ballCount)];
        NextBallSprite.sprite = NextBall.GetComponent<SpriteRenderer>().sprite;
    }
    public void GenerateBall() {
        int ballCount = hexGridManager.BallPool.Count;
     
        if (ballCount <= 0) return;


        if (!NextBall) return;
        
        CurrentBall = NextBall;
        CurrentBallSprite.sprite = CurrentBall.GetComponent<SpriteRenderer>().sprite;
        NextBall = hexGridManager.BallPool[Random.Range(0, ballCount)];
        NextBallSprite.sprite = NextBall.GetComponent<SpriteRenderer>().sprite;
    }

    public void ShootBall(GameObject ball) {
        if (!ball) return;

        GameObject spawnedBall = Instantiate(ball, FirePoint.transform.position, FirePoint.transform.rotation);
        Ball shotBall = spawnedBall.GetComponent<Ball>();
        shotBall.Initialize();
        WaitToShoot(1f);
        //shotBall.Evt_OnHit.AddListener(hexGridManager.AttachBallToGrid);

    }

    public async void WaitToShoot(float duration) {
        canShoot = false;
        float endTime = Time.time + duration;
        while (Time.time < endTime) {
            await Task.Yield();
        }

        canShoot = true;
    }

}
