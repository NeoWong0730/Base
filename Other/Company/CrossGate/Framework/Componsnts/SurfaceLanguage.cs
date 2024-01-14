using ILRuntime.CLR.TypeSystem;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace Framework {
    public class SurfaceLanguage : MonoBehaviour {
        public uint key;

        private void Start() {
            Preview();
        }

#if UNITY_EDITOR
        [ContextMenu(nameof(Preview))]
#endif
        private void Preview() {
            Text text = GetComponent<Text>();
            LogicStaticMethodDispatcher.TextHelper_SetText(text, key);
        }
    }
}