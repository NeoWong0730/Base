using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TipScrollView : MonoBehaviour {
    // itemProto都是左上角为锚点，而且中心点在左上角
    public RectTransform itemProto;
    public GridLayoutGroup itemGroupLayout;
    private COWComponent<RectTransform> items = new COWComponent<RectTransform>();

    public RectTransform content;

    // tipParent是左上角为锚点，而且中心点在左上角
    public RectTransform tipNode;
    public RectTransform tipParent;

    public int ItemCount {
        get { return items.Count; }
    }

    // 如果没有tip，content的高度
    public float ContentHeight(int realCount) {
        int row = Mathf.CeilToInt(realCount * 1f / itemGroupLayout.constraintCount);
        return row * (itemGroupLayout.cellSize.y + itemGroupLayout.spacing.y) - itemGroupLayout.spacing.y;
    }

    public TipScrollView TryBuildOrRefresh(int targetCount, Action<RectTransform, int> onInit = null, Action<RectTransform, int> onRefresh = null) {
        items.TryBuildOrRefresh(itemProto.gameObject, transform, targetCount, onRefresh, onInit);
        return this;
    }

    // 负责腾开空间
    public void Sort(int realCount, bool toExpand = false, int itemIndex = 0, float expandHeight = 0f) {
        if (!toExpand) {
            tipParent.gameObject.SetActive(false);
            // 借助GridLayoutGroup排序
            itemGroupLayout.enabled = true;
            content.sizeDelta = new Vector2(content.sizeDelta.x, ContentHeight(realCount) + itemGroupLayout.spacing.y);
        }
        else {
            tipNode.SetParent(tipParent, false);
            tipNode.anchoredPosition = Vector3.zero;
            tipNode.gameObject.SetActive(true);
            tipParent.gameObject.SetActive(true);
            // 手动排序
            itemGroupLayout.enabled = false;
            content.sizeDelta = new Vector2(content.sizeDelta.x, ContentHeight(realCount) + expandHeight + itemGroupLayout.spacing.y);

            int row = Mathf.FloorToInt(1f * itemIndex / itemGroupLayout.constraintCount) + 1;
            int startIndex = row * itemGroupLayout.constraintCount;
            Vector3 startPos = items[0].localPosition;
            float xSpace = itemGroupLayout.cellSize.x + itemGroupLayout.spacing.x;
            float ySpace = itemGroupLayout.cellSize.y + itemGroupLayout.spacing.y;
            for (int i = 1, length = ItemCount; i < length; ++i) {
                int y = i / itemGroupLayout.constraintCount;
                int x = i % itemGroupLayout.constraintCount;

                if (i < startIndex) {
                    items[i].localPosition = new Vector3(startPos.x + x * xSpace, startPos.y - y * ySpace, startPos.z);
                }
                else {
                    items[i].localPosition = new Vector3(startPos.x + x * xSpace,
                        startPos.y - y * ySpace - expandHeight - itemGroupLayout.spacing.y, startPos.z);
                }
            }

            // 设置TipParent位置
            Vector3 localPos = tipParent.localPosition;
            tipParent.localPosition = new Vector3(localPos.x, items[itemIndex].localPosition.y - ySpace, localPos.z);
        }
    }
}