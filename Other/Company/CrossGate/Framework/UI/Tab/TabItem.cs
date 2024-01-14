using UnityEngine;

public class TabItem : MonoBehaviour {
    // 辅助处理各种复杂的SetActive
    public CP_TransformCollector transformCollector;
    public GameObject highLight;

    protected virtual void Start() {
    }

    public void SetHighlight(bool toHighlight) {
        if (this.highLight != null) {
            this.highLight.SetActive(toHighlight);
        }
    }
}
