using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public AnimationCurve tweenCurve;
    public float tweenDuration;
    public Image fade;
    public float fadeDuration;
    public List<GameObject> panels;

    public int defaultPanelIndex;
    private int lastIndex;
    private bool transitioning;

    public bool enableEscape;
    public bool menuOpen;

    private void Start()
    {
        lastIndex = 0;
        transitioning = false;
        menuOpen = enableEscape ? false : true;
        StartCoroutine(Init());

        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].SetActive(i == defaultPanelIndex && menuOpen ? true : false);
        }
    }

    private void Update()
    {
        if (enableEscape && !transitioning && Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuOpen)
            {
                menuOpen = false;
                FadeOut();
            }
            else
            {
                menuOpen = true;
                FadeIn(defaultPanelIndex);
            }
        }
    }

    public void SetActivePanel(int index)
    {
        if (transitioning) return;
        if (index >= panels.Count) return;
        StartCoroutine(CrossFade(index));
    }


    public void FadeOut()
    {
        if (transitioning) return;
        StartCoroutine(FadeOutCR());
    }

    IEnumerator FadeOutCR()
    {
        transitioning = true;

        float t = 0f;
        float tSmooth = t;
        GameObject panel = panels[lastIndex];
        panel.SetActive(true);
        while (t < 1f)
        {
            tSmooth = tweenCurve.Evaluate(t);
            panel.transform.localScale = Vector3.one * Mathf.Lerp(1f, 0f, tSmooth);
            t += Time.deltaTime / tweenDuration;
            yield return new WaitForEndOfFrame();
        }
        panel.SetActive(false);
        menuOpen = false;
        transitioning = false;
    }

    public void FadeIn(int index)
    {
        if (transitioning) return;
        if (index >= panels.Count) return;
        StartCoroutine(FadeInCR(index));
    }

    IEnumerator FadeInCR(int index)
    {
        transitioning = true;

        float t = 0f;
        float tSmooth = t;
        GameObject panel = panels[index];
        panel.SetActive(true);
        while (t < 1f)
        {
            tSmooth = tweenCurve.Evaluate(t);
            panel.transform.localScale = Vector3.one * Mathf.Lerp(0f, 1f, tSmooth);
            t += Time.deltaTime / tweenDuration;
            yield return new WaitForEndOfFrame();
        }
        panel.SetActive(true);
        lastIndex = index;
        menuOpen = true;
        transitioning = false;
    }

    IEnumerator CrossFade(int index)
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
        newPanel.SetActive(true);
        oldPanel.SetActive(false);
        lastIndex = index;
        transitioning = false;
    }

    public void LoadScene(int index)
    {
        if (index < 0 || index >= SceneManager.sceneCountInBuildSettings) return;
        StartCoroutine(LoadSceneCR(index));
    }

    IEnumerator Init()
    {
        fade.gameObject.SetActive(true);
        float t = 0;
        Color start = new Color(0f, 0f, 0f, 1f);
        Color end = new Color(0f, 0f, 0f, 0f);
        while (t < 1f)
        {
            fade.color = Color.Lerp(start, end, t);
            t += Time.deltaTime / fadeDuration;
            yield return new WaitForEndOfFrame();
        }
        fade.gameObject.SetActive(false);
    }

    IEnumerator LoadSceneCR(int index)
    {
        float t = 0;
        Color start = new Color(0f, 0f, 0f, 0f);
        Color end = new Color(0f, 0f, 0f, 1f);
        fade.gameObject.SetActive(true);
        while (t < 1f)
        {
            fade.color = Color.Lerp(start, end, t);
            t += Time.deltaTime / fadeDuration;
            yield return new WaitForEndOfFrame();
        }
        SceneManager.LoadScene(index);
    }

    public void LoadNextScene()
    {
        int index = (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings;
        LoadScene(index);
    }

    public void Quit()
    {
        StartCoroutine(QuitCR());
    }

    IEnumerator QuitCR()
    {
        fade.gameObject.SetActive(true);
        float t = 0;
        Color start = new Color(0f, 0f, 0f, 0f);
        Color end = new Color(0f, 0f, 0f, 1f);
        while (t < 1f)
        {
            fade.color = Color.Lerp(start, end, t);
            t += Time.deltaTime / fadeDuration;
            yield return new WaitForEndOfFrame();
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
