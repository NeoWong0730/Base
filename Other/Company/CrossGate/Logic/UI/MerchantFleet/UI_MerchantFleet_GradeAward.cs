using Lib.Core;
using Logic;
using Logic.Core;
using Packet;
using System.Text;
using Table;
using UnityEngine;
using UnityEngine.UI;
using System;
using Framework;
using System.Collections.Generic;


namespace Logic
{
    //等级奖励界面
    public class UI_MerchantFleet_GradeAward : UIBase
    {
        private Button btn_Close;
        private GameObject go_Grid;
        private GameObject go_AwardCeil;
        private Button btn_All;
        private List<MerchantFleetAward> awardList=new List<MerchantFleetAward>();
        protected override void OnLoaded()
        {
            btn_Close = transform.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>();
            go_Grid = transform.Find("Animator/Rect/Rectlist").gameObject;
            go_AwardCeil = transform.Find("Animator/Rect/Rectlist/Image_line").gameObject;
            btn_All= transform.Find("Animator/Btn_01").GetComponent<Button>();
            btn_Close.onClick.AddListener(OnCloseButtonClicked);
            btn_All.gameObject.SetActive(false);
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_MerchantFleet.Instance.eventEmitter.Handle(Sys_MerchantFleet.EEvents.UpdateLevelAward, Refresh, toRegister);
        }

        protected override void OnShow()
        {
            
            PanelShow();
        }
        protected override void OnHide()
        {
            DefaultItem();
        }
        private void PanelShow()
        {
            DefaultItem();
            awardList.Clear();
            var count = Sys_MerchantFleet.Instance.MerchantGradeAwardTake.Count;
            FrameworkTool.CreateChildList(go_Grid.transform, count);
            for (int i=0;i<count; i++)
            {
                MerchantFleetAward award = new MerchantFleetAward(i);
                Transform trans = go_Grid.transform.GetChild(i);
                award.BindObject(trans);
                award.InitData();
                awardList.Add(award);
            }
        }
        private void Refresh()
        {
            for (int i=0;i<awardList.Count;i++)
            {
                awardList[i].InitData();
            }
        }
        private void DefaultItem()
        {
            FrameworkTool.DestroyChildren(go_Grid.gameObject, go_AwardCeil.transform.name);
        }
        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_MerchantFleet_GradeAward);
        }
        public class MerchantFleetAward
        {
            private Text tradeLevel;
            private GameObject propItem;
            private Text function;
            private Button btn;
            private Text btnText;
            private Image btnImg;
            private GameObject btnRedPoint;
            private GameObject goReceive;
            CSVMerchantFleetLevel.Data data;
            private bool canReceive=false;
            private bool hasReceive = false;
            private bool cantReceive = true;
            private int index;
            private uint tableId;

            public MerchantFleetAward(int index)
            {
                this.index = index;
            }

            public void BindObject(Transform trans)
            {
                tradeLevel = trans.Find("Text_Level").GetComponent<Text>();
                propItem = trans.Find("Scroll/Grid/PropItem").gameObject;
                function = trans.Find("Text_Function").GetComponent<Text>();
                btn = trans.Find("Btn_01_Small").GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btnImg = trans.Find("Btn_01_Small/Image").GetComponent<Image>();
                btnText = trans.Find("Btn_01_Small/Text_01").GetComponent<Text>();
                btnRedPoint= trans.Find("Btn_01_Small/Image_Dot").gameObject;
                goReceive= trans.Find("Image_Text").gameObject;
                btn.onClick.AddListener(OnButtonClicked);
            }
            public void InitData()
            {
                if (Sys_MerchantFleet.Instance.MerchantGradeAwardTake.Count<index)
                {
                    DebugUtil.Log(ELogType.eNone, "MerchantGradeAwardTake Error");
                    return;
                }
                tableId = Sys_MerchantFleet.Instance.MerchantGradeAwardTake[index][0];
                data = CSVMerchantFleetLevel.Instance.GetConfData(tableId);
                tradeLevel.text = LanguageHelper.GetTextContent(2028603, data.id.ToString());
                if (data.functionLan!=0)
                {
                    function.text = LanguageHelper.GetTextContent(data.functionLan);
                    function.gameObject.SetActive(true);
                }
                else
                {
                    function.gameObject.SetActive(false);
                }
                if (data.levelReward!=0)
                {
                    var list = CSVDrop.Instance.GetDropItem(data.levelReward);
                    Sys_MerchantFleet.Instance.InitPropItem(propItem, list, EUIID.UI_MerchantFleet_GradeAward, false);
                }
                //按钮
                cantReceive= (Sys_MerchantFleet.Instance.MerchantGradeAwardTake[index][1] == 0);
                canReceive = (Sys_MerchantFleet.Instance.MerchantGradeAwardTake[index][1] == 1);
                hasReceive = (Sys_MerchantFleet.Instance.MerchantGradeAwardTake[index][1] == 2);
                btnRedPoint.SetActive(canReceive);
                if (hasReceive)
                {
                    btnText.text = LanguageHelper.GetTextContent(2028645);
                }
                else
                {
                    btnText.text = LanguageHelper.GetTextContent(2028644);
                }
                btnText.gameObject.SetActive(!hasReceive);
                goReceive.SetActive(hasReceive);
                ImageHelper.SetImageGray(btnImg, hasReceive|| cantReceive);
            }
            private void OnButtonClicked()
            {
                if (hasReceive) return;
                if (canReceive)
                {
                    Sys_MerchantFleet.Instance.OnMerchantTakeAwardReq(tableId);
                    return;
                }
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2028657));//不可领取
              
            }
        }
    }

}