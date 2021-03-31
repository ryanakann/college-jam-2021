using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavableUIToggle : SavableUI
{
    public Toggle toggle;

    public override void Save()
    {
        PlayerPrefs.SetInt(propertyName, toggle.isOn ? 1 : 0);
    }

    public override void Load()
    {
        toggle.isOn = PlayerPrefs.GetInt(propertyName) == 0 ? false : true;
    }
}
