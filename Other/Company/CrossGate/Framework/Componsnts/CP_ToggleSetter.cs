using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CP_ToggleSetter : MonoBehaviour
{
    public enum EToggleSetType
    {
        ShowOrHide,
    }
    public Toggle toggle;
    public EToggleSetType toggleType = EToggleSetType.ShowOrHide;
    public CP_TransformContainer transformContainer;

    private void Start()
    {
        toggle.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(bool active)
    {
        switch (toggleType)
        {
            case EToggleSetType.ShowOrHide:
                OnShowOrHide(active);
                break;
        }
    }

    public void OnShowOrHide(bool active) { transformContainer.ShowHideBySetActive(active); }
}
