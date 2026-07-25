using UnityEngine;

public class MovePlatform: MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("위로 이동할 총 거리")]
    [SerializeField] private float moveDistance = 3f; 
    
    [Tooltip("이동 속도")]
    [SerializeField] private float moveSpeed = 2f;    
    
    [Tooltip("최고/최저점에 도달했을 때 잠시 멈추는 대기 시간 (초)")]
    [SerializeField] private float waitTime = 0.5f;   

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool movingUp = true;
    private bool isWaiting = false;
    private float waitTimer = 0f;

    private void Start()
    {
        startPos = transform.position;
        targetPos = startPos + Vector3.up * moveDistance;
    }

    private void Update()
    {
        // 1. 대기 시간 처리
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                waitTimer = 0f;
            }
            return;
        }

        // 2. 목표 지점으로 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        // 3. 목표 지점 도달 시 방향 전환 및 대기 시작
        if (Vector3.Distance(transform.position, targetPos) < 0.001f)
        {
            movingUp = !movingUp;
            targetPos = movingUp ? startPos + Vector3.up * moveDistance : startPos;
            isWaiting = true;
        }
    }

    // 💡 플레이어가 발판 위에 올라탔을 때 발판을 따라 움직이도록 설정
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    // 💡 플레이어가 발판에서 내렸을 때 부모 관계 해제
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }

    // 💡 에디터 Scene 뷰에서 이동 범위(청록색 선) 시각화
    private void OnDrawGizmosSelected()
    {
        Vector3 start = Application.isPlaying ? startPos : transform.position;
        Vector3 end = start + Vector3.up * moveDistance;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawWireCube(end, transform.localScale);
    }
}