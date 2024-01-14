using UnityEngine;
using UnityEngine.UI;

public class DelayClearText : MonoBehaviour
{
    public Text text;
    private void Awake()
    {
        if (text == null)
        {
            text = GetComponent<Text>();
        }
    }

    public void InvokeDelay(float seconds)
    {
        CancelInvoke("Delay");
        Invoke("Delay", seconds);
    }
    private void Delay()
    {
        if (text != null)
        {
            text.text = null;
        }
    }
}
