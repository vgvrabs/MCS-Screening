using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlinkAnimation : MonoBehaviour {
    public float BlinkInterval;

    [SerializeField] private TextMeshProUGUI text;
    void Start() {
        StartCoroutine(WaitAndBlink());
    }

    private void OnEnable() {
        StartCoroutine(WaitAndBlink());
    }

    private void OnDisable() {
        StopCoroutine(WaitAndBlink());
    }

    IEnumerator WaitAndBlink() {

        while (true) {
            text.enabled = false;

            yield return new WaitForSeconds(BlinkInterval);

            text.enabled = true;
            
            yield return new WaitForSeconds(BlinkInterval);
        }
    }
}
