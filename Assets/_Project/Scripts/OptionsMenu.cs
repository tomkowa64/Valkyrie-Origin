using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
   public void SetValue(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
        //Debug.Log(value);
    }
}