using System;
using System.Collections.Generic;

using Logic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
  
    public partial class UI_BlessingInfo_Layout
    {

        Button BtnClose;

        Button BtnBless;

        public Toggle TogAuto;

        public Text TexInfo;

        public PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, false, false, false, false, false, true, false);
        public PropItem propItem = new PropItem();

        public PropIconLoader.ShowItemData m_NeedItemData = new PropIconLoader.ShowItemData(0, 1, false, false, false, false, false, true, false);
        public PropItem NeedPropItem = new PropItem();

        public MessageBoxEvt NeedBoxEvt = new MessageBoxEvt();


        public void Load(Transform root)
        {
        

            propItem.BindGameObject(root.Find("Animator/View_Content/Text_Award/PropItem").gameObject);

            BtnClose = root.Find("Animator/View_TipsBgNew06/Btn_Close").GetComponent<Button>();

            TexInfo = root.Find("Animator/View_Content/Text_Title/Text").GetComponent<Text>();

            TogAuto = root.Find("Animator/View_Content/Toggle").GetComponent<Toggle>();

            BtnBless = root.Find("Animator/View_Content/Btn").GetComponent<Button>();

            NeedPropItem.BindGameObject(root.Find("Animator/View_Content/PropItem").gameObject);
        }

        public interface IListener
        {
            void OnClickClose();

            void OnClickBless();

            void OnClickTogAuto(bool state);


        }


        public void SetListener(IListener listener)
        {
            BtnClose.onClick.AddListener(listener.OnClickClose);
            BtnBless.onClick.AddListener(listener.OnClickBless);
            TogAuto.onValueChanged.AddListener(listener.OnClickTogAuto);
        }

        public void SetNeedItemCount(uint needcount, long havecount)
        {
            uint contentId = 0;

            if (needcount > havecount)
            {

                contentId = 1601000004;
            }

            else
            {
                contentId = 1601000005;
            }
            NeedPropItem.btnNone.gameObject.SetActive(needcount > havecount);

            NeedPropItem.txtNumber.text = string.Format(LanguageHelper.GetTextContent(contentId), needcount.ToString(), havecount.ToString());
        }
    }
}
