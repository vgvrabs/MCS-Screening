using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBounce : MonoBehaviour {
    
    [SerializeField] private Rigidbody rb;
    private Vector3 previousPosition;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        previousPosition = rb.velocity;
    }

    private void OnCollisionEnter(Collision other) {
        GetComponent<Ball>().IsShot = false;
        
        if (other.gameObject.CompareTag("Border")) {
            print("bounce");
            float speed = previousPosition.magnitude;
            Vector3 direction = Vector3.Reflect(previousPosition.normalized,
                other.contacts[0].normal);
            rb.velocity = direction * MathF.Max(speed, 0f);
        }
    }
}
