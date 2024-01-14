using UnityEngine;
using UnityEngine.UI;

// 忽略layout组件的排序
public class CP_LayoutIgnorer : MonoBehaviour, ILayoutIgnorer {
    public bool ignoreLayout {
        get {
            return true;
        }
    }
}
