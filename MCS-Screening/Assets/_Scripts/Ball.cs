using System;
using System.Collections;
using System.Collections.Generic;
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
    public UnityEvent<GameObject, Collision> Evt_OnHit = new();
    public bool IsShot = false;
    public float Speed = 10;

    private Rigidbody rigidbody;
    private SphereCollider sphereCollider;
    private Vector3 collisionPosition;


    public void Initialize() {
        rigidbody = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        Evt_OnHit.AddListener(SingletonManager.Get<HexGridManager>().SnapToGrid);
        rigidbody.isKinematic = false;
        IsShot = true;
        GetComponent<SpriteRenderer>().sortingOrder = 2;
        //Evt_OnHit.AddListener(HexGridManager.AttachBallToGrid);
    }

    private void FixedUpdate() {
        if (IsShot) {
            MoveBall(transform.up);
        }
    }

    public void MoveBall(Vector2 direction) {
        rigidbody.velocity = (direction * Speed);
    }

    private void OnCollisionEnter(Collision other) {
        if (!other.gameObject.GetComponent<Ball>()) return;

        if (IsShot) {
            SetCollisionPosition(other.contacts[0].point);
            IsShot = false;
            rigidbody.isKinematic = true;
            Evt_OnHit?.Invoke(gameObject, other);
            Evt_OnHit?.RemoveAllListeners();
            gameObject.SetActive(false);
            Destroy(gameObject, 1f);
        }
    }

    public void SetCollisionPosition(Vector3 position) {
        collisionPosition = position;
    }
    public Vector3 GetCollisionPosition() {
        return collisionPosition;
    }

    public bool IsConnectedTo(Ball otherBall) {
        float distance = Vector3.Distance(transform.position, otherBall.transform.position);
        return distance <= 1.2f;
    }
}
