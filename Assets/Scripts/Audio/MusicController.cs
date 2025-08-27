using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip initialMusic;
    [SerializeField] private AudioClip[] otherMusic;


    [SerializeField] private AudioMixer mixer;

    private AudioSource musicSource;

    public Action OnSongEnd;

    private void Awake()
    {
        musicSource = GetComponent<AudioSource>();
    }

    public void SetMusic(EMusic music, bool force = false)
    {
        if (musicSource.isPlaying == true && force == false) return;

        musicSource.Stop();
        musicSource.loop = false;

        switch (music)
        {
            case EMusic.Menu:
                musicSource.clip = menuMusic;
                musicSource.loop = true;

                musicSource.outputAudioMixerGroup = mixer.FindMatchingGroups("Menu")[0];

                CancelInvoke();
                break;

            case EMusic.Initial:
                musicSource.clip = initialMusic;

                musicSource.outputAudioMixerGroup = mixer.FindMatchingGroups("Gameplay")[0];

                CancelInvoke();
                break;

            case EMusic.Other:
                int random = UnityEngine.Random.Range(0, otherMusic.Length);
                musicSource.clip = otherMusic[random];
                break;
        }

        musicSource.Play();

        if(musicSource.loop == false)
        { 
            Invoke(nameof(SongEnded), musicSource.clip.length); 
        }
    }

    private void SongEnded()
    {
        musicSource.Stop();
        OnSongEnd?.Invoke();
    }

    public void SetMasterVolume(float volume)
    {
        mixer.SetFloat("masterVolume", volume);
    }
}

public enum EMusic { Menu, Initial, Other }