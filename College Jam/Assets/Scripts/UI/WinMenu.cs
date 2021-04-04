using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinMenu : MonoBehaviour
{
    public static WinMenu instance;

    public AnimationCurve tween;
    public float tweenDuration = 1f;
    public List<GameObject> objectsToDisable;
    private bool triggered;
    public TMP_Text playerNameText;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        triggered = false;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Win(string winnerName)
    {
        if (triggered) return;
        playerNameText.SetText($"{winnerName} wins!");
        triggered = true;

        MenuManager.instance.enableEscape = false;
        if (MenuManager.instance.menuOpen) MenuManager.instance.FadeOut();
        foreach (var obj in objectsToDisable)
        {
            obj.SetActive(false);
        }
        StartCoroutine(WinCR());
    }

    IEnumerator WinCR()
    {
        float t = 0f;
        while (t < 1f)
        {
            print(t);
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, tween.Evaluate(t));
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
