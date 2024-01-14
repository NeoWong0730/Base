using UnityEngine;
using UnityEngine.UI;

// 灰化
[DisallowMultipleComponent]
public class GraphicGrayer : MonoBehaviour {
    public Material grayMaterial;

    private static Material _default;
    public static Material Default {
        get {
            if (_default == null) {
                _default = new Material(Shader.Find("UI/DefaultGray"));
            }

            return _default;
        }
    }

    public Material finalMaterial {
        get {
            if (grayMaterial == null) {
                return Default;
            }

            return grayMaterial;
        }
    }

    public bool Status {
        set {
            if (finalMaterial != null && graphics != null) {
                if (!value) {
                    foreach (var graphic in graphics) {
                        graphic.material = finalMaterial;
                    }
                }
                else {
                    foreach (var graphic in graphics) {
                        graphic.material = null;
                    }
                }
            }
        }
    }

#if UNITY_EDITOR
    [SerializeField]
#endif
    private Graphic[] graphics = new Graphic[0];

#if UNITY_EDITOR
    [ContextMenu(nameof(ReCollect))]
#endif
    public void ReCollect() {
        graphics = GetComponentsInChildren<Graphic>();
    }

    protected void Awake() {
        ReCollect();
    }
}