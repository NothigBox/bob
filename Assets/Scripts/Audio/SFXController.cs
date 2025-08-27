using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXController : MonoBehaviour
{
    [SerializeField] private AudioClip bubblePop;
    [SerializeField] private AudioClip needlePop;
    [SerializeField] private AudioClip needleAbility;
    [SerializeField] private AudioClip wind;
    [SerializeField] private AudioMixer mixer;

    public float masterVolume;

    private List<AudioSource> availableSources;
    private List<AudioSource> unavailableSources;

    private void Awake()
    {
        availableSources = new List<AudioSource>();
        unavailableSources = new List<AudioSource>();
    }

    public void PlayBubblePop()
    {
        PlayClip(bubblePop, "Bubble_Pop", new Vector2(0.8f, 1.5f));
    }

    public void PlayNeedlePop()
    {
        PlayClip(needlePop, "Needle_Pop");
    }

    public void PlayNeedleAbility()
    {
        PlayClip(needleAbility, "Needle_Ability");
    }

    public void PlayWind()
    {
        PlayClip(wind, "Wind");
    }

    private void PlayClip(AudioClip clip, string mixerGroupName = null, Vector2 randomPitchLimits = default)
    {
        AudioSource source = GetAudioSource();
        source.clip = clip;

        if(randomPitchLimits != default)
        {
            source.pitch = UnityEngine.Random.Range(randomPitchLimits.x, randomPitchLimits.y);
        }
        else
        {
            source.pitch = 1f;
        }

        source.Play();

        if(mixerGroupName != null)
        {
            AudioMixerGroup[] mixer = this.mixer.FindMatchingGroups(mixerGroupName);

            source.outputAudioMixerGroup = mixer[0];
        }

        StartCoroutine(RecycleAudioSourceCoroutine(source, clip.length));
    }

    private AudioSource GetAudioSource()
    {
        AudioSource result = null;

        if (availableSources.Count <= 0)
        {
            result = gameObject.AddComponent<AudioSource>();
            unavailableSources.Add(result);
        }
        else
        {
            result = availableSources[0];
            availableSources.RemoveAt(0);
        }


        return result;
    }

    private void ReturnAudioSource(AudioSource source)
    {
        availableSources.Add(source);
        unavailableSources.Remove(source);
    }

    IEnumerator RecycleAudioSourceCoroutine(AudioSource source, float clipLength)
    {
        yield return new WaitForSeconds(clipLength);

        ReturnAudioSource(source);
    }

    public void SetMasterVolume(float volume)
    {
        mixer.SetFloat("masterVolume", volume);
    }
}
