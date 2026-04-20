using PlayerInput;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Shared;

namespace Player.Movement
{
    public class PlayerMovement : NetworkBehaviour
    {

        private PlayerInputAction inputActions;
        private Vector2 moveInput;

        [SerializeField] private WheelControl wheel;

        private bool isSpringCompressed;
        ulong owner = 100;

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
            if (!IsServer || !GameStatusManager.canVehiclesMove) return;

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
            wheel.WheelCollider.brakeTorque =
                Mathf.Abs(moveInput.y) * wheel.carControl._brakeTorque;
            wheel.WheelCollider.motorTorque = 0;
        }

        private void DoMovement()
        {
            DoSteer();
            DoAccelerate();
        }

        private void DoAccelerate()
        {
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

        [ClientRpc]
        internal void AssignOwnerClientRpc(ulong client)
        {
            owner = client;
            Debug.Log("owner:" + owner + "id rueda: " + GetInstanceID());
        }

        #region InputActions
        private void OnMove(InputAction.CallbackContext context)
        {
            if (owner != NetworkManager.Singleton.LocalClientId) return;
            //   Debug.Log("owner:" + owner + "id rueda: " + GetInstanceID() + "Client:" + NetworkManager.Singleton.LocalClientId);

            var input = context.ReadValue<Vector2>();

            SendMovementInputServerRpc(input);
        }

        private void OnJumpPressed(InputAction.CallbackContext context)
        {
            if (owner != NetworkManager.Singleton.LocalClientId) return;

            JumpPressedServerRpc();
        }

        private void OnJumpReleased(InputAction.CallbackContext context)
        {
            if (owner != NetworkManager.Singleton.LocalClientId) return;

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
