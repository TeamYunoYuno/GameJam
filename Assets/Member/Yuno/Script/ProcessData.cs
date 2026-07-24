using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewProcess", menuName = "SpaceJam/ProcessData")]
public class ProcessData : ScriptableObject
{
    [Header("Process Info")]
    public string processName = "NewProcess.exe";
    public int ramCost;
    public bool isDefaultOn;

    [Header("Constraints")]
    [Tooltip("체크 시 플레이어가 땅(또는 천장)에 닿아있을 때만 토글할 수 있습니다.")]
    public bool requiresGrounded = false; // 👈 추가된 변수

    public bool IsActive { get; private set; }

    public event Action<bool> OnStateChanged;
    public event Action<bool> OnHoverChanged; 
    
    public void InitState()
    {
        IsActive = isDefaultOn;
        OnStateChanged?.Invoke(IsActive);
    }

    public void SetState(bool state)
    {
        IsActive = state;
        OnStateChanged?.Invoke(IsActive);
    }

    public void SetHoverState(bool isHovered)
    {
        OnHoverChanged?.Invoke(isHovered);
    }
}