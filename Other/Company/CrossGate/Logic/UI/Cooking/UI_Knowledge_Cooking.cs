using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.EventSystems;
using Net;

namespace Logic
{
    public partial class UI_Knowledge_Cooking : UIBase
    {
        private uint m_SkipId;

        protected override void OnInit()
        {
            m_CurRecipeSelectIndex = 0;
            m_CurAtlasTab = 0;
            m_CurSpecialTab = 0;
        }

        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                m_SkipId = (uint)arg;

            }
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Cooking.Instance.eventEmitter.Handle(Sys_Cooking.EEvents.OnUpdateBookRightSubmitState, OnUpdateSubmitState, toRegister);
            Sys_Cooking.Instance.eventEmitter.Handle<uint>(Sys_Cooking.EEvents.OnUpdateBookRightSubmitState, OnUpdateSubmitState, toRegister);
            Sys_Cooking.Instance.eventEmitter.Handle(Sys_Cooking.EEvents.OnRefreshLeftSubmitRedPoint, OnRefreshLeftSubmitRedPoint, toRegister);
            Sys_Cooking.Instance.eventEmitter.Handle(Sys_Cooking.EEvents.OnUpdateScore, RefreshScore, toRegister);
            Sys_Cooking.Instance.eventEmitter.Handle<uint>(Sys_Cooking.EEvents.OnActiveCook, OnActiveCook, toRegister);
            Sys_Cooking.Instance.eventEmitter.Handle<uint>(Sys_Cooking.EEvents.OnRefreshWatchState, OnRefreshWatch, toRegister);
            Sys_Cooking.Instance.eventEmitter.Handle<uint>(Sys_Cooking.EEvents.OnGetReward, RefreshReward, toRegister);
        }

        protected override void OnLoaded()
        {
            RegisterRight();
            RegisterLeft();
        }

        protected override void OnShow()
        {
            Sys_Cooking.Instance.SortCooking();
            Sys_Cooking.Instance.UpdateAllCookingSubmitState();
            Sys_Cooking.Instance.UpdateRewards();

            if (m_SkipId != 0)
            {
                m_Cookings = Sys_Cooking.Instance.cookings;

                Cooking cooking = m_Cookings.Find(_ => _.id == m_SkipId);
                if (cooking != null)
                {
                    m_CurRecipeSelectIndex = m_Cookings.IndexOf(cooking);
                    m_CurSelectCooking = m_Cookings[m_CurRecipeSelectIndex];
                    curSelectSubmitIndex = m_CurSelectCooking.submitIndex;
                    m_SkipId = 0;
                }
                RefreshLeftView();
                RefreshRightView();
            }
            else
            {
                m_CP_ToggleRegistry_Special.SwitchTo(m_CurSpecialTab);
            }
          
            m_UI_CurrencyTitle.InitUi();
        }

        protected override void OnDestroy()
        {
            m_UI_CurrencyTitle.Dispose();
        }
    }
}


