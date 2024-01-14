using UnityEngine;
using System.Collections;
using Framework;

namespace Lib.Core
{
    /// <summary>
    /// 协程管理器///
    /// </summary>
    public class CoroutineManager : TSingleton<CoroutineManager>
    {
        private GameObject mCoroutineRootObj;

        public void Initialize()
        {
            this.mCoroutineRootObj = new GameObject("Root_Coroutine");
            this.mCoroutineRootObj.transform.position = Vector3.zero;
            GameObject.DontDestroyOnLoad(this.mCoroutineRootObj);
        }

        public CoroutineHandler StartHandler(IEnumerator rIEnum)
        {            
            var rCourtineObj = FrameworkTool.CreateGameObject(this.mCoroutineRootObj.transform, "coroutine");
            CoroutineHandler rHandler = rCourtineObj.GetOrAddComponent<CoroutineHandler>();
            rHandler.StartHandler(rIEnum);
            return rHandler;
        }

        public Coroutine Start(IEnumerator rIEnum)
        {
            return this.StartHandler(rIEnum).Coroutine;
        }

        public void Stop(CoroutineHandler rCoroutineHandler)
        {
            if (rCoroutineHandler != null)
            {
                rCoroutineHandler.StopAllCoroutines();
                GameObject.DestroyImmediate(rCoroutineHandler.gameObject);
                rCoroutineHandler.Coroutine = null;
            }
            rCoroutineHandler = null;
        }
    }
}

