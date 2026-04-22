using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Global;
using UnityEngine;

namespace _Project.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovementDetector : MonoBehaviour
    {
        
        public event Action OnMovingChanged;
        public bool IsMoving { get; private set; }
        
        private float movementThreshold = 0.1f;
        private bool newState;
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
                newState = CheckMoving();
                if (newState != IsMoving)
                {
                    IsMoving = newState;
                    Debug.Log($"Movement detector: {newState}");
                    OnMovingChanged?.Invoke();
                }
                await UniTask.WaitForSeconds(.1f,cancellationToken: cancellationToken);
            }
        }
        
        private bool CheckMoving()
        {
            return rb.linearVelocity.sqrMagnitude > movementThreshold * movementThreshold;
        }

        private void OnDestroy()
        {
            cts.Cancel();
            cts.Dispose();
        }
    }
}