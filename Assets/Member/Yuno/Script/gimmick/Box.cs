using UnityEngine;

public class Box: MonoBehaviour
{
    // 이 오브젝트가 수신할 특정 무전기(Physics_Process 등)를 인스펙터에서 할당받습니다.
    [SerializeField] private ProcessData targetProcess;

    // 매 프레임 무거운 GetComponent 호출을 피하기 위해 멤버 변수를 선언합니다.
    private Rigidbody2D _rb;

    private void Awake()
    {
        // 런타임 성능 극대화를 위해 물리 컴포넌트를 미리 캐싱해 둡니다.
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        // 오브젝트 활성화 시 SO 이벤트에 등록(구독)하여 상태 변화를 수신할 준비를 합니다.
        targetProcess.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        // 파괴되거나 꺼질 때 구독을 해제하지 않으면 심각한 메모리 누수(Leak)가 발생하므로 반드시 끊어줍니다.
        targetProcess.OnStateChanged -= HandleStateChanged;
    }

    // isProcessOn 이라는 변수 안에 True(켜짐) 또는 False(꺼짐) 정보가 담겨서 날아옵니다.
    private void HandleStateChanged(bool isProcessOn)
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