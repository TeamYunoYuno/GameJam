using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 땅을 우클릭했을 때 뜨는 팝업 (이름 + 켜기/끄기 버튼).
/// 프리팹 루트에 CanvasGroup을 붙이고, 자식으로 TMP_Text(이름)와 Button(토글)을 배치한 뒤
/// 인스펙터에서 아래 필드들을 연결해서 프리팹으로 저장해두세요.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class PopupUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button toggleButton;
    [SerializeField] private TextMeshProUGUI toggleButtonLabel;
    [SerializeField] private Button closeButton;

    private ProcessData _currentProcess;

    private void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        toggleButton.onClick.AddListener(OnToggleClicked);

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseClicked);
        }
    }

    private void OnEnable()
    {
        if (_currentProcess != null)
        {
            _currentProcess.OnStateChanged += HandleStateChanged;
        }
    }

    private void OnDisable()
    {
        if (_currentProcess != null)
        {
            _currentProcess.OnStateChanged -= HandleStateChanged;
        }
    }

    public void Setup(ProcessData process)
    {
        // 기존에 구독 중이던 process가 있다면 해제
        if (_currentProcess != null)
        {
            _currentProcess.OnStateChanged -= HandleStateChanged;
        }

        _currentProcess = process;
        // ProcessData가 ScriptableObject라면 별도 필드 없이도 에셋 이름(name)을 그대로 사용할 수 있습니다.
        // 만약 ProcessData 안에 표시용 이름 필드(예: displayName)가 따로 있다면 그걸로 바꿔주세요.
        nameText.text = _currentProcess.processName;

        _currentProcess.OnStateChanged += HandleStateChanged;

        UpdateButtonLabel(_currentProcess.IsActive);
    }

    private void OnToggleClicked()
    {
        MemoryManager.Instance.RequestToggleProcess(_currentProcess);
        // RequestToggleProcess가 성공하지 못하는 경우(RAM 부족)도 있으므로,
        // 버튼 라벨은 HandleStateChanged(OnStateChanged 이벤트)에서 실제 상태 변경 시점에 갱신합니다.
    }

    public void OnCloseClicked()
    {
       PopupManager.Instance.ClosePopup();
    }

    private void HandleStateChanged(bool isProcessOn)
    {
        UpdateButtonLabel(isProcessOn);
    }

    private void UpdateButtonLabel(bool isOn)
    {
        if (toggleButtonLabel != null)
        {
            toggleButtonLabel.text = isOn ? "OFF" : "ON";
        }
    }
}