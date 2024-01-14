using UnityEngine;
using UnityEngine.UI;

namespace UI.Extensions
{
    // https://www.jianshu.com/p/2cf4dbb96c56
    // https://www.jianshu.com/p/3edce67cb473
    // https://zhuanlan.zhihu.com/p/55566751
    // https://blog.csdn.net/cyf649669121/article/details/83661023
    [RequireComponent(typeof(Canvas))]
    public class GraphicRaycasterAdapter : GraphicRaycaster
    {
        private new Camera camera;

        protected override void Awake()
        {
            base.Awake();

            FindComponents();
            ProcessComponents();
        }
        private void FindComponents()
        {
        }
        private void ProcessComponents()
        {
            ProcessOriginal();
            m_BlockingMask = 1 << LayerMask.NameToLayer("UI");
        }

        // 父类没有缓存Camera,所以这里缓存
        public override Camera eventCamera
        {
            get
            {
                if (camera == null)
                {
                    camera = base.eventCamera;
                }
                return camera;
            }
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            ClearAllRaycastTarget();
            ProcessOriginal();
        }
#endif

        [ContextMenu("ClearAllRaycastTarget")]
        public void ClearAllRaycastTarget()
        {
            Graphic[] graphics = GetComponentsInChildren<Graphic>(true);
            foreach (var k in graphics)
            {
                k.raycastTarget = false;
            }
        }
        private void ProcessOriginal()
        {
            GraphicRaycaster[] grs = GetComponents<GraphicRaycaster>();
            foreach (var k in grs)
            {
                if (!(k is GraphicRaycasterAdapter))
                {
                    GameObject.Destroy(k);
                }
            }
        }
    }
}
