using UnityEngine;

public abstract class AbstractGimmick : MonoBehaviour //[cite: 5]
{
    [SerializeField] protected ProcessData targetProcess; //[cite: 5]
    
    // --- 추가된 부분: 호버 시 보여줄 이펙트 오브젝트 (예: 외곽선 이미지) ---
    [Header("Hover Feedback")]
    [Tooltip("마우스를 올렸을 때 켜질 하이라이트 오브젝트")]
    [SerializeField] protected GameObject highlightEffect; 
    
    protected virtual void OnEnable() //[cite: 5]
    {
        if (targetProcess != null)
        {
            targetProcess.OnStateChanged += HandleStateChanged; //[cite: 5]
            targetProcess.OnHoverChanged += HandleHoverState; // 이벤트 구독 추가
        }
    }

    protected virtual void OnDisable() //[cite: 5]
    {
        if (targetProcess != null)
        {
            targetProcess.OnStateChanged -= HandleStateChanged; //[cite: 5]
            targetProcess.OnHoverChanged -= HandleHoverState; // 이벤트 구독 해제
        }
    }

    protected abstract void HandleStateChanged(bool isProcessOn); //[cite: 5]

    // --- 추가된 부분: 호버 상태에 따라 이펙트를 켜고 끄기 ---
    protected virtual void HandleHoverState(bool isHovered)
    {
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(isHovered);
        }
    }
}