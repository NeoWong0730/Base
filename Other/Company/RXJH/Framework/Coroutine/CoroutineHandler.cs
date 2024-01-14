using UnityEngine;
using System.Collections;

namespace Lib.Core
{
    public class CoroutineHandler : MonoBehaviour
    {
        public Coroutine    Coroutine;
        public bool         IsCompleted;
        public bool         IsRunning;

        public Coroutine StartHandler(IEnumerator rIEnum)
        {
            this.IsCompleted = false;
            this.IsRunning = true;
            this.Coroutine = this.StartCoroutine(this.StartHandler_Async(rIEnum));
            return this.Coroutine;
        }
        
        private IEnumerator StartHandler_Async(IEnumerator rIEnum)
        {
            yield return rIEnum;
            this.IsRunning = false;
            this.IsCompleted = true;

            // 要等一帧才能把自己删掉，要不然会崩溃
            yield return null;

            // 自己把自己删掉，并将对应的数据都清空
            CoroutineManager.Instance.Stop(this);            
        }
    }
}
