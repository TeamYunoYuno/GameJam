using UnityEngine;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Collider2D))]
public class GameEndingFile : MonoBehaviour
{
    [Header("Clear Settings")]
    public int requiredFreeRAM = 128;

    [Header("End Game Option")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            ExecuteConsoleEndingAsync().Forget();
        }
    }

    private async UniTaskVoid ExecuteConsoleEndingAsync()
    {
        // 1. 플레이어 조작 잠금
        if (PlayerMovement2D.Instance != null)
        {
            PlayerMovement2D.Instance.enabled = false;
            PlayerMovement2D.Instance.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }

        ConsoleManager.Instance.Log("--------------------------------");
        ConsoleManager.Instance.LogSystem("Executing Final_Extract.bat...");
        await UniTask.Delay(400);

        int freeRAM = MemoryManager.Instance.maxRAM - MemoryManager.Instance.CurrentRAM;
        
        ConsoleManager.Instance.LogSystem("Preparing final data extraction...");
        await UniTask.Delay(300);

        // 2. 게이지 바 연출
        string progressBase = "Allocating: [";
        ConsoleManager.Instance.Log(progressBase + "]"); 
        
        int totalBars = 20;
        int targetBars = (int)((float)freeRAM / requiredFreeRAM * totalBars);
        if (targetBars > totalBars) targetBars = totalBars;

        SoundManager.instance?.PlayLoopSFX("Check");

        string currentBars = "";
        for (int i = 0; i < totalBars; i++)
        {
            if (i >= targetBars && freeRAM < requiredFreeRAM) 
            {
                await UniTask.Delay(200);
                break;
            }

            currentBars += "#";
            ConsoleManager.Instance.UpdateLastLine(progressBase + currentBars + "]", "#FFFFFF");
            await UniTask.Delay(50);
        }

        await UniTask.Delay(400);
        SoundManager.instance?.StopLoopSFX();

        // 3. 성공 시 엔딩 UI 전환 연출 실행
        if (freeRAM >= requiredFreeRAM)
        {
            SoundManager.instance?.PlaySFX("Clear");
            ConsoleManager.Instance.LogSuccess("[OK] Final memory allocation complete.");
            
            await UniTask.Delay(600);

            // 🎬 화면 전체 UI 연출 팝업 실행!
            if (StageTransitionUI.Instance != null)
            {
                await StageTransitionUI.Instance.PlayEndingTransitionAsync(mainMenuSceneName);
            }
        }
        else
        {
            // ❌ 실패 시 처리
            int deficitRAM = requiredFreeRAM - freeRAM;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowWarningGlitchAsync().Forget();
            }

            ConsoleManager.Instance.LogError("[FATAL ERROR] EXTRACTION FAILED: OUT OF MEMORY");
            ConsoleManager.Instance.Log($" > Required RAM : {requiredFreeRAM} KB");
            ConsoleManager.Instance.Log($" > Current Free : {freeRAM} KB");
            ConsoleManager.Instance.LogError($" > {deficitRAM} KB More RAM Space");
            
            await UniTask.Delay(1500);
            ConsoleManager.Instance.Log("--------------------------------");

            if (PlayerMovement2D.Instance != null)
            {
                PlayerMovement2D.Instance.enabled = true;
            }
            isTriggered = false;
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