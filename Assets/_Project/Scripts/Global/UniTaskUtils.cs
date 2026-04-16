using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Global
{
    public static class UniTaskUtils
    {
        /// <summary>
        /// Запускает асинхронный метод с CancellationToken и обрабатывает отмену.
        /// </summary>
        /// <param name="asyncMethod">Делегат, принимающий CancellationToken и возвращающий UniTask.</param>
        /// <param name="token">Токен отмены.</param>
        /// <param name="callerName">Имя вызывающего метода (автоматически).</param>
        public static async UniTask RunWithCancellationAsync(
            Func<CancellationToken, UniTask> asyncMethod,
            CancellationToken token,
            [CallerMemberName] string callerName = "")
        {
            try
            {
                await asyncMethod(token);
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning($"[UniTaskUtils] Async method '{callerName}' was cancelled.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UniTaskUtils] Unexpected error in '{callerName}': {ex}");
            }
        }

        /// <summary>
        /// Версия с возвращаемым значением.
        /// </summary>
        public static async UniTask<T> RunWithCancellationAsync<T>(
            Func<CancellationToken, UniTask<T>> asyncMethod,
            CancellationToken token,
            [CallerMemberName] string callerName = "")
        {
            try
            {
                return await asyncMethod(token);
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning($"[UniTaskUtils] Async method '{callerName}' was cancelled.");
                return default;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UniTaskUtils] Unexpected error in '{callerName}': {ex}");
                return default;
            }
        }
    }
}