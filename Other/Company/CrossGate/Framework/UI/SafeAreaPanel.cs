using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 安全区域面板(适配iPhone x)
    /// </summary>
    public class SafeAreaPanel : MonoBehaviour
    {
        private RectTransform target;

#if UNITY_EDITOR
        [SerializeField]
        private bool Simulate_X = false;
#endif

        private void Awake()
        {
            target = GetComponent<RectTransform>();
            ApplySafeArea();
        }

        private void ApplySafeArea()
        {
            var area = SafeAreaUtils.Get();
#if UNITY_EDITOR
            /*
             iPhone X 横持手机方向:
             iPhone X 分辨率
             2436 x 1125 px

             safe area
             2172 x 1062 px

             左右边距分别
             132 px

             底边距(有home条)
             63px

             顶边距
             0px
             */
            float xWidth = 2436f;
            float xHieght = 1125f;
            float Margin = 132f;
            //float InsetsBottom = 63f;

            if ((Screen.width == (int)xWidth && Screen.height == (int)xHieght)
                || (Screen.width == 812 && Screen.height == 375))
            {
                Simulate_X = true;
            }

            if (Simulate_X)
            {
                var insets = area.width * Margin / xWidth;
                var positionOffset = new Vector2(insets, 0);
                var sizeOffset = new Vector2(insets * 2, 0);
                area.position = area.position + positionOffset;
                area.size = area.size - sizeOffset;
            }
#endif      

            var anchorMin = area.position;
            var anchorMax = area.position + area.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            target.anchorMin = anchorMin;
            target.anchorMax = anchorMax;
        }
    }

}

