using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameSetting : MonoBehaviour
{
    [Header("In-Game Top Buttons")]
    [Tooltip("현재 씬을 다시 로드하는 리셋 버튼")]
    [SerializeField] private Button restartButton;
    [Tooltip("오디오 설정창을 여는 오디오 버튼")]
    [SerializeField] private Button audioButton;

    [Header("Audio Settings Popup Panel")]
    [Tooltip("Hierarchy에 있는 AudioSettingsWindow 패널 오브젝트")]
    [SerializeField] private GameObject audioSettingsPanel;

    private void Start()
    {
        // 1. 리셋 버튼 연결 (현재 활성화된 씬 재로드)
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartCurrentScene);
        }

        // 2. 오디오 설정 버튼 연결
        if (audioButton != null)
        {
            audioButton.onClick.AddListener(OpenAudioSettings);
        }

        // 3. 시작 시 오디오 창 비활성화
        if (audioSettingsPanel != null)
        {
            audioSettingsPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 현재 씬 재로드 (씬 초기화)
    /// </summary>
    public void RestartCurrentScene()
    {
        ConsoleManager.Instance?.LogSystem("[SYSTEM] Rebooting stage process...");
        
        string activeSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(activeSceneName);
    }

    /// <summary>
    /// 오디오 설정 팝업 창 토글/열기
    /// </summary>
    public void OpenAudioSettings()
    {
        if (audioSettingsPanel != null)
        {
            bool currentState = audioSettingsPanel.activeSelf;
            audioSettingsPanel.SetActive(!currentState);
            
            ConsoleManager.Instance?.LogSystem(!currentState ? "[SYSTEM] Opening Audio.cpl..." : "[SYSTEM] Closing Audio.cpl...");
        }
    }
}