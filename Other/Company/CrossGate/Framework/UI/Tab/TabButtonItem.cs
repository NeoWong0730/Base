using System;
using UnityEngine.UI;

public class TabButtonItem : TabItem {
    public Button btn;
    public Action onBtnClickedAction;

    protected override void Start() {
        this.btn.onClick.AddListener(this.OnBtnClicked);
    }

    private void OnBtnClicked() {
        this.onBtnClickedAction?.Invoke();
    }
}
