using UnityEngine;

public class BossMusicManager : MonoBehaviour
{
    [Header("Audio Sources y Clips")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip phase1Music;
    [SerializeField] private AudioClip phase2Music;

    private bool phase2Started = false;

    public void PlayPhase1Music()
    {
        if (musicSource != null && phase1Music != null)
        {
            musicSource.clip = phase1Music;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void SwitchToPhase2Music()
    {
        if (phase2Started) return;

        phase2Started = true;
        if (musicSource != null && phase2Music != null)
        {
            musicSource.clip = phase2Music;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
}
