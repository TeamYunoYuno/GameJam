using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; 
using Cysharp.Threading.Tasks;

public class MenuProcessHandler : MonoBehaviour
{
    [Header("Target Scene")]
    [SerializeField] private string stage1SceneName = "Stage 1"; 

    [Header("UI Window Settings (Audio)")]
    [Tooltip("Hierarchy에 있는 오디오 설정창 패널")]
    [SerializeField] private GameObject audioSettingsWindow;
    [Tooltip("오디오 창 내부의 'X' 닫기 버튼 (비워두면 자동 찾기)")]
    [SerializeField] private Button audioCloseButton;
    [Tooltip("오디오 창이 켜질 때의 시작 위치 (Anchored Position 기준)")]
    [SerializeField] private Vector2 audioStartPosition = new Vector2(-200f, 100f);

    [Header("UI Window Settings (Image)")] 
    [Tooltip("껐다 켰다 할 이미지 창 패널")]
    [SerializeField] private GameObject imageWindow;
    [Tooltip("이미지 창 내부의 닫기 버튼 (옵션, 비워두면 자동 찾기)")]
    [SerializeField] private Button imageCloseButton;
    [Tooltip("이미지 창이 켜질 때의 시작 위치")]
    [SerializeField] private Vector2 imageStartPosition = new Vector2(0f, 0f);

    [Header("Blink Settings")]
    [Tooltip("1초 간격으로 깜빡일 TMP 텍스트")]
    [SerializeField] private TextMeshProUGUI targetBlinkText; 

    [Header("Fake Error Window Settings")]
    [SerializeField] private GameObject errorWindowPrefab; 
    [SerializeField] private Transform canvasTransform;   
    [Tooltip("에러 창의 첫 시작 위치 (Anchored Position 기준)")]
    [SerializeField] private Vector2 errorStartPosition = new Vector2(-150f, 150f);
    [Tooltip("에러 창이 겹치지 않게 밀려날 간격")]
    [SerializeField] private Vector2 offsetPerWindow = new Vector2(40f, -40f); 

    private int errorWindowCount = 0;
    
    // 프로세스 데이터 캐싱
    private ProcessData audioProcessData;
    private ProcessData imageProcessData; 

    private void Start()
    {
        var cancellationToken = this.GetCancellationTokenOnDestroy();

        SoundManager.instance?.PlaySFX("Start");

        // 💡 시작 3초 후 MenuBGM 실행 (비동기)
        PlayDelayedMenuBGMAsync(cancellationToken).Forget();

        // 1. 시작 시 창 숨기기
        if (audioSettingsWindow != null) audioSettingsWindow.SetActive(false);
        if (imageWindow != null) imageWindow.SetActive(false); 

        // 2. 1초 간격 TMP 깜빡이기 시작
        BlinkAsync(cancellationToken).Forget();

        if (MemoryManager.Instance != null)
        {
            foreach (var process in MemoryManager.Instance.AllProcesses)
            {
                if (process == null) continue;

                string pName = process.processName.Trim();

                // 3. 프로세스 이름에 맞춰 닫기 버튼 연결 준비
                if (pName.StartsWith("Audio", System.StringComparison.OrdinalIgnoreCase))
                {
                    audioProcessData = process;
                    SetupCloseButton(ref audioCloseButton, audioSettingsWindow, audioProcessData);
                }
                else if (pName.StartsWith("Image", System.StringComparison.OrdinalIgnoreCase) || 
                         pName.StartsWith("Picture", System.StringComparison.OrdinalIgnoreCase))
                {
                    imageProcessData = process;
                    SetupCloseButton(ref imageCloseButton, imageWindow, imageProcessData);
                }

                // 이벤트 구독
                process.OnStateChanged -= (isActive) => OnProcessToggled(process, isActive);
                process.OnStateChanged += (isActive) => OnProcessToggled(process, isActive);
            }
        }
    }

    // 💡 3초 후 BGM을 재생하는 UniTask 메서드
    private async UniTaskVoid PlayDelayedMenuBGMAsync(CancellationToken cancellationToken)
    {
        try
        {
            // 3초 대기 (Time.timeScale의 영향을 받음 / 필요시 DelayType.UnscaledTime 지정 가능)
            await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: cancellationToken);
            
            SoundManager.instance?.MenuBGM();
        }
        catch (OperationCanceledException)
        {
            // 3초가 지나기 전에 씬이 이동하거나 오브젝트가 파괴되면 안전하게 종료
        }
    }

    // 1초 간격으로 TMP 텍스트를 껐다 켜는 비동기 함수
    private async UniTaskVoid BlinkAsync(CancellationToken cancellationToken)
    {
        if (targetBlinkText == null) return;

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                targetBlinkText.enabled = !targetBlinkText.enabled;
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // 오브젝트 파괴 시 안전한 정상 종료
        }
    }

    private void SetupCloseButton(ref Button closeBtn, GameObject windowObj, ProcessData targetProcess)
    {
        if (windowObj == null) return;

        if (closeBtn == null)
        {
            closeBtn = windowObj.GetComponentInChildren<Button>();
        }

        if (closeBtn != null)
        {
            closeBtn.onClick.RemoveAllListeners();
            closeBtn.onClick.AddListener(() =>
            {
                if (targetProcess != null && MemoryManager.Instance != null)
                {
                    MemoryManager.Instance.RequestToggleProcess(targetProcess); 
                }
                else
                {
                    windowObj.SetActive(false);
                }
            });
        }
    }

    private void OnProcessToggled(ProcessData process, bool isActive)
    {
        string pName = process.processName.Trim();

        // [Start 프로세스]
        if (pName.StartsWith("Start", System.StringComparison.OrdinalIgnoreCase))
        {
            if (isActive) BootGameSequenceAsync().Forget();
        }
        // [Audio 프로세스]
        else if (pName.StartsWith("Audio", System.StringComparison.OrdinalIgnoreCase))
        {
            ToggleWindow(audioSettingsWindow, isActive, audioStartPosition, pName);
        }
        // [Image / Picture 프로세스] 
        else if (pName.StartsWith("Readme", System.StringComparison.OrdinalIgnoreCase) || 
                 pName.StartsWith("Picture", System.StringComparison.OrdinalIgnoreCase))
        {
            ToggleWindow(imageWindow, isActive, imageStartPosition, pName);
        }
        // [Exit 프로세스]
        else if (pName.StartsWith("Exit", System.StringComparison.OrdinalIgnoreCase) || 
                 pName.StartsWith("Shutdown", System.StringComparison.OrdinalIgnoreCase))
        {
            if (isActive)
            {
                ShowErrorWindow();
                process.SetState(false); 
            }
        }
    }

    private void ToggleWindow(GameObject windowObj, bool isActive, Vector2 startPos, string processName)
    {
        if (windowObj != null) 
        {
            windowObj.SetActive(isActive);
            
            if (isActive)
            {
                RectTransform rect = windowObj.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchoredPosition = startPos;
                }
            }
            ConsoleManager.Instance?.LogSystem(isActive ? $"[SYSTEM] Opening {processName}..." : $"[SYSTEM] Closing {processName}...");
        }
    }

    private async UniTaskVoid BootGameSequenceAsync()
    {
        ConsoleManager.Instance?.LogSystem("[SYSTEM] Executing Start.exe...");
        await UniTask.Delay(500);
        ConsoleManager.Instance?.Log("Loading stage data...");
        await UniTask.Delay(500);
        ConsoleManager.Instance?.LogSuccess("[OK] Initializing Stage 1.");
        await UniTask.Delay(300);

        if (Application.CanStreamedLevelBeLoaded(stage1SceneName))
        {
            SceneManager.LoadScene(stage1SceneName);
        }
        else
        {
            ConsoleManager.Instance?.LogError($"[ERROR] Scene '{stage1SceneName}' not in Build Settings!");
        }
    }

    private void ShowErrorWindow()
    {
        ConsoleManager.Instance?.LogError("[ERROR] Shutdown failed. ACCESS DENIED.");

        if (errorWindowPrefab != null && canvasTransform != null)
        {
            GameObject newWindow = Instantiate(errorWindowPrefab, canvasTransform);
            RectTransform rect = newWindow.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = errorStartPosition + (offsetPerWindow * errorWindowCount);
                errorWindowCount++;
            }

            Button[] allButtons = newWindow.GetComponentsInChildren<Button>();
            foreach (Button btn in allButtons)
            {
                btn.onClick.AddListener(() => Destroy(newWindow));
            }
        }
    }
}