using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsWindow : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Close Button")]
    [SerializeField] private Button closeButton;

    private void Start()
    {
        // 1. 슬라이더 기본값 설정 (0.0 ~ 1.0)
        if (masterSlider != null) masterSlider.value = 0.25f;
        if (bgmSlider != null) bgmSlider.value = 0.25f;
        if (sfxSlider != null) sfxSlider.value = 0.25f;

        // 2. 슬라이더 변경 이벤트 연결
        if (masterSlider != null) 
            masterSlider.onValueChanged.AddListener(val => SoundManager.instance.SetMasterVolume(val));
        
        if (bgmSlider != null) 
            bgmSlider.onValueChanged.AddListener(val => SoundManager.instance.SetBGMVolume(val));
        
        if (sfxSlider != null) 
            sfxSlider.onValueChanged.AddListener(val => SoundManager.instance.SetSFXVolume(val));

        // 3. 닫기 버튼 연결
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseWindow);
    }

    private void CloseWindow()
    {
        ConsoleManager.Instance?.LogSystem("[SYSTEM] Audio.cpl closed.");
        gameObject.SetActive(false);
    }
}