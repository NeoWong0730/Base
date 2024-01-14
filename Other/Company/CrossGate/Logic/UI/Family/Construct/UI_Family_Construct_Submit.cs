using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine.UI;
using Lib.Core;
using UnityEngine;
using System;

namespace Logic
{
    /// <summary> 家族建设 </summary>
    /// 交互： 关闭,快速放入,提交, 单击放入取出,双击放入取出。
    public class UI_Family_Construct_Submit : UIBase
    {
        #region 界面组件 
        /// <summary> 特殊道具非通用预设 </summary>
        public class UI_Family_SubmitItem
        {
            public Transform transform;
            public Item0_Layout Layout;
            public Image imgSelect;
            public Text txtNumber;
            public Text idText;
            public ItemData ItemData { get; private set; }
            private Image btnImage;
            private Action<UI_Family_SubmitItem> onDoubleClick;
            private Action<UI_Family_SubmitItem> onClick;
            public bool beUsePreview;
            public void BindGameObject(GameObject go)
            {
                transform = go.transform;
                Layout = new Item0_Layout();
                Layout.BindGameObject(transform.Find("Btn_Item").gameObject);
                imgSelect = transform.Find("Image_Select").GetComponent<Image>();
                txtNumber = transform.Find("Text_Number").GetComponent<Text>();
                idText = transform.Find("Id")?.GetComponent<Text>();
                btnImage = transform.Find("Btn_Item/Image_BG").GetComponent<Image>();
                Lib.Core.DoubleClickEvent eventListenerClick = Lib.Core.DoubleClickEvent.Get(btnImage.gameObject);
                eventListenerClick.onClick += (ret) => { OnClick(); };
                eventListenerClick.onDoubleClick += (ret) => { OnDoubleClick(); };
            }

            public void SetAction(Action<UI_Family_SubmitItem> onClick, Action<UI_Family_SubmitItem> onDoubleClick)
            {
                this.onClick = onClick;
                this.onDoubleClick = onDoubleClick;
            }

            public void SetData(ItemData itemData)
            {
                ItemData = itemData;
                this.Refresh();
            }

            public void RemoveData()
            {
                ItemData = null;
            }
            public void Refresh()
            {
                var csv = CSVItem.Instance.GetConfData(ItemData.Id);
                if (csv != null)
                {
                    Layout.imgIcon.gameObject.SetActive(true);
                    ImageHelper.SetIcon(Layout.imgIcon, csv.icon_id);
                    ImageHelper.SetImgAlpha(Layout.imgIcon, 1f);

                    Layout.imgQuality.gameObject.SetActive(true);

                    //设置quality
                    uint tempQuality = 0u;
                    if (ItemData.Quality == 0u)
                        tempQuality = csv.quality;
                    else
                        tempQuality = ItemData.Quality;

                    ImageHelper.GetQualityColor_Frame(Layout.imgQuality, (int)tempQuality);
                }
                txtNumber.gameObject.SetActive(true);
                txtNumber.text = ItemData.Count.ToString();
                beUsePreview = ItemData.Count == 0;
                ImageHelper.SetImageGray(this.transform, beUsePreview, true);
                if (idText != null)
                {
#if UNITY_EDITOR
                    idText.text = ItemData.Id.ToString();
#else
                    idText.text = " ";
#endif
                }
            }

            public void RefreshCount(long count)
            {
                txtNumber.gameObject.SetActive(true);
                txtNumber.text = count.ToString();
            }

            private void OnClick()
            {
                if(beUsePreview)
                {
                    PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(ItemData.Id, 0, false, false, false, false, false, false, true);
                    UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_Family_Construct_Submit, itemData));
                }
                else
                {
                    onClick?.Invoke(this);
                }
            }

            private void OnDoubleClick()
            {
                if (!beUsePreview)
                {
                    onDoubleClick?.Invoke(this);
                }
            }
        }
        /// <summary> 确定提交按钮 </summary>
        private Button pushBtn;
        /// <summary> 规则按钮 </summary>
        private Button ruleBtn;
        /// <summary> 商店按钮 </summary>
        private Button mallBtn;
        /// <summary> 左无限滚动 </summary>
        private InfinityGrid leftInfinityGrid;
        /// <summary> 右无限滚动 </summary>
        private InfinityGrid rightInfinityGrid;
        /// <summary> 货币标题通用界面 </summary>
        private UI_CurrencyTitle ui_CurrencyTitle;
        /// <summary> 行业名称 </summary>
        private Text constructNameText;
        /// <summary> 增加进度量 </summary>
        private Text addExpText;
        /// <summary> 行业等级进度 </summary>
        private Text constructLevelText;
        /// <summary> 行业等级进度条 </summary>
        private Slider constructLevelSlider;
        /// <summary> 行业进度预览进度条 </summary>
        private RectTransform sliderRect;
        /// <summary> 个人功勋获得 </summary>
        private Text personalContributionText;
        /// <summary> 家族资金获得 </summary>
        private Text familyCoinText;
        /// <summary> 个人贡献获得 </summary>
        private Text familyGxText;
        /// <summary> 家族令牌数量 </summary>
        private Text staminaNumText;
        #endregion
        #region 数据定义
        /// <summary> 对应类型的道具列表 </summary>
        List<ItemData> leftItemDatas = null;
        List<ItemData> rightItemDatas = null;
        /// <summary> 预览条最长 </summary>
        private float maxSliderWidth;
        /// <summary> 行业名称语言id基础 </summary>
        private readonly uint constructNameId = 3200000000;
        /// <summary> 当前建设提交类型 </summary>
        private EConstructs eConstructs = EConstructs.Agriculture;
        /// <summary> 需求的令牌 </summary>
        private long _staminaNum;
        public long staminaNum
        {
            get => _staminaNum;
            set
            {
                _staminaNum = value;
                SetAddExp();
                SetPersonalContribution();
                SetFamilyFGx();
                SetStaminaNum();
                SetFamilyCoin();
                SetPushBtnState();
            }
        }
        /// <summary> 繁荣度添加量 </summary>
        private long addExp;
        /// <summary> 功勋 </summary>
        private long fetsNum;
        /// <summary> 个人贡献 </summary>
        private long guildCurrency;
        #endregion
        #region 系统函数        
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
            if (arg is Tuple<uint, object>)
            {
                Tuple<uint, object> tuple = arg as Tuple<uint, object>;
                uint type = Convert.ToUInt32(tuple.Item2);
                eConstructs = (EConstructs)type;
            }
        }

        protected override void OnShow()
        {
            ui_CurrencyTitle.InitUi();
            SetInitView();
        }

        protected override void OnHide()
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
            leftInfinityGrid = transform.Find("Animator/View_Left/Scroll_View_Bag").GetComponent<InfinityGrid>();
            leftInfinityGrid.onCreateCell += OnLeftCreateCell;
            leftInfinityGrid.onCellChange += OnLeftCellChange;
            rightInfinityGrid = transform.Find("Animator/View_Right/Scroll_View_Bag").GetComponent<InfinityGrid>();
            rightInfinityGrid.onCreateCell += OnRightCreateCell;
            rightInfinityGrid.onCellChange += OnRightCellChange;
            pushBtn = transform.Find("Animator/View_Below/Button_Repair").GetComponent<Button>();
            pushBtn.onClick.AddListener(OnClick_PushOK);
            transform.Find("Animator/View_Right/Btn_01_Small").GetComponent<Button>().onClick.AddListener(OnClick_QuickPush);
            ui_CurrencyTitle = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            ui_CurrencyTitle.SetData(new List<uint>() { 17 });
            transform.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
            ruleBtn = transform.Find("Btn_Details").GetComponent<Button>();
            ruleBtn.onClick.AddListener(OnClick_Rule);

            mallBtn = transform.Find("Animator/View_Right/Btn_Shop").GetComponent<Button>();
            mallBtn.onClick.AddListener(OnClick_Mall);
            
            constructNameText = transform.Find("Animator/View_Below/Image/Text").GetComponent<Text>();
            addExpText = transform.Find("Animator/View_Below/Text_Up/Text_Mp_Num").GetComponent<Text>();
            constructLevelText = transform.Find("Animator/View_Below/Text_Lv").GetComponent<Text>();
            constructLevelSlider = transform.Find("Animator/View_Below/Slider_Lv").GetComponent<Slider>();
            maxSliderWidth = constructLevelSlider.GetComponent<RectTransform>().rect.width;
            sliderRect = transform.Find("Animator/View_Below/Slider_Lv/Image1").GetComponent<RectTransform>();
            personalContributionText = transform.Find("Animator/View_Below/Personal/Text_Add").GetComponent<Text>();
            familyCoinText =transform.Find("Animator/View_Below/Family/Text_Add").GetComponent<Text>();
            staminaNumText = transform.Find("Animator/View_Below/Text_Cost").GetComponent<Text>();
            familyGxText = transform.Find("Animator/View_Below/Personal1/Text_Add").GetComponent<Text>();
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Bag.Instance.eventEmitter.Handle<uint, long>(Sys_Bag.EEvents.OnCurrencyChanged, SetInitView, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.GuildCurrencyChange, SetInitView, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.GuildConstructLvChange, SetInitView, toRegister);
        }

        #endregion
        #region 界面显示

        /// <summary>
        /// 设置初始数据视图
        /// </summary>
        private void SetInitView(uint id, long value)
        {
            if (id != (uint)ECurrencyType.FamilyStamina)
            {
                return;
            }
            SetInitView();
        }

        private void SetInitView()
        {
            rightItemDatas?.Clear();
            GetCurrentConstructItem();
            RestInfinityGrid(true);
            SetSubmitViewBaseInfo();
        }

        /// <summary>
        /// 删选列表道具
        /// </summary>
        private void GetCurrentConstructItem()
        {
            uint level = Sys_Family.Instance.familyData.GetConstructLevel();
            List<ItemData> items =  Sys_Bag.Instance.GetItemDatasByItemType(1810);
            rightItemDatas = new List<ItemData>(items.Count);
            int count = items.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                var itemData =  items[i];
                CSVFamilyItem.Data familyItem = CSVFamilyItem.Instance.GetConfData(itemData.Id);
                if(null != familyItem && level >= familyItem.submissionMinLevel)
                {
                    if(eConstructs == (EConstructs)familyItem.industryType || familyItem.industryType == 0)
                    {
                        rightItemDatas.Add(CopyItemData(itemData));
                    }
                }
            }
            List<uint> thisConstructItemIds = GetCurrentConstructItemIds();
            for (int i = 0; i < rightItemDatas.Count; i++)
            {
                for (int j = thisConstructItemIds.Count - 1; j >= 0; j--)
                {
                    if(rightItemDatas[i].Id == thisConstructItemIds[j])
                    {
                        thisConstructItemIds.RemoveAt(j);
                        break;
                    }
                }
            }

            for (int i = 0; i < thisConstructItemIds.Count; i++)
            {
                rightItemDatas.Add(new ItemData(0, 0, thisConstructItemIds[i], 0, 0, false
                , false, null, null, 0));
            }
            
            leftItemDatas?.Clear();
            leftItemDatas = new List<ItemData>(rightItemDatas.Count);
        }

        /// <summary> 获取本行业的需求道具id </summary>
        private List<uint> GetCurrentConstructItemIds()
        {
            uint level = Sys_Family.Instance.familyData.GetConstructLevel();
            List<uint> ids = new List<uint>();

            var dataList = CSVFamilyItem.Instance.GetAll();
            for (int i = 0, len = dataList.Count; i < len; i++)
            {
                CSVFamilyItem.Data it = dataList[i];
                if ((it.industryType == (uint)eConstructs || it.industryType == 0) && level >= it.submissionMinLevel)
                {
                    ids.Add(it.id);
                }
            }
            return ids;
        }

        /// <summary>
        /// 克隆出格子数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ItemData CopyItemData(ItemData data)
        {
            return new ItemData(data.BoxId, data.Uuid, data.Id, data.Count, data.Position, data.bNew
                , data.bBind, data.Equip, data.essence, data.MarketendTime);
        }

        /// <summary>
        /// 初始界面
        /// </summary>
        private void SetSubmitViewBaseInfo()
        {
            TextHelper.SetText(constructNameText, constructNameId + (uint)eConstructs);
            uint level = Sys_Family.Instance.familyData.GetConstructLevel();
            CSVFamilyProsperity.Data familyConstructLevelData = CSVFamilyProsperity.Instance.GetConfData(level);
            var currentExp = Sys_Family.Instance.familyData.GetConstructExp(eConstructs);
            var configExp = Sys_Family.Instance.GetClientDataExp(eConstructs, familyConstructLevelData);
            constructLevelText.text = string.Format("{0}/{1}", currentExp.ToString(), configExp.ToString());
            constructLevelSlider.value = (currentExp + 0f) / configExp;
            addExp = 0;
            fetsNum = 0;
            staminaNum = 0;
            guildCurrency = 0;
        }

        /// <summary>
        /// 设置预览经验
        /// </summary>
        private void SetAddExp()
        {
            bool showAdd = addExp > 0;
            addExpText.transform.parent.gameObject.SetActive(showAdd);
            sliderRect.transform.gameObject.SetActive(showAdd);
            addExpText.text = addExp.ToString();
            if (showAdd)
            {
                addExpText.text = addExp.ToString();
                uint level = Sys_Family.Instance.familyData.GetConstructLevel();
                CSVFamilyProsperity.Data familyConstructLevelData = CSVFamilyProsperity.Instance.GetConfData(level);
                if (null != familyConstructLevelData)
                {
                    var currentExp = Sys_Family.Instance.familyData.GetConstructExp(eConstructs);
                    var configExp = Sys_Family.Instance.GetClientDataExp(eConstructs, familyConstructLevelData);
                    var exp = currentExp + addExp;
                    float sliderValue = ((exp + 0f) / configExp);
                    sliderValue = sliderValue > 1 ? 1f : sliderValue;
                    sliderRect.sizeDelta = new Vector2(maxSliderWidth * sliderValue, sliderRect.rect.height);
                }
            }
        }

        /// <summary>
        /// 设置个人功勋
        /// </summary>
        private void SetPersonalContribution()
        {
            personalContributionText.text = (fetsNum).ToString();
        }

        /// <summary>
        /// 设置家族资金
        /// </summary>
        private void SetFamilyCoin()
        {
            Sys_Ini.Instance.Get<IniElement_Int>(1002, out IniElement_Int p);
            familyCoinText.text = (addExp * (p.value / 100.0f)).ToString();
        }

        /// <summary>
        /// 设置个人贡献
        /// </summary>
        private void SetFamilyFGx()
        {
            familyGxText.text = (guildCurrency).ToString();
        }

        /// <summary>
        /// 设置令牌
        /// </summary>
        private void SetStaminaNum()
        {
            long allStamina = Sys_Family.Instance.familyData.GetGuidStamina();
            uint contentId = staminaNum <= allStamina ? 1601000005u : 1601000004u;
            TextHelper.SetText(this.staminaNumText, contentId, staminaNum.ToString(), allStamina.ToString());
            //staminaNumText.text = string.Format("{0}/{1}", staminaNum.ToString(), Sys_Family.Instance.familyData.GetGuidStamina().ToString()); 
        }

        /// <summary>
        /// 设置提交按钮状态
        /// </summary>
        private void SetPushBtnState()
        {
            ButtonHelper.Enable(pushBtn, addExp > 0);
        }

        /// <summary>
        /// 重置所有列表
        /// </summary>
        private void RestInfinityGrid(bool needSort)
        {
            ResetRightInfinityGrid(needSort);
            ResetLeftInfinityGrid(needSort);
        }

        /// <summary>
        /// 重置右列表
        /// </summary>
        private void ResetRightInfinityGrid(bool needSort)
        {
            if(needSort)
            {
                SortItemByStamina(rightItemDatas);
            }
            rightInfinityGrid.CellCount = rightItemDatas.Count;
            rightInfinityGrid.ForceRefreshActiveCell();
        }

        /// <summary>
        /// 重置左列表
        /// </summary>
        private void ResetLeftInfinityGrid(bool needSort)
        {
            if (needSort)
            {
                SortItemByStamina(leftItemDatas);
            }
            leftInfinityGrid.CellCount = leftItemDatas.Count;
            leftInfinityGrid.ForceRefreshActiveCell();
        }

        /// <summary>
        /// 左滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        private void OnLeftCreateCell(InfinityGridCell cell)
        {
            GameObject go = cell.mRootTransform.gameObject;
            UI_Family_SubmitItem submitItem = new UI_Family_SubmitItem();
            submitItem.BindGameObject(go);
            submitItem.SetAction(LeftItemBeClick, LeftItemBeDoubleClick);
            cell.BindUserData(submitItem);
        }

        /// <summary>
        /// 右滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        private void OnRightCreateCell(InfinityGridCell cell)
        {
            GameObject go = cell.mRootTransform.gameObject;
            UI_Family_SubmitItem submitItem = new UI_Family_SubmitItem();
            submitItem.BindGameObject(go);
            submitItem.SetAction(RightItemBeClick, RightItemBeDoubleClick);
            cell.BindUserData(submitItem);
        }

        /// <summary>
        /// 左滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        private void OnLeftCellChange(InfinityGridCell cell, int index)
        {
            GameObject go = cell.mRootTransform.gameObject;
            var itemData = leftItemDatas[index];
            var uuid = itemData.Uuid;
            var submitItem = cell.mUserData as UI_Family_SubmitItem;
            submitItem.SetData(itemData);
        }

        /// <summary>
        /// 右滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        private void OnRightCellChange(InfinityGridCell cell, int index)
        {
            GameObject go = cell.mRootTransform.gameObject;
            var itemData = rightItemDatas[index];
            var uuid = itemData.Uuid;
            var submitItem = cell.mUserData as UI_Family_SubmitItem;
            submitItem.SetData(itemData);
        }
        
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            UIManager.HitButton(EUIID.UI_Family_Construct_Submit, "OnClick_Close");
            CloseSelf();
        }

        public void OnClick_Rule()
        {
            
            UIManager.OpenUI(EUIID.UI_Construt_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(3290000005)});
        }

        public void OnClick_Mall()
        {
            MallPrama mallPrama = new MallPrama();
            mallPrama.shopId = 13001;
            mallPrama.mallId = 1301;
            UIManager.OpenUI(EUIID.UI_Mall, false, mallPrama);
        }

        /// <summary>
        /// 快捷放入
        /// </summary>
        public void OnClick_QuickPush()
        {
            UIManager.HitButton(EUIID.UI_Family_Construct_Submit, "OnClick_QuickPush");
            SetInitView();
            var staminaAbs = Sys_Family.Instance.familyData.GetGuidStamina();
            int removeIndex = -1;
            for (int i = 0; i < rightItemDatas.Count; i++)
            {
                var item = rightItemDatas[i];
                if(item.Uuid == 0)
                {
                    continue;
                }
                CSVFamilyItem.Data csvFamilyItem = CSVFamilyItem.Instance.GetConfData(item.Id);
                if (null != csvFamilyItem)
                {
                    //唯一id
                    var uuid = item.Uuid;
                    //消耗得令牌数量
                    var costItemNum = csvFamilyItem.consumeNum;
                    // 获取可放入数量  列表提前排序过 
                    var canpushItemNum = staminaAbs / costItemNum;
                    //当前道具数量
                    var canUseItemNum = item.Count;
                    if (canUseItemNum > canpushItemNum && canpushItemNum > 0)
                    {
                        RewardChangeWhenItemChange(item, (int)canpushItemNum, true, out uint addNum);
                        item.SetCount(canUseItemNum - (uint)canpushItemNum);
                        ChangeListData(leftItemDatas, item, (uint)canpushItemNum, true);
                        var needCostStamiaNum = canpushItemNum * costItemNum;
                        staminaAbs -= needCostStamiaNum;
                    }
                    else if(canUseItemNum <= canpushItemNum)
                    {
                        RewardChangeWhenItemChange(item, (int)canUseItemNum, true, out uint addNum);
                        ChangeListData(leftItemDatas, item, canUseItemNum, true);
                        removeIndex = i;
                        staminaAbs -= canUseItemNum * costItemNum;
                    }
                    else
                    {
                        //如果当前道具不足就不需要继续查找下个道具
                        if(i == 0)
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3290000002));
                        break;
                    }
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3290000002));
                }
            }
            //移除已经转移的数据
            for (int i = removeIndex; i >= 0; i--)
            {
                rightItemDatas.RemoveAt(i);
            }
            
            var _staminaNum = Sys_Family.Instance.familyData.GetGuidStamina() - staminaAbs;
            if (_staminaNum != staminaNum)
            {
                staminaNum = _staminaNum;
                RestInfinityGrid(true);
            }
        }


        /// <summary>
        /// 确定提交
        /// </summary>
        public void OnClick_PushOK()
        {
            UIManager.HitButton(EUIID.UI_Family_Construct_Submit, "OnClick_PushOK");
            if (leftItemDatas.Count > 0)
            {
                if(staminaNum <= Sys_Family.Instance.familyData.GetGuidStamina())
                {
                    Sys_Family.Instance.SendGuildHandInItemProsperityReq(leftItemDatas);
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3290000004));
                }
            }
        }
        #endregion
        #region 提供功能
        /// <summary>
        /// 左道具点击
        /// </summary>
        private void LeftItemBeClick(UI_Family_SubmitItem item)
        {
            if (RewardChangeWhenItemChange(item.ItemData, -1, false, out uint addNum))
            {
                uint currentNum = item.ItemData.Count - 1;
                var uuid = item.ItemData.Uuid;
                if (currentNum > 0)
                {
                    item.ItemData.SetCount(currentNum);
                    ChangeListData(rightItemDatas, item.ItemData, 1);
                }
                else
                {
                    for (int i = 0; i < leftItemDatas.Count; i++)
                    {
                        if (leftItemDatas[i].Uuid == uuid)
                        {
                            leftItemDatas.RemoveAt(i);
                            break;
                        }
                    }
                    ChangeListData(rightItemDatas, item.ItemData, 1);
                    item.RemoveData();
                }
            }
        }

        /// <summary>
        /// 右道具点击
        /// </summary>
        private void RightItemBeClick(UI_Family_SubmitItem item)
        {
            if(RewardChangeWhenItemChange(item.ItemData, 1, false, out uint addnum))
            {
                uint currentNum = item.ItemData.Count - 1;
                var uuid = item.ItemData.Uuid;
                if (currentNum > 0)
                {
                    item.ItemData.SetCount(currentNum);
                    ChangeListData(leftItemDatas, item.ItemData, 1);
                }
                else
                {
                    for (int i = 0; i < rightItemDatas.Count; i++)
                    {
                        if (rightItemDatas[i].Uuid == uuid)
                        {
                            rightItemDatas.RemoveAt(i);
                            break;
                        }
                    }
                    ChangeListData(leftItemDatas, item.ItemData, 1);
                    item.RemoveData();
                }
            }
        }

        /// <summary>
        /// 左道具双击
        /// </summary>
        private void LeftItemBeDoubleClick(UI_Family_SubmitItem item)
        {
            if (RewardChangeWhenItemChange(item.ItemData, -(int)item.ItemData.Count, false, out uint addNum))
            {
                var uuid = item.ItemData.Uuid;
                for (int i = 0; i < leftItemDatas.Count; i++)
                {
                    if (leftItemDatas[i].Uuid == uuid)
                    {
                        leftItemDatas.RemoveAt(i);
                        break;
                    }
                }
                ChangeListData(rightItemDatas, item.ItemData, item.ItemData.Count);
                item.RemoveData();
            }
        }

        /// <summary>
        /// 右道具双击
        /// </summary>
        private void RightItemBeDoubleClick(UI_Family_SubmitItem item)
        {
            if (RewardChangeWhenItemChange(item.ItemData, (int)item.ItemData.Count, false, out uint addNum))
            {
                uint currentNum = item.ItemData.Count - addNum;
                var uuid = item.ItemData.Uuid;
                if (currentNum > 0)
                {
                    item.ItemData.SetCount(currentNum);
                    ChangeListData(leftItemDatas, item.ItemData, addNum);
                }
                else
                {
                    for (int i = 0; i < rightItemDatas.Count; i++)
                    {
                        if (rightItemDatas[i].Uuid == uuid)
                        {
                            rightItemDatas.RemoveAt(i);
                            break;
                        }
                    }
                    ChangeListData(leftItemDatas, item.ItemData, addNum);
                    item.RemoveData();
                }
            }
        }

        /// <summary>
        /// 全局变量计算
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private bool RewardChangeWhenItemChange(ItemData item, int count, bool changeStaminaNum, out uint changeNum)
        {
            changeNum = 0;
            if (null != item)
            {
                CSVFamilyItem.Data csvFamilyItem = CSVFamilyItem.Instance.GetConfData(item.Id);
                if (null != csvFamilyItem)
                {
                    var costItemNum = csvFamilyItem.consumeNum; // 道具单个消耗
                    var maItemNum = staminaNum + costItemNum * count; //令牌需求数量
                    CheckDropData(count, csvFamilyItem.submission_drop, maItemNum, changeStaminaNum);
                    changeNum = (uint)count;

                    return true;
                    /*var costItemNum = csvFamilyItem.consumeNum; // 道具单个消耗
                    var maItemNum = staminaNum + costItemNum * count; //令牌需求数量
                    var canUseNum = Sys_Family.Instance.familyData.GetGuidStamina();  //总令牌数量
                    if (maItemNum <= canUseNum && maItemNum >= 0) // 全部
                    {
                        CheckDropData(count, csvFamilyItem.submission_drop, maItemNum, changeStaminaNum);
                        changeNum = (uint)count;
                        return true;
                    }
                    else // 全部不够
                    {
                        count = (int)(canUseNum- staminaNum) / (int)costItemNum; // 向下取整
                        maItemNum = staminaNum + costItemNum * count;
                        CheckDropData(count, csvFamilyItem.submission_drop, maItemNum, changeStaminaNum);
                        changeNum = (uint)count;

                        if (count > 0)
                        {
                            
                            return true;
                        }
                        else
                        {
                            //Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3290000002));
                            return true;
                        }
                    }*/
                }
                else
                {
                    DebugUtil.Log(ELogType.eNone, $"Table CSVFamilyItem  Not Find id = {item.Id}");
                    return false;
                }
            }
            else
            {
                DebugUtil.Log(ELogType.eNone, $"ItemData  is Null");
                return false;
            }
        }

        /// <summary>
        /// 设置令牌消耗。经验贡 献度
        /// </summary>
        /// <param name="count"></param>
        /// <param name="dropId"></param>
        /// <param name="maItemNum"></param>
        /// <param name="changeStaminaNum"></param>
        private void CheckDropData(int count, uint dropId, long maItemNum, bool changeStaminaNum)
        {
            List<ItemIdCount> dropList = CSVDrop.Instance.GetDropItem(dropId);
            for (int i = 0; i < dropList.Count; i++)
            {
                var itemIdCount = dropList[i];
                var id = itemIdCount.id;
                var DropCount = itemIdCount.count;
                if (id == (uint)ECurrencyType.Feats)
                {
                    fetsNum += DropCount * count;
                }
                else if (id == (uint)ECurrencyType.GuildCurrency)
                {
                    guildCurrency += DropCount * count;
                }
                else if (id >= (uint)ECurrencyType.Max)
                {
                    addExp += DropCount * count;
                }
                
            }
            if (!changeStaminaNum)
                staminaNum = maItemNum;
        }

        /// <summary>
        /// 对数据列表操作添加
        /// </summary>
        /// <param name="list"></param>
        /// <param name="data"></param>
        /// <param name="count"></param>
        private void ChangeListData(List<ItemData> list, ItemData data, uint count, bool waitRefrush = false)
        {
            bool needInstan = true;
            for (int i = 0; i < list.Count; i++)
            {
                var tempItemData = list[i];
                if (tempItemData.Uuid == data.Uuid)
                {
                    needInstan = false;
                    tempItemData.SetCount(tempItemData.Count + count);
                    break;
                }
            }

            if (needInstan)
            {
                ItemData itemData = new ItemData();
                itemData.SetData(data.BoxId, data.Uuid, data.Id, count, data.Position, data.bNew
                    , data.bBind, data.Equip, data.essence, data.MarketendTime);

                bool isNotInsert = true;
                for (int i = 0; i < list.Count; i++)
                {
                    if(list[i].Uuid == 0)
                    {
                        isNotInsert = false;
                        list.Insert(i, itemData);
                        break;
                    }
                }

                if(isNotInsert)
                {
                    list.Add(itemData);
                }
                
                //重置数据
            }
            if(!waitRefrush)
            {
                RestInfinityGrid(needInstan);
            }
        }

        /// <summary>
        /// 对列表进行排序
        /// </summary>
        /// <param name="items"></param>
        private void SortItemByStamina(List<ItemData> items)
        {
            if (items.Count > 1)
            {
                int _count = items.Count;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Uuid == 0)
                    {
                        _count = i;
                        break;
                    }
                }
                items.Sort(0, _count, new ItemCoparer());
            }
        }

        #endregion
    }

    /// <summary>
    /// 排序辅助类
    /// </summary>
    public class ItemCoparer : IComparer<object>
    {
        public int Compare(object _a, object _b)
        {
            ItemData a = _a as ItemData;
            ItemData b = _b as ItemData;
            CSVFamilyItem.Data csvA = CSVFamilyItem.Instance.GetConfData(a.Id);
            CSVFamilyItem.Data csvB = CSVFamilyItem.Instance.GetConfData(b.Id);
            if (null != csvA && null != csvB)
            {
                if ((int)csvA.consumeNum == (int)csvB.consumeNum)
                {
                    return (int)a.Count - (int)b.Count;
                }
                else
                {
                    return (int)csvA.consumeNum - (int)csvB.consumeNum;
                }
            }
            return 0;
        }
    }
}