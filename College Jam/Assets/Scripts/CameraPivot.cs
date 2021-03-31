using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs;

public class CameraPivot : MonoBehaviour {

    public static CameraPivot instance;

    [Header("Position")]
    public Transform target;
    public Vector3 targetPosition;
    public float positionDamping = 0.2f;
    private Vector3 positionVelRef;

    [Header("Rotation")]
    public float rotationDamping = 0.2f;
    public float rotationSensitivity = 1f;
    private float rotationVelRef;
    private Vector3 rotationAxis;
    private float targetRotation;

    private bool dragging;
    private float dragAmount;
    private Vector3 mousePosition;
    private Vector3 mousePositionLastFrame;
    private Vector3 mouseDirection;

    [Header("Scale")]
    public float scaleDamping = 0.2f;
    public float scaleSensitivity = 1f;
    public float minScale = 0.1f;
    public float maxScale = 16f;
    private float scaleVelRef;
    [SerializeField] private float scaleInput;
    private float targetScale = 1f;
    public bool invertScale;


    // Start is called before the first frame update
    void Start() {
        instance = this;
        dragging = false;
    }

    // Update is called once per frame
    void Update() {
        // Position
        if (target) {
            targetPosition = target.position;
        }

        // Rotation
        if (Input.GetMouseButtonDown(0)) {
            dragging = true;
            mousePosition = Input.mousePosition;
            mousePositionLastFrame = mousePosition;
        } else if (Input.GetMouseButtonUp(0)) {
            dragging = false;
        }

        if (dragging) {
            mousePosition = Input.mousePosition;
            mousePosition.z = 0f;

            mouseDirection = mousePositionLastFrame - mousePosition;
            dragAmount = mouseDirection.magnitude;

            mouseDirection = Camera.main.transform.right * mouseDirection.x + Camera.main.transform.up * mouseDirection.y;
            rotationAxis = Vector3.Cross(mouseDirection, Camera.main.transform.forward).normalized;

            mousePositionLastFrame = mousePosition;
        } else {
            dragAmount = Mathf.SmoothDamp(dragAmount, 0f, ref rotationVelRef, rotationDamping);
        }

        // Scale
        int invertMultiplier = (invertScale ? -1 : 1);
        scaleInput = invertMultiplier * Input.GetAxisRaw("Mouse ScrollWheel") * scaleSensitivity;
        targetScale = Mathf.SmoothDamp(targetScale,
            targetScale - scaleInput,
            ref scaleVelRef, scaleDamping);
        targetScale = Mathf.Clamp(targetScale, minScale, maxScale);

        // Apply all transformations
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref positionVelRef, positionDamping);

        targetRotation = dragAmount * rotationSensitivity * Time.deltaTime;
        transform.rotation = Quaternion.AngleAxis(targetRotation, rotationAxis) * transform.rotation;

        transform.localScale = Vector3.one * targetScale;
    }

    public void SetTarget(Transform target) {
        this.target = target;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(Camera.main.transform.position, mouseDirection * 10f);
    }
}
