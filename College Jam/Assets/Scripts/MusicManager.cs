using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [System.Serializable]
    public class Music
    {
        public string name;
        public AudioClip clip;
        public float duration;
    }

    public static MusicManager instance;

    public UnityEngine.Audio.AudioMixerGroup group;
    public List<Music> music;
    private Dictionary<string, Music> musicMap;
    private AudioSource source1;
    private AudioSource source2;
    private int activeSource;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        musicMap = new Dictionary<string, Music>();
        foreach (var item in music)
        {
            musicMap.Add(item.name, item);
        }
        source1 = AddAudioSource();
        source2 = AddAudioSource();

        activeSource = 0;
        Play(music[0]);
    }

    private AudioSource AddAudioSource()
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = group;
        source.playOnAwake = false;
        source.loop = false;
        return source;
    }

    public void Play(Music music)
    {
        StartCoroutine(PlayCR(music));
    }

    IEnumerator PlayCR(Music music)
    {
        AudioSource source = activeSource++ % 2 == 0 ? source1 : source2;
        source.clip = music.clip;
        source.Play();
        yield return new WaitForSeconds(music.duration);
        Play(music);
    }
}
