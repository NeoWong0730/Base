using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Framework;
using Logic.Core;
using Packet;
using UnityEngine.Playables;

namespace Logic
{
    public class UI_SpecialCardPresent : UIBase
    {
        private Button btnClose;
        private InputField inputText;
        private Button btnSeek;
        private Text txtTips;

        private InfinityGrid infinityDesc;

        private uint cardId;
        private string InputkeyWord;

        private List<Sys_Society.RoleInfo> listFirends = new List<Sys_Society.RoleInfo>();
        #region 系统函数
        protected override void OnOpen(object arg)
        {
            if (arg != null && arg.GetType() == typeof(uint))
            {
                cardId = (uint)arg;
            }
        }
        protected override void OnLoaded()
        {
            Parse();
        }
        protected override void OnShow()
        {
            Sys_Society.Instance.ReqGetSocialRoleInfo(false);
            Sys_OperationalActivity.Instance.ReqRefreshSpecialCardInfo(cardId);
            UpdateView();
        }
        protected override void OnHide()
        {
        }
        protected override void OnDestroy()
        {

        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Society.Instance.eventEmitter.Handle(Sys_Society.EEvents.OnRoleInfoUpdate, OnRoleInfoUpdate, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateSpecialCardData, OnUpdateSpecialCardData, toRegister);
        }
        #endregion

        #region func
        private void Parse()
        {
            btnClose = transform.Find("Animator/View_Bg/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);
            inputText = transform.Find("Animator/Content/InputField_Describe").GetComponent<InputField>();
            btnSeek = transform.Find("Animator/Content/InputField_Describe/Button_Delete").GetComponent<Button>();
            btnSeek.onClick.AddListener(OnBtnSeekClick);
            txtTips = transform.Find("Animator/Content/Text_tips").GetComponent<Text>();

            infinityDesc = transform.Find("Animator/Content/ScrollView").gameObject.GetNeedComponent<InfinityGrid>();
            infinityDesc.onCreateCell += OnCreateCell;
            infinityDesc.onCellChange += OnCellChange;
        }
        private void UpdateView(bool showHint = false)
        {
            CSVMonthCard.Data cardData = CSVMonthCard.Instance.GetConfData(cardId);
            if (cardData != null)
            {
                uint residueNum = Sys_OperationalActivity.Instance.GetSpecialCardPresentNum(cardId);//剩余次数
                txtTips.text = LanguageHelper.GetTextContent(12259, LanguageHelper.GetTextContent(cardData.Pirviege_Title), residueNum.ToString());//剩余赠送{0}月卡次数:{1}

                listFirends = Sys_OperationalActivity.Instance.GetSpecialCardPresentFriendList(InputkeyWord);
                if(showHint && listFirends.Count <= 0)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10703));
                }
                infinityDesc.CellCount = listFirends.Count;
                infinityDesc.ForceRefreshActiveCell();
            }
        }
        #endregion

        #region event
        private void OnBtnCloseClick()
        {
            this.CloseSelf();
        }
        private void OnBtnSeekClick()
        {
            InputkeyWord = inputText.text;
            UpdateView(true);
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_PresentCell mCell = new UI_PresentCell();
            mCell.Init(cell.mRootTransform.transform);
            cell.BindUserData(mCell);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_PresentCell mCell = cell.mUserData as UI_PresentCell;
            mCell.UpdateCellView(listFirends[index], cardId);
        }
        private void OnRoleInfoUpdate()
        {
            UpdateView();
        }
        private void OnUpdateSpecialCardData()
        {
            UpdateView();
        }
        #endregion

        public class UI_PresentCell
        {
            private Transform transform;

            private Image imgIcon;
            private Image imgIconFrame;
            private Text txtName;
            private Image imgJob;//职业标志
            private Text txtLevel;
            private Text txtJob;
            private Button btnPresent;

            private uint cardId;
            Sys_Society.RoleInfo roleInfo;
            public void Init(Transform trans)
            {
                transform = trans;
                imgIcon = transform.Find("Image_BG/Head").GetComponent<Image>();
                imgIconFrame = transform.Find("Image_BG/Head/Image_Before_Frame").GetComponent<Image>();
                txtName = transform.Find("Text_Name").GetComponent<Text>();
                imgJob = transform.Find("Image_Prop").GetComponent<Image>();
                txtLevel = transform.Find("Text_Number").GetComponent<Text>();
                txtJob = transform.Find("Text_Profession").GetComponent<Text>();
                btnPresent = transform.Find("Button").GetComponent<Button>();
                btnPresent.onClick.AddListener(OnBtnPresentClick);
            }

            public void UpdateCellView(Sys_Society.RoleInfo _roleInfo, uint id)
            {
                roleInfo = _roleInfo;
                cardId = id;

                txtName.text = roleInfo.roleName;
                ImageHelper.SetIcon(imgIcon, Sys_Head.Instance.GetHeadImageId(roleInfo.heroID, roleInfo.iconId));
                ImageHelper.SetIcon(imgIconFrame, CSVHeadframe.Instance.GetConfData(roleInfo.iconFrameId).HeadframeIcon, true);
                ImageHelper.SetIcon(imgJob, OccupationHelper.GetCareerLogoIcon(roleInfo.occ));
                txtLevel.text = LanguageHelper.GetTextContent(2011127, roleInfo.level.ToString());
                txtJob.text = LanguageHelper.GetTextContent(CSVCareer.Instance.GetConfData(roleInfo.occ).name);
            }

            private void OnBtnPresentClick()
            {
                uint num = Sys_OperationalActivity.Instance.GetSpecialCardPresentNum(cardId);
                if (num > 0)
                {
                    //赠送二次确认
                    CSVMonthCard.Data cardData = CSVMonthCard.Instance.GetConfData(cardId);
                    if (cardData != null && roleInfo.roleID > 0)
                    {
                        PromptBoxParameter.Instance.Clear();
                        PromptBoxParameter.Instance.SetConfirm(true, () =>
                        {
                            Sys_OperationalActivity.Instance.ReportSpecialCardClickEventHitPoint("GotoGiftCharge:" + cardData.Present_Change_Id + "_TargetId_" + roleInfo.roleID);
                            Sys_OperationalActivity.Instance.lastSpecialCardPresentTargetName = roleInfo.roleName;
                            Sys_Charge.Instance.OnChargeReq(cardData.Present_Change_Id, 0, roleInfo.roleID, EUIID.UI_SpecialCardPresent);
                        });
                        PromptBoxParameter.Instance.SetCancel(true, null);
                        PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(12256, LanguageHelper.GetTextContent(cardData.Pirviege_Title), roleInfo.roleName);//是否赠送{0},给您的好友{1}？
                        UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
                    }
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12258));//您当前赠送特权卡剩余次数不足
                }
            }
        }
    }
}
