using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GraphicGrayer : MonoBehaviour {
    public Material grayMaterial;

    private Dictionary<Graphic, Material> graphics = new Dictionary<Graphic, Material>();

    private void Awake() {
        var ls = this.GetComponentsInChildren<Graphic>();
        foreach (var item in ls) {
            graphics[item] = item.material;
        }
    }

    public void SetGray(bool toGray) {
        if (graphics.Count <= 0) {
            Awake();
        }

        if (toGray) {
            foreach (var kvp in graphics) {
                kvp.Key.material = grayMaterial;
            }
        }
        else {
            foreach (var kvp in graphics) {
                kvp.Key.material = kvp.Value;
            }
        }
    }
}
