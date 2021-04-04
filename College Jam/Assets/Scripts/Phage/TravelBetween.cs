using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelBetween : MonoBehaviour {
    public Transform from;

    public Transform to;

    public float transitionTime = 2f;

    float frac = 0;
    private void Start() {
        TurnManager.instance.phageCounter++;
        transform.position = from.position;
        frac = 0;
    }

    void FixedUpdate() {
        Vector3 towardTarget = to.position - from.position;
        frac += Time.fixedDeltaTime / transitionTime;
        frac = Mathf.Clamp(frac, 0f, 1f);
        transform.position = from.position + frac * towardTarget;
    }
}
