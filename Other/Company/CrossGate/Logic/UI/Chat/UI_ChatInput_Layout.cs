//this file is auto created by QuickCode,you can edit it 
//do not need to care initialization of ui widget any more 
//------------------------------------------------------------
/*
*   @Author:TR
*   DateTime:2019/12/25 12:45:58
*   Purpose:UI Componments Data Binding
*/
//-------------------------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
namespace Logic
{

    public class UI_ChatInput_Layout
    {
        #region UI Variable Statement 
        public GameObject mRoot { get; private set; }
        public Transform mTrans { get; private set; }
        public CP_ToggleRegistry tg_funTab_CP_ToggleRegistry { get; private set; }
        public InfinityGrid sv_BQ_InfinityGrid { get; private set; }
        public InfinityGrid sv_LS_InfinityGrid { get; private set; }
        public InfinityGrid sv_DJ_InfinityGrid { get; private set; }
        public InfinityGrid sv_RW_InfinityGrid { get; private set; }
        public InfinityGrid sv_CW_InfinityGrid { get; private set; }
        public InfinityGrid sv_CH_InfinityGrid { get; private set; }
        public InfinityGrid sv_CJ_InfinityGrid { get; private set; }
        public Button btn_CloseFunc_Button { get; private set; }
        public Transform btn_RW { get; private set; }
        #endregion
        public void Parse(GameObject root)
        {
            mRoot = root;
            mTrans = root.transform;
            tg_funTab_CP_ToggleRegistry = mTrans.Find("Animator/rt_Function/_tg_funTab").GetComponent<CP_ToggleRegistry>();
            sv_BQ_InfinityGrid = mTrans.Find("Animator/rt_Function/rtBQ/_sv_BQ").GetComponent<InfinityGrid>();
            sv_LS_InfinityGrid = mTrans.Find("Animator/rt_Function/rtLS/_sv_LS").GetComponent<InfinityGrid>();
            sv_DJ_InfinityGrid = mTrans.Find("Animator/rt_Function/rtDJ/_sv_DJ").GetComponent<InfinityGrid>();
            sv_RW_InfinityGrid = mTrans.Find("Animator/rt_Function/rtRW/_sv_RW").GetComponent<InfinityGrid>();
            sv_CH_InfinityGrid = mTrans.Find("Animator/rt_Function/rtCH/_sv_CH").GetComponent<InfinityGrid>();
            sv_CW_InfinityGrid = mTrans.Find("Animator/rt_Function/rtCW/_sv_CW").GetComponent<InfinityGrid>();
            sv_CJ_InfinityGrid = mTrans.Find("Animator/rt_Function/rtCJ/_sv_CJ").GetComponent<InfinityGrid>();
            btn_CloseFunc_Button = mTrans.Find("Animator/_btn_CloseFunc").GetComponent<Button>();
            btn_RW = mTrans.Find("Animator/rt_Function/_tg_funTab/Content/btnRW");
            Transform btn_CJ = mTrans.Find("Animator/rt_Function/_tg_funTab/Content/btnCJ");
            btn_CJ.gameObject.SetActive(Sys_Achievement.Instance.CheckAchievementIsCanShow());
            ChangeButtonNav();
        }

        public void RegisterEvents(IListener listener)
        {
            btn_CloseFunc_Button.onClick.AddListener(listener.OnCloseFunc_ButtonClicked);
        }

        public interface IListener
        {
            void OnCloseFunc_ButtonClicked();
        }

        private void ChangeButtonNav()
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            if (sv_BQ_InfinityGrid._element.GetComponent<UIButtonNavigation>() == null)
                sv_BQ_InfinityGrid._element.AddComponent<UIButtonNavigation>();
            if (sv_DJ_InfinityGrid._element.GetComponent<UIButtonNavigation>() == null)
                sv_DJ_InfinityGrid._element.AddComponent<UIButtonNavigation>();
            if (sv_LS_InfinityGrid._element.GetComponent<UIButtonNavigation>() == null)
                sv_LS_InfinityGrid._element.AddComponent<UIButtonNavigation>();
            if (sv_RW_InfinityGrid._element.GetComponent<UIButtonNavigation>() == null)
                sv_RW_InfinityGrid._element.AddComponent<UIButtonNavigation>();
            if (sv_CH_InfinityGrid._element.GetComponent<UIButtonNavigation>() == null)
                sv_CH_InfinityGrid._element.AddComponent<UIButtonNavigation>();
            if (sv_CW_InfinityGrid._element.GetComponent<UIButtonNavigation>() == null)
                sv_CW_InfinityGrid._element.AddComponent<UIButtonNavigation>();
            if (sv_CJ_InfinityGrid._element.GetComponent<UIButtonNavigation>() == null)
                sv_CJ_InfinityGrid._element.AddComponent<UIButtonNavigation>();
#endif

        }
    }

}
