using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Logic
{
    /// <summary>
    ///pvp 结算
    /// </summary>
    public partial class UI_Pvp_Single_Over : UIBase, UI_Pvp_Single_Over_Layout.IListener
    {
        UI_Pvp_Single_Over_Layout m_Layout = new UI_Pvp_Single_Over_Layout();



        public void OnClickClose()
        {
            CloseSelf();

            UIManager.OpenUI(EUIID.UI_Pvp_Single);
        }

        protected override void OnShow()
        {            
            Refresh();
        }
        protected override void OnLoaded()
        {            
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }        

        private void Refresh()
        {
           var result = Sys_Pvp.Instance.FinghtEndInfo.IsWin ? 0 : 1;
            m_Layout.SetOverResult(result);

            m_Layout.SetReward(Sys_Pvp.Instance.FinghtEndInfo.ItemIds, Sys_Pvp.Instance.FinghtEndInfo.ItemNums);
        }
   
        
    }


   
}
