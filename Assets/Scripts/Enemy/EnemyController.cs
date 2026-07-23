using System;
using UnityEngine;

[RequireComponent(typeof(AgentMovement))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private float raycastDistance;
    
    public AgentMovement AgentMovement { get; private set; }
    
    private void Awake()
    {
        AgentMovement = GetComponent<AgentMovement>();
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    private void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, raycastDistance);
        
        if (hit.collider == null) return;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * raycastDistance);
    }
}
