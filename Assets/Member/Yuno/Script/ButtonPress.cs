using UnityEngine;
using UnityEngine.Tilemaps; // 💡 Tilemap 사용을 위해 추가

public class ButtonPress : MonoBehaviour
{
    [Header("Door Settings")]
    [Tooltip("연결할 문 타일맵 오브젝트를 끌어다 넣으세요.")]
    [SerializeField] private GameObject targetDoor;
    
    [Tooltip("문이 열렸을 때(콜라이더가 꺼졌을 때)의 색상")]
    [SerializeField] private Color doorOpenColor = new Color(1f, 1f, 1f, 0.25f);

    [Header("Trigger Settings")]
    [Tooltip("버튼을 누를 수 있는 레이어 (예: Enemy, Player, Box 등)")]
    [SerializeField] private LayerMask triggerLayers;

    public SpriteRenderer sp;

    private Collider2D _doorCollider;
    private Tilemap _doorTilemap; // 💡 SpriteRenderer -> Tilemap으로 변경
    private Color _doorOriginalColor;
    
    // 발판을 밟고 있는 오브젝트의 수
    private int _pressCount = 0;

    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        sp.color = new Color(1f, 0f, 0f, 0.25f); 
        if (targetDoor != null)
        {
            _doorCollider = targetDoor.GetComponent<Collider2D>();
            _doorTilemap = targetDoor.GetComponent<Tilemap>(); // 💡 Tilemap 컴포넌트 가져오기

            if (_doorTilemap != null)
            {
                _doorOriginalColor = _doorTilemap.color; // 문 타일맵의 원래 색상 저장
            }
            else
            {
                Debug.LogWarning($"[ButtonPress] {targetDoor.name}에 Tilemap 컴포넌트가 없습니다!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 들어온 오브젝트의 레이어가 triggerLayers에 포함되어 있는지 확인
        if (((1 << other.gameObject.layer) & triggerLayers) != 0)
        {
            sp.color = new Color(1f, 0f, 0f, 1f); 
            _pressCount++;
            
            // 처음 밟혔을 때 문 열기
            if (_pressCount == 1)
            {
                SetDoorState(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & triggerLayers) != 0)
        {
            sp.color = new Color(1f, 0f, 0f, 0.25f); 
            _pressCount--;
            
            // 발판에서 모두 내려갔을 때 문 닫기
            if (_pressCount <= 0)
            {
                _pressCount = 0;
                SetDoorState(false);
            }
        }
    }

    private void SetDoorState(bool isOpen)
    {
        if (targetDoor == null) return;

        // 1. 문 콜라이더 끄기/켜기 (통과 처리)
        if (_doorCollider != null)
        {
            _doorCollider.enabled = !isOpen; 
        }

        // 2. 문 타일맵 색상 변경 (열리면 반투명, 닫히면 원래 색상)
        if (_doorTilemap != null)
        {
            _doorTilemap.color = isOpen ? doorOpenColor : _doorOriginalColor;
        }

        // 3. 콘솔 시스템 로그 출력
        if (isOpen)
        {
            ConsoleManager.Instance?.LogSystem($"[SECURITY] {targetDoor.name} access granted. Door opened.");
        }
        else
        {
            ConsoleManager.Instance?.Log($"[SECURITY] {targetDoor.name} locked.");
        }
    }
}