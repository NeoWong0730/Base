using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class CP_SliderText : MonoBehaviour
{
    public string format;
    public Text targetText;
    Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnValueChanged);

        if (targetText)
        {
            targetText.text = string.Format(format, slider.value);
        }
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(float v)
    {
        if (targetText)
        {
            targetText.text = string.Format(format, v);
        }
    }
}
