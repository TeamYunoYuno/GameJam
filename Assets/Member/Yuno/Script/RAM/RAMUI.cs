using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RAMBlockMeterUI : MonoBehaviour
{
    [Header("LED 블록 배열 (맨 아래 블록부터 0번부터 순서대로 넣으세요)")]
    [SerializeField] private Image[] blockImages;

    [Header("하단 수치 텍스트")]
    [SerializeField] private TextMeshProUGUI usageText;

    private void Update()
    {
        if (MemoryManager.Instance == null) return;

        // 0.0 ~ 1.0 비율 계산
        float ratio = Mathf.Clamp01((float)MemoryManager.Instance.CurrentRAM / MemoryManager.Instance.maxRAM);

        // 전체 블록 개수 중 켜져야 할 개수 계산
        int activeCount = Mathf.RoundToInt(ratio * blockImages.Length);

        // LED 블록 하나씩 켜기 / 끄기
        for (int i = 0; i < blockImages.Length; i++)
        {
            if (blockImages[i] != null)
            {
                blockImages[i].enabled = (i < activeCount);
            }
        }

        // 하단 텍스트 업데이트 (예: 320K)
        if (usageText != null)
        {
            usageText.text = $"{MemoryManager.Instance.CurrentRAM}K";
        }
    }
}