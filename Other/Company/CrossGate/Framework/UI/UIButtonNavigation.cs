using UnityEngine;
using UnityEngine.UI;


public class UIButtonNavigation : MonoBehaviour
{
    private void Awake()
    {
        var button = GetComponentInChildren<Button>();
        if (button != null)
            ChangeNavigationToNone(button);
    }
    void ChangeNavigationToNone(Selectable component)
    {
#if UNITY_STANDALONE_WIN && (OPEN_PC_KEYCODE_FUN || !UNITY_EDITOR)
        if (component.navigation.mode == Navigation.Mode.None)
            return;
        Navigation customNav = new Navigation();
        customNav.mode = Navigation.Mode.None;
        component.navigation = customNav;
#endif

    }
}

