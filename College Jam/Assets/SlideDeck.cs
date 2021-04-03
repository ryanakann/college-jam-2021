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
}
