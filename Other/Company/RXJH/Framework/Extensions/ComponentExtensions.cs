using System;
using UnityEngine;

namespace Framework {
    public static class ComponentExtensions {
        public static T GetOrAddComponent<T>(this Component component) where T : Component {
            if (component == null) {
                return null;
            }

            T rlt = null;
            if (!component.TryGetComponent<T>(out rlt)) {
                rlt = component.gameObject.AddComponent<T>();
            }

            return rlt;
        }
    }
}