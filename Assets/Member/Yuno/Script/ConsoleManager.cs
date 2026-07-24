using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ConsoleManager : MonoBehaviour
{
    public static ConsoleManager Instance { get; private set; }

    [Header("UI Reference")]
    [Tooltip("콘솔 텍스트를 출력할 TextMeshProUGUI 컴포넌트")]
    [SerializeField] private TextMeshProUGUI consoleText;

    [Header("Settings")]
    [Tooltip("콘솔에 유지할 최대 줄 수")]
    [SerializeField] private int maxLines = 15;

    private List<string> logLines = new List<string>();

    private void Awake()
    {
        Instance = this;
        consoleText.text = "";
        LogSystem("OS Booted Successfully. Welcome, User.");
    }

    // 기본 흰색 로그
    public void Log(string message)
    {
        AddLine($"<color=#FFFFFF>> {message}</color>");
    }

    // 성공/시스템 초록색 로그
    public void LogSuccess(string message)
    {
        AddLine($"<color=#00FF00>> {message}</color>");
    }

    // 에러 빨간색 로그
    public void LogError(string message)
    {
        AddLine($"<color=#FF0000>> {message}</color>");
    }

    // 시스템 안내 사이언(하늘색) 로그
    public void LogSystem(string message)
    {
        AddLine($"<color=#00FFFF>> {message}</color>");
    }

    // 💡 텍스트 프로그레스 바 연출 등을 위해 "마지막 줄"을 수정하는 함수
    public void UpdateLastLine(string newText, string colorHex = "#FFFFFF")
    {
        if (logLines.Count == 0) return;
        
        logLines[logLines.Count - 1] = $"<color={colorHex}>> {newText}</color>";
        UpdateTextUI();
    }

    private void AddLine(string formattedMessage)
    {
        logLines.Add(formattedMessage);
        
        // 최대 줄 수를 넘어가면 가장 오래된 위쪽 텍스트 삭제
        if (logLines.Count > maxLines)
        {
            logLines.RemoveAt(0);
        }
        
        UpdateTextUI();
    }

    private void UpdateTextUI()
    {
        if (consoleText != null)
        {
            consoleText.text = string.Join("\n", logLines);
        }
    }
}