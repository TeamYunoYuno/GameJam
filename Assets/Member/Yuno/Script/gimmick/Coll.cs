using System;
using UnityEngine;

public class Coll : AbstractGimmick
{
    private SpriteRenderer _sr;
    
    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    protected override void HandleStateChanged(bool isProcessOn)
    {
        if (isProcessOn == true) 
        {
            gameObject.layer = 6; //Ground
            _sr.color = new Color(1f, 0.7075472f, 0.7075472f, 1f);
            Debug.Log("콜라이더 복구 완료!");
        }
        else 
        {
            gameObject.layer = 7; //Ignore
            _sr.color = new Color(1f, 1f, 1f, .5f);
            Debug.Log("콜라이더 기능 삭제!");
        }
    }
}
