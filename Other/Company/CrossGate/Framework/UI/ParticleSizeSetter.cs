using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ParticleSizeSetter : MonoBehaviour {
    [Range(0, 4)] [SerializeField] private int index;
    public List<Vector3> scales = new List<Vector3>();
    public List<Vector3> specialScales = new List<Vector3>();
    public List<RectTransform> rts = new List<RectTransform>();
    // 两行1,1,1
    // 一行1,0.7125

    public Text text;

    public Vector3 GetScale() {
        if (0 <= this.index && this.index < this.scales.Count) {
            if (_isSpecial) {
                return this.specialScales[this.index];
            }
            else {
                return this.scales[this.index];
            }
        }
        return Vector3.one;
    }

    private bool _isSpecial;
    public void Set(bool isSpecial = false, float cd = 0.2f) {
        this._isSpecial = isSpecial;

        this.CancelInvoke(nameof(_DoSet));
        this.Invoke(nameof(_DoSet), cd);
    }

    private void _DoSet() {
        int lineCount = this.text.cachedTextGeneratorForLayout?.lineCount ?? 0;
        this.index = lineCount - 1;
        var finalScale = this.GetScale();

        foreach (var rt in this.rts) {
            if (rt != null) {
                rt.localScale = finalScale;
            }
        }
    }
}
