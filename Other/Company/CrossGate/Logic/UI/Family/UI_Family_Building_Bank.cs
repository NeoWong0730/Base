using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using Lib.Core;
using Packet;
using static Packet.GuildDetailInfo.Types;
using System.Text;

namespace Logic
{
    /// <summary> 家族银行(金库) </summary>
    public class UI_Family_Building_Bank : UIBase
    {
        #region 界面组件
        /// <summary> 金库图标 </summary>
        private Image image_Icon;
        /// <summary> 金库图标 </summary>
        private Text text_Level;
        /// <summary> 金库进度条 </summary>
        private Slider slider_Coin;
        /// <summary> 金库进度条 </summary>
        private Text text_Coin;
        /// <summary> 个人贡献 </summary>
        private Text text_PersonalContribution;
        /// <summary> 个人贡献 图标 </summary>
        private Image image_PersonalContribution;
        /// <summary> 每日获得家族资金 </summary>
        private Text text_TodayFamilyCoin;
        /// <summary> 每日获得家族资金进度条 </summary>
        private Slider slider_TodayFamilyCoin;
        /// <summary> 奖励节点 </summary>
        private RectTransform rt_Award;
        /// <summary> 奖励模版 </summary>
        private Transform tr_AwardItem;
        /// <summary> 捐赠节点 </summary>
        private List<GameObject> list_Donate = new List<GameObject>();
        /// <summary> 领奖节点 </summary>
        private List<GameObject> list_Award = new List<GameObject>();
        /// <summary> 提示界面 </summary>
        private GameObject go_TipsView;
        /// <summary> 货币标题通用界面 </summary>
        private UI_CurrencyTitle ui_CurrencyTitle;
        /// <summary> 捐献滚动脚本 </summary>
        private ScrollRect sr_Donate;
        /// <summary> 捐献文本 </summary>
        private Text text_Donate;
        #endregion
        #region 数据定义
        /// <summary> 捐献记录 </summary>
        private List<DonateRecord> list_DonateRecord = new List<DonateRecord>();
        #endregion
        #region 系统函数
        protected override void OnInit()
        {
        }
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnDestroy()
        {
            ui_CurrencyTitle.Dispose();
        }
        protected override void OnOpen(object arg)
        {
            Sys_Family.Instance.OnUpdateGuildSceneInfo();
        }
        protected override void OnShow()
        {
            ui_CurrencyTitle.InitUi();
            SetData();
            RefreshView();
        }
        protected override void OnHide()
        {

        }
        protected override void OnUpdate()
        {

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
            image_Icon = transform.Find("Animator/View_Top/Image/Image_Icon").GetComponent<Image>();
            text_Level = transform.Find("Animator/View_Top/Text_Grade/Text").GetComponent<Text>();
            slider_Coin = transform.Find("Animator/View_Top/Image_Percent").GetComponent<Slider>();
            text_Coin = transform.Find("Animator/View_Top/Text_Family/Text").GetComponent<Text>();
            text_PersonalContribution = transform.Find("Animator/View_Top/Text_Contribution/Text_Cost").GetComponent<Text>();
            image_PersonalContribution = transform.Find("Animator/View_Top/Text_Contribution/Text_Cost/Image_Coin").GetComponent<Image>();
            text_TodayFamilyCoin = transform.Find("Animator/View_Down/Text_Add").GetComponent<Text>();
            slider_TodayFamilyCoin = transform.Find("Animator/View_Down/Image_Percent").GetComponent<Slider>();
            go_TipsView = transform.Find("Animator/View_Tips").gameObject;

            list_Donate.Add(transform.Find("Animator/View_Middle/DonateGold").gameObject);
            list_Donate.Add(transform.Find("Animator/View_Middle/DonateDiamond").gameObject);

            rt_Award = transform.Find("Animator/View_Down/Viewport").transform as RectTransform;
            tr_AwardItem = transform.Find("Animator/View_Down/Award");
            tr_AwardItem.gameObject.SetActive(false);

            ui_CurrencyTitle = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            sr_Donate = transform.Find("Animator/View_Middle/Image_Title/Scroll_View").GetComponent<ScrollRect>();
            text_Donate = transform.Find("Animator/View_Middle/Image_Title/Scroll_View/Viewport/Text").GetComponent<Text>();

            transform.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
            transform.Find("Animator/View_Down/Button_Tips").GetComponent<Button>().onClick.AddListener(OnClick_OpenTips);
            transform.Find("Animator/View_Tips/Close").GetComponent<Button>().onClick.AddListener(OnClick_CloseTips);

            for (int i = 0, count = list_Donate.Count; i < count; i++)
            {
                var item = list_Donate[i];
                GameObject go = item.gameObject;
                item.transform.Find("Btn_01").GetComponent<Button>().onClick.AddListener(() => { OnClick_Donate(go); });
            }
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, OnItemChange, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateDonateData, OnUpdateDonateData, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateDonateRewardData, OnUpdateDonateRewardData, toRegister);
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        private void SetData()
        {
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            list_DonateRecord.Clear();
            var donateRecords = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.DonateRecords;
            if (null != donateRecords)
            {
                list_DonateRecord.AddRange(donateRecords);
            }
        }
        /// <summary>
        /// 创建足够的列表
        /// </summary>
        /// <param name="node"></param>
        /// <param name="child"></param>
        /// <param name="number"></param>
        private void CreateItemList(Transform node, Transform child, int number)
        {
            while (node.childCount < number)
            {
                Transform tr = GameObject.Instantiate(child, node);
                RectTransform rt = tr.GetComponent<RectTransform>();
                rt.anchoredPosition3D = Vector3.zero;
                tr.Find("Image").GetComponent<Button>().onClick.AddListener(() => { OnClick_Receive(tr.gameObject); });
                list_Award.Add(tr.gameObject);
            }
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            var donate = Sys_Family.Instance.familyData.familyPlayerInfo.cmdGuildSceneInfoNtf.Donate;
            var indexs = Sys_Family.Instance.familyData.familyPlayerInfo.cmdGuildSceneInfoNtf.DonateRewardIndex;
            List<CmdGuildSceneInfoNtf.Types.DonateInfo> list_DonateInfo = new List<CmdGuildSceneInfoNtf.Types.DonateInfo>(donate);
            for (int i = 0; i < list_Donate.Count; i++)
            {
                uint id = (uint)i + 1;
                uint count = 0;
                var DonateInfo = list_DonateInfo.Find(x => x.DonateId == id);
                if (null != DonateInfo)
                {
                    count = DonateInfo.Count;
                }
                SetDonateItem(list_Donate[i].transform, id, count);
            }
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            var allBuilds = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllBuildings;
            var building = allBuilds.Count > (int)Sys_Family.FamilyData.EBuildingIndex.Bank ? allBuilds[(int)Sys_Family.FamilyData.EBuildingIndex.Bank] : null;
            if (null == building) return;
            uint Id = (int)Sys_Family.FamilyData.EBuildingIndex.Bank * 100 + building.Lvl;
            CSVFamilyArchitecture.Data cSVFamilyArchitectureData = CSVFamilyArchitecture.Instance.GetConfData(Id);
            if (null == cSVFamilyArchitectureData) return;

            ImageHelper.SetIcon(image_Icon, cSVFamilyArchitectureData.iconId);
            text_Level.text = LanguageHelper.GetTextContent(10087, building.Lvl.ToString());

            uint curCoin = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.GuildCoin;
            uint maxCoin = Sys_Family.Instance.familyData.familyBuildInfo.capitalCeiling;
            uint todayDonateCoin = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.TodayDonateCoin;
            uint maxDonateCoin = Sys_Family.Instance.familyData.familyBuildInfo.dailyDonationLimit;
            text_Coin.text = string.Format("{0}/{1}", Sys_Bag.Instance.GetValueFormat(curCoin), Sys_Bag.Instance.GetValueFormat(maxCoin));
            slider_Coin.value = maxCoin == 0 ? 0 : (float)curCoin / (float)maxCoin;
            text_PersonalContribution.text = Sys_Bag.Instance.GetItemCount((uint)ECurrencyType.GuildCurrency).ToString();
            CSVItem.Data PersonalContributionItem = CSVItem.Instance.GetConfData((uint)ECurrencyType.GuildCurrency);
            if (null != PersonalContributionItem)
                ImageHelper.SetIcon(image_PersonalContribution, PersonalContributionItem.small_icon_id);
            text_TodayFamilyCoin.text = Sys_Bag.Instance.GetValueFormat(todayDonateCoin);

            var collectionlist = Sys_Family.Instance.familyData.familyBuildInfo.rewardCollectionTrigger;
            var dailyRewardlist = Sys_Family.Instance.familyData.familyBuildInfo.dailyReward;
            //重新定节点数量
            List<uint> points = new List<uint>();
            points.Add(0);
            if (null != collectionlist) points.AddRange(collectionlist);
            //实际奖励的节点
            List<uint> rewards = new List<uint>();
            rewards.Add(0);
            if (null != dailyRewardlist) rewards.AddRange(dailyRewardlist);
            //points.Add(maxDonateCoin);
            //计算当前领奖进度
            float ratio = 1F / (float)(points.Count - 1);
            float value = 0;
            for (int i = 0; i < points.Count - 1; i++)
            {
                float curPoint = (float)points[i];
                float nextPoint = (float)points[i + 1];

                float D_value = nextPoint - curPoint;
                if (nextPoint >= todayDonateCoin)
                {
                    value += ratio * (1f - (nextPoint - todayDonateCoin) / D_value);
                    break;
                }
                else
                {
                    value += ratio;
                }
            }
            slider_TodayFamilyCoin.value = value;
            CreateItemList(rt_Award, tr_AwardItem, points.Count);
            float weight = rt_Award.sizeDelta.x;

            for (int i = 0, count = list_Award.Count; i < count; i++)
            {
                GameObject go = list_Award[i];
                bool isShow = i < points.Count;
                go.SetActive(isShow);
                if (!isShow) continue;
                uint point = points[i];
                bool isShowText = true;      // i != points.Count - 1; //尾巴不显示文字
                bool isShowIcon = i != 0 && i < rewards.Count;
                int index = i - 1;
                SetAwardItemPoint(go.transform as RectTransform, weight, (float)i / (float)(points.Count - 1));
                SetAwardItem(go.transform, isShowText, isShowIcon, point, point <= todayDonateCoin, index < 0 ? false : indexs.Contains((uint)index));
            }

            StringBuilder stringBuilder = StringBuilderPool.GetTemporary();
            for (int i = 0; i < list_DonateRecord.Count; i++)
            {
                DonateRecord donateRecord = list_DonateRecord[i];
                CSVFamilyDonate.Data cSVFamilyDonateData = CSVFamilyDonate.Instance.GetConfData(donateRecord.ItemId);
                if (null != cSVFamilyDonateData)
                {
                    CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(cSVFamilyDonateData.DonationType);
                    if(null != cSVItemData)
                    {
                        uint AddCoin = donateRecord.GuildCoin > cSVFamilyDonateData.IncreaseFamilyFunding ? donateRecord.GuildCoin - cSVFamilyDonateData.IncreaseFamilyFunding : 0;
                        stringBuilder.Append(
                           LanguageHelper.GetTextContent(10067,
                           donateRecord.Name.ToStringUtf8(),
                           donateRecord.ItemCount.ToString(),
                           LanguageHelper.GetTextContent(cSVItemData.name_id),
                           donateRecord.GuildCoin.ToString(),
                           AddCoin.ToString()));
                        stringBuilder.Append("\n");
                    }
                }
            }
            text_Donate.text = stringBuilder.ToString();
            StringBuilderPool.ReleaseTemporary(stringBuilder);
            LayoutRebuilder.ForceRebuildLayoutImmediate(text_Donate.rectTransform);
            sr_Donate.verticalNormalizedPosition = 0f;
        }
        /// <summary>
        /// 设置捐献模版
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="id"></param>
        /// <param name="count"></param>
        private void SetDonateItem(Transform tr, uint donateId, uint count)
        {
            CSVFamilyDonate.Data cSVFamilyDonateData = CSVFamilyDonate.Instance.GetConfData(donateId);
            if (null == cSVFamilyDonateData) return;
            /// <summary> 捐赠次数 </summary>
            Text text_Number = tr.Find("Text_Number/Text").GetComponent<Text>();
            text_Number.text = string.Format("{0}/{1}", count, cSVFamilyDonateData.DonationNum.ToString());
            /// <summary> 增加个人贡献 </summary>
            Transform tr_PersonalContribution = tr.Find("Image1/Text_Add").transform;
            if(null != cSVFamilyDonateData.ItemAcquisition && cSVFamilyDonateData.ItemAcquisition.Count >= 2)
            {
                SetCostItem(tr_PersonalContribution, cSVFamilyDonateData.ItemAcquisition[0], cSVFamilyDonateData.ItemAcquisition[1]);
            }
            else
            {
                SetCostItem(tr_PersonalContribution, (uint)ECurrencyType.GuildCurrency, cSVFamilyDonateData.DonationReward);
            }
            
            /// <summary> 增加家族金币 </summary>
            Transform tr_FamilyCoin = tr.Find("Image2/Text_Add").transform;
            SetCostItem(tr_FamilyCoin, (uint)ECurrencyType.FamilyCoin, cSVFamilyDonateData.IncreaseFamilyFunding);
            /// <summary> 花费 </summary>
            Transform tr_Cost = tr.Find("Text_Cost").transform;
            /// <summary> 捐献 </summary>
            Button button_Donate = tr.Find("Btn_01").GetComponent<Button>();
            uint id = count + 1;
            CSVFamilyDonateTime.Data cSVFamilyDonateTimeData = CSVFamilyDonateTime.Instance.GetConfData(id);
            if (null == cSVFamilyDonateTimeData)
            {
                tr_Cost.gameObject.SetActive(false);
                button_Donate.gameObject.SetActive(false);
            }
            else
            {
                tr_Cost.gameObject.SetActive(true);
                button_Donate.gameObject.SetActive(true);
                switch (donateId)
                {
                    // 严格按照 id 表格列数
                    case 2:
                        SetCostItem(tr_Cost, cSVFamilyDonateData.DonationType, cSVFamilyDonateTimeData.DiamondsSingleCost, true);
                        break;
                    case 1:
                        SetCostItem(tr_Cost, cSVFamilyDonateData.DonationType, cSVFamilyDonateTimeData.GoldSingleCost, true);
                        break;
                }
            }
            GameObject go_RedPoint = tr.Find("Btn_01/Image_Dot").gameObject;
            go_RedPoint.SetActive(Sys_Family.Instance.IsRedPoint_Donate());
        }
        /// <summary>
        /// 设置货币模版
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="id"></param>
        /// <param name="number"></param>
        private void SetCostItem(Transform tr, uint id, uint number, bool isSetTextStyle = false)
        {
            Text text_Cost = tr.GetComponent<Text>();
            if (isSetTextStyle)
            {
                uint TextStyleId = Sys_Bag.Instance.GetItemCount(id) >= (long)number ? (uint)87 : 15;
                TextHelper.SetText(text_Cost,
                   number.ToString(),
                   LanguageHelper.GetTextStyle(TextStyleId));
            }
            else
            {
                text_Cost.text = number.ToString();
            }

            Image image_Cost = tr.Find("Image_Coin").GetComponent<Image>();
            CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(id);
            if (null != cSVItemData)
                ImageHelper.SetIcon(image_Cost, cSVItemData.small_icon_id);
        }
        /// <summary>
        /// 设置奖励坐标
        /// </summary>
        /// <param name="item"></param>
        /// <param name="weight"></param>
        /// <param name="value"></param>
        private void SetAwardItemPoint(RectTransform item, float weight, float value)
        {
            item.anchoredPosition = new Vector3(weight * value, item.anchoredPosition.y);
        }
        /// <summary>
        /// 设置奖励模版
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="isShowText"></param>
        /// <param name="isShowIcon"></param>
        /// <param name="point"></param>
        /// <param name="finish"></param>
        /// <param name="receive"></param>
        private void SetAwardItem(Transform tr, bool isShowText, bool isShowIcon, uint point, bool finish, bool receive)
        {
            Text text_point = tr.Find("Text").GetComponent<Text>();
            Image image_Icon = tr.Find("Image").GetComponent<Image>();
            Image image_Light = tr.Find("Image_Light").GetComponent<Image>();
            GameObject go_Received = tr.Find("Image_Received").gameObject;
            GameObject go_RedPoint = tr.Find("Image_Dot").gameObject;

            if (isShowIcon)
            {
                image_Icon.gameObject.SetActive(true);
                ImageHelper.SetImageGray(image_Icon, receive, true);
                image_Light.gameObject.SetActive(finish && !receive);
                go_Received.gameObject.SetActive(receive);
            }
            else
            {
                image_Icon.gameObject.SetActive(false);
                image_Light.gameObject.SetActive(false);
            }
            if (isShowText)
                text_point.text = point.ToString();
            else
                text_point.text = string.Empty;

            go_RedPoint.SetActive(isShowIcon && finish && !receive);
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            UIManager.HitButton(EUIID.UI_Family_Bank, "OnClick_Close");
            CloseSelf();
        }
        /// <summary>
        /// 捐献
        /// </summary>
        private void OnClick_Donate(GameObject go)
        {
            uint donateId = 0;
            switch (go.name)
            {
                case "DonateGold":
                    donateId = 1;
                    break;
                case "DonateDiamond":
                    donateId = 2;
                    break;
            }
            UIManager.HitButton(EUIID.UI_Family_Bank, "OnClick_Donate:" + donateId.ToString());
            if (Sys_Family.Instance.familyData.IsLimit_Donate(donateId))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10068));
            }
            else if (!Sys_Family.Instance.familyData.IsEnough_Donate(donateId))
            {
                //Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11007));
            }
            else
            {
                Sys_Family.Instance.SendGuildDonateReq(donateId);
            }
        }
        /// <summary>
        /// 领取奖励
        /// </summary>
        /// <param name="go"></param>
        private void OnClick_Receive(GameObject go)
        {
            int index = list_Award.IndexOf(go) - 1; //列表去头去尾
            if (index < 0) return;

            bool isFinish = Sys_Family.Instance.familyData.IsFinish_DonateReward(index);
            bool isReceived = Sys_Family.Instance.familyData.IsReceived_DonateReward(index);
            UIManager.HitButton(EUIID.UI_Family_Bank, "OnClick_Receive:" + index.ToString());
            if (isFinish && !isReceived)
            {
                Sys_Family.Instance.SendGuildGetDonateRewardReq((uint)index);
            }
            else
            {
                var dailyRewardlist = Sys_Family.Instance.familyData.familyBuildInfo.dailyReward;
                UIManager.OpenUI(EUIID.UI_Family_BankAward, false, dailyRewardlist[index]);
            }
        }
        /// <summary>
        /// 打开提示
        /// </summary>
        private void OnClick_OpenTips()
        {
            UIManager.HitButton(EUIID.UI_Family_Bank, "OnClick_OpenTips");
            go_TipsView.SetActive(true);
        }
        /// <summary>
        /// 关闭提示
        /// </summary>
        private void OnClick_CloseTips()
        {
            go_TipsView.SetActive(false);
        }
        /// <summary>
        /// 更新捐献数据
        /// </summary>
        private void OnUpdateDonateData()
        {
            SetData();
            RefreshView();
        }
        /// <summary>
        /// 兑换界面兑换货币完之后
        /// </summary>
        private void OnItemChange(int changeType, int curBoxId)
        {
            OnUpdateDonateData();
        }
        /// <summary>
        /// 更新领奖数据
        /// </summary>
        private void OnUpdateDonateRewardData()
        {
            SetData();
            RefreshView();
        }
        #endregion
        #region 提供功能
        #endregion
    }
}