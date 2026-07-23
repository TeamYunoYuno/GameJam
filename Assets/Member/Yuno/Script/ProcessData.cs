using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewProcess", menuName = "SpaceJam/ProcessData")]
public class ProcessData : ScriptableObject
{
    public int ramCost; //[cite: 4]
    
    // 게임 시작 시 기본적으로 켜져 있어야 하는 필수 프로세스인지 인스펙터에서 설정합니다.
    public bool isDefaultOn; 

    public bool IsActive { get; private set; } //[cite: 4]
    public event Action<bool> OnStateChanged; //[cite: 4]

    // 게임 시작 시 무조건 호출되어 SO의 상태를 초기화합니다.
    public void InitState()
    {
        IsActive = isDefaultOn;
        OnStateChanged?.Invoke(IsActive);
    }

    public void SetState(bool state) //[cite: 4]
    {
        IsActive = state;
        OnStateChanged?.Invoke(IsActive); //[cite: 4]
    }
}