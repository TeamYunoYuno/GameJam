using System.Collections.Generic;
using UnityEngine;

public class MemoryManager : MonoBehaviour
{
    public static MemoryManager Instance { get; private set; } //[cite: 5]

    public int maxRAM = 512; //[cite: 5]
    public int CurrentRAM { get; private set; } //[cite: 5]

    [SerializeField] private List<ProcessData> allProcesses; //[cite: 5]
    
    // UI에서 접근할 수 있도록 리스트를 반환하는 프로퍼티 추가
    public IReadOnlyList<ProcessData> AllProcesses => allProcesses; 

    private void Awake()
    {
        Instance = this; //[cite: 5]
    }

    private void Start()
    {
        CurrentRAM = 0; //[cite: 5]
        
        foreach (var process in allProcesses) //[cite: 5]
        {
            process.InitState(); //[cite: 5]
            if (process.isDefaultOn) //[cite: 5]
            {
                CurrentRAM += process.ramCost; //[cite: 5]
            }
        }
    }

    public void RequestToggleProcess(ProcessData process) //[cite: 5]
    {
        if (process.IsActive) //[cite: 5]
        {
            CurrentRAM -= process.ramCost; //[cite: 5]
            process.SetState(false); //[cite: 5]
        }
        else
        {
            if (CurrentRAM + process.ramCost <= maxRAM) //[cite: 5]
            {
                CurrentRAM += process.ramCost; //[cite: 5]
                process.SetState(true); //[cite: 5]
            }
            else
            {
                UIManager.Instance.ShowWarningGlitchAsync().Forget(); //[cite: 5]
            }
        }
    }
}