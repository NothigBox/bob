using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private MusicController music;
    [SerializeField] private SFXController sfx;
    [SerializeField] private AnimationCurve curve;

    private void Awake()
    {
        music.OnSongEnd += () => SetMusic(EMusic.Other);
    }

    public void SetMusic(EMusic music, bool force = false)
    {
        this.music.SetMusic(music, force);
    }

    public void PlayNeedlePop()
    {
        sfx.PlayNeedlePop();
    }

    public void PlayBubblePop()
    {
        sfx.PlayBubblePop();
    }

    public void PlayNeedleAbility()
    {
        sfx.PlayNeedleAbility();
    }

    public void PlayWind()
    {
        sfx.PlayWind();
    }

    public void UpdateMusicVolume(float volume)
    {
        float curveVolume = curve.Evaluate(volume);

        float decibels = Mathf.Lerp(-60f, 0f, curveVolume);

        music.SetMasterVolume(decibels);
    }

    public void UpdateSFXVolume(float volume)
    {
        float curveVolume = curve.Evaluate(volume);

        float decibels = Mathf.Lerp(-70f, 0f, curveVolume);

        sfx.SetMasterVolume(decibels);
    }
}
