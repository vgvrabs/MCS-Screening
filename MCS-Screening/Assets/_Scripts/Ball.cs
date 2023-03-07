using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Events;

public class Ball : MonoBehaviour {
    public enum BallColor {
        Brown,
        White,
        Gold,
        Red,
        Blue
    };

    public int Row;
    public int Col;
    public BallColor Color;
    public UnityEvent<Ball, Collision> Evt_OnHit = new();
    public bool IsShot = false;
    public float Speed = 10;
    public float Radius;
    public List<Ball> ConnectedBalls = new List<Ball>();
    public Ball TopBall;
    public Collider Collider;


    [SerializeField]private Rigidbody rigidbody;
    
    private Vector3 collisionPosition;

  

    public void Initialize() {
        rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponent<Collider>();
        
        Evt_OnHit.AddListener(SingletonManager.Get<HexGridManager>().SnapToGrid);

        rigidbody.isKinematic = false;
        IsShot = true;
        GetComponent<SpriteRenderer>().sortingOrder = 2;
    }

    private void FixedUpdate() {
        if (IsShot) {
            MoveBall(transform.up);
        }
    }

    public void MoveBall(Vector2 direction) {
        rigidbody.velocity = (direction * Speed);
    }

    public Ball GetNeighbor(Ball firstBall) {
        List<Collider> colliders = Physics.OverlapSphere(transform.position, Radius).ToList();
        colliders.Remove(Collider);

        foreach (Collider col in colliders) {
            Ball ball = col.GetComponent<Ball>();

            if (ball) {
                if (ball.Color == Color) {
                    if (!firstBall.ConnectedBalls.Contains(ball)) {
                        
                        firstBall.ConnectedBalls.Add(ball);
                        ball.GetNeighbor(firstBall);
                    }
                }
            }
        }

        return null;
    }
    
    private void OnCollisionEnter(Collision other) {
        if (!other.gameObject.GetComponent<Ball>()) return;
        
        if (!rigidbody.isKinematic) {
            BallManager ballManager = SingletonManager.Get<BallManager>();
            IsShot = false;
            rigidbody.isKinematic = true;
            
            Evt_OnHit?.Invoke(this, other);
            ConnectedBalls.Add(this);
            GetNeighbor(this);
            ballManager.DestroyBalls(ConnectedBalls);
            //ballManager.CheckDisconnectedBalls(ballManager.activeBalls[0]);
            ballManager.StartCoroutine(ballManager.WaitAndCheck());
            //Evt_OnHit.RemoveAllListeners();
        }
    }

    public void SetCollisionPosition(Vector3 position) {
        collisionPosition = position;
    }
    public Vector3 GetCollisionPosition() {
        return collisionPosition;
    }

    public bool IsConnectedTo() {
        //float distance = Vector3.Distance(transform.position, otherBall.transform.position);
        //return distance <= 1.3f;
        if (TopBall != null) return true;

        return false;
    }

    public void SetBallPosition(int x, int y) {
        Row = x;
        Col = y;
    }
    
#if UNITY_EDITOR

    void OnDrawGizmosSelected() {

        Gizmos.DrawWireSphere(transform.position, Radius);    
    }
    
#endif
}
