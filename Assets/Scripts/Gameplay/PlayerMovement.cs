using PlayerInput;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Movement
{
    public class PlayerMovement : NetworkBehaviour
    {
        public float speed = 5f;
        public float springCompressionFactor = 1120000f; // Factor para incrementar la compresión del resorte

        private PlayerInputAction inputActions;
        private Vector2 moveInput;

        [SerializeField] private WheelControl wheel;

        private bool isSpringCompressed;

        public override void OnNetworkSpawn()
        {
            inputActions = new PlayerInputAction();

            inputActions.Player.Movement.performed += OnMove;
            inputActions.Player.Movement.canceled += OnMove;
            inputActions.Player.Jump.performed += OnJumpPressed;
            inputActions.Player.Jump.canceled += OnJumpReleased;
            inputActions.Enable();
        }

        protected override void OnOwnershipChanged(ulong previous, ulong current)
        {
            Debug.Log("changed ownership");
        }

        public override void OnDestroy()
        {
            inputActions.Player.Movement.performed -= OnMove;
            inputActions.Player.Movement.canceled -= OnMove;
            inputActions.Player.Jump.performed -= OnJumpPressed;
            inputActions.Player.Jump.canceled -= OnJumpReleased;
            inputActions.Disable();
        }

        private void FixedUpdate()
        {
            if (!IsServer) return;

            if (IsAcelerating())
            {
                DoMovement();
            }
            else
            {
                DoBreak();
            }
        }

        private bool IsAcelerating()
        {
            return moveInput.y != 0 && Mathf.Sign(moveInput.y).Equals(Mathf.Sign(wheel.carControl.forwardSpeed));
        }

        private void DoBreak()
        {
            // If the user is trying to go in the opposite direction
            // apply brakes to all wheelsInstances
            wheel.WheelCollider.brakeTorque = Mathf.Abs(moveInput.y) * wheel.carControl._brakeTorque;
            wheel.WheelCollider.motorTorque = 0;
        }

        private void DoMovement()
        {
            DoSteer();
            DoAccelerate();
        }

        private void DoAccelerate()
        {
            Debug.Log($"server: {OwnerClientId} is accelerating");
            if (wheel.motorized)
            {
                wheel.WheelCollider.motorTorque = moveInput.y * wheel.carControl.currentMotorTorque;
            }
            wheel.WheelCollider.brakeTorque = 0;
        }

        private void DoSteer()
        {
            if (wheel.steerable)
            {
                wheel.WheelCollider.steerAngle = moveInput.x * wheel.carControl.currentSteerRange;
            }
        }

        private IEnumerator CompressSpring()
        {
            JointSpring spring = wheel.WheelCollider.suspensionSpring;
            float elapsedTime = 0f;
            float duration = 0.3f;
            while (elapsedTime < duration && isSpringCompressed)
            {
                var currentTargetPos = Mathf.Lerp(0.5f, 1f, elapsedTime / duration);
                var currentSpringForce = Mathf.Lerp(2, 15, elapsedTime / duration) * 100000;

                elapsedTime += Time.deltaTime;

                spring.targetPosition = currentTargetPos;
                spring.spring = currentSpringForce;

                wheel.WheelCollider.suspensionSpring = spring;

                yield return null;
            }
        }

        private IEnumerator ReleaseSpring()
        {
            JointSpring spring = wheel.WheelCollider.suspensionSpring;
            spring.targetPosition = 0.5f;
            wheel.WheelCollider.suspensionSpring = spring;

            yield return DictionaryOfWaitForSeconds.GetWaitForSeconds(0.1f);

            spring = wheel.WheelCollider.suspensionSpring;
            spring.spring = 35000;
            wheel.WheelCollider.suspensionSpring = spring;
        }

        #region InputActions

        private void OnMove(InputAction.CallbackContext context)
        {
            if (!IsOwner) return;

            var input = context.ReadValue<Vector2>();

            SendMovementInputServerRpc(input);
        }

        private void OnJumpPressed(InputAction.CallbackContext context)
        {
            if (!IsOwner) return;

            JumpPressedServerRpc();
        }

        private void OnJumpReleased(InputAction.CallbackContext context)
        {
            if (!IsOwner) return;

            JumpReleasedServerRpc();
        }
        #endregion

        #region ServerRPC
        [ServerRpc]
        private void SendMovementInputServerRpc(Vector2 inputVector)
        {
            // Asigna el inputVector al moveInput del servidor
            moveInput = inputVector;
        }

        [ServerRpc]
        private void JumpPressedServerRpc()
        {
            isSpringCompressed = true;
            StartCoroutine(CompressSpring());
        }

        [ServerRpc]
        private void JumpReleasedServerRpc()
        {
            isSpringCompressed = false;

            StopCoroutine(CompressSpring());

            StartCoroutine(ReleaseSpring());
        }
        #endregion
    }
}

public static class DictionaryOfWaitForSeconds
{
    private static readonly Dictionary<float, WaitForSeconds> dictOfWaitForSeconds = new();

    public static WaitForSeconds GetWaitForSeconds(float time)
    {
        if (dictOfWaitForSeconds.TryGetValue(time, out WaitForSeconds result))
        {
            return result;
        }
        else
        {
            WaitForSeconds wait = new(time);
            dictOfWaitForSeconds.Add(time, wait);
            return wait;
        }
    }
}
