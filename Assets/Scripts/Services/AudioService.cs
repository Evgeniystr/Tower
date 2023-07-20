using System;
using UnityEngine;
using Zenject;

public class AudioService : MonoBehaviour
{
    public event Action OnVolumeChange;

    [SerializeField]
    private AudioSource _themeAudioSource;
    [SerializeField]
    private AudioSource _splatterAudioSource;

    [Inject]
    private SoundSettings _soundSettings;

    public float Volume { get; private set; }
    public bool IsMuted { get; private set; }

    private void Start()
    {
        //recover sound settings
        SetMasterVolume(PlayerPrefs.GetFloat(Constants.VolumePrefsKey));
        SetMute(PlayerPrefs.GetInt(Constants.IsMutePrefsKey)==1);

        _themeAudioSource.clip = _soundSettings.mainTheme;
        _themeAudioSource.loop = true;
        _themeAudioSource.Play();

        _splatterAudioSource.clip = _soundSettings.splatter;
        _splatterAudioSource.loop = false;
    }

    public void DoSplatterSound()
    {
        float pitch = UnityEngine.Random.Range(_soundSettings.splatterMinPitch, _soundSettings.splatterMaxPitch);
        _splatterAudioSource.pitch = pitch;
        _splatterAudioSource.Play();
    }

    public void DoPerfectSplatterSound()
    {
        float pitch = UnityEngine.Random.Range(_soundSettings.perfectMoveMinPitch, _soundSettings.perfectMoveMaxPitch);
        _splatterAudioSource.pitch = pitch;
        _splatterAudioSource.Play();
    }

    public void SetMasterVolume(float volume)
    {
        Volume = volume;
        _themeAudioSource.volume = volume;
        _splatterAudioSource.volume = volume;

        OnVolumeChange?.Invoke();
    }

    public void SetMute(bool state)
    {
        IsMuted = state;

        var resultVolume = IsMuted ? 0 : Volume;
        _themeAudioSource.volume = resultVolume;
        _splatterAudioSource.volume = resultVolume;

        OnVolumeChange?.Invoke();
    }

    public void SwitchMute()
    {
        IsMuted = !IsMuted;

        var resultVolume = IsMuted ? 0 : Volume;
        _themeAudioSource.volume = resultVolume;
        _splatterAudioSource.volume = resultVolume;

        OnVolumeChange?.Invoke();
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetFloat(Constants.VolumePrefsKey, Volume);
        PlayerPrefs.SetInt(Constants.IsMutePrefsKey, IsMuted?1:0);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat(Constants.VolumePrefsKey, Volume);
        PlayerPrefs.SetInt(Constants.IsMutePrefsKey, IsMuted ? 1 : 0);
    }
}
