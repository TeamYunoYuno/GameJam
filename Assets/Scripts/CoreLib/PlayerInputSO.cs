using System;
using UnityEngine;
using UnityEngine.InputSystem;

    [CreateAssetMenu(fileName = "PlayerInput", menuName = "SO/PlayerInput", order = 0)]
    public class PlayerInputSO : ScriptableObject, Controls.IPlayerActions
    {
        public Controls Controls;

        public Vector2 MovementInput { get; private set; }

        public event Action OnAttackKeyPressed;
        public event Action OnJumpKeyPressed;

        private void OnEnable()
        {
            if (Controls == null)
            {
                Controls = new Controls();
                Controls.Player.SetCallbacks(this);
            }
            
            Controls.Enable();
        }

        private void OnDisable()
        {
            if (Controls != null)
                Controls.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MovementInput = context.ReadValue<Vector2>();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            OnAttackKeyPressed?.Invoke();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
                OnJumpKeyPressed?.Invoke();
        }
    }
