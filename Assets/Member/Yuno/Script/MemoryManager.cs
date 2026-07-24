using System.Collections.Generic;
using UnityEngine;

public class MemoryManager : MonoBehaviour
{
    public static MemoryManager Instance { get; private set; }

    public int maxRAM = 512;
    public int CurrentRAM { get; private set; }

    [SerializeField] private List<ProcessData> allProcesses;
    
    public IReadOnlyList<ProcessData> AllProcesses => allProcesses; 

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CurrentRAM = 0;
        
        foreach (var process in allProcesses)
        {
            process.InitState();
            if (process.isDefaultOn)
            {
                CurrentRAM += process.ramCost;
            }
        }
    }

    public void RequestToggleProcess(ProcessData process)
    {
        if (process == null) return;

        // 1. 공중 조작 불가 로그
        if (process.requiresGrounded && PlayerMovement2D.Instance != null && !PlayerMovement2D.Instance.IsGrounded)
        {
            ConsoleManager.Instance.LogError($"[DENIED] Cannot execute '{process.processName}'. Ground contact required.");
            return;
        }

        // 2. 프로세스 끄기 (RAM 반환)
        if (process.IsActive)
        {
            CurrentRAM -= process.ramCost;
            process.SetState(false);
            
            // 💡 프로세스 강제 종료 로그
            ConsoleManager.Instance.LogError($"[TERMINATED] {process.processName} killed. (RAM +{process.ramCost}KB freed)");
        }
        // 3. 프로세스 켜기 시도
        else
        {
            if (CurrentRAM + process.ramCost <= maxRAM)
            {
                CurrentRAM += process.ramCost;
                process.SetState(true);
                
                // 💡 프로세스 실행 성공 로그
                ConsoleManager.Instance.LogSystem($"[EXECUTED] {process.processName} running. (RAM -{process.ramCost}KB allocated)");
            }
            else
            {
                // 💡 RAM 부족 에러 로그
                ConsoleManager.Instance.LogError($"[SYS ERROR] Memory allocation failed! Cannot run '{process.processName}'.");
                UIManager.Instance.ShowWarningGlitchAsync().Forget();
            }
        }
    }
}