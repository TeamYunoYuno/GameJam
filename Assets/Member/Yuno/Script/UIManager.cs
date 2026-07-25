using UnityEngine;
using TMPro; //[cite: 8]
using Cysharp.Threading.Tasks; //[cite: 8]

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } //[cite: 8]

    [Header("RAM Info")]
    [SerializeField] private TextMeshProUGUI ramText; //[cite: 8]
    [SerializeField] private GameObject warningPanel; //[cite: 8]

    [Header("Scroll View Settings")]
    [SerializeField] private Transform processListContent; // 스크롤뷰의 Content 지정
    [SerializeField] private GameObject processUIItemPrefab; // 만들어둘 ProcessUIItem 프리팹

    private void Awake()
    {
        Instance = this; //[cite: 8]
    }

    private void Start()
    {
        // 시작 시 모든 프로세스 UI를 스크롤뷰에 생성합니다.
        InitializeProcessUI();
    }

    private void Update()
    {
        ramText.text = $"RAM: {MemoryManager.Instance.CurrentRAM} / {MemoryManager.Instance.maxRAM} KB"; //[cite: 8]
    }

    private void InitializeProcessUI()
    {
        foreach (var processData in MemoryManager.Instance.AllProcesses)
        {
            GameObject uiObj = Instantiate(processUIItemPrefab, processListContent);
            ProcessUI uiItem = uiObj.GetComponent<ProcessUI>();
            uiItem.Setup(processData);
        }
    }

    public async UniTaskVoid ShowWarningGlitchAsync() //[cite: 8]
    {
        SoundManager.instance.PlaySFX("Error");
        warningPanel.SetActive(true); //[cite: 8]
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.2)); //[cite: 8]
        warningPanel.SetActive(false); //[cite: 8]
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.2));
        warningPanel.SetActive(true);
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.2)); //[cite: 8]
        warningPanel.SetActive(false); //[cite: 8]
    }
}