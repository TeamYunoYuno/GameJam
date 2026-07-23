using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnOffGround : AbstractGimmick, IPointerDownHandler
{
    private SpriteRenderer _sr;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    protected override void HandleStateChanged(bool isProcessOn)
    {
        if (isProcessOn) 
        {
            gameObject.layer = 6; //Ground
            _sr.color = new Color(0.7754717f, 0.7754717f, 1f, 1f);
            Debug.Log("땅 복구 완료!");
        }
        else 
        {
            gameObject.layer = 7; //Ignore
            _sr.color = new Color(1f, 1f, 1f, .5f);
            Debug.Log("땅 기능 삭제!");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MemoryManager.Instance.RequestToggleProcess(targetProcess);
    }
}
