using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [Header("Audio Toggles (independent)")]
    public Toggle bgmToggle;
    public Toggle sfxToggle;

    // Theme toggles disabled for now
    // [Header("Theme Toggles (mutually exclusive)")]
    // public Toggle regularThemeToggle;
    // public Toggle invertedThemeToggle;
    // public GameObject invertOverlay;

    void Start()
    {
        // Load saved audio settings
        bgmToggle.isOn = PlayerPrefs.GetInt("BGMEnabled", 1) == 1;
        sfxToggle.isOn = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;

        // Theme loading disabled
        // int savedTheme = PlayerPrefs.GetInt("Theme", 0);
        // if (savedTheme == 1)
        // {
        //     invertedThemeToggle.isOn = true;
        //     invertOverlay.SetActive(true);
        // }
        // else
        // {
        //     regularThemeToggle.isOn = true;
        //     invertOverlay.SetActive(false);
        // }

        bgmToggle.onValueChanged.AddListener(OnBGMChanged);
        sfxToggle.onValueChanged.AddListener(OnSFXChanged);
        // regularThemeToggle.onValueChanged.AddListener(OnRegularTheme);
        // invertedThemeToggle.onValueChanged.AddListener(OnInvertedTheme);
    }

    public void OnBGMChanged(bool isOn)
    {
        AudioManager.Instance.ToggleBGM(isOn);
    }

    public void OnSFXChanged(bool isOn)
    {
        AudioManager.Instance.ToggleSFX(isOn);
    }

    // Theme methods disabled
    // public void OnRegularTheme(bool isOn) { ... }
    // public void OnInvertedTheme(bool isOn) { ... }
}