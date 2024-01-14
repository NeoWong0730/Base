using System.Collections;
using UnityEngine;

namespace Framework
{
    public class CoroutineHandler : MonoBehaviour
    {
        public Coroutine Coroutine;
        public bool IsCompleted;
        public bool IsRunning;

        public Coroutine StartHandler(IEnumerator rIEum)
        {
            IsCompleted = false;
            IsRunning = true;
            Coroutine = StartCoroutine(StartHandlerAsync(rIEum));
            return Coroutine;
        }

        IEnumerator StartHandlerAsync(IEnumerator rIEum)
        {
            yield return rIEum;
            IsRunning = false;
            IsCompleted = true;

            yield return null;

            CoroutineManager.Instance.Stop(this);
        }
    }
}