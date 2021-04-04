using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotificationMenu : MonoBehaviour
{
    public AnimationCurve tween;
    public float tweenDuration = 2f;
    public TMP_Text textArea;
    public Queue<string> notificationQueue;
    private bool animating;
    private RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        notificationQueue = new Queue<string>();
        rectTransform = textArea.GetComponent<RectTransform>();
    }

    public void AddToQueue(string message)
    {
        notificationQueue.Enqueue(message);
        if (!animating)
        {
            animating = true;
            StartCoroutine(Animate());
        }
    }

    IEnumerator Animate()
    {
        Vector2 min = new Vector2(-Screen.width, 0f);
        Vector2 max = new Vector2(2 * Screen.width, 0f);
        Vector2 target;
        Rect rect = rectTransform.rect;
        while (notificationQueue.Count > 0)
        {
            string msg = notificationQueue.Dequeue();
            textArea.SetText(msg);
            float t = 0f;
            while (t < 1f)
            {
                target = Vector2.Lerp(min, max, tween.Evaluate(t));
                rectTransform.rect.Set(target.x, target.y, rect.width, rect.height);
                t += Time.deltaTime / tweenDuration;
                yield return new WaitForEndOfFrame();
            }
        }
        animating = false;
    }
}
