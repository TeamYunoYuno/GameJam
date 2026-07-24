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
        if (isProcessOn == true) 
        {
            _rb.gravityScale = 9.8f;
            
            // 💡 상자 물리 복구 로그
            ConsoleManager.Instance.LogSystem($"[PHYSICS] {gameObject.name} gravity module online.");
        }
        else 
        {
            _rb.gravityScale = -9.8f; // (의도하신 대로 중력 반전 또는 무중력 상태로 유지)
            
            // 💡 상자 물리 정지 로그
            ConsoleManager.Instance.LogError($"[PHYSICS] {gameObject.name} gravity module offline. Anomaly detected.");
        }
    }
}