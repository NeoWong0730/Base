using Framework;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class ButtonAudio : MonoBehaviour, IPointerClickHandler {
    public uint audioId;
    public bool closeWhenDisable = true;

    private AudioEntry audioEntry;

    public void OnPointerClick(PointerEventData eventData) {
        this.audioEntry = LogicStaticMethodDispatcher.AudioUtil_PlayAudio(this.audioId);
    }

    // 按钮被隐藏的时候，进行音效关闭
    private void OnDisable() {
        if (this.closeWhenDisable) {
            this.audioEntry?.Stop();
            this.audioEntry = null;
        }
    }
}
