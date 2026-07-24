using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ProcessUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    
    [Header("Indicator")]
    [SerializeField] private Image statusLight;
    [SerializeField] private Color onColor = Color.green;
    [SerializeField] private Color offColor = Color.gray;

    [Header("Button Settings")]
    [SerializeField] private Button toggleButton;
    [SerializeField] private Button toggleButton2;
    [SerializeField] private Image playImage;   // 프로세스가 꺼졌을 때 보일 실행(▶) Image
    [SerializeField] private Image pauseImage;  // 프로세스가 켜졌을 때 보일 중지(■) Image

    [Header("Dimming Overlay")]
    [Tooltip("전체 항목을 덮는 반투명 회색 이미지 (회색빛 틴트 효과)")]
    [SerializeField] private Image dimmedOverlay;

    [SerializeField] private GameObject highlight;

    private ProcessData _processData;

    public void Setup(ProcessData processData) //[cite: 3]
    {
        _processData = processData; //[cite: 3]
        
        nameText.text = _processData.processName; //[cite: 3]
        costText.text = $"{_processData.ramCost}KB"; //[cite: 3]

        toggleButton.onClick.AddListener(OnButtonClicked); //[cite: 3]
        toggleButton2.onClick.AddListener(OnButtonClicked);
        _processData.OnStateChanged += UpdateStateUI; //[cite: 3]
        highlight.SetActive(false);

        UpdateStateUI(_processData.IsActive); //[cite: 3]
    }

    private void UpdateStateUI(bool isActive)
    {
        if (isActive)
        {
            // [실행 중 상태]
            statusLight.color = onColor;
            
            // 켜졌을 때는 Pause 아이콘만 보이게 세팅
            if (playImage != null) playImage.gameObject.SetActive(false);
            if (pauseImage != null) pauseImage.gameObject.SetActive(true);

            // 전체 회색빛 해제 (꺼짐)
            if (dimmedOverlay != null) dimmedOverlay.gameObject.SetActive(false);
        }
        else
        {
            // [중지됨 상태]
            statusLight.color = offColor;
            
            // 꺼졌을 때는 Play 아이콘만 보이게 세팅
            if (playImage != null) playImage.gameObject.SetActive(true);
            if (pauseImage != null) pauseImage.gameObject.SetActive(false);

            // 전체 회색빛 적용 (켜짐)
            if (dimmedOverlay != null) dimmedOverlay.gameObject.SetActive(true);

        }
    }

    private void OnButtonClicked()
    {
        MemoryManager.Instance.RequestToggleProcess(_processData);
    }

    private void OnDestroy() //[cite: 3]
    {
        if (_processData != null) //[cite: 3]
        {
            _processData.OnStateChanged -= UpdateStateUI; //[cite: 3]
            _processData.SetHoverState(false); // 혹시 파괴될 때 호버가 켜져있을 경우를 대비한 안전장치
        }
        toggleButton.onClick.RemoveListener(OnButtonClicked); //[cite: 3]
        toggleButton2.onClick.RemoveListener(OnButtonClicked);
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_processData != null)
        {
            _processData.SetHoverState(true);
            highlight.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_processData != null)
        {
            _processData.SetHoverState(false);
            highlight.SetActive(false);
        }
    }
}