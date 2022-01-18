using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSoundEffect : MonoBehaviour
{
    [SerializeField] string soundName;

    public void ClickSound()
    {
        if (AudioManager.instance == null) return;
        AudioManager.instance.PlaySFX(soundName);
    }
}
