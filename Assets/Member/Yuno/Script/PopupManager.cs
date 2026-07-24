using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 씬에 빈 GameObject 하나 만들어서 붙여주세요 (싱글톤).
/// popupPrefab: GroundPopupUI가 붙어있는 팝업 프리팹
/// parentCanvas: 팝업이 생성될 Canvas (Screen Space - Camera 또는 World Space 권장)
/// </summary>
public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [SerializeField] private PopupUI popupPrefab;
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.2f, 0f);

    private PopupUI _currentPopup;
    private Camera _cam;

    private void Awake()
    {
        Instance = this;
        _cam = Camera.main;
    }

    private void Update()
    {
        if (_currentPopup == null) return;

        // 좌클릭이든 우클릭이든, 팝업 바깥을 클릭하면 닫기
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (!IsPointerOverPopup())
            {
                ClosePopup();
            }
        }
    }

    public void ShowPopup(ProcessData process, Vector3 worldPosition)
    {
        // 같은 땅을 다시 우클릭해도, 다른 땅을 우클릭해도 기존 팝업은 정리하고 새로 띄움
        ClosePopup();
 
        _currentPopup = Instantiate(popupPrefab, parentCanvas.transform);
 
        RectTransform rt = _currentPopup.GetComponent<RectTransform>();
 
        if (parentCanvas.renderMode == RenderMode.WorldSpace)
{
    _currentPopup.transform.position = worldPosition + worldOffset;
}
        else if (parentCanvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            // 1) 씬을 찍고 있는 카메라 기준으로 월드 -> 스크린 좌표 변환
            Camera sceneCam = _cam != null ? _cam : Camera.main;
            Vector3 screenPos = sceneCam.WorldToScreenPoint(worldPosition + worldOffset);
 
            // 2) 스크린 좌표 -> Canvas의 로컬(anchored) 좌표로 변환
            //    이때 카메라는 Canvas에 물려있는 카메라(parentCanvas.worldCamera)를 써야 정확함
            RectTransform canvasRect = parentCanvas.transform as RectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPos,
                parentCanvas.worldCamera,
                out Vector2 localPoint
            );
 
            rt.anchoredPosition = localPoint;
        }
        else // Screen Space - Overlay
        {
            Camera sceneCam = _cam != null ? _cam : Camera.main;
            Vector3 screenPos = sceneCam.WorldToScreenPoint(worldPosition + worldOffset);
            rt.position = screenPos;
        }
 
        _currentPopup.Setup(process);
    }

    public void ClosePopup()
    {
        if (_currentPopup != null)
        {
            Destroy(_currentPopup.gameObject);
            _currentPopup = null;
        }
    }

    private bool IsPointerOverPopup()
    {
        if (EventSystem.current == null) return false;

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject.transform.IsChildOf(_currentPopup.transform))
            {
                return true;
            }
        }
        return false;
    }
}