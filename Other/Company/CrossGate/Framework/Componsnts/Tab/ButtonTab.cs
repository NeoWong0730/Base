using UnityEngine.UI;

public class ButtonTab : Tab {
    public Button btn;

    private void Awake() {
        this.btn = this.GetComponent<Button>();
        this.btn.onClick.AddListener(this.OnBtnClicked);
    }

    private void OnBtnClicked() {
        this.collector.SetSelected(this.id);
    }
}
