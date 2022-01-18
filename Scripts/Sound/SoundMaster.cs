using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System; // OnSceneLoaded 쓸 때 System 선언 위치 중요함.

public class SoundMaster : MonoBehaviour
{
    public static SoundMaster instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this);
            //SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        foreach (var bg in bgList)
        {
            if (arg0.name == bg.name) BgSoundPlay(bg);
        }
        throw new NotImplementedException();
    }

    public UnityEngine.Audio.AudioMixer mixer;
    public AudioSource bgSound;
    [SerializeField] List<AudioClip> bgList;

    private void OnEnable()
    {
        
    }
    public void SFXPlay(string sfxName, AudioClip clip) 
    {
        GameObject go = new GameObject(sfxName + "Sound");
        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        audioSource.clip = clip;
        audioSource.Play();

        Destroy(go, clip.length);
    }
    public void SetSoundVolume(Slider slider)
    {
        mixer.SetFloat("BGSoundVolume", Mathf.Log10(slider.value));
    }

    private void BgSoundPlay(AudioClip clip)
    {
        bgSound.outputAudioMixerGroup = mixer.FindMatchingGroups("BGSound")[0];
        bgSound.clip = clip;
        bgSound.loop = true;
        bgSound.volume = 0.1f;
        bgSound.Play();
    }
}
