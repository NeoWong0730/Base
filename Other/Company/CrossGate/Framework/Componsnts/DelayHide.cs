using UnityEngine;
using UnityEngine.UI;

// 多長時間之後設置gameobject的set active
[DisallowMultipleComponent]
public class DelayHide : MonoBehaviour
{
    public float cd = 1f;

    private void OnEnable()
    {
        Invoke("AutoExec", cd);
    }
    private void OnDisable()
    {
        CancelInvoke("AutoExec");
    }
    private void AutoExec()
    {
        gameObject.SetActive(false);
    }
}
