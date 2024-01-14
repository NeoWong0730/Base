using System;
using UnityEngine;
using UnityEngine.UI;

public class CP_PopdownList : MonoBehaviour {
    public bool isExpand = false;
    public Transform arrow;
    public Button expandButton;

    public GameObject optionProto;
    public Transform optionParent;
    public GameObject dropItemsNode;

    public Text selectedText;
    public Vector3 arrowAnchorPosition = new Vector3(-84f, -1.1f, 0f);

    private ScrollRect _scrollRect;

    public ScrollRect scrollRect {
        get {
            if (_scrollRect == null) {
                _scrollRect = this.GetComponentInChildren<ScrollRect>(true);
            }

            return _scrollRect;
        }
    }

    private void Awake() {
        expandButton.onClick.AddListener(OnBtnClicked);
        Expand(false);
    }

    private void OnBtnClicked() {
        if (!isExpand) {
            Expand(true);
        }
        else {
            Expand(false);
        }
    }

    public void SetSelected(string text) {
        selectedText.text = text;
    }

    public void Expand(bool toShow) {
        dropItemsNode.gameObject.SetActive(toShow);
        isExpand = toShow;
        arrow.localEulerAngles = new Vector3(0f, 0f, toShow ? 0f : 90f);
        (arrow as RectTransform).anchoredPosition3D = arrowAnchorPosition;

        scrollRect.normalizedPosition = readyNormal;
    }

    private Vector2 readyNormal = new Vector2(0, 1);

    public void MoveTo(bool horOrVer, float normal) {
        if (!horOrVer) {
            readyNormal = new Vector2(0, 1 - normal);
        }
        else {
            readyNormal = new Vector2(normal, 0);
        }
    }
}