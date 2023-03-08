using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    
    // used for rotating the shooter
    public float MaxZRot = 90f;
    public float MinZRot = -90f;
    public float ShotDelay = 1f;

    public Ball ShotBall;
    public GameObject FirePoint;
    public SpriteRenderer CurrentBallSprite;
    public GameObject CurrentBall;
    public SpriteRenderer NextBallSprite;
    public GameObject NextBall;

    private HexGridManager hexGridManager;
    private BallManager ballManager;
    
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

    private void Initialize() {
        hexGridManager = SingletonManager.Get<HexGridManager>();
        ballManager = SingletonManager.Get<BallManager>();
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
        int ballCount = ballManager.activeBalls.Count;
        
        NextBall = ballManager.BallPool[Random.Range(0, ballCount)];
        NextBallSprite.sprite = NextBall.GetComponent<SpriteRenderer>().sprite;
    }
    public void GenerateBall() {
        //ballManager.UpdateBallPool();
        int ballCount = ballManager.activeBalls.Count;
        
        if (ballCount <= 0) return;

        /*if (!NextBall) {
            return;
        }*/
        
        CurrentBall = NextBall;
        CurrentBallSprite.sprite = CurrentBall.GetComponent<SpriteRenderer>().sprite;
        NextBall = ballManager.BallPool[Random.Range(0, ballCount)];
        NextBallSprite.sprite = NextBall.GetComponent<SpriteRenderer>().sprite;
    }

    public void ShootBall(GameObject ball) {
        if (!ball) return;

        GameObject spawnedBall = Instantiate(ball, FirePoint.transform.position, FirePoint.transform.rotation);
        ShotBall = spawnedBall.GetComponent<Ball>();
        ShotBall.Initialize();
        WaitToShoot(1f);
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
