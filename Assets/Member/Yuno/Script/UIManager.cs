using UnityEngine;
using TMPro; //[cite: 5]
using Cysharp.Threading.Tasks; //[cite: 5]

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } //[cite: 5]

    [SerializeField] private TextMeshProUGUI ramText; //[cite: 5]
    [SerializeField] private GameObject warningPanel; //[cite: 5]

    private void Awake()
    {
        Instance = this; //[cite: 5]
    }

    private void Update()
    {
        ramText.text = $"RAM: {MemoryManager.Instance.CurrentRAM} / {MemoryManager.Instance.maxRAM} KB"; //[cite: 5]
    }

    // 용량이 꽉 찼는데 버튼을 누르면 0.2초간 빨간 화면(warningPanel)이 번쩍입니다[cite: 5].
    public async UniTaskVoid ShowWarningGlitchAsync() //[cite: 5]
    {
        warningPanel.SetActive(true); //[cite: 5]
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.2)); //[cite: 5]
        warningPanel.SetActive(false); //[cite: 5]
    }
}