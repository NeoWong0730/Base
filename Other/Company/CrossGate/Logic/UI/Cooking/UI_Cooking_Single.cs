using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.EventSystems;

namespace Logic
{
    public class OpenCookingSingleParm
    {
        public uint cookFunId;
        public uint cookId;
    }

    public partial class UI_Cooking_Single : UIBase
    {
        private List<uint> m_SupportModel;
        private List<uint> m_SupportKitchen = new List<uint>();
        private uint m_CookFunId;
        private uint m_CookId;
        private uint m_JumpId;
        private OpenCookingSingleParm m_OpenCookingSingleParm;

        protected override void OnOpen(object arg)
        {
            m_OpenCookingSingleParm = arg as OpenCookingSingleParm;
            if (m_OpenCookingSingleParm != null)
            {
                m_CookFunId = m_OpenCookingSingleParm.cookFunId;
                m_CookId = m_OpenCookingSingleParm.cookId;
                m_JumpId = m_CookId;
                CSVCookFunction.Data cSVCookFunctionData = CSVCookFunction.Instance.GetConfData(m_CookFunId);
                if (cSVCookFunctionData != null)
                {
                    m_SupportModel = CSVCookFunction.Instance.GetConfData(m_CookFunId).allow_type;
                    m_SupportKitchen = CSVCookFunction.Instance.GetConfData(m_CookFunId).allow_tool;
                    UpdateSelectKitchen();
                }
                else
                {
                    DebugUtil.LogFormat(ELogType.eCooking, $"没有找到烹饪功能{m_CookFunId}");
                }
            }
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Cooking.Instance.eventEmitter.Handle(Sys_Cooking.EEvents.OnClearMidSelection, OnClearMidSelection, toRegister);
            Sys_Cooking.Instance.eventEmitter.Handle(Sys_Cooking.EEvents.OnCookEndPlay, OnSingleCookingEnd, toRegister);
        }

        protected override void OnInit()
        {
            //if (Sys_Cooking.Instance.HasRecipeActive())
            //{
            //    m_CurSelectCookingModel = 3;
            //}
            m_CurSelectCookingModel = 3;
            m_CurSelectRecipeTab = 0;
            m_CurSelectRecipeIndex = 0;
            m_CurSelectIngredientIndex = 0;
            m_CurIngredientTab = 0;
            m_CurSelectMidGridIndex = 0;
            m_CurStep = 1;
            Sys_Cooking.Instance.SortCooking();
            SetCookingData();
        }

        protected override void OnLoaded()
        {
            RegisterLeft();
            RegisterMid();
            RegisterRight();
        }

        protected override void OnShow()
        {
            m_CP_ToggleRegistry_CookingModel.SwitchTo(m_CurSelectCookingModel);
            SetCookingData();
            m_UI_CurrencyTitle.InitUi();
            RefreshLeftView();
            RefreshMiddleView();
            RefreshRightView();
        }

        private void OnSingleCookingEnd()
        {
            m_CookId = Sys_Cooking.Instance.lastCookId;
        }

        protected override void OnDestroy()
        {
            m_UI_CurrencyTitle.Dispose();
            m_Timer?.Cancel();
        }
    }
}


