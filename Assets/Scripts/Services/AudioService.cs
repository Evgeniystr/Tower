using UnityEngine;
using Zenject;

public class AudioService : MonoBehaviour
{
    [SerializeField]
    private AudioSource _themeAudioSource;
    [SerializeField]
    private AudioSource _splatterAudioSource;

    [Inject]
    private SoundSettings _soundSettings;


    private void Start()
    {
        _themeAudioSource.clip = _soundSettings.mainTheme;
        _themeAudioSource.loop = true;
        _themeAudioSource.Play();

        _splatterAudioSource.clip = _soundSettings.splatter;
        _splatterAudioSource.loop = false;
    }

    public void DoSplatterSound()
    {
        float pitch = Random.Range(_soundSettings.splatterMinPitch, _soundSettings.splatterMaxPitch);
        _splatterAudioSource.pitch = pitch;
        _splatterAudioSource.Play();
    }

    public void DoPerfectSplatterSound()
    {
        float pitch = Random.Range(_soundSettings.perfectMoveMinPitch, _soundSettings.perfectMoveMaxPitch);
        _splatterAudioSource.pitch = pitch;
        _splatterAudioSource.Play();
    }
}
