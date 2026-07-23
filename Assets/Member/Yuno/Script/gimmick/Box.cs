using UnityEngine;

public class Box: AbstractGimmick
{
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    
    // isProcessOn 이라는 변수 안에 True(켜짐) 또는 False(꺼짐) 정보가 담겨서 날아옵니다.
    protected override void HandleStateChanged(bool isProcessOn)
    {
        // 1. 만약 방송 내용이 "켜짐(True)" 이라면?
        if (isProcessOn == true) 
        {
            // 켜졌을 때 할 행동을 여기에 씁니다.
            _rb.gravityScale = 9.8f;
            Debug.Log("물리 엔진 복구 완료! 상자가 다시 떨어집니다.");
            
            // (여기에 색상을 원래대로 돌리거나 파티클을 켜는 등 원하는 코드를 맘껏 추가하세요!)
        }
        // 2. 만약 방송 내용이 "꺼짐(False)" 이라면?
        else 
        {
            // 꺼졌을 때 할 행동을 여기에 씁니다.
            _rb.gravityScale = -9.8f;
            Debug.Log("물리 엔진 정지! 상자가 허공에 고정됩니다.");
            
            // (여기에 글리치 효과음을 재생하거나 색상을 흑백으로 바꾸는 코드를 추가하세요!)
        }
    }
}