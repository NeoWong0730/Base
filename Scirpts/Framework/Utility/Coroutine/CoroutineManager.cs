using System.Collections;
using UnityEngine;

namespace Framework
{
    public class CoroutineManager : TSingleton<CoroutineManager>
    {
        private GameObject mCoroutineRootObj;

        public void Initialize()
        {
            mCoroutineRootObj = new GameObject("CoroutineManager");
            mCoroutineRootObj.transform.position = Vector3.zero;
            GameObject.DontDestroyOnLoad(mCoroutineRootObj);
        }

        public CoroutineHandler StartHandler(IEnumerator rIEnum)
        {
            var rCoroutineObj = FrameworkTool.CreateGameObject(mCoroutineRootObj.transform, "coroutine");
            CoroutineHandler rHandler = rCoroutineObj.GetOrAddComponent<CoroutineHandler>();
            rHandler.StartHandler(rIEnum);
            return rHandler;
        }

        public Coroutine Start(IEnumerator rIEnum)
        {
            return StartHandler(rIEnum).Coroutine;
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
