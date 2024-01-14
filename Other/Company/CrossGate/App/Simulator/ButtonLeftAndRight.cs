#if UNITY_STANDALONE_WIN
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonLeftAndRight : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this.ceil.OnClick_AppBtn();
    }

    UISimulator.AppItemCeil ceil;

    internal void Init(UISimulator.AppItemCeil appItemCeil)
    {
        this.ceil = appItemCeil;
    }
}
#endif

