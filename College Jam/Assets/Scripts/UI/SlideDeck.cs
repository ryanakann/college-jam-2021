using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideDeck : MonoBehaviour
{
    public List<GameObject> slides;
    int activeIndex;

    // Start is called before the first frame update
    void Start()
    {
        slides = new List<GameObject>();
        foreach (Transform child in transform)
        {
            slides.Add(child.gameObject);
        }

        activeIndex = 0;
        for (int i = 0; i < slides.Count; i++)
        {
            slides[i].SetActive(i == activeIndex);
        }
    }

    public void NextSlide()
    {
        slides[activeIndex].SetActive(false);
        activeIndex = (activeIndex + 1) % slides.Count;
        slides[activeIndex].SetActive(true);
    }

    public void PreviousSlide()
    {
        slides[activeIndex].SetActive(false);
        activeIndex--;
        if (activeIndex < 0)
        {
            activeIndex = slides.Count - 1;
        }
        slides[activeIndex].SetActive(true);
    }
}
