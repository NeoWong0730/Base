using UnityEngine;
using UnityEngine.UI;

// 防止按钮的连续点击,给一个cd时间
[RequireComponent(typeof(Button))]
[DisallowMultipleComponent]
public class ButtonCtrl : MonoBehaviour {
    public Button button = null;
    public bool grayWhenDisable = false;
    public Material grayMaterial;
    public float cd = 0.3f;

    private Graphic[] graphics;

    public bool buttonStatus {
        get {
            if (this.button != null) {
                return this.button.enabled;
            }
            return false;
        }
        set {
            if (this.button != null && this.button.enabled != value) {
                this.button.enabled = value;

                if (this.grayMaterial != null && this.graphics != null) {
                    if (!value) {
                        if (this.grayWhenDisable) {
                            foreach (var graphic in this.graphics) {
                                graphic.material = this.grayMaterial;
                            }
                        }
                    }
                    else {
                        foreach (var graphic in this.graphics) {
                            graphic.material = null;
                        }
                    }
                }
            }
        }
    }

    private void Awake() {
        if (this.button == null) {
            this.button = this.GetComponent<Button>();
        }
        if (this.button != null) {
            this.button.onClick.AddListener(this.OnClick);
        }
        if (this.grayMaterial != null) {
            this.Collect();
        }
    }

    public void Collect() {
        this.graphics = this.GetComponentsInChildren<Graphic>();
    }

    private void OnClick() {
        if (this.buttonStatus) {
            this.buttonStatus = false;
            this.ProcessCd();
        }
    }

    private void ProcessCd() {
        this.Invoke("AutoEnableCollider", this.cd);
    }
    private void AutoEnableCollider() {
        this.buttonStatus = true;
    }
}
