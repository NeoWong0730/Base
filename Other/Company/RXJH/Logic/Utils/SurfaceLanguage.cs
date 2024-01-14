using Logic;
using Table;
using UnityEngine;
using TMPro;

namespace Framework {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class SurfaceLanguage : MonoBehaviour {
        public uint key;

        private void Start() {
            Preview();
        }

#if UNITY_EDITOR
        [ContextMenu("预览")]
#endif
        private void Preview() {
            TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
            TextHelper.SetText<CSVLanguage>(text, key);
        }

#if UNITY_EDITOR
        [ContextMenu("清空")]
#endif
        private void Empty() {
            TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
            TextHelper.SetText(text, string.Empty);
        }
    }
}