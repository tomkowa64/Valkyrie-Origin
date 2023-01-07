using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    
    public TMPro.TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;
    #region Resolutiions
    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " X " + resolutions[i].height;
            options.Add(option);
        }
        resolutionDropdown.AddOptions(options);
    }
    #endregion
    #region SetVolume
    public void SetValue(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }
    #endregion
    public void SetFullScreen( bool isFullScrean)
    {
        Screen.fullScreen = isFullScrean;
    }
}