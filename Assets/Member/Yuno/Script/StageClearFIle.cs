using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class StageClearFile : MonoBehaviour
{
    [Header("Clear Settings")]
    public int requiredFreeRAM = 128;
    public string nextSceneName = "Stage2";

    private bool isTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            ExecuteConsoleClearAsync().Forget();
        }
    }

    private async UniTaskVoid ExecuteConsoleClearAsync()
    {
        // 1. 플레이어 조작 잠금
        if (PlayerMovement2D.Instance != null)
        {
            PlayerMovement2D.Instance.enabled = false;
            PlayerMovement2D.Instance.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }

        ConsoleManager.Instance.Log("--------------------------------");
        ConsoleManager.Instance.LogSystem("Executing Extract.bat...");
        await UniTask.Delay(400);

        int freeRAM = MemoryManager.Instance.maxRAM - MemoryManager.Instance.CurrentRAM;
        
        ConsoleManager.Instance.LogSystem("Preparing data extraction...");
        await UniTask.Delay(300);

        // 2. 진행 게이지 바 연출
        string progressBase = "Allocating: [";
        ConsoleManager.Instance.Log(progressBase + "]"); 
        
        int totalBars = 20;
        int targetBars = (int)((float)freeRAM / requiredFreeRAM * totalBars);
        if (targetBars > totalBars) targetBars = totalBars;

        // 💡 검사 진행 시작: Check 효과음 루프 재생
        SoundManager.instance?.PlayLoopSFX("Check");

        string currentBars = "";
        for (int i = 0; i < totalBars; i++)
        {
            if (i >= targetBars && freeRAM < requiredFreeRAM) 
            {
                await UniTask.Delay(200);
                break; // 램 부족 시 멈춤
            }

            currentBars += "#";
            ConsoleManager.Instance.UpdateLastLine(progressBase + currentBars + "]", "#FFFFFF");
            await UniTask.Delay(50);
        }

        await UniTask.Delay(400);

        // 💡 게이지 채우기 종료: Check 효과음 중지
        SoundManager.instance?.StopLoopSFX();

        // 3. 성공 / 실패 판정
        if (freeRAM >= requiredFreeRAM)
        {
            // ⭕ 성공
            SoundManager.instance?.PlaySFX("Clear"); // 💡 Clear 효과음 재생

            ConsoleManager.Instance.LogSuccess("[OK] Memory allocation and extraction complete.");
            ConsoleManager.Instance.LogSystem("Transferring system control to next stage...");
            
            await UniTask.Delay(800);

            // 🎬 4. UI 전환 연출 실행 후 씬 전환
            if (StageTransitionUI.Instance != null)
            {
                await StageTransitionUI.Instance.PlayStageClearTransitionAsync(nextSceneName);
            }
            else
            {
                // UI가 없을 때의 예비 동작
                SceneManager.LoadScene(nextSceneName);
            }
        }
        else
        {
            // ❌ 실패: 부족한 용량 계산 및 상세 에러 출력
            int deficitRAM = requiredFreeRAM - freeRAM;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowWarningGlitchAsync().Forget();
            }

            ConsoleManager.Instance.LogError("[FATAL ERROR] EXTRACTION FAILED: OUT OF MEMORY");
            ConsoleManager.Instance.Log($" > Required RAM : {requiredFreeRAM} KB");
            ConsoleManager.Instance.Log($" > Current Free : {freeRAM} KB");
            ConsoleManager.Instance.LogError($" > {deficitRAM} KB More RAM Space");
            ConsoleManager.Instance.LogSystem("Please terminate processes to free up more memory.");
            
            await UniTask.Delay(1500);
            ConsoleManager.Instance.Log("--------------------------------");

            // 플레이어 조작 다시 허용
            if (PlayerMovement2D.Instance != null)
            {
                PlayerMovement2D.Instance.enabled = true;
            }
            isTriggered = false; // 다시 시도 가능하게 리셋
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !PlayerMovement2D.Instance.enabled)
        {
            isTriggered = false;
        }
    }
}