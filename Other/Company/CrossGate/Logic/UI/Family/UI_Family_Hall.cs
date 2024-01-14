using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine;
using UnityEngine.UI;
using Lib.Core;
using Packet;
using System;

namespace Logic
{
    /// <summary> 家族大厅子界面 </summary>
    public class UI_Family_Hall : UIComponent
    {
        #region 界面组件
        /// <summary> 家族名称 </summary>
        private Text text_Name;
        /// <summary> 家族等级 </summary>
        private Text text_Lv;
        /// <summary> 家族职位 </summary>
        private Text text_Status;
        /// <summary> 家族宣言 </summary>
        private Text text_Declaration;
        /// <summary> 家族族长 </summary>
        private Text text_Patriarch;
        /// <summary> 家族编号 </summary>
        private Text text_ID;
        /// <summary> 家族人数 </summary>
        private Text text_Member;
        /// <summary> 家族状态 </summary>
        private Text text_State;
        /// <summary> 家族维护费用 </summary>
        private Text text_FamilyCost;
        /// <summary> 家族分红 </summary>
        private Text text_FamilyBonus;
        /// <summary> 家族资金 </summary>
        private Text text_FamilyFund;
        /// <summary> 显示我的消息 </summary>
        private Toggle toggle_ShowMyMessage;
        /// <summary> 签到 </summary>
        private Button button_Sign;
        /// <summary> 分红 </summary>
        private Button button_Bonus;
        /// <summary> 红包 </summary>
        //private Button button_RedPacket;
        /// <summary> 改名按钮 </summary>
        private Button button_ModifyName;
        /// <summary> 改宣言按钮 </summary>
        private Button button_ModifyDeclaration;
        /// <summary> 群发送按钮 </summary>
        private Button button_GroupSending;
        /// <summary> 家族商店按钮 </summary>
        private Button button_OpenFamilyMall;
        /// <summary> 家族商店按钮红点按钮 </summary>
        private GameObject go_familyMallRedPoint;
        /// <summary> 提示界面 </summary>
        private GameObject go_TipView;
        /// <summary> 无限滚动 </summary>
        private ScrollGridVertical scrollGridVertical;
        #endregion
        #region 数据定义
        /// <summary> 家族数据 </summary>
        private GuildDetailInfo guildDetailInfo
        {
            get;
            set;
        }
        /// <summary> 家族新闻 </summary>
        private List<GuildDetailInfo.Types.GuildNews> guildNews = new List<GuildDetailInfo.Types.GuildNews>();
        #endregion
        #region 系统函数        
        protected override void Loaded()
        {
            OnParseComponent();
            DefaultView();
        }
        public override void Show()
        {
            base.Show();
            SetData();
            RefreshView();
        }
        public override void Hide()
        {
            base.Hide();
        }
        protected override void Update()
        {
        }
        protected override void Refresh()
        {
            SetMallRedPoint();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        #endregion
        #region 初始化
        /// <summary>
        /// 检测组件
        /// </summary>
        private void OnParseComponent()
        {
            text_Name = transform.Find("View_Left/Image_Name/Text_Name").GetComponent<Text>();
            text_Lv = transform.Find("View_Left/Image_Name/Text_Level").GetComponent<Text>();
            text_Status = transform.Find("View_Left/Image_Name/Text_Position").GetComponent<Text>();
            text_Declaration = transform.Find("View_Left/Text_Declaration/Scroll_View01/Viewport/Text").GetComponent<Text>();
            text_Patriarch = transform.Find("View_Left/Text_Head/Text").GetComponent<Text>();
            text_ID = transform.Find("View_Left/Text_Number/Text").GetComponent<Text>();
            text_Member = transform.Find("View_Left/Text_Member/Text").GetComponent<Text>();
            text_State = transform.Find("View_Left/Text_State/Text").GetComponent<Text>();
            text_FamilyCost = transform.Find("View_Left/Image_Cost/Text_Cost").GetComponent<Text>();
            text_FamilyBonus = transform.Find("View_Left/Image_Bonus/Text_Cost").GetComponent<Text>();
            text_FamilyFund = transform.Find("View_Left/Image_Coin/Text_Cost").GetComponent<Text>();
            toggle_ShowMyMessage = transform.Find("View_Right/Toggle").GetComponent<Toggle>();
            button_Sign = transform.Find("View_Left/Sign/Button_Sign").GetComponent<Button>();
            button_Bonus = transform.Find("View_Left/Bonus/Button_Bonus").GetComponent<Button>();
            //button_RedPacket = transform.Find("View_Left/Bonus/Button_RedPacket").GetComponent<Button>();
            button_ModifyName = transform.Find("View_Left/Image_Name/Button").GetComponent<Button>();
            button_ModifyDeclaration = transform.Find("View_Left/Text_Declaration/Button").GetComponent<Button>();
            button_GroupSending = transform.Find("View_Left/Button_News").GetComponent<Button>();
            go_TipView = transform.Find("View_rule").gameObject;

            scrollGridVertical = transform.Find("View_Right/ScrollView_News").GetComponent<ScrollGridVertical>();
            scrollGridVertical.AddCellListener(OnCellUpdateCallback);

            button_Sign.onClick.AddListener(OnClick_Sign);
            button_Bonus.onClick.AddListener(OnClick_ReceiveBonus);
            //button_RedPacket.onClick.AddListener(()=> UIManager.OpenUI(EUIID.UI_Family_PacketRecord));
            button_ModifyName.onClick.AddListener(OnClick_ModifyName);
            button_ModifyDeclaration.onClick.AddListener(OnClick_ModifyDeclaration);
            button_GroupSending.onClick.AddListener(OnClick_GroupSending);
            toggle_ShowMyMessage.onValueChanged.AddListener(OnChanged_ShowMyMessage);
            transform.Find("Button_Merge").GetComponent<Button>().onClick.AddListener(OnClick_MergeFamily);
            transform.Find("Button_Go").GetComponent<Button>().onClick.AddListener(OnClick_GotoFamily);
            transform.Find("View_Left/Button_Tips").GetComponent<Button>().onClick.AddListener(OnClick_OpenFamilyRuleTips);
            transform.Find("View_rule/close").GetComponent<Button>().onClick.AddListener(OnClick_CloseFamilyRuleTips);
            transform.Find("Button_Shop").GetComponent<Button>().onClick.AddListener(OnClick_OpenFamilyMall);
            transform.Find("View_Left/Button_Fund").GetComponent<Button>().onClick.AddListener(OnClick_OpenFamilyFunds);
            go_familyMallRedPoint = transform.Find("Button_Shop/Image_Dot").gameObject;
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateFamilyName, OnUpdateFamilyName, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateFamilyDeclaration, OnUpdateFamilyDeclaration, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UdateSignState, OnUdateButtonState, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UdateBonusState, OnUdateButtonState, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.ChangeMyNews, OnChangeMyNews, toRegister);
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        private void SetData()
        {
            guildDetailInfo = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info;
            if (null == guildDetailInfo) return;
            bool displayMyNews = Sys_Family.Instance.familyData.familyPlayerInfo.cmdGuildSceneInfoNtf.DisplayMyNews;
            guildNews.Clear();
            for (int i = 0, count = guildDetailInfo.ListNews.Count; i < count; i++)
            {
                var info = guildDetailInfo.ListNews[i];
                guildNews.Add(info);
            }
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 默认界面
        /// </summary>
        private void DefaultView()
        {
            text_Name.text = string.Empty;
            text_Lv.text = string.Empty;
            text_Declaration.text = string.Empty;
            text_Patriarch.text = string.Empty;
            text_ID.text = string.Empty;
            text_Member.text = string.Empty;
            text_State.text = string.Empty;
            text_FamilyCost.text = string.Empty;
            text_FamilyBonus.text = string.Empty;
            text_FamilyFund.text = string.Empty;
            text_Status.text = string.Empty;
            button_ModifyName.gameObject.SetActive(false);
            button_ModifyDeclaration.gameObject.SetActive(false);
            button_GroupSending.gameObject.SetActive(false);
        }
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            SetMallRedPoint();
            SetFamilyInfoView();
            SetButtonState();
            SetMessageView();
        }
        /// <summary>
        /// 设置家族信息界面
        /// </summary>
        private void SetFamilyInfoView()
        {
            if (null == guildDetailInfo) return;
            text_Name.text = guildDetailInfo.GName.ToStringUtf8();
            uint lv = guildDetailInfo.AllBuildings.Count > 0 ? guildDetailInfo.AllBuildings[0].Lvl : 1;
            text_Lv.text = LanguageHelper.GetTextContent(10087, lv.ToString());
            text_Declaration.text = guildDetailInfo.Notice.ToStringUtf8();
            text_Patriarch.text = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.LeaderName.ToStringUtf8();
            text_ID.text = (guildDetailInfo.GuildId % 100000000UL).ToString();
            text_Member.text = string.Format("{0}/{1}",
                Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.MemberCount,
                Sys_Family.Instance.familyData.familyBuildInfo.membershipCap.ToString());
            uint lanId = 0;
            switch (guildDetailInfo.GuildStatus)
            {
                case 1: { lanId = 10088; } break;
                case 2: { lanId = 10089; } break;
                case 3: { lanId = 10090; } break;
            }
            text_State.text = LanguageHelper.GetTextContent(lanId);
            text_FamilyCost.text = LanguageHelper.GetTextContent(10602, Sys_Bag.Instance.GetValueFormat(Sys_Family.Instance.familyData.familyBuildInfo.maintenanceCost));
            text_FamilyBonus.text = string.Format("{0}/{1}", Sys_Bag.Instance.GetValueFormat(guildDetailInfo.RewardBonus), Sys_Bag.Instance.GetValueFormat(Sys_Family.Instance.familyData.familyBuildInfo.dividendCap));
            text_FamilyFund.text = string.Format("{0}/{1}", Sys_Bag.Instance.GetValueFormat(guildDetailInfo.GuildCoin), Sys_Bag.Instance.GetValueFormat(Sys_Family.Instance.familyData.familyBuildInfo.capitalCeiling));
            uint myPosition = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.MyPosition;
            uint BranchId = myPosition / 10000;
            uint Position = myPosition % 10000;
            CSVFamilyPostAuthority.Data cSVFamilyPostAuthorityData = CSVFamilyPostAuthority.Instance.GetConfData(Position);
            if (null == cSVFamilyPostAuthorityData)
            {
                text_Status.text = string.Empty;
            }
            else if (BranchId > 0)
            {
                var BranchInfo = Sys_Family.Instance.familyData.CheckBranchInfo(BranchId);
                string BranchName = BranchInfo == null ? string.Empty : BranchInfo.Name.ToStringUtf8();
                text_Status.text = string.Concat(BranchName, LanguageHelper.GetTextContent(cSVFamilyPostAuthorityData.PostName));
            }
            else
            {
                text_Status.text = LanguageHelper.GetTextContent(cSVFamilyPostAuthorityData.PostName);
            }

            bool isModifyName = Sys_Family.Instance.familyData.GetPostAuthority((Sys_Family.FamilyData.EFamilyStatus)Position, Sys_Family.FamilyData.EFamilyAuthority.ModifyName);
            bool isModifyDeclaration = Sys_Family.Instance.familyData.GetPostAuthority((Sys_Family.FamilyData.EFamilyStatus)Position, Sys_Family.FamilyData.EFamilyAuthority.ModifyDeclaration);
            bool isGroupMessage = Sys_Family.Instance.familyData.GetPostAuthority((Sys_Family.FamilyData.EFamilyStatus)Position, Sys_Family.FamilyData.EFamilyAuthority.GroupMessage);
            button_ModifyName.gameObject.SetActive(isModifyName);
            button_ModifyDeclaration.gameObject.SetActive(isModifyDeclaration);
            button_GroupSending.gameObject.SetActive(isGroupMessage);
        }
        /// <summary>
        /// 设置按钮状态
        /// </summary>
        private void SetButtonState()
        {
            if (null == Sys_Family.Instance.familyData.familyPlayerInfo.cmdGuildSceneInfoNtf) return;
            bool bSignIn = Sys_Family.Instance.familyData.familyPlayerInfo.cmdGuildSceneInfoNtf.BSignIn;
            button_Sign.interactable = !bSignIn;
            button_Sign.transform.Find("Image").gameObject.SetActive(bSignIn);
            button_Bonus.interactable = true;
            button_Bonus.transform.Find("Image").gameObject.SetActive(false);
        }
        /// <summary>
        /// 设置消息界面
        /// </summary>
        private void SetMessageView()
        {
            if (null == Sys_Family.Instance.familyData.familyPlayerInfo.cmdGuildSceneInfoNtf) return;
            bool displayMyNews = Sys_Family.Instance.familyData.familyPlayerInfo.cmdGuildSceneInfoNtf.DisplayMyNews;
            toggle_ShowMyMessage.SetIsOnWithoutNotify(!displayMyNews);
            scrollGridVertical.SetCellCount(guildNews.Count);
            scrollGridVertical.MoveToTopOrBotton(false);
        }

        /// <summary>
        /// 商店红点
        /// </summary>
        public void SetMallRedPoint()
        {
            go_familyMallRedPoint.SetActive(Sys_Mall.Instance.IsMallRed(901));
        }
        /// <summary>
        /// 设置消息模版
        /// </summary>
        /// <param name="tr"></param>
        private void SetMessageItem(Transform tr, GuildDetailInfo.Types.GuildNews guildNews)
        {
            /// <summary> 头像 </summary>
            Image image_Head = tr.Find("Image_Icon").GetComponent<Image>();
            //CSVCharacter.Data heroData = CSVCharacter.Instance.GetConfData(guildNews.HeadIcon);
            ImageHelper.SetIcon(image_Head, 2801);
            /// <summary> 信息 </summary>
            Text text_Message = tr.Find("Text_News").GetComponent<Text>();
            switch (guildNews.NewsType)
            {
                case 1://会长转让
                    text_Message.text = LanguageHelper.GetTextContent(10546, guildNews.Role1.ToStringUtf8(), guildNews.Role2.ToStringUtf8());
                    break;
                case 2://系统不上踢出
                    text_Message.text = LanguageHelper.GetTextContent(10545, guildNews.Role1.ToStringUtf8());
                    break;
                case 3://审批进入
                    text_Message.text = LanguageHelper.GetTextContent(10544, guildNews.Role2.ToStringUtf8());
                    break;
                case 4://踢人
                    text_Message.text = LanguageHelper.GetTextContent(10548, guildNews.Role2.ToStringUtf8(), guildNews.Reason.ToStringUtf8(), guildNews.Role1.ToStringUtf8());
                    break;
                case 5://离开
                    text_Message.text = LanguageHelper.GetTextContent(10545, guildNews.Role1.ToStringUtf8());
                    break;
                case 6://拒绝进入
                    text_Message.text = LanguageHelper.GetTextContent(10634, guildNews.Role1.ToStringUtf8(), guildNews.Role2.ToStringUtf8());
                    break;
                case 7://自动加入公会
                    text_Message.text = LanguageHelper.GetTextContent(10635, guildNews.Role1.ToStringUtf8());
                    break;
                case 8://邀请加入
                    text_Message.text = LanguageHelper.GetTextContent(11631, guildNews.Role1.ToStringUtf8(), guildNews.Role2.ToStringUtf8());
                    break;
                case 9://玩家改名
                    text_Message.text = LanguageHelper.GetTextContent(12283, guildNews.Role1.ToStringUtf8(), guildNews.Role2.ToStringUtf8());
                    break;
                case 10://玩家转职
                    CSVCareer.Data oldData = CSVCareer.Instance.GetConfData(guildNews.Career1);
                    CSVCareer.Data newData = CSVCareer.Instance.GetConfData(guildNews.Career2);
                    text_Message.text = LanguageHelper.GetTextContent(5182, guildNews.Role1.ToStringUtf8(), LanguageHelper.GetTextContent(oldData.name),
                        LanguageHelper.GetTextContent(newData.name));
                    break;
                case 11://玩家职位变化
                    //CSVFamilyPostAuthority.Data oldPostAuthorityData = CSVFamilyPostAuthority.Instance.GetConfData(guildNews.OldPosition);
                    CSVFamilyPostAuthority.Data newPostAuthorityData = CSVFamilyPostAuthority.Instance.GetConfData(guildNews.NewPosition % 10000);
                    text_Message.text = LanguageHelper.GetTextContent(15176, guildNews.Role1.ToStringUtf8(), guildNews.Role2.ToStringUtf8(),
                        LanguageHelper.GetTextContent(newPostAuthorityData.PostName));
                    break;
                default:
                    text_Message.text = string.Empty;
                    break;
            }
            /// <summary> 时间 </summary>
            Text text_Time = tr.Find("Text_Time").GetComponent<Text>();
            DateTime DT = Sys_Time.ConvertToLocalTime(guildNews.HappenTime);
            text_Time.text = string.Format("{0}/{1}/{2}", DT.Year, DT.Month, DT.Day);
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 修改家族名称
        /// </summary>
        private void OnClick_ModifyName()
        {
            UIManager.HitButton(EUIID.UI_Family, "OnClick_ModifyName");
            UIManager.OpenUI(EUIID.UI_Family_ModifyName);
        }
        /// <summary>
        /// 修改家族宣言
        /// </summary>
        private void OnClick_ModifyDeclaration()
        {
            UIManager.HitButton(EUIID.UI_Family, "OnClick_ModifyDeclaration");
            UIManager.OpenUI(EUIID.UI_Family_ModifyDeclaration);
        }
        /// <summary>
        /// 签到
        /// </summary>
        private void OnClick_Sign()
        {
            UIManager.HitButton(EUIID.UI_Family, "OnClick_Sign");
            Sys_Family.Instance.SendGuildSigninReq();
        }
        /// <summary>
        /// 领取分红
        /// </summary>
        private void OnClick_ReceiveBonus()
        {
            UIManager.HitButton(EUIID.UI_Family, "OnClick_ReceiveBonus");
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10624));
            Sys_Family.Instance.SendGuildGetBonusReq();
        }
        /// <summary>
        /// 群发消息
        /// </summary>
        private void OnClick_GroupSending()
        {
            UIManager.HitButton(EUIID.UI_Family, "OnClick_GroupSending");
            UIManager.OpenUI(EUIID.UI_Family_GroupSending);
        }
        /// <summary>
        /// 家族合并
        /// </summary>
        private void OnClick_MergeFamily()
        {
            UIManager.HitButton(EUIID.UI_Family, "OnClick_MergeFamily");
            UIManager.OpenUI(EUIID.UI_Family_Merge);
        }
        /// <summary>
        /// 前往家族
        /// </summary>
        private void OnClick_GotoFamily()
        {
            UIManager.HitButton(EUIID.UI_Family, "OnClick_GotoFamily");
            CSVParam.Data cSVParamData = CSVParam.Instance.GetConfData(772);
            if (null == cSVParamData)
                return;

            uint npcId = uint.Parse(cSVParamData.str_value);
            ActionCtrl.Instance.MoveToTargetNPC(npcId);
            UIManager.CloseUI(EUIID.UI_Family);
        }
        /// <summary>
        /// 打开家族规则提示
        /// </summary>
        private void OnClick_OpenFamilyRuleTips()
        {
            UIManager.HitButton(EUIID.UI_Family, "OnClick_OpenFamilyRuleTips");
            go_TipView.SetActive(true);
        }

        /// <summary>
        /// 打开家族商店
        /// </summary>
        private void OnClick_OpenFamilyMall()
        {
            UIManager.HitButton(EUIID.UI_Family, "OnClick_OpenFamilyMall");
            MallPrama mall = new MallPrama();
            mall.mallId = 901;
            mall.shopId = 9001;
            UIManager.OpenUI(EUIID.UI_Mall, false, mall);
        }

        private void OnClick_OpenFamilyFunds()
        {
            bool ed = Sys_Family.Instance.familyData.GetMyPostAuthority(Sys_Family.FamilyData.EFamilyAuthority.FamilyBonus);
            if(ed)
            {
                UIManager.OpenUI(EUIID.UI_Family_Funds);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(15170));
            }
        }
        /// <summary>
        /// 关闭家族规则提示
        /// </summary>
        private void OnClick_CloseFamilyRuleTips()
        {
            go_TipView.SetActive(false);
        }
        /// <summary>
        /// 是否显示我的信息
        /// </summary>
        /// <param name="value"></param>
        private void OnChanged_ShowMyMessage(bool value)
        {
            UIManager.HitButton(EUIID.UI_Family, "OnChanged_ShowMyMessage:" + value.ToString());
            Sys_Family.Instance.SendGuildChangeMyNewsReq(!value);
        }
        /// <summary>
        /// 更新滚动子节点
        /// </summary>
        /// <param name="cell"></param>
        private void OnCellUpdateCallback(ScrollGridCell cell)
        {
            GuildDetailInfo.Types.GuildNews news = guildNews[cell.index];
            SetMessageItem(cell.gameObject.transform, news);
        }
        /// <summary>
        /// 更新家族名称
        /// </summary>
        private void OnUpdateFamilyName()
        {
            SetFamilyInfoView();
        }
        /// <summary>
        /// 更新家族描述
        /// </summary>
        private void OnUpdateFamilyDeclaration()
        {
            SetFamilyInfoView();
        }
        /// <summary>
        /// 更新按钮状态
        /// </summary>
        private void OnUdateButtonState()
        {
            SetButtonState();
        }
        /// <summary>
        /// 更新我的信息状态
        /// </summary>
        private void OnChangeMyNews()
        {
            SetData();
            RefreshView();
        }
        #endregion
        #region 提供功能
        #endregion
    }
}