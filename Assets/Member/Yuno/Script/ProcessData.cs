using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewProcess", menuName = "SpaceJam/ProcessData")] //[cite: 7]
public class ProcessData : ScriptableObject //[cite: 7]
{
    [Header("Process Info")]
    public string processName = "NewProcess.exe"; // UI에 표시될 이름 (추가됨)
    public int ramCost; //[cite: 7]
    public bool isDefaultOn; //[cite: 7]

    public bool IsActive { get; private set; } //[cite: 7]
    public event Action<bool> OnStateChanged; //[cite: 7]

    public void InitState() //[cite: 7]
    {
        IsActive = isDefaultOn; //[cite: 7]
        OnStateChanged?.Invoke(IsActive); //[cite: 7]
    }

    public void SetState(bool state) //[cite: 7]
    {
        IsActive = state; //[cite: 7]
        OnStateChanged?.Invoke(IsActive); //[cite: 7]
    }
}