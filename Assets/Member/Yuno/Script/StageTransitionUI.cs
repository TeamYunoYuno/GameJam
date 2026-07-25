using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class StageTransitionUI : MonoBehaviour
{
    public static StageTransitionUI Instance { get; private set; }

    [Header("UI Elements")]
    [Tooltip("화면 전체를 덮는 흑백/그린 톤 패널")]
    [SerializeField] private GameObject transitionPanel;
    
    [Tooltip("부팅 로그가 출력될 TMP 텍스트")]
    [SerializeField] private TextMeshProUGUI bootLogText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (transitionPanel != null)
            transitionPanel.SetActive(false);
    }

    /// <summary>
    /// 일반 스테이지 클리어 UI 연출
    /// </summary>
    public async UniTask PlayStageClearTransitionAsync(string nextSceneName)
    {
        if (transitionPanel == null)
        {
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        transitionPanel.SetActive(true);
        if (bootLogText != null) bootLogText.text = "";

        await AppendLogAsync("> SYSTEM REBOOT INITIATED...", 200);
        await AppendLogAsync("> DUMPING CURRENT RAM STATE... [OK]", 250);
        await AppendLogAsync("> MOUNTING NEXT STAGE SECTOR...", 300);
        await AppendLogAsync($"> EXECUTING {nextSceneName.ToUpper()}.EXE...", 400);

        await UniTask.Delay(300);
        SceneManager.LoadScene(nextSceneName);
    }

    /// <summary>
    /// 🎬 [게임 최종 엔딩 전용 UI 연출]
    /// </summary>
    public async UniTask PlayEndingTransitionAsync(string mainMenuSceneName = "MainMenu")
    {
        if (transitionPanel == null) return;

        transitionPanel.SetActive(true);
        if (bootLogText != null) bootLogText.text = "";

        // ----------------------------------------------------
        // [1단계: 시스템 부팅 및 에러 출력]
        // ----------------------------------------------------
        await AppendLogAsync("> SYSTEM REBOOT INITIATED...", 200);
        await AppendLogAsync("> SEARCHING FOR NEXT_STAGE.EXE...", 400);
        await AppendLogAsync("> [FATAL ERROR] STAGE_INDEX_OUT_OF_BOUNDS!", 500);
        await AppendLogAsync("> NO MORE STAGES AVAILABLE IN SYSTEM REPOSITORY.", 600);

        // 플레이어가 에러 로그를 읽을 수 있도록 1.2초간 정지
        await UniTask.Delay(1200);

        // ----------------------------------------------------
        // [2단계: 터미널 화면 지우기 (CLS 연출 - 텍스트 짤림 방지)]
        // ----------------------------------------------------
        if (bootLogText != null) bootLogText.text = ""; 
        SoundManager.instance?.PlaySFX("On"); // 화면 초기화 비프음
        await UniTask.Delay(300);

        // ----------------------------------------------------
        // [3단계: 깨끗해진 화면에 엔딩 감사 문구 출력]
        // ----------------------------------------------------
        await AppendLogAsync("========================================", 150);
        await AppendLogAsync("        THANK YOU FOR PLAYING!          ", 300);
        await AppendLogAsync("  You have successfully cleared all stages.", 300);
        await AppendLogAsync("========================================\n", 200);

        await AppendLogAsync("> Returning to Main Menu in 5 seconds...", 1000);

        // 4초 후 메인 메뉴로 복귀
        await UniTask.Delay(4000);

        if (!string.IsNullOrEmpty(mainMenuSceneName) && Application.CanStreamedLevelBeLoaded(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }

    private async UniTask AppendLogAsync(string line, int delayMs)
    {
        if (bootLogText != null)
        {
            bootLogText.text += line + "\n";
            SoundManager.instance?.PlaySFX("On"); // 글자 추가 시 효과음
        }
        await UniTask.Delay(delayMs);
    }
}