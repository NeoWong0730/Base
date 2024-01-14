using System;
using UnityEngine;
using UnityEngine.UI;

public enum EGiftStatus {
    Locked = 0, // 未达标
    UnGot = 1, // 未获取
    Got = 2, // 已获取
}

public class GiftStatus : MonoBehaviour {
    public GameObject lockedGo;
    public GameObject unGotGo;
    public GameObject gotGo;

    public Button btn;
    public Action<EGiftStatus> onClicked { get; private set; }

    public EGiftStatus status { get; private set; } = EGiftStatus.Locked;

    public void Awake() {
        if (this.btn != null) {
            this.btn.onClick.AddListener(this.OnBtnClciked);
        }
    }

    public GiftStatus SetAction(Action<EGiftStatus> action) {
        this.onClicked = action;
        return this;
    }
    public GiftStatus Refresh(int targetStatus) {
        this.Refresh((EGiftStatus)targetStatus);
        return this;
    }

    public void Refresh(EGiftStatus targetStatus) {
        this.status = targetStatus;

        if (this.lockedGo != null) {
            this.lockedGo.SetActive(this.status == EGiftStatus.Locked);
        }
        if (this.unGotGo != null) {
            this.unGotGo.SetActive(this.status == EGiftStatus.UnGot);
        }
        if (this.gotGo != null) {
            this.gotGo.SetActive(this.status == EGiftStatus.Got);
        }
    }

    private void OnBtnClciked() {
        this.onClicked?.Invoke(this.status);
    }
}