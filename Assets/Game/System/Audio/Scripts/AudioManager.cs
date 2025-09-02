using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public class AudioManager : MonoBehaviour
{
    [Header("Main Configuration")]
    [SerializeField] private AudioMixer audioMixer = null;
    [SerializeField] private AudioSource musicAudioSource = null;
    [SerializeField] private Transform sfxHolder = null;
    [SerializeField] private AudioSfx audioSfxPrefab = null;

    [Header("Lerp Times Configuration")]
    [SerializeField] private float musicLerpTime = 1.5f;

    private ObjectPool<AudioSfx> sfxAudioSourcesPool = null;
    private List<AudioSfx> activeSfxAudioSources = null;
    private Dictionary<string, AudioMixerGroup> audioMixerGroupsDic = null;

    private bool sfxEnabled = false;
    private bool musicEnabled = false;

    private float musicVolume = 0f;
    private float sfxVolume = 0f;

    private float sfxMixerVolume = 0f;
    private float musicMixerVolume = 0f;

    private AudioEvent currentMusicEvent = null;
    private bool isStopingMusic = false;

    private const string masterMixerName = "Master";
    private const string musicMixerName = "Music";
    private const string sfxMixerName = "Sfx";
    private const string musicVolumeParameter = "musicVolume";
    private const string sfxVolumeParameter = "sfxVolume";

    private const float defaultMaxMixerVolume = 0.0f;
    private const float defaultMinMixerVolume = -80.0f;

    public float MusicVolume { get => musicVolume; }
    public float SfxVolume { get => sfxVolume; }
    public bool SfxEnabled { get => sfxEnabled; }
    public bool MusicEnabled { get => musicEnabled; }

    public void Init()
    {
        audioMixerGroupsDic = new Dictionary<string, AudioMixerGroup>();
        AudioMixerGroup[] groups = audioMixer.FindMatchingGroups(masterMixerName);
        for (int i = 0; i < groups.Length; i++)
        {
            audioMixerGroupsDic.Add(groups[i].name, groups[i]);
        }

        sfxAudioSourcesPool = new ObjectPool<AudioSfx>(GenerateSFXSource, GetSFXSource, ReleaseSFXSource);
        activeSfxAudioSources = new List<AudioSfx>();

        UpdateSfxVolume(1f);
        UpdateMusicVolume(1f);

        sfxEnabled = true;
        musicEnabled = true;
    }

    public void PlayAudio(AudioEvent audioEvent, Vector3 position = new Vector3())
    {
        if (audioEvent == null) return;

        switch (audioEvent.AudioType)
        {
            case AudioEvent.AUDIO_TYPE.SFX:
                PlaySFX(audioEvent);
                break;

            case AudioEvent.AUDIO_TYPE.MUSIC:
                PlayMusic(audioEvent);
                break;
        }
    }

    private void PlaySFX(AudioEvent audioEvent, Vector3 position = new Vector3())
    {
        if (!sfxEnabled) return;

        AudioSfx audio = sfxAudioSourcesPool.Get();
        audio.transform.position = position;
        audio.SetAudioValues(audioEvent);

        ToggleSFX(sfxEnabled);

        IEnumerator ReleaseSource()
        {
            yield return new WaitForSecondsRealtime(audioEvent.Clip.length);
            sfxAudioSourcesPool.Release(audio);
        }

        StartCoroutine(ReleaseSource());
    }

    private void PlayMusic(AudioEvent audioEvent)
    {
        if (!musicEnabled) return;

        StopCurrentMusic(onSuccess: () => { StartCoroutine(StartMusicCoroutine()); });
        IEnumerator StartMusicCoroutine()
        {
            currentMusicEvent = audioEvent;
            musicAudioSource.clip = audioEvent.Clip;
            musicAudioSource.volume = audioEvent.Volume;
            musicAudioSource.Play();

            float timer = 0f;
            while (timer < musicLerpTime)
            {
                timer += Time.deltaTime;

                float musicVolume = Mathf.Lerp(defaultMinMixerVolume, musicMixerVolume, timer / musicLerpTime);
                UpdateMusicVolumeMixer(musicVolume);

                yield return new WaitForEndOfFrame();
            }

            UpdateMusicVolumeMixer(musicMixerVolume);
        }
    }

    public void StopCurrentMusic(Action onSuccess = null)
    {
        StartCoroutine(StopMusicCoroutine());
        IEnumerator StopMusicCoroutine()
        {
            if (currentMusicEvent == null || isStopingMusic)
            {
                yield return new WaitUntil(predicate: () => !isStopingMusic);
            }
            else
            {
                float timer = 0f;
                float startMusicVolume = sfxVolume > 0f ? currentMusicEvent.Volume / sfxVolume : defaultMinMixerVolume;
                isStopingMusic = true;
                
                while (timer < musicLerpTime)
                {
                    timer += Time.deltaTime;

                    float musicVolume = Mathf.Lerp(startMusicVolume, defaultMinMixerVolume, timer / musicLerpTime);
                    UpdateMusicVolumeMixer(musicVolume);

                    yield return new WaitForEndOfFrame();
                }
                UpdateMusicVolumeMixer(defaultMinMixerVolume);

                currentMusicEvent = null;
                musicAudioSource.Stop();
                isStopingMusic = false;
            }

            onSuccess?.Invoke();
        }
    }

    public void ToggleAll(bool status)
    {
        ToggleMusic(status);
        ToggleSFX(status);
    }

    public void ToggleMusic(bool status)
    {
        musicEnabled = status;
        if (musicEnabled)
        {
            if (!musicAudioSource.isPlaying)
            {
                musicAudioSource.Play();
            }
        }
        else
        {
            musicAudioSource.Pause();
        }
    }

    public void ToggleSFX(bool status)
    {
        if (!status)
        {
            for (int i = 0; i < activeSfxAudioSources.Count; i++)
            {
                activeSfxAudioSources[i].StopAudio();
            }
        }

        sfxEnabled = status;
    }

    public void UpdateSfxVolume(float volume)
    {
        sfxVolume = volume;
        sfxMixerVolume = Mathf.Lerp(defaultMinMixerVolume, defaultMaxMixerVolume, volume);
        UpdateSfxVolumeMixer(sfxMixerVolume);
    }

    public void UpdateMusicVolume(float volume)
    {
        musicVolume = volume;
        musicMixerVolume = Mathf.Lerp(defaultMinMixerVolume, defaultMaxMixerVolume, volume);
        UpdateMusicVolumeMixer(musicMixerVolume);
    }

    private void UpdateSfxVolumeMixer(float volume)
    {
        audioMixerGroupsDic[sfxMixerName].audioMixer.SetFloat(sfxVolumeParameter, volume);
    }

    private void UpdateMusicVolumeMixer(float volume)
    {
        audioMixerGroupsDic[musicMixerName].audioMixer.SetFloat(musicVolumeParameter, volume);
    }

    private AudioSfx GenerateSFXSource()
    {
        return Instantiate(audioSfxPrefab, sfxHolder);
    }

    private void GetSFXSource(AudioSfx audio)
    {
        activeSfxAudioSources.Add(audio);
        audio.OnGet();
    }

    private void ReleaseSFXSource(AudioSfx audio)
    {
        activeSfxAudioSources.Remove(audio);
        audio.OnRelease();
    }
}
