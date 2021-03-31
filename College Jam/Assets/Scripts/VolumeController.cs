using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public string groupName;
    public AudioMixer mixer;
    public Slider slider;

    public void SetVolume()
    {
        float t = slider.value;
        float value = Mathf.Lerp(-80f, 20f, Mathf.Log(t+1, 2f));
        mixer.SetFloat(groupName, value);
    }
}
