using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip menuMusic, sailingMusic, scavRadio, backgroundBoat, bargeNoise, trashSuckerStartUp, trashSuckerRunning, trashSuckerSpinDown;


    [SerializeField] public AudioSource Music1;
    [SerializeField] public AudioSource Music2;
    [SerializeField] public AudioSource Background1;
    [SerializeField] public AudioSource Background2;
    [SerializeField] public AudioSource Sounds;
    [SerializeField] public float fadetime = 1f;
    [SerializeField] public float radiofadetime = 5f;


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


    /// 
    /// Menu music on load
    /// Sailing music as you're cruising, 
    /// start from pressing start -- stop menu music, play saoiling music
    /// Radio
    /// Scan radio has it's own audio source
    /// Fade out sailing music
    /// Pause - stop sailing music and play menu music
    /// 



    public void PlayStartMusic()
    {
        //
    }



    public void TransToSail()
    {

        StartCoroutine(FadeOut(Music1, fadetime));
        StartCoroutine(FadeIn(Music2, fadetime));
    }

    public void FadeScavIn()
    {
        StartCoroutine(FadeOut(Music2, radiofadetime));
    }

    public void FadeScavOut()
    {
        StartCoroutine(FadeIn(Music2, radiofadetime));
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


    // references
    //public void StopAllSounds()
    //{
    //    Music1.Stop();
    //    Music2.Stop();
    //    Background1.Stop();
    //    Background2.Stop();
    //    Sounds.Stop();
    //}

    //public void StartNewSceneAudio()
    //{
    //    StopAllSounds();
    //    PlayMusic();
    //    PlayAmbientSounds();
    //}

    //public void PlayOnlyAmbientSounds()
    //{
    //    Music1.Stop();
    //    Music2.Stop();
    //    Background1.clip = background;
    //    Background1.Play();
    //}

    //public void PlayThisAmbientSound(AudioClip ambientClip)
    //{
    //    Background1.clip = ambientClip;
    //    Background1.Play();
    //}
    //public void PlayAmbientSounds()
    //{
    //    Background1.clip = background;
    //    Background1.Play();
    //}

    //public void PlayThisMusic(AudioClip sail)
    //{
    //    Music1.clip = sail;
    //    Music1.Play();
    //}

    //public void PlayMusic()
    //{
    //    Music1.clip = music;
    //    Music1.Play();
    //}



}
