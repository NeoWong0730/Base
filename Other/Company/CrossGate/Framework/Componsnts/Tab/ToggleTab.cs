using UnityEngine.UI;

public class ToggleTab : Tab {
    public Toggle toggle;

    private void Awake() {
        this.toggle = this.GetComponent<Toggle>();
        this.toggle.onValueChanged.AddListener(this.OnValueChanged);
    }

    private void OnValueChanged(bool flag) {
        this.collector.SetSelected(this.id);
    }
}