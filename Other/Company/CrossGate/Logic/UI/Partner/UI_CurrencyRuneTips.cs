using Logic.Core;
using Lib.Core;
using System;
using Table;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Logic
{
    public class UI_CurrencyRuneTips : UIBase
    {
        public class TipCell
        {
            private Transform transform;
            private Text txtName;
            private Button btn;
            private Text txtBtn;

            private uint uId; //1,符文背包，2 决胜竞技场
            public void Init(Transform trans)
            {
                transform = trans;

                txtName = transform.Find("Text_01").GetComponent<Text>();
                btn = transform.Find("Btn_01_Small").GetComponent<Button>();
                btn.onClick.AddListener(OnClick);
                txtBtn = transform.Find("Btn_01_Small/Text_01").GetComponent<Text>();
            }

            public void UpdateInfo(uint id)
            {
                this.uId = id;

                txtName.text = LanguageHelper.GetTextContent(2006209 + this.uId);
                txtBtn.text = LanguageHelper.GetTextContent(2006212 + this.uId);
            }

            private void OnClick()
            {
                if (this.uId == 1)
                {
                    if (UIManager.IsVisibleAndOpen(EUIID.UI_Partner))
                    {
                        Sys_Partner.Instance.eventEmitter.Trigger(Sys_Partner.EEvents.OnTelPartnerTab, 4);
                        return;
                    }

                    PartnerUIParam param = new PartnerUIParam();
                    param.tabIndex = 4;
                    UIManager.OpenUI(EUIID.UI_Partner, false, param);
                }
                else if (this.uId == 2)
                {
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(1400400);

                    if (UIManager.IsVisibleAndOpen(EUIID.UI_Partner))
                    {
                        UIManager.CloseUI(EUIID.UI_Partner);
                    }
                }

                UIManager.CloseUI(EUIID.UI_CurrencyRuneTips);
            }
        }

        private Text txtNum;
        private Transform goTemplate;

        protected override void OnLoaded()
        {
            Button btnClose = transform.Find("Animator/ClickClose").GetComponent<Button>();
            btnClose.onClick.AddListener(() => { this.CloseSelf(); });

            txtNum = transform.Find("Animator/Item01/Text_02").GetComponent<Text>();
            
            goTemplate = transform.Find("Animator/List/Content/Item01");
            goTemplate.gameObject.SetActive(false);
        }

        protected override void OnOpen(object arg)
        {

        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            txtNum.text = Sys_Bag.Instance.GetItemCount(9).ToString();

            FrameworkTool.DestroyChildren(goTemplate.parent.gameObject, goTemplate.name);
            uint[] arr = new uint[2] { 1u, 2u }; //符文背包，符文竞技场
            for (int i = 0; i < arr.Length; ++i)
            {
                GameObject go = FrameworkTool.CreateGameObject(goTemplate.gameObject, goTemplate.parent.gameObject);
                go.SetActive(true);
                go.name = string.Format("{0}(Clone)", goTemplate.name);

                TipCell cell = new TipCell();
                cell.Init(go.transform);
                cell.UpdateInfo(arr[i]);
            }
        }

    }
}


