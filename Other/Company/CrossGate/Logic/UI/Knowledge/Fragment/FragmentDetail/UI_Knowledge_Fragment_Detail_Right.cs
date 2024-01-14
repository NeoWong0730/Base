using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Lib.Core;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Knowledge_Fragment_Detail_Right
    {
        

        private Transform transform;

        //private List<CellToggle> listEvents = new List<CellToggle>(4);

        public void Init(Transform trans)
        {
            transform = trans;

            //int count = transform.childCount;
            //for (int i = 0; i < count; ++i)
            //{
            //    Transform temp = transform.GetChild(i);
            //    CellToggle evt = new CellToggle();
            //    evt.Init(temp);
            //    listEvents.Add(evt);
            //}
        }

        public void OnShow()
        {
            transform.gameObject.SetActive(true);
        }

        public void OnHide()
        {
            transform.gameObject.SetActive(false);
        }

        public void OnDestroy()
        {
            //for (int i = 0; i < listEvents.Count; ++i)
            //    listEvents[i].OnDestroy();
        }
    }
}


