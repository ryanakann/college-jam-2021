using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

public class ButtonSounds : MonoBehaviour
{
    public AudioClip hoverSound;
    public AudioClip clickSound;

    public AudioMixerGroup group;

    private AudioSource source;
    private EventTrigger trigger;

    // Start is called before the first frame update
    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = group;

        trigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry hoverEntry = new EventTrigger.Entry();
        hoverEntry.eventID = EventTriggerType.PointerEnter;
        hoverEntry.callback.AddListener((data) => { PlayHoverSound((PointerEventData)data); });
        trigger.triggers.Add(hoverEntry);

        EventTrigger.Entry clickEntry = new EventTrigger.Entry();
        clickEntry.eventID = EventTriggerType.PointerClick;
        clickEntry.callback.AddListener((data) => { PlayClickSound((PointerEventData)data); });
        trigger.triggers.Add(clickEntry);
    }

    public void PlayHoverSound(PointerEventData data)
    {
        source.PlayOneShot(hoverSound);
    }
    public void PlayClickSound(PointerEventData data)
    {
        source.PlayOneShot(clickSound);
    }
}
