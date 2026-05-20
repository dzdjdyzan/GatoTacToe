using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Clips")]
    public AudioClip buttonClick;
    public AudioClip placement;
    public AudioClip strikeWin;
    public AudioClip popupAnim;

    private bool bgmEnabled = true;
    private bool sfxEnabled = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        Debug.Log("AudioManager Start running");
        bgmEnabled = PlayerPrefs.GetInt("BGMEnabled", 1) == 1;
        sfxEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
        ApplySettings();
        if (bgmEnabled && bgmSource != null && !bgmSource.isPlaying)
        {
            Debug.Log("Attempting to play BGM, clip = " + (bgmSource.clip ? bgmSource.clip.name : "NULL"));
            bgmSource.Play();
        }
        else
        {
            Debug.Log("BGM not playing: bgmEnabled=" + bgmEnabled + ", bgmSource null? " + (bgmSource == null));
        }
    }

    public void ToggleBGM(bool enable)
    {
        bgmEnabled = enable;
        PlayerPrefs.SetInt("BGMEnabled", enable ? 1 : 0);
        ApplySettings();
        if (bgmEnabled && bgmSource != null && !bgmSource.isPlaying) bgmSource.Play();
        else if (!bgmEnabled && bgmSource != null) bgmSource.Stop();
    }

    public void ToggleSFX(bool enable)
    {
        sfxEnabled = enable;
        PlayerPrefs.SetInt("SFXEnabled", enable ? 1 : 0);
    }

    void ApplySettings()
    {
        if (bgmSource != null) bgmSource.mute = !bgmEnabled;
        // SFX is handled by checking sfxEnabled before PlayOneShot
    }

    public void PlayButtonClick() { if (sfxEnabled && buttonClick) sfxSource.PlayOneShot(buttonClick); }
    public void PlayPlacement() { if (sfxEnabled && placement) sfxSource.PlayOneShot(placement); }
    public void PlayStrikeWin() { if (sfxEnabled && strikeWin) sfxSource.PlayOneShot(strikeWin); }
    public void PlayPopupAnim() { if (sfxEnabled && popupAnim) sfxSource.PlayOneShot(popupAnim); }
}