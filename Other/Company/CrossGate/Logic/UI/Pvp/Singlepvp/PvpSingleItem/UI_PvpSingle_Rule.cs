using Logic.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{ 
    public class UI_PvpSingle_Rule : UIComponent, UI_PvpSingle_Rule_Layout.IListener
    {
        private UI_PvpSingle_Rule_Layout m_Layout = new UI_PvpSingle_Rule_Layout();


        public void OnClickClose()
        {
            Hide();
        }

        protected override void Loaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        public override void Show()
        {
            base.Show();


        }

        public void SetSort(int sort)
        {
            m_Layout.SetSort(sort);
        }
    }

}
