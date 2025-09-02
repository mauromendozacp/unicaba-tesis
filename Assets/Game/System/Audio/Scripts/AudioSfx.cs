using UnityEngine;

public class AudioSfx : MonoBehaviour
{
    private AudioSource audioSource = null;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetAudioValues(AudioEvent audioEvent)
    {
        audioSource.volume = audioEvent.Volume;
        
        switch (audioEvent.SoundType)
        {
            case AudioEvent.SOUND_TYPE.MONO:
                audioSource.spatialBlend = 0f;

                break;
            case AudioEvent.SOUND_TYPE.STEREO:
                audioSource.spatialBlend = 1f;

                break;
        }

        audioSource.PlayOneShot(audioEvent.Clip);
    }

    public void StopAudio()
    {
        audioSource.Stop();
    }

    public void OnGet()
    {
        gameObject.SetActive(true);
        audioSource.enabled = true;
    }

    public void OnRelease()
    {
        audioSource.enabled = false;
        gameObject.SetActive(false);
    }
}