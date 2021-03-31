using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public AnimationCurve tweenCurve;
    public float tweenDuration;
    public List<GameObject> panels;

    public int defaultPanelIndex;
    private int lastIndex;
    private bool transitioning;

    private void Start()
    {
        lastIndex = 0;
        transitioning = false;

        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].SetActive(i == defaultPanelIndex ? true : false);
        }
    }

    public void SetActivePanel(int index)
    {
        if (index < 0 || index >= panels.Count) return;
        if (transitioning) return;
        StartCoroutine(Tween(index));
    }

    IEnumerator Tween(int index)
    {
        transitioning = true;
        float t = 0f;
        float tSmooth = t;
        GameObject oldPanel = panels[lastIndex];
        GameObject newPanel = panels[index];
        oldPanel.SetActive(true);
        newPanel.SetActive(true);
        while (t < 1f)
        {
            tSmooth = tweenCurve.Evaluate(t);
            oldPanel.transform.localScale = Vector3.one * Mathf.Lerp(1f, 0f, tSmooth);
            newPanel.transform.localScale = Vector3.one * Mathf.Lerp(0f, 1f, tSmooth);
            t += Time.deltaTime / tweenDuration;
            yield return new WaitForEndOfFrame();
        }
        oldPanel.SetActive(false);
        newPanel.SetActive(true);
        lastIndex = index;
        transitioning = false;
    }
}
