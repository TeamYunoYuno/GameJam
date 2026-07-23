using System.Collections.Generic;
using UnityEngine;

public class MemoryManager : MonoBehaviour
{
    public static MemoryManager Instance { get; private set; } //[cite: 2]

    [SerializeField] private int maxRAM = 512; //[cite: 2]
    public int CurrentRAM { get; private set; } //[cite: 2]

    // 인스펙터에서 전체 프로세스 에셋(SO)을 등록해 시작 시 용량을 계산합니다.
    [SerializeField] private List<ProcessData> allProcesses;

    private void Awake()
    {
        Instance = this; //[cite: 2]
    }

    private void Start()
    {
        CurrentRAM = 0;
        
        // 등록된 프로세스들을 순회하며 초기 램 점유율을 정확히 계산합니다.
        foreach (var process in allProcesses)
        {
            process.InitState();
            if (process.isDefaultOn)
            {
                CurrentRAM += process.ramCost;
            }
        }
    }

    public void RequestToggleProcess(ProcessData process) //[cite: 2]
    {
        if (process.IsActive) //[cite: 2]
        {
            // 끌 때는 조건 없이 용량을 반환합니다.
            CurrentRAM -= process.ramCost; //[cite: 2]
            process.SetState(false); //[cite: 2]
        }
        else
        {
            // 켤 때는 반드시 남은 용량이 충분한지 조건문으로 검사하여 한도 초과를 원천 차단합니다[cite: 2].
            if (CurrentRAM + process.ramCost <= maxRAM) //[cite: 2]
            {
                CurrentRAM += process.ramCost; //[cite: 2]
                process.SetState(true); //[cite: 2]
            }
            else
            {
                // 용량 초과 시 프로세스는 켜지지 않고 비동기 경고 연출(Glitch)만 호출합니다[cite: 2].
                UIManager.Instance.ShowWarningGlitchAsync().Forget(); //[cite: 2]
            }
        }
    }
}