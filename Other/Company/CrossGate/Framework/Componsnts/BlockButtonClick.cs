using Lib.Core;
using UnityEngine;
using UnityEngine.UI;

public class BlockButtonClick : MonoBehaviour {
    public float time = 0f;
    public Button button;
    private bool oldStatus;

    private Timer timer;

    private void Awake() {
        if (this.button == null) {
            this.button = this.GetComponent<Button>();
            this.oldStatus = this.button.interactable;
        }
    }

    private void OnEnable() {
        this.Drive();
    }

    private void Drive() {
        if (this.button != null && this.time > 0f) {
            this.button.interactable = false;
            this.timer?.Cancel();
            this.timer = Timer.Register(this.time, () => {
                this.button.interactable = true;
            });
        }
    }
    private void OnDisable() {
        if (this.button != null) {
            this.button.interactable = this.oldStatus;
        }
        this.timer?.Cancel();
    }
}
