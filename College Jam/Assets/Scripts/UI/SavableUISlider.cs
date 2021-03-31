using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavableUISlider : SavableUI
{
    public Slider slider;

    public override void Save()
    {
        PlayerPrefs.SetFloat(propertyName, slider.value);
    }

    public override void Load()
    {
        slider.value = PlayerPrefs.GetFloat(propertyName);
    }
}
