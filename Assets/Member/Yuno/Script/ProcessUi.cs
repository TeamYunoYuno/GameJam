using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProcessUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private Button toggleButton;
    [SerializeField] private TextMeshProUGUI buttonText;

    private ProcessData _processData;

    public void Setup(ProcessData processData)
    {
        _processData = processData;
        
        // 기본 텍스트 갱신
        nameText.text = _processData.processName;
        costText.text = $"{_processData.ramCost} KB";

        // 버튼 클릭 및 상태 변화 이벤트 연결
        toggleButton.onClick.AddListener(OnButtonClicked);
        _processData.OnStateChanged += UpdateStateUI;

        // 초기 UI 렌더링
        UpdateStateUI(_processData.IsActive);
    }

    private void UpdateStateUI(bool isActive)
    {
        if (isActive)
        {
            stateText.text = "Run";
            stateText.color = Color.green;
            buttonText.text = "Pause";
        }
        else
        {
            stateText.text = "Pause";
            stateText.color = Color.gray;
            buttonText.text = "Run";
        }
    }

    private void OnButtonClicked()
    {
        // 켜거나 끄는 로직은 매니저에게 위임합니다.
        MemoryManager.Instance.RequestToggleProcess(_processData);
    }

    private void OnDestroy()
    {
        if (_processData != null)
        {
            _processData.OnStateChanged -= UpdateStateUI;
        }
        toggleButton.onClick.RemoveListener(OnButtonClicked);
    }
}