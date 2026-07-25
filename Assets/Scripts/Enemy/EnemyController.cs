using System;
using UnityEngine;

[RequireComponent(typeof(AgentMovement))]
public class EnemyController : MonoBehaviour
{
    [Header("Process Settings")]
    [Tooltip("1. 적 이동 ON/OFF 프로세스")]
    [SerializeField] private ProcessData moveProcess;
    
    [Tooltip("2. 낭떠러지 감지(땅 없을 때 반전) ON/OFF 프로세스")]
    [SerializeField] private ProcessData edgeTurnProcess;

    [Header("Hover Highlight")]
    [Tooltip("마우스를 올렸을 때 켜질 하이라이트 효과 오브젝트")]
    [SerializeField] private GameObject highlightEffect;

    [Header("Movement Settings")]
    [SerializeField] private float raycastDistance = 0.75f;
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private LayerMask whatIsGround;
    
    public AgentMovement AgentMovement { get; private set; }

    private bool _isRight = true;
    private bool _isMoveProcessOn = true;
    private bool _isEdgeTurnProcessOn = true;

    private void Awake()
    {
        AgentMovement = GetComponent<AgentMovement>();
    }

    private void OnEnable()
    {
        // 1. AgentMovement의 접지 상태 변화 이벤트 구독
        if (AgentMovement != null)
        {
            AgentMovement.IsGrounded.OnValueChanged += OnGroundedChanged;
        }

        // 2. 이동 프로세스 이벤트 구독
        if (moveProcess != null)
        {
            moveProcess.OnStateChanged += HandleMoveProcessChanged;
            moveProcess.OnHoverChanged += HandleHoverState;
            HandleMoveProcessChanged(moveProcess.IsActive);
        }

        // 3. 낭떠러지 감지 프로세스 이벤트 구독
        if (edgeTurnProcess != null)
        {
            edgeTurnProcess.OnStateChanged += HandleEdgeTurnProcessChanged;
            edgeTurnProcess.OnHoverChanged += HandleHoverState;
            HandleEdgeTurnProcessChanged(edgeTurnProcess.IsActive);
        }
    }

    private void OnDisable()
    {
        if (AgentMovement != null)
        {
            AgentMovement.IsGrounded.OnValueChanged -= OnGroundedChanged;
        }

        if (moveProcess != null)
        {
            moveProcess.OnStateChanged -= HandleMoveProcessChanged;
            moveProcess.OnHoverChanged -= HandleHoverState;
        }

        if (edgeTurnProcess != null)
        {
            edgeTurnProcess.OnStateChanged -= HandleEdgeTurnProcessChanged;
            edgeTurnProcess.OnHoverChanged -= HandleHoverState;
        }
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        if (_isMoveProcessOn)
        {
            AgentMovement.SetMovementDir(moveSpeed);
        }
    }

    private void FixedUpdate()
    {
        // 이동 프로세스가 꺼져있으면 이동 및 벽 감지 안 함
        if (!_isMoveProcessOn) return;

        if (!CheckWall()) return;

        ChangeMoveDir();
    }

    #region Process Handlers

    // 💡 [프로세스 1] 이동 ON / OFF
    private void HandleMoveProcessChanged(bool isProcessOn)
    {
        _isMoveProcessOn = isProcessOn;

        if (AgentMovement != null)
        {
            AgentMovement.ChangeMoveState(isProcessOn);
            
            if (isProcessOn)
            {
                AgentMovement.SetMovementDir(_isRight ? moveSpeed : -moveSpeed);
                ConsoleManager.Instance?.LogSystem($"[PROCESS] {gameObject.name} movement process online.");
            }
            else
            {
                AgentMovement.SetMovementDir(0f);
                ConsoleManager.Instance?.LogError($"[PROCESS] {gameObject.name} movement process killed.");
            }
        }
    }

    // 💡 [프로세스 2] 낭떠러지 감지 반전 ON / OFF
    private void HandleEdgeTurnProcessChanged(bool isProcessOn)
    {
        _isEdgeTurnProcessOn = isProcessOn;

        if (isProcessOn)
        {
            ConsoleManager.Instance?.LogSystem($"[PROCESS] {gameObject.name} edge detection sensor online.");
        }
        else
        {
            ConsoleManager.Instance?.LogError($"[PROCESS] {gameObject.name} edge detection sensor offline.");
        }
    }

    // 💡 마우스 호버 시 외곽선 하이라이트
    private void HandleHoverState(bool isHovered)
    {
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(isHovered);
        }
    }

    #endregion

    #region Movement & Collision Logic

    private bool CheckWall()
    {
        var hit = Physics2D.Raycast(transform.position, transform.right, raycastDistance, whatIsGround);
        return hit.collider != null;
    }
    
    private void ChangeMoveDir()
    {
        AgentMovement.SetMovementDir(_isRight ? -moveSpeed : moveSpeed);
        transform.rotation = Quaternion.Euler(0f, _isRight ? 180 : 0, 0f);
        
        var offset = AgentMovement.checkOffset;
        AgentMovement.checkOffset = new Vector3(-offset.x, offset.y, offset.z);
        _isRight = !_isRight;
    }
    
    // 💡 발판이 끝났을 때(IsGrounded = false) 불려오는 콜백
    private void OnGroundedChanged(bool prev, bool now)
    {
        // 땅에 닿았을 때(now = true)는 무시
        if (now) return;
        
        // ❌ 낭떠러지 감지 프로세스가 꺼져(OFF)있다면 방향을 바꾸지 않고 그대로 떨어집니다!
        if (!_isEdgeTurnProcessOn) return;

        // ⭕ 켜져(ON)있을 때만 방향 반전
        ChangeMoveDir();
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * raycastDistance);
    }
}