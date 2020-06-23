using UnityEngine.Audio;
using System;
using UnityEngine;

public class MyAudioManager : MonoBehaviour
{
    public static MyAudioManager Instance;
    public Sound[] Sounds;
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume;
        [Range(0.1f, 3f)]
        public float pitch;
        public bool loop;
        [HideInInspector]
        public AudioSource source;

    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        
        foreach (Sound s in Sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(Sounds, sound => sound.name == name);
        if (s == null) return;
        s.source.Play();
    }
    public void Stop(string name)
    {
        Sound s = Array.Find(Sounds, sound => sound.name == name);
        if (s == null) return;
        s.source.Stop();
    }
    public void SetPitch(string name,float val)
    {
        Sound s = Array.Find(Sounds, sound => sound.name == name);
        if (s == null) return;
        s.source.pitch = val;
    }
    
}
