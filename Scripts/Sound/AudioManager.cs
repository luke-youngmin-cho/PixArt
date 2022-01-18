using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    static public AudioManager instance;
    [SerializeField] public Sound[] BGMs;
    [SerializeField] public Sound[] SFXs;
    private string currentBGMName;

    [SerializeField] AudioMixerGroup BGMMixerGroup;
    [SerializeField] AudioMixerGroup SFXMixerGroup;

    [HideInInspector] public CMDState CMDState = CMDState.BUSY;
    private void Awake()
    {
        if (instance == null) instance = this;
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        foreach (Sound sound in BGMs)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.outputAudioMixerGroup = BGMMixerGroup;
        }
        foreach (Sound sound in SFXs)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.outputAudioMixerGroup = SFXMixerGroup;
        }
        BGMMixerGroup.audioMixer.SetFloat("VolumeExposed", Mathf.Log10(SettingsManager.instance.Sound_BGMVolume) * 20);
        SFXMixerGroup.audioMixer.SetFloat("VolumeExposed", Mathf.Log10(SettingsManager.instance.Sound_SFXVolume) * 20);
        StopAll();
        CMDState = CMDState.IDLE;
    }
    public void PlayBGM(string name)
    {
        if (name == "Random")
        {
            PlayRandomBGM();
            return;
        }
        Sound s = Array.Find(BGMs, sound => sound.name == name);

        if (s == null) 
            return;
        StopAll();
        s.source.Play();
    }
    public void PlayBGM()
    {
        string name = ((e_BGMDropdownType)SettingsManager.instance.Sound_BGMTypeIndex).ToString();
        PlayBGM(name);
    }
    public void PlayBGM(int index)
    {
        BGMDropdown.instance.SetDropdownValue(index);
        SettingsManager.instance.Sound_BGMTypeIndex = index;
    }
    public void PlaySFX(string name)
    {
        Sound s = Array.Find(SFXs, sound => sound.name == name);

        if (s == null)
            return;
        s.source.Play();
    }
    public void SetVolume(string name , float volume)
    {
        Sound s = Array.Find(BGMs, sound => sound.name == name);
        s.source.volume = volume;
    }
    public void SetPitch(string name, float pitch)
    {
        Sound s = Array.Find(BGMs, sound => sound.name == name);
        s.source.pitch = pitch;
    }

    public void PlayRandomBGM()
    {
        int idx = UnityEngine.Random.Range(1, 4);
        e_BGMDropdownType bgmType = (e_BGMDropdownType)idx;
        string name = bgmType.ToString();
        PlayBGM(name);
    }
    public void SetCurrentBGMVolume(float volume)
    {
        Sound s = Array.Find(BGMs, sound => sound.name == currentBGMName);
        s.source.volume = volume;
    }
    public void Stop(string name)
    {
        Sound s = Array.Find(BGMs, sound => sound.name == name);
        s.source.Stop();
    }
    public void StopCurrentBGM()
    {
        Stop(currentBGMName);
    }

    public void StopAll()
    {
        for (int i = 0; i < BGMs.Length; i++)
        {
            BGMs[i].source.Stop();
        } 
    }

}
