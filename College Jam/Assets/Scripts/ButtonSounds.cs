using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSounds : MonoBehaviour
{
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private AudioSource source;
    private EventTrigger trigger;

    // Start is called before the first frame update
    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
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
        print("OWO");
        source.PlayOneShot(hoverSound);
    }
    public void PlayClickSound(PointerEventData data)
    {
        print("UWU");
        source.PlayOneShot(clickSound);
    }
}
