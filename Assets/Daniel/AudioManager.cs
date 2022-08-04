using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip background, music, sfx;

    [SerializeField] public AudioSource Music1;
    [SerializeField] public AudioSource Music2;
    [SerializeField] public AudioSource Background1;
    [SerializeField] public AudioSource Background2;
    [SerializeField] public AudioSource Sounds;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void StopAllSounds()
    {
        Music1.Stop();
        Music2.Stop();
        Background1.Stop();
        Background2.Stop();
        Sounds.Stop();
    }
    
    public void StartNewSceneAudio()
    {
        StopAllSounds();
        PlayMusic();
        PlayAmbientSounds();
    }

    public void PlayOnlyAmbientSounds()
    {
        Music1.Stop();
        Music2.Stop();
        Background1.clip = background;
        Background1.Play();
    }

    public void PlayThisAmbientSound(AudioClip ambientClip)
    {
        Background1.clip = ambientClip;
        Background1.Play();
    }
    public void PlayAmbientSounds()
    {
        Background1.clip = background;
        Background1.Play();
    }

    public void PlayThisMusic(AudioClip newSceneMusic)
    {
        Music1.clip = newSceneMusic;
        Music1.Play();
    }

    public void PlayMusic()
    {
        Music1.clip = music;
        Music1.Play();
    }

     public void PlaySound(AudioClip sound)
    {
        Sounds.PlayOneShot(sound);
    }


    public IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
    public IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;
        audioSource.volume = 0;
        audioSource.Play();
        while (audioSource.volume < startVolume)
        {
            audioSource.volume += Time.deltaTime * FadeTime;

            yield return null;
        }
        audioSource.volume = startVolume;
    }

}
