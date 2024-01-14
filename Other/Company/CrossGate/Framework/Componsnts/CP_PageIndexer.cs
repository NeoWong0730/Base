using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CP_PageIndexer : MonoBehaviour
{
    public Text leftText;
    public Text rightText;

    public void Refresh<T>(T left, T right)
    {
        leftText.text = left.ToString();
        rightText.text = right.ToString();
    }
}
