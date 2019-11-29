using UnityEngine;

public class SoundManager
{
    SoundSettings settings;

    AudioSource theme;
    AudioSource splatter;

    public SoundManager(GameObject parentGO, SoundSettings settings)
    {
        this.settings = settings;

        parentGO.AddComponent<AudioListener>();

        theme = parentGO.AddComponent<AudioSource>();
        theme.clip = settings.mainTheme;
        theme.loop = true;
        theme.Play();

        splatter = parentGO.AddComponent<AudioSource>();
        splatter.clip = settings.splatter;
        splatter.loop = false;
    }


    public void DoSplatterSound()
    {
        float pitch = Random.Range(settings.splatterMinPitch, settings.splatterMaxPitch);
        splatter.pitch = pitch;
        splatter.Play();
    }

    public void DoPerfectSplatterSound()
    {
        float pitch = Random.Range(settings.perfectMoveMinPitch, settings.perfectMoveMaxPitch);
        splatter.pitch = pitch;
        splatter.Play();
    }
}
