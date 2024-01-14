using UnityEngine;
using UnityEngine.UI;

// 防止按钮的连续点击,给一个cd时间
[RequireComponent(typeof(CP_Toggle))]
public class ToggleCD : MonoBehaviour
{
    public CP_Toggle toggle = null;
    public float cd = 0.3f;
    public System.Action onValueTrue;

    public bool toggleStatus
    {
        get
        {
            if (toggle != null)
            {
                return toggle.enabled;
            }
            return false;
        }
        set
        {
            if (toggle != null && toggle.enabled != value)
            {
                toggle.enabled = value;
            }
        }
    }

    private void Awake()
    {
        if (toggle == null)
        {
            toggle = GetComponent<CP_Toggle>();
        }
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(OnValueChanged);
        }
    }

    private void OnValueChanged(bool arg)
    {
        if (arg && toggleStatus)
        {
            onValueTrue?.Invoke();
            toggleStatus = false;
            ProcessCd();
        }
    }

    private void ProcessCd()
    {
        Invoke("AutoEnableCollider", cd);
    }
    private void AutoEnableCollider()
    {
        toggleStatus = true;
    }
}
