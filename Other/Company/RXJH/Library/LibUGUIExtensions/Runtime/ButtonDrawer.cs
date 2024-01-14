using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UI.Extensions
{
    public class ButtonDrawer : MonoBehaviour
    {
        private static Vector3[] fourCorners = new Vector3[4];
        public Color color = Color.blue;
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var objects = GameObject.FindObjectsOfType<MaskableGraphic>();
            foreach (MaskableGraphic g in objects)
            {
                // 能否使用GUI.changed优化？
                if (g.raycastTarget)
                {
                    RectTransform rectTransform = g.transform as RectTransform;
                    rectTransform.GetWorldCorners(fourCorners);
                    Gizmos.color = color;
                    for (int i = 0; i < 4; i++)
                    {
                        Gizmos.DrawLine(fourCorners[i], fourCorners[(i + 1) % 4]);
                    }
                }
            }
        }
#endif
    }
}
