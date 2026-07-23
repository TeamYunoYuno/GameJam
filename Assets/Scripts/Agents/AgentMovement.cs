using System;
using UnityEngine;

namespace Agents
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class AgentMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpPower = 15f;
        [SerializeField] private float gravityScale = -2f;
        [SerializeField] private LayerMask whatIsGround;

        [Header("Ground Checker")] 
        [SerializeField] private Vector2 checkSize;
        [SerializeField] private Vector3 checkOffset;
        
        public NotifyValue<bool> IsGrounded = new NotifyValue<bool>();
        
        private Rigidbody2D _rb;
        private float _dirX;
        private bool _canMove = true;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (_canMove)
            {
                _rb.linearVelocityX = _dirX * moveSpeed;
                FlipController();
            }

            CheckGround();
        }

        private void CheckGround()
        {
            var coll = Physics2D.OverlapBox(transform.position + checkOffset, checkSize, 0f, whatIsGround);
            IsGrounded.Value = coll != null;
            
            if (coll == null)
                ApplyGravity();
        }

        public void SetMovementDir(float dirX)
        {
            _dirX = dirX;
        }

        public void ChangeMoveState(bool active)
        {
            _canMove = active;
        }

        public void ApplyVelocity(Vector3 velocity, ForceMode2D forceMode = ForceMode2D.Impulse)
        {
            _rb.AddForce(velocity, forceMode);    
        }

        public void Jump()
        {
            _rb.AddForceY(jumpPower, ForceMode2D.Impulse);
        }

        private void ApplyGravity()
        {
            _rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (gravityScale * -1f) * Time.fixedDeltaTime); 
        }

        private void FlipController()
        {
            if (_dirX < 0)
                transform.rotation = Quaternion.Euler(0, 180, 0);
            else if (_dirX > 0)
                transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + checkOffset, checkSize);
        }
    }
}