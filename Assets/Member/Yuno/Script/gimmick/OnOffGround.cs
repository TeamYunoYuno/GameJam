using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps; // 💡 Tilemap이 아닌 Tilemaps 네임스페이스 사용

// IPointerEnterHandler, IPointerExitHandler 추가
public class OnOffGround : AbstractGimmick, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Tilemap _tilemap; // 💡 SpriteRenderer에서 Tilemap으로 변경
    private Collider2D _col; 

    private void Awake()
    {
        _tilemap = GetComponent<Tilemap>(); // 💡 타일맵 컴포넌트 가져오기
        _col = GetComponent<Collider2D>(); 
    }

    protected override void HandleStateChanged(bool isProcessOn)
    {
        if (isProcessOn) 
        {
            gameObject.layer = 6; // Ground 레이어
            if (_col != null) 
            {
                _col.enabled = true;
                _col.isTrigger = false; // 켜졌을 때는 단단한 발판
            }
            
            // 💡 타일맵 색상 불투명하게 (켜짐)
            if (_tilemap != null)
            {
                _tilemap.color = new Color(1f, 1f, 1f, 1f);
            }
        }
        else 
        {
            gameObject.layer = 7; // Ignore 레이어
            if (_col != null) 
            {
                _col.enabled = true;   // 꺼져도 마우스 클릭 감지를 위해 Collider 유지
                _col.isTrigger = true; // 플레이어 충돌 X
            }
            
            // 💡 타일맵 색상 투명하게 (꺼짐)
            if (_tilemap != null)
            {
                _tilemap.color = new Color(1f, 1f, 1f, .25f);
            }
        }
    }

    // --- 마우스가 발판 위로 올라왔을 때 ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        // targetProcess 호버 상태 전달 (외곽선 하이라이트용)
        if (targetProcess != null)
        {
            targetProcess.SetHoverState(true); 
        }
    }

    // --- 마우스가 발판에서 벗어났을 때 ---
    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetProcess != null)
        {
            targetProcess.SetHoverState(false);
        }
    }

    // 💡 마우스 버튼 구분 없이, 클릭 시 팝업 표시
    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 clickWorldPos = eventData.pointerCurrentRaycast.worldPosition;
        clickWorldPos.z = transform.position.z;

        PopupManager.Instance.ShowPopup(targetProcess, clickWorldPos);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }
}