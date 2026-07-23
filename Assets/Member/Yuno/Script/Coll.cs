using System;
using UnityEngine;

public class Coll : AbstractGimmick
{
    private Collider2D _coll;
    private SpriteRenderer _sr;
    
    private void Awake()
    {
        _coll = GetComponent<Collider2D>();
        _sr = GetComponent<SpriteRenderer>();
    }

    protected override void HandleStateChanged(bool isProcessOn)
    {
        // 1. 만약 방송 내용이 "켜짐(True)" 이라면?
        if (isProcessOn == true) 
        {
            // 켜졌을 때 할 행동을 여기에 씁니다.
            _coll.enabled = true;
            _sr.color = new Color(1f, 0.7075472f, 0.7075472f, 1f);
            Debug.Log("콜라이더 복구 완료!");
        }
        // 2. 만약 방송 내용이 "꺼짐(False)" 이라면?
        else 
        {
            // 꺼졌을 때 할 행동을 여기에 씁니다.
            _coll.enabled = false;
            _sr.color = new Color(1f, 1f, 1f, .5f);
            Debug.Log("콜라이더 기능 삭제!");
        }
    }
}
