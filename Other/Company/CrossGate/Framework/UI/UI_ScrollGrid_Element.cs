using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class UI_ScrollGrid_Element : UIBehaviour//, IPointerClickHandler
{

    public class ValueChangeEvent : UnityEvent<bool>
    {
        //public void AddListener(Action<bool> action)
        //{
        //    this.AddListener(action);
        //}
    }


    // public UI_ScrollGridEx ScrollGrid { get; set; }

    public ValueChangeEvent ValueChange = new ValueChangeEvent();
    //protected override void Start()
    //{
        
    //}

    //void OnTransformChildrenChanged()
    //{

    //}

    //public void OnPointerClick(PointerEventData eventData)
    //{
       
    //}

    public void Foucs()
    {
        ValueChange.Invoke(true);
    }

    public void LostFocus()
    {
        ValueChange.Invoke(false);
    }
}
