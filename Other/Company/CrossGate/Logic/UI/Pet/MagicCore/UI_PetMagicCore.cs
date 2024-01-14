using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using System;

namespace Logic
{
    public enum EMagicCore
    {
        /// <summary>制作</summary>
        Make = 1,
        /// <summary>炼化属性</summary>
        Remake = 2,
    }
    public class MagicCorePrama
    {
        public uint pageType = 1;
        public uint itemId;
        public uint uiType;
    }
    public  class UI_PetMagicCore : UIBase, UI_PetMagicCore_ViewRight.IListener
    {
        private Button btnClose;

        private UI_CurrencyTitle currency;
        private UI_PetMagicCore_ViewRight tabsRight;
        private UI_PetMagicCore_Make makeView;
        private UI_PetMagicCore_ReMake remakeView;

        private List<uint> pageList = new List<uint>();
        private Dictionary<uint, UIComponent> pageDict = new Dictionary<uint, UIComponent>();
        private uint openType = (uint)EMagicCore.Make;
        private bool isInit = false;//是否初始化过
        private MagicCorePrama magicCorePrama = new MagicCorePrama();
        //private uint openType = 1u;
        #region 系统函数
        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                if(arg.GetType() == typeof(MagicCorePrama))
                {
                    magicCorePrama = arg as MagicCorePrama;
                }
                else if(arg is Tuple<uint, object>)
                {
                    Tuple<uint, object> tuple = arg as Tuple<uint, object>;
                    magicCorePrama.pageType = Convert.ToUInt32(tuple.Item2);
                }
                openType = magicCorePrama.pageType;
            }
            
        }
        protected override void OnLoaded()
        {
            Parse();
        }

        protected override void OnShow()
        {
            if (magicCorePrama.itemId <= 0 && !isInit)
            {
                makeView.ResetDefaultState();
                isInit = true;
            }
            else
            {
                if(magicCorePrama.uiType != 0)
                {
                    makeView.InitItemInfo(magicCorePrama.uiType, magicCorePrama.itemId);
                }
                isInit = true;
            }
            currency?.InitUi();
            //OnPageSelect(openType);
            tabsRight.OnPageBtnInit(openType);
        }
        protected override void OnHide()
        {
            makeView?.Hide();
            remakeView?.Hide();            
        }
        protected override void OnDestroy()
        {
            currency?.Dispose();
            makeView.OnDestroy();
            remakeView.OnDestroy();
        }
        #endregion

        #region func
        private void Parse()
        {
            btnClose = transform.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);
            currency = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);

            makeView = AddComponent<UI_PetMagicCore_Make>(transform.Find("Animator/View_Make"));
            pageList.Add(makeView.PageType);
            pageDict.Add(makeView.PageType, makeView);
            remakeView = AddComponent<UI_PetMagicCore_ReMake>(transform.Find("Animator/View_Artifice"));
            pageList.Add(remakeView.PageType);
            pageDict.Add(remakeView.PageType, remakeView);

            tabsRight = AddComponent<UI_PetMagicCore_ViewRight>(transform.Find("Animator/View_Left_Tabs"));
            tabsRight.Register(this);
        }
        #endregion

        #region event
        private void OnBtnCloseClick()
        {
            this.CloseSelf();
        }
        public void OnPageSelect(uint type)
        {
            openType = type;
            for (int i = 0; i < pageList.Count; i++)
            {
                uint key = pageList[i];
                if (key == type)
                {
                    pageDict[key].Show();
                }
                else
                {
                    pageDict[key].Hide();
                }
            }
        }
        #endregion

    }

}
