using UnityEngine;
using UnityEngine.UI;

namespace UI.Extensions
{
    public class CanvasAdapter : MonoBehaviour
    {
        public static int ScreenWidth = 1280;
        public static int ScreenHeight = 720;
        public static float ScreenRatio = 1f * ScreenWidth / ScreenHeight;
        
        private Canvas cvs = null;
        private CanvasScaler cvsScaler = null;
        private RectTransform rectTransform = null;
        private GraphicRaycasterAdapter graphicRaycaster = null;
        private ButtonDrawer buttonDrawer;
        private SafeAreaController safeAreaController;

        private void Awake()
        {
            cvsScaler = gameObject.GetComponent<CanvasScaler>();
            cvs = gameObject.GetComponent<Canvas>();
            rectTransform = GetComponent<RectTransform>();

            graphicRaycaster = gameObject.GetComponent<GraphicRaycasterAdapter>();
            if (graphicRaycaster == null)
            {
                graphicRaycaster = gameObject.AddComponent<GraphicRaycasterAdapter>();
            }

            /*有些界面不需要此脚本，引导判断是否需要安全区域时候需要判断此脚本
            if (safeAreaController == null)
            {
                safeAreaController = gameObject.AddComponent<SafeAreaController>();
            }
            */
#if UNITY_EDITOR
            buttonDrawer = GetComponent<ButtonDrawer>();
            if (buttonDrawer == null)
            {
                buttonDrawer = gameObject.AddComponent<ButtonDrawer>();
            }
#endif
        }

        public void Start()
        {
            if (cvsScaler != null)
            {
                cvsScaler.referenceResolution = new Vector2(ScreenWidth, ScreenHeight);
            }
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition3D = Vector3.zero;
            }
//             if (cvs != null)
//             {
//                 cvs.renderMode = RenderMode.ScreenSpaceCamera;
//                 cvs.pixelPerfect = false;
//                 // 这个值是UICamera的世界Z值绝对值，目的是确保所有ui的根节点z都是0，因为如果不是0，会被当做3dui处理
//                 // 影响hatch和批
//                 cvs.planeDistance = 10f;
//             }
            if (graphicRaycaster != null)
            {
                graphicRaycaster.ignoreReversedGraphics = false;
            }
        }

        public void AdjustSortOrder(int order)
        {
            if (cvs != null)
            {
                if (cvs.sortingOrder != order)
                {
                    cvs.sortingOrder = order;
                }
            }
        }
    }
}
