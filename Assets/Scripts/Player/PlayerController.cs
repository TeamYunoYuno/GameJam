using System.Collections;
using Agents;
using CoreLib;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerInputSO playerInput;
        public AgentMovement AgentMovement { get;  private set; }

        private bool _canJump = true;
        private bool _isJumping;
        
        private void Awake()
        {
            AgentMovement = GetComponent<AgentMovement>();

            playerInput.OnJumpKeyPressed += HandlePlayerJumpKey;
            AgentMovement.IsGrounded.OnValueChanged += HandleGroundChanged;
        }

        private void OnDestroy()
        {
            playerInput.OnJumpKeyPressed -= HandlePlayerJumpKey;
            AgentMovement.IsGrounded.OnValueChanged -= HandleGroundChanged;
        }

        private void FixedUpdate()
        {
            AgentMovement.SetMovementDir(playerInput.MovementInput.x != 0 ? Mathf.Sign(playerInput.MovementInput.x) : 0);
        }

        private void HandlePlayerJumpKey()
        {
            if (_isJumping) return;

            if (_canJump)
            {
                AgentMovement.Jump();
            }
        }

        private void HandleGroundChanged(bool prev, bool now)
        {
            if (now)
            {
                _canJump = true;
                _isJumping = false;
            } else
            {
                StartCoroutine(CanJumpCoroutine());
            }
        }

        private IEnumerator CanJumpCoroutine()
        {
            yield return new WaitForSeconds(0.1f);
            _canJump = false;
        }
    }
}