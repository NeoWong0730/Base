using System;
using System.Collections.Generic;
using Logic.Core;
using UnityEngine;


namespace Logic
{ 
    public class UI_Daily_DetailCom : UIBase
    {
        public UI_Daily_Detail m_detail = new UI_Daily_Detail();

        protected override void OnLoaded()
        {            
            m_detail.Init(gameObject.transform);

            m_detail.SetJoinEnable(false);

            m_detail.OnCloseEvent.AddListener(OnclickClose);
        }

        /// <summary>
        /// 打开详细界面
        /// </summary>
        /// <param name="arg">日常表ID</param>
        protected override void OnOpen(object arg)
        {            
            m_detail.ConfigID = (uint)arg;
        }
        protected override void OnShow()
        {         
            m_detail.Show();
        }


        protected override void OnHide()
        {            
            m_detail.Hide();
        }

        private void OnclickClose()
        {
            CloseSelf();
        }
    }

}
