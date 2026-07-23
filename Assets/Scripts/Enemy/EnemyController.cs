using System;
using UnityEngine;

[RequireComponent(typeof(AgentMovement))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 0.75f;
    [SerializeField] private float moveSpeed = 0.5f;

    [SerializeField] private LayerMask whatIsGround;
    
    public AgentMovement AgentMovement { get; private set; }

    private bool _isRight = true;

    private void Awake()
    {
        AgentMovement = GetComponent<AgentMovement>();
        AgentMovement.IsGrounded.OnValueChanged += ChangeMoveDir;
        Init();
    }

    private void Init()
    {
        AgentMovement.SetMovementDir(moveSpeed);
    }

    private void FixedUpdate()
    {
        if (!CheckWall()) return;

        ChangeMoveDir();
    }

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
    
    private void ChangeMoveDir(bool prev, bool now)
    {
        Debug.LogWarning(now);
        if (now) return;
        
        AgentMovement.SetMovementDir(_isRight ? -moveSpeed : moveSpeed);
        transform.rotation = Quaternion.Euler(0f, _isRight ? 180 : 0, 0f);
        
        var offset = AgentMovement.checkOffset;
        AgentMovement.checkOffset = new Vector3(-offset.x, offset.y, offset.z);
        _isRight = !_isRight;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * raycastDistance);
    }
}