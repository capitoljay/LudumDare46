using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PrefManager : MonoBehaviour
{
    public Toggle pumpAudioToggle;
    public AudioSource pumpAudioSource;

    public Slider musicVolumeSlider;
    public AudioMixerGroup musicVolumeMixer;

    public Slider soundVolumeSlider;
    public AudioMixerGroup soundVolumeMixer;

    public Slider uiVolumeSlider;
    public AudioMixerGroup uiVolumeMixer;

    // Start is called before the first frame update
    void Start()
    {
        SetControls();
    }

    private void SetControls()
    {
        //Set the pump sound toggle events
        if (pumpAudioToggle != null)
        {
            Debug.Log("Wiring up pumpAudioToggle");
            pumpAudioToggle.onValueChanged.RemoveAllListeners();
            pumpAudioToggle.onValueChanged.AddListener(delegate
            {
                Preferences.PumpSoundOn = pumpAudioToggle.isOn;
                if (pumpAudioSource != null)
                {
                    pumpAudioSource.mute = !Preferences.PumpSoundOn;
                    if (!pumpAudioSource.isPlaying)
                        pumpAudioSource.Play();
                }
            });

            pumpAudioToggle.isOn = Preferences.PumpSoundOn;
            if (pumpAudioSource != null)
            {
                pumpAudioSource.mute = !Preferences.PumpSoundOn;
                if (!pumpAudioSource.isPlaying)
                    pumpAudioSource.Play();
            }
        }

        //Set the sound volume slider events
        if (soundVolumeSlider != null)
        {
            Debug.Log("Wiring up soundVolumeSlider");
            soundVolumeSlider.onValueChanged.RemoveAllListeners();
            soundVolumeSlider.onValueChanged.AddListener(delegate
            {
                Preferences.SfxVolume = soundVolumeSlider.value;
                SetAudio(soundVolumeMixer, Preferences.SfxVolume);
            });
            soundVolumeSlider.value = Preferences.SfxVolume;
            SetAudio(soundVolumeMixer, Preferences.SfxVolume);
        }

        //Set the ui volume slider events
        if (uiVolumeSlider != null)
        {
            Debug.Log("Wiring up uiVolumeSlider");
            uiVolumeSlider.onValueChanged.RemoveAllListeners();
            uiVolumeSlider.onValueChanged.AddListener(delegate
            {
                Preferences.UiVolume = uiVolumeSlider.value;
                SetAudio(uiVolumeMixer, Preferences.UiVolume);
            });
            uiVolumeSlider.value = Preferences.UiVolume;
            SetAudio(uiVolumeMixer, Preferences.UiVolume);
        }

        //Set the music volume slider events
        if (musicVolumeSlider != null)
        {
            Debug.Log("Wiring up musicVolumeSlider");
            musicVolumeSlider.onValueChanged.RemoveAllListeners();
            musicVolumeSlider.onValueChanged.AddListener(delegate
            {
                Preferences.MusicVolume = musicVolumeSlider.value;
                SetAudio(musicVolumeMixer, Preferences.MusicVolume);
            });
            musicVolumeSlider.value = Preferences.MusicVolume;
            SetAudio(musicVolumeMixer, Preferences.MusicVolume);
        }

        //***Example of a dropdown
        ////Set the Resolution dropdown events
        //if (ResolutionsDropDown != null)
        //{
        //    Debug.Log("Wiring up ResolutionsDropDown");
        //    ResolutionsDropDown.onValueChanged.RemoveAllListeners();
        //    ResolutionsDropDown.onValueChanged.AddListener(delegate
        //    {
        //        SetResolution(SavedResolution.Parse(ResolutionsDropDown.captionText.text), Screen.fullScreen);
        //    });
        //    var currentRes = SavedResolution.Parse(Screen.currentResolution);
        //    var foundRes = ResolutionsDropDown.options.SingleOrDefault(r => r.text == currentRes.ToString());
        //    if (foundRes != null)
        //        ResolutionsDropDown.value = ResolutionsDropDown.options.IndexOf(foundRes);
        //    //});
        //}
    }

    //Minimum and maximum values to set a given volume to.
    private static float audioMinimum = -80f;
    private static float audioMaximum = 0f;

    //Sets a given mixer group's volume to a value between the audio minimum and maximum above
    private static void SetAudio(AudioMixerGroup mixerGroup, float volume)
    {
        //Note that exposed volume parameters don't always have to be named the same as their MixerGroup.

        //It would probably be better to create a mapping between the name of the exposed volume parameter
        //and the name of the mixer group but since I know I named the parameters the same as their mixer
        //groups, I can assume that passing the mixerGroup.name in here will work.

        if (mixerGroup != null && mixerGroup.audioMixer != null)
            mixerGroup.audioMixer.SetFloat(mixerGroup.name, Mathf.Lerp(audioMinimum, audioMaximum, volume));
    }

}
