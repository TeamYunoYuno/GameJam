using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class MemoryManager : MonoBehaviour
{
    public static MemoryManager Instance { get; private set; }

    [Header("RAM Allocation Settings")]
    public int maxRAM = 512;
    public int CurrentRAM { get; private set; }

    [Header("Process List")]
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
            if (process == null) continue;

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

        // 1. 공중 조작 불가 검사
        if (process.requiresGrounded && PlayerMovement2D.Instance != null && !PlayerMovement2D.Instance.IsGrounded)
        {
            ConsoleManager.Instance?.LogError($"[DENIED] Cannot execute '{process.processName}'. Ground contact required.");
            return;
        }

        // 2. 프로세스 끄기 (RAM 반환)
        if (process.IsActive)
        {
            CurrentRAM -= process.ramCost;
            if (CurrentRAM < 0) CurrentRAM = 0;

            process.SetState(false);
            
            // 🔊 [SFX] 프로세스 OFF 효과음 재생
            SoundManager.instance?.PlaySFX("Off");
            
            ConsoleManager.Instance?.LogError($"[TERMINATED] {process.processName} killed. (RAM +{process.ramCost}KB freed)");
        }
        // 3. 프로세스 켜기 시도
        else
        {
            if (CurrentRAM + process.ramCost <= maxRAM)
            {
                CurrentRAM += process.ramCost;
                process.SetState(true);
                
                // 🔊 [SFX] 프로세스 ON 효과음 재생
                SoundManager.instance?.PlaySFX("On");
                
                ConsoleManager.Instance?.LogSystem($"[EXECUTED] {process.processName} running. (RAM -{process.ramCost}KB allocated)");
            }
            else
            {
                ConsoleManager.Instance?.LogError($"[SYS ERROR] Memory allocation failed! Cannot run '{process.processName}'.");
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowWarningGlitchAsync().Forget();
                }
            }
        }
    }
}