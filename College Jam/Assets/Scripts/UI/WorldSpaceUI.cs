using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceUI : MonoBehaviour {
    //public Canvas canvas;
    public RectTransform panelTransform;
    public float xOffset;
    public float yOffset;

    // Start is called before the first frame update
    void Start() {
        //if (canvas == null)
        //{
        //    canvas = GetComponent<Canvas>();
        //}

        //if (canvas.worldCamera == null)
        //{
        //    canvas.worldCamera = Camera.main;
        //}
    }

    // Update is called once per frame
    void LateUpdate() {
        //transform.LookAt(canvas.worldCamera.transform, canvas.worldCamera.transform.up);
        transform.LookAt(Camera.main.transform, Camera.main.transform.up);
        //Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
        //camPos.x += xOffset;
        //camPos.y += yOffset;
        //panelTransform.anchoredPosition = camPos;
    }
}
