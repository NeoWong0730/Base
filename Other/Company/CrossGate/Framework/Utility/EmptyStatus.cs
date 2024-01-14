using System.Collections.Generic;
using UnityEngine;

public enum EEmptyStatus {
    Empty = 0,
    UnEmpty = 1,
}

public class EmptyStatus : MonoBehaviour {
    [Header("空状态节点")]
    public List<Transform> emptyList = new List<Transform>(0);
    [Header("展示状态节点")]
    public List<Transform> unemptyList = new List<Transform>(0);

    [SerializeField]
    private EEmptyStatus status = EEmptyStatus.Empty;

    public void Refresh(EEmptyStatus targetStatus) {
        this.status = targetStatus;

        if (targetStatus == EEmptyStatus.Empty) {
            foreach (var item in this.emptyList) {
                if (item != null) {
                    item.gameObject.SetActive(true);
                }
            }
            foreach (var item in this.unemptyList) {
                if (item != null) {
                    item.gameObject.SetActive(false);
                }
            }
        }
        else {
            foreach (var item in this.emptyList) {
                if (item != null) {
                    item.gameObject.SetActive(false);
                }
            }
            foreach (var item in this.unemptyList) {
                if (item != null) {
                    item.gameObject.SetActive(true);
                }
            }
        }
    }
}