using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bubble : MonoBehaviour {
    public enum Color {
        Brown,
        White,
        Gold,
        Red,
        Blue
    }
    
    
    Collider Collider;
    public Color color;
    public float Radius;
    public List<Bubble> ConnectedBubble = new List<Bubble>();

    private void Start() {
        Collider = GetComponent<Collider>();
    }

    public Bubble GetNeighbor(Bubble firstBubble) {
        
        List<Collider> Colliders = Physics.OverlapSphere(transform.position, Radius).ToList();

        Colliders.Remove(Collider);
        
        foreach (Collider c in Colliders) {
            
            Bubble bubble = c.GetComponent<Bubble>();

            if (bubble) {
                
                if (bubble.color == color) {
                    
                    if (!firstBubble.ConnectedBubble.Contains(bubble)) {
                        
                        firstBubble.ConnectedBubble.Add(bubble);
                        bubble.GetNeighbor(firstBubble);
                    }
                    //bubble.GetNeighbor(bubble);
                    //ConnectedBubble.Add(bubble.GetNeighbor(this));
                }
            }
        }
        
        return null;
    }

    public void DestroyBubbles() {
        
        if (ConnectedBubble.Count >= 3) {
            
            for (int i = ConnectedBubble.Count - 1; i >= 0; i--) {
                
                Destroy(ConnectedBubble[i].gameObject);
            } 
            
            Destroy(this.gameObject);
        }
    }
    
#if UNITY_EDITOR

    void OnDrawGizmosSelected() {

        Gizmos.DrawWireSphere(transform.position, Radius);    
    }
    
#endif
    
}
