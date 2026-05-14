using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Global;

namespace _Project.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovementDetector : MonoBehaviour
    {
        public bool CanMove => isGrounded || !isMoving;
        public event Action OnCanBeStopped; 

        private bool isGrounded;
        private bool isMoving;
        private float movementThreshold = 0.1f;
        private Rigidbody2D rb;
        private CancellationTokenSource cts = new();
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            UniTaskUtils.RunWithCancellationAsync(CheckMovement, cts.Token).Forget();
        }

        private async UniTask CheckMovement(CancellationToken cancellationToken)
        {
            while (cancellationToken.IsCancellationRequested == false)
            {
                isMoving = CheckMoving();
                await UniTask.WaitForSeconds(.1f,cancellationToken: cancellationToken);
            }
        }
        
        private bool IsMovingX()
        {
            return Mathf.Abs(rb.linearVelocity.x) > movementThreshold;
        }
        
        private bool CheckMoving()
        {
            return rb.linearVelocity.sqrMagnitude > movementThreshold * movementThreshold;
        }

        private void OnTriggerEnter2D(Collider2D trig)
        {
            if (trig.gameObject.CompareTag("obstacle") || trig.gameObject.CompareTag("stopper"))
            {
                isGrounded = true;
                if (!IsMovingX())
                    OnCanBeStopped?.Invoke();
            }
        }

        /*private void OnTriggerStay2D(Collider2D trig)
        {
            if (trig.gameObject.CompareTag("obstacle") || trig.gameObject.CompareTag("stopper"))
            {
                isGrounded = true;
            }
        }*/

        private void OnTriggerExit2D(Collider2D trig)
        {
            if (trig.gameObject.CompareTag("obstacle") || trig.gameObject.CompareTag("stopper"))
            {
                isGrounded = false;
            }
        }
        
        private void OnDestroy()
        {
            cts.Cancel();
            cts.Dispose();
        }
    }
}