using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource ambientSource;
    public AudioSource sfxSource;

    [Header("Clips")]
    public AudioClip ambientClip;
    public AudioClip placementClip;
    public AudioClip carSpawnClip;
    public AudioClip carDestroyClip;
    public AudioClip ratDieClip;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (ambientSource != null && ambientClip != null)
        {
            ambientSource.clip = ambientClip;
            ambientSource.loop = true;
            ambientSource.Play();
        }
    }

    public void PlayPlacement() => PlaySFX(placementClip);
    public void PlayCarSpawn() => PlaySFX(carSpawnClip);
    public void PlayCarDestroy() => PlaySFX(carDestroyClip);
    public void PlayRatDie() => PlaySFX(ratDieClip);

    private void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);
    }
}
