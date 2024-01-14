using System;
using System.Collections.Generic;
using Logic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_BattlePass_Layout
    {
        public enum BattlePassState
        {
            BattlePass_Reward,
            BattlePass_Task,
            BattleShop
        }
        Toggle m_TogReward;
        Toggle m_TogTask;
        Toggle m_TogShop;


        UI_Reward m_UIReward = new UI_Reward();

        UI_Task m_UITask = new UI_Task();

        UI_Shop m_Shop = new UI_Shop();

        Button m_BtnClose;

        IListener m_Listener;

        Text m_TexTime;

        RawImage m_RImgModel;


        private Text m_AwardText;
        private Text m_TaskAwardText;

        Transform m_TransRewardRedDot;
        Transform m_TransTaskRedDot;
        Transform m_TransShopRedDot;
        public BattlePassState m_BattleState = BattlePassState.BattlePass_Reward;
        public void OnLoaded(Transform root)
        {
            m_UIReward.Init(root.Find("Animator/Image_Bg/Reward"));

            m_UITask.Init(root.Find("Animator/Image_Bg/Task"));

            m_Shop.Init(root.Find("Animator/Image_Bg/Shop"));

            m_TogReward = root.Find("Animator/Image_Bg/Menu/Scroll View/Viewport/Content/Toggle").GetComponent<Toggle>();
            m_TogTask = root.Find("Animator/Image_Bg/Menu/Scroll View/Viewport/Content/Toggle (1)").GetComponent<Toggle>();
            m_TogShop = root.Find("Animator/Image_Bg/Menu/Scroll View/Viewport/Content/Toggle (2)").GetComponent<Toggle>();

            m_BtnClose = root.Find("Animator/Image_Bg/View_Title07/Btn_Close").GetComponent<Button>();

            m_TexTime = root.Find("Animator/Image_Bg/Image_Time/Time").GetComponent<Text>();

            m_RImgModel = root.Find("Animator/Image_Bg/RawImage").GetComponent<RawImage>();

            m_TransRewardRedDot = m_TogReward.transform.Find("Image_Dot");
            m_TransTaskRedDot = m_TogTask.transform.Find("Image_Dot");
            m_TransShopRedDot = m_TogShop.transform.Find("Image_Dot");

            m_AwardText = root.Find("Animator/Image_Bg/Reward/Image1/Award/Text_Name").GetComponent<Text>();

            m_TaskAwardText = root.Find("Animator/Image_Bg/Task/Award/Text_Name").GetComponent<Text>();
        }

        public void SetListener(IListener listener)
        {
            m_UIReward.SetListener(listener);
            m_UITask.SetListener(listener);
            m_Shop.SetListener(listener);

            m_TogReward.onValueChanged.AddListener(listener.OnToggleReward);
            m_TogTask.onValueChanged.AddListener(listener.OnToggleTask);
            m_TogShop.onValueChanged.AddListener(listener.OnToggleShop);

            m_BtnClose.onClick.AddListener(listener.OnClickClose);

            m_Listener = listener;
        }

        public void SetTime(uint day, uint hours, uint mins)
        {
            TextHelper.SetText(m_TexTime, 3910010117, day.ToString(), hours.ToString(), mins.ToString());
        }

        public void SetModelController(CutSceneModelShowController controller)
        {
            m_RImgModel.texture = controller.m_ShowSceneControl.GetTemporary(256, 512, 16, RenderTextureFormat.ARGB32,1);
        }

        public void SetModelImageActive(bool active)
        {
            if (m_RImgModel.gameObject.activeSelf != active)
            {
                m_RImgModel.gameObject.SetActive(active);
            }
        }
        public void SetFacusTask()
        {
            m_TogTask.isOn = true;
        }

        public void SetFacusShop()
        {
            m_TogShop.isOn = true;
        }
        public void SetRewardRedDotActive(bool active)
        {
            if (m_TransRewardRedDot.gameObject.activeSelf != active)
                m_TransRewardRedDot.gameObject.SetActive(active);
        }

        public void SetTaskRedDotActive(bool active)
        {
            if (m_TransTaskRedDot.gameObject.activeSelf != active)
                m_TransTaskRedDot.gameObject.SetActive(active);
        }

        public void SetShopRedDotActive(bool active)
        {
            if (m_TransShopRedDot.gameObject.activeSelf != active)
                m_TransShopRedDot.gameObject.SetActive(active);
        }

        public void SetAwardName(uint langueId)
        {
            TextHelper.SetText(m_AwardText, langueId);
            TextHelper.SetText(m_TaskAwardText, langueId); 
        }
    }

    public partial class UI_BattlePass_Layout
    {
        public interface IListener
        {
            void OnClickClose();
            void OnToggleReward(bool state);
            void OnToggleTask(bool state);
            void OnToggleShop(bool state);
            void OnRewardInfinityChange(InfinityGridCell cell, int index);

            void OnRewardOnekeyGet();
            void OnRewardLevelUp();

            void OnClickRewardBuyLevel();

            void OnClickRewardGetExp();
            void OnClickTaskOneKeyGet();

            void OnTaskInfinityChange(InfinityGridCell cell, int index);

            void OnClickTaskGo(uint id, int index);
            void OnClickTaskGet(uint id, int index);
            void OnTogTaskDay(bool state);
            void OnTogTaskWeek(bool state);

            void OnTogTaskSeason(bool state);
            void OnShopInfinityChange(InfinityGridCell cell, int index);
            void OnClickShopItemAdd();
            void OnClickShopItemSub();
            void OnClickShopItemMax();
            void OnClickShopItemSure();

            void OnInputValueChanged(uint input);

            void OnClickShopItemToggle(uint id);

            void OnClickGetReward(uint level);
        }
    }

    /// <summary>
    /// 战令奖励
    /// </summary>
    public partial class UI_BattlePass_Layout
    {
        class RewardPropItem
        {
            PropItem m_Item;
            PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, true, false);


            private Transform m_TransFx;

            public void Load(Transform transform)
            {

                m_Item = new PropItem();

                m_Item.BindGameObject(transform.gameObject);


                m_TransFx = transform.Find("Special");
            }

            public void SetItem(uint id, uint count)
            {
                if (id == 0)
                {
                    m_Item.SetActive(false);
                    return;
                }

                if (m_Item.transform.gameObject.activeSelf == false)
                    m_Item.SetActive(true);

                m_ItemData.id = id;
                m_ItemData.count = count;
                m_ItemData.SetQuality(0);

                m_Item.SetData(new MessageBoxEvt() { sourceUiId = EUIID.UI_BattlePass, itemData = m_ItemData });
            }

            public void SetGot(bool isgot)
            {
                m_Item.SetGot(isgot);
            }

            public void SetFxActive(bool active)
            {
                m_TransFx.gameObject.SetActive(active);
            }
        }
        public class RewardChildItem
        {
            protected Transform m_TransGet;
            protected Transform m_TransGot;

            private Transform m_Transform;
            public virtual void Load(Transform transform)
            {
                m_TransGet = transform.Find("Get");

                m_TransGot = transform.Find("Got");

                m_Transform = transform;
            }

            public void SetGotActive(bool active)
            {
                if (m_TransGot.gameObject.activeSelf != active)
                    m_TransGot.gameObject.SetActive(active);
            }

            public void SetCanGetActive(bool active)
            {
                if (m_TransGet.gameObject.activeSelf != active)
                    m_TransGet.gameObject.SetActive(active);
            }
        }

        public class NormalRewardChildItem : RewardChildItem
        {
            private Text m_TexLevel;

            private Text m_TexName;

            RewardPropItem m_rewardPropItem = new RewardPropItem();

            private Transform m_TransCanGet;
            private Transform m_TransHadGet;


            public Button m_BtnGet;
            public override void Load(Transform transform)
            {
                base.Load(transform);

                m_TexLevel = transform.Find("LV").GetComponent<Text>();

               // m_TexName = transform.Find("Name").GetComponent<Text>();

                m_rewardPropItem.Load(transform.Find("PropItem"));

                m_TransCanGet = transform.Find("Get");
                m_TransHadGet = transform.Find("Got");


                m_BtnGet = m_TransCanGet.Find("Button").GetComponent<Button>();

            }

            public void SetName(uint name)
            {
                //TextHelper.SetText(m_TexName, name);
            }

            public void SetLevel(uint level)
            {
                TextHelper.SetText(m_TexLevel, level.ToString());
            }

            public void SetItem(uint id, uint count)
            {
                m_rewardPropItem.SetItem(id, count);
            }

            public void SetState(uint state,uint normalstate)
            {
                m_TransCanGet.gameObject.SetActive(state == 1);
                m_TransHadGet.gameObject.SetActive(state == 2);

                m_rewardPropItem.SetGot(normalstate == 2);
            }

            public void SetFxActive(bool active)
            {
                m_rewardPropItem.SetFxActive(active);
            }
        }

        public class VIPRewardChildItem : RewardChildItem
        {

            RewardPropItem m_rewardPropItem0 = new RewardPropItem();

            RewardPropItem m_rewardPropItem1 = new RewardPropItem();
            public override void Load(Transform transform)
            {
                base.Load(transform);

                m_rewardPropItem0.Load(transform.Find("Props/PropItem"));
                m_rewardPropItem1.Load(transform.Find("Props/PropItem (1)"));
            }

            public void SetItem0(uint id, uint count)
            {
                m_rewardPropItem0.SetItem(id, count);
            }

            public void SetItem1(uint id, uint count)
            {

                m_rewardPropItem1.SetItem(id, count);
            }

            public void SetState(uint state)
            {
                m_rewardPropItem0.SetGot(state == 2);
                m_rewardPropItem1.SetGot(state == 2);
            }

            public void SetFxActive(bool active)
            {
                m_rewardPropItem0.SetFxActive(active);
                m_rewardPropItem1.SetFxActive(active);
            }
        }

        public class RewardItem
        {
            public NormalRewardChildItem m_Normal = new NormalRewardChildItem();
            public VIPRewardChildItem m_Vip = new VIPRewardChildItem();

            private IListener m_Listenter;

            private uint level = 0;
            public void Load(Transform transform)
            {
                m_Normal.Load(transform.Find("Normal"));
                m_Vip.Load(transform.Find("Vip"));

                m_Normal.m_BtnGet.onClick.AddListener(OnClickGet);
            }

            public void SetConfig(UIRewardCardConfig config)
            {
                level = config.level;

                m_Normal.SetName(config.name);
                m_Normal.SetLevel(config.level);

                m_Normal.SetItem(config.normalItem, config.normalItemCount);

                m_Normal.SetState(config.State,config.NormalState);

                m_Normal.SetFxActive(config.IsSpceReward);

                m_Vip.SetItem0(config.vipItem0, config.vipItem0Count);

                m_Vip.SetItem1(config.vipItem1, config.vipItem1Count);

                m_Vip.SetState(config.VipState);

                m_Vip.SetFxActive(config.IsSpceReward);
            }

            private void OnClickGet()
            {
                m_Listenter?.OnClickGetReward(level);
            }

            public void SetListener(IListener listener)
            {
                m_Listenter = listener;
            }
        }

        public struct UIRewardCardConfig
        {
            public uint level;
            public uint name;

            public uint normalItem;
            public uint normalItemCount;

            public uint vipItem0;
            public uint vipItem0Count;

            public uint vipItem1;
            public uint vipItem1Count;

            public uint State;

            public uint VipState;
            public uint NormalState;

            public bool IsSpceReward;
        }
        class UI_Reward : UIComponent
        {
            public Text m_TexLv;
            public Slider m_SliderExp;
            public Text m_TexPrecent;
            private Button m_BtnGetExp;
            private Button m_BtnBuyLevle;

            private Button m_BtnOneKey;
            public Button m_BtnLevelUp;

            public InfinityGrid m_GridRewardGoup;

            private RewardItem m_Review = new RewardItem();

            public Transform m_TransLockMask;

            public Transform m_TransOnekeyRedDot;

            IListener m_Listener;
            protected override void Loaded()
            {
                m_TexLv = transform.Find("Title/LV").GetComponent<Text>();
                m_SliderExp = transform.Find("Title/Slider_Exp").GetComponent<Slider>();
                m_TexPrecent = transform.Find("Title/Slider_Exp/Text_Percent").GetComponent<Text>();

                m_BtnGetExp = transform.Find("Title/Btn_01").GetComponent<Button>();
                m_BtnBuyLevle = transform.Find("Title/Btn_02").GetComponent<Button>();

                m_GridRewardGoup = transform.Find("Card/Scroll View").GetComponent<InfinityGrid>();

                m_Review.Load(transform.Find("Card/Review"));

                m_BtnOneKey = transform.Find("Btn_01").GetComponent<Button>();
                m_BtnLevelUp = transform.Find("Btn_02").GetComponent<Button>();

                m_TransLockMask = transform.Find("Card/Lock");

                m_TransOnekeyRedDot = m_BtnOneKey.transform.Find("Image_Dot");

            }

            public void SetListener(IListener listener)
            {
                m_GridRewardGoup.onCreateCell = OnCreateInfinityCell;

                m_GridRewardGoup.onCellChange = listener.OnRewardInfinityChange;

                m_BtnOneKey.onClick.AddListener(listener.OnRewardOnekeyGet);

                m_BtnLevelUp.onClick.AddListener(listener.OnRewardLevelUp);

                m_BtnBuyLevle.onClick.AddListener(listener.OnClickRewardBuyLevel);
                m_BtnGetExp.onClick.AddListener(listener.OnClickRewardGetExp);

                m_Listener = listener;
            }

            private void OnCreateInfinityCell(InfinityGridCell cell)
            {
                RewardItem item = new RewardItem();

                item.Load(cell.mRootTransform);

                item.SetListener(m_Listener);

                cell.BindUserData(item);
            }


            public void SetLevel(uint level, uint exp, uint maxexp)
            {
                m_TexLv.text = level.ToString();

                m_SliderExp.value = maxexp == 0 ? 1 : exp * 1f / maxexp;

                m_TexPrecent.text =  maxexp == 0 ? LanguageHelper.GetTextContent(3910010316): (exp.ToString() + "/" + maxexp.ToString());
            }


            public void SetRewardPreView(UIRewardCardConfig config)
            {
                m_Review.SetConfig(config);
            }



        }

        public void SetRewardCard(InfinityGridCell cell, UIRewardCardConfig uIRewardCardConfig)
        {
            RewardItem item = cell.mUserData as RewardItem;

            if (item == null)
                return;

            item.SetConfig(uIRewardCardConfig);
        }


        public void SetRewardPreView(UIRewardCardConfig uIRewardCardConfig)
        {
            m_UIReward.SetRewardPreView(uIRewardCardConfig);
        }
        public void SetRewardLevel(uint level, uint exp, uint maxexp)
        {
            m_UIReward.SetLevel(level, exp, maxexp);
        }

        public void SetRewardCardCount(int count)
        {
            m_UIReward.m_GridRewardGoup.CellCount = count;
            m_UIReward.m_GridRewardGoup.ForceRefreshActiveCell();
            m_UIReward.m_GridRewardGoup.MoveToIndex(0);
        }

        public void RefreshRewardItme()
        {
            m_UIReward.m_GridRewardGoup.ForceRefreshActiveCell();
        }

        public void SetRewardLockActive(bool active)
        {
            if (m_UIReward.m_TransLockMask.gameObject.activeSelf != active)
                m_UIReward.m_TransLockMask.gameObject.SetActive(active);
        }

        public void SetVipBtnActive(bool active)
        {
            if (m_UIReward.m_BtnLevelUp.gameObject.activeSelf != active)
                m_UIReward.m_BtnLevelUp.gameObject.SetActive(active);
        }

        public void SetRewardFocusIndex(int index)
        {
            if (index < 0)
                index = 0;

            var rect = m_UIReward.m_GridRewardGoup.Content;

            var cellsize = m_UIReward.m_GridRewardGoup.CellSize;

            var space = m_UIReward.m_GridRewardGoup.Spacing;

            float offsetx = cellsize.x * (index) + space.x * index;

            float xpos =  m_UIReward.m_GridRewardGoup.ScrollView.viewport.sizeDelta.x  * rect.anchorMin.x- offsetx;

            float maxpos = m_UIReward.m_GridRewardGoup.ScrollView.viewport.sizeDelta.x * rect.anchorMin.x - (rect.sizeDelta.x - m_UIReward.m_GridRewardGoup.ScrollView.viewport.sizeDelta.x);

            float minpos = m_UIReward.m_GridRewardGoup.ScrollView.viewport.sizeDelta.x * (-rect.anchorMin.x);

            if (Mathf.Abs(minpos) > Mathf.Abs(xpos))
                xpos = minpos;
            else if (Mathf.Abs(xpos) > Mathf.Abs(maxpos))
                xpos = maxpos;

            m_UIReward.m_GridRewardGoup.Content.anchoredPosition = new Vector2(xpos, rect.anchoredPosition.y);
        }

        public void SetRewardOneKeyRedDot(bool active)
        {
            if(m_UIReward.m_TransOnekeyRedDot.gameObject.activeSelf != active)
                m_UIReward.m_TransOnekeyRedDot.gameObject.SetActive(active);
        }
    }


    /// <summary>
    /// 战令任务
    /// </summary>
    public partial class UI_BattlePass_Layout
    {
        class TaskItem:ClickItem
        {

            Button m_BtnGo;
            Button m_BtnGet;
            Transform m_TransHad;

            Image m_ImgIcon;
            Text m_TexNumber;

            Slider m_SdTask;
            Text m_TexTaskName;
            Text m_TexTaskPercent;


            IListener m_Listener;

            uint TaskID = 0;
            int mIndex = -1;

            //Toggle m_TogDay;
            //Toggle m_TogWeek;

            public override void Load(Transform root)
            {
                base.Load(root);

                m_BtnGo = root.Find("Image_bg/State/Btn_02").GetComponent<Button>();

                m_BtnGet = root.Find("Image_bg/State/Btn_01").GetComponent<Button>();

                m_TransHad = root.Find("Image_bg/State/Image");


                m_ImgIcon = root.Find("Image_bg/Image_BG/Image_Icon").GetComponent<Image>();
                m_TexNumber = root.Find("Image_bg/Image_BG/Text_Number").GetComponent<Text>();

                m_SdTask = root.Find("Image_bg/Slider_Task").GetComponent<Slider>();

                m_TexTaskName = root.Find("Image_bg/Slider_Task/Text_TaskName").GetComponent<Text>();
                m_TexTaskPercent = root.Find("Image_bg/Slider_Task/Text_Percent").GetComponent<Text>();

                //m_TogDay = root.Find("Image_bg/Title/Toggles/Toggle0").GetComponent<Toggle>();
                //m_TogWeek = root.Find("Image_bg/Title/Toggles/Toggle1").GetComponent<Toggle>();
            }

            public void SetListener(IListener listener)
            {
                m_Listener = listener;

                m_BtnGet.onClick.AddListener(OnClickGet);
                m_BtnGo.onClick.AddListener(OnClickGo);

            }

            private void OnClickGet()
            {
                m_Listener?.OnClickTaskGet(TaskID,mIndex);
            }

            private void OnClickGo()
            {
                m_Listener?.OnClickTaskGo(TaskID, mIndex);
            }


            private void SetState(uint state,bool isHidegoto)
            {
                bool isGo = state == 0;
                bool isGet = state == 1;
                bool isHadGet = state == 2;

                m_BtnGo.gameObject.SetActive(isGo && !isHidegoto);
                m_BtnGet.gameObject.SetActive(isGet);
                m_TransHad.gameObject.SetActive(isHadGet);
            }

            
            public void SetConfig(TaskItemConfig config)
            {
                SetState(config.State,config.HideGoto);

              //  ImageHelper.SetIcon(m_ImgIcon, config.ItemID);
                TextHelper.SetText(m_TexNumber, config.ItemCount.ToString());

                m_SdTask.value = config.CurStep * 1f / config.MaxStep;

                m_TexTaskName.text = config.Name;

                m_TexTaskPercent.text = config.CurStep.ToString() + "/" + config.MaxStep.ToString();

                TaskID = config.ID;

                mIndex = config.Index;
            }

        }

        public struct TaskItemConfig
        {
            public uint ID;

            public int Index;

            public uint State;

            public uint CurStep;
            public uint MaxStep;

            public string Name;

            public uint ItemID;
            public uint ItemCount;

            public bool HideGoto;

        }
        class UI_Task : UIComponent
        {
            public Toggle m_TogDay;
            public Toggle m_TogWeek;

            public Toggle m_TogSeason;

            Button m_BtnOneKeyGet;

            public InfinityGrid m_GridTaskGoup;

            IListener m_Listener;

            public Transform m_TransTaskOneKeyRedDot;
            protected override void Loaded()
            {
                m_TogDay = transform.Find("Title/Toggles/Toggle0").GetComponent<Toggle>();
                m_TogWeek = transform.Find("Title/Toggles/Toggle1").GetComponent<Toggle>();
                m_TogSeason = transform.Find("Title/Toggles/Toggle2").GetComponent<Toggle>();

                m_BtnOneKeyGet = transform.Find("Btn_01").GetComponent<Button>();

                m_GridTaskGoup = transform.Find("Scroll View").GetComponent<InfinityGrid>();

                m_TransTaskOneKeyRedDot = transform.Find("Btn_01/Image_Dot");
            }

            public void SetListener(IListener listener)
            {
                m_GridTaskGoup.onCreateCell = OnCreateInfinityCell;

                m_GridTaskGoup.onCellChange = listener.OnTaskInfinityChange;

                m_BtnOneKeyGet.onClick.AddListener(listener.OnClickTaskOneKeyGet);

                m_TogDay.onValueChanged.AddListener(listener.OnTogTaskDay);
                m_TogWeek.onValueChanged.AddListener(listener.OnTogTaskWeek);
                m_TogSeason.onValueChanged.AddListener(listener.OnTogTaskSeason);

                m_Listener = listener;


            }

            private void OnCreateInfinityCell(InfinityGridCell cell)
            {
                TaskItem item = new TaskItem();

                item.Load(cell.mRootTransform);

                item.SetListener(m_Listener);

                cell.BindUserData(item);
            }
        }


        public void SetTaskItem(InfinityGridCell cell, TaskItemConfig config)
        {
            TaskItem item = cell.mUserData as TaskItem;

            if (item == null)
                return;

            item.SetConfig(config);

        }

        public void SetTaskCount(int count)
        {
            m_UITask.m_GridTaskGoup.CellCount = count;

            m_UITask.m_GridTaskGoup.ForceRefreshActiveCell();
            m_UITask.m_GridTaskGoup.MoveToIndex(0);
        }

        public void RefreshTaskItmes()
        {
            m_UITask.m_GridTaskGoup.ForceRefreshActiveCell();
        }

        public void SetTaskDailyToggleFocus()
        {
            m_UITask.m_TogDay.isOn = true;
        }

        public void SetTaskWeekToggleFocus()
        {
            m_UITask.m_TogWeek.isOn = true;
        }

        public void SetTaskOneKeyRedDotActive(bool active)
        {
            if(m_UITask.m_TransTaskOneKeyRedDot.gameObject.activeSelf != active)
                m_UITask.m_TransTaskOneKeyRedDot.gameObject.SetActive(active);
        }
    }

    /// <summary>
    /// 战令商店
    /// </summary>
    public partial class UI_BattlePass_Layout
    {
        class ShopItem : ClickItem
        {
            Text m_TexName;

            Transform m_TransSellLimite;
            Transform m_TransSellScale;

            Transform m_TransSellOut;

            public Transform m_TransLimite;
            Text m_TextLimite;


            Transform m_TransOriginal;

            Text m_TexOriginal;
            Text m_TexNow;

            Image m_ImgIcon;

            public CP_Toggle m_Toggle;

            uint ID;

            IListener m_Listener;

            Image m_ImgOriginal;
            Image m_ImgNow;
            public override void Load(Transform root)
            {
                base.Load(root);

                m_TexName = root.Find("Text_Name").GetComponent<Text>();

                m_TransSellLimite = root.Find("MarkGrid/Image_Mark02");

                m_TransSellScale = root.Find("MarkGrid/Image_Mark01");

                m_TransSellOut = root.Find("Image_Sellout");

                m_TransLimite = root.Find("Limit");
                m_TextLimite = root.Find("Limit/Text").GetComponent<Text>();

                m_TransOriginal = root.Find("Cost/Cost_Original");
                m_TexOriginal = root.Find("Cost/Cost_Original/Text_Original/Image_Icon/Text").GetComponent<Text>();
                m_TexNow = root.Find("Cost/Cost_Now/Image_Icon/Text").GetComponent<Text>();

                m_ImgOriginal = root.Find("Cost/Cost_Original/Text_Original/Image_Icon").GetComponent<Image>();
                m_ImgNow = root.Find("Cost/Cost_Now/Image_Icon").GetComponent<Image>();

                m_ImgIcon = root.Find("Image_ICON").GetComponent<Image>();

                m_Toggle = root.GetComponent<CP_Toggle>();
            }

            public void SetShopItem(CSVShopItem.Data data)
            {
                var itemdata = CSVItem.Instance.GetConfData(data.item_id);

                TextHelper.SetText(m_TexName, itemdata.name_id);

                m_TransOriginal.gameObject.SetActive(data.price_before != data.price_now);

                TextHelper.SetText(m_TexOriginal, data.price_before.ToString());

                TextHelper.SetText(m_TexNow, data.price_now.ToString());

                ImageHelper.SetIcon(m_ImgIcon, itemdata.icon_id);

                var itempricedata = CSVItem.Instance.GetConfData(data.price_type);
                ImageHelper.SetIcon(m_ImgOriginal, itempricedata.small_icon_id);
                ImageHelper.SetIcon(m_ImgNow, itempricedata.small_icon_id);

                if (data.need_senior_BP && Sys_BattlePass.Instance.isVip == false)
                {
                    TextHelper.SetText(m_TextLimite, 3910010309);
                }
                else if (data.need_BP_lv > Sys_BattlePass.Instance.Info.Level)
                {
                    TextHelper.SetText(m_TextLimite, 3910010307, data.need_BP_lv.ToString());
                }

                ID = data.id;
            }

            private void OnClickToggle(bool state)
            {
                if (state)
                    m_Listener.OnClickShopItemToggle(ID);
            }

            public void Setlistener(IListener listener)
            {
                m_Toggle.onValueChanged.AddListener(OnClickToggle);

                m_Listener = listener;
            }
        }

        class ShopItemDetail:ClickItem
        {
            Text m_TexCanBuy;
            Text m_TextLimiteType;

            RewardPropItem propItem = new RewardPropItem();

            Text m_TexTips;
            //InputField m_InputField;

            Button m_BtnAdd;
            Button m_BtnSub;
            Button m_BtnMax;

            Button m_BtnSure;
            Text m_TextPrice;

            

            Transform m_TransNone;
            Transform m_TransItem;

            Image m_ImgPriceIcon;

            UI_Common_Num m_InputNum = new UI_Common_Num();
            public override void Load(Transform root)
            {
                base.Load(root);

                m_TransNone = root.Find("View_None");
                m_TransItem = root.Find("View_Item");

                m_TextLimiteType = root.Find("View_Item/View_Controll/Text_Limit01").GetComponent<Text>();
                m_TexCanBuy = root.Find("View_Item/View_Controll/Text_Limit01/Text").GetComponent<Text>();
                propItem.Load(root.Find("View_Item/PropItem"));

                m_TexTips = root.Find("View_Item/Scroll_View/Viewport/Content/Text_Tips").GetComponent<Text>();

                //m_InputField = root.Find("View_Item/View_Controll/Image_Number/InputField_Number").GetComponent<InputField>();


                m_BtnAdd = root.Find("View_Item/View_Controll/Image_Number/Button_Add").GetComponent<Button>();
                m_BtnSub = root.Find("View_Item/View_Controll/Image_Number/Button_Sub").GetComponent<Button>();
                m_BtnMax = root.Find("View_Item/View_Controll/Image_Number/Button_Max").GetComponent<Button>();

                m_BtnSure = root.Find("View_Item/View_Controll/Btn_01").GetComponent<Button>();
                m_TextPrice = root.Find("View_Item/View_Controll/Btn_01/Image_Icon/Text").GetComponent<Text>();
                m_ImgPriceIcon = root.Find("View_Item/View_Controll/Btn_01/Image_Icon").GetComponent<Image>();

                m_InputNum.Init(root.Find("View_Item/View_Controll/Image_Number/InputField_Number"), 99999);
            }

            public void SetListener(IListener listener)
            {
                m_BtnAdd.onClick.AddListener(listener.OnClickShopItemAdd);
                m_BtnSub.onClick.AddListener(listener.OnClickShopItemSub);
                m_BtnMax.onClick.AddListener(listener.OnClickShopItemMax);
                m_BtnSure.onClick.AddListener(listener.OnClickShopItemSure);
                //m_InputField.onValueChanged.AddListener(listener.OnInputValueChanged);

                m_InputNum.RegEnd(listener.OnInputValueChanged);
            }

            public void SetShopItem(CSVShopItem.Data data, Packet.ShopItem sitem)
            {
                var itemdata = CSVItem.Instance.GetConfData(data.item_id);

                propItem.SetItem(itemdata.id, 1);

                TextHelper.SetText(m_TexTips, itemdata.describe_id);

                SetCount(1, (int)data.price_now);
                //TextHelper.SetText(m_TextPrice, data.price_now.ToString());

                //int defaultcount = 1;
                //m_InputField.text = defaultcount.ToString();


                var itempricedata = CSVItem.Instance.GetConfData(data.price_type);
                ImageHelper.SetIcon(m_ImgPriceIcon, itempricedata.small_icon_id);

                SetLimiteCount(data, sitem);
            }

            public void SetCount(int count, int price)
            {
                TextHelper.SetText(m_TextPrice, (price*count).ToString());
                long currency = Sys_Bag.Instance.GetItemCount(19);
                m_TextPrice.color = currency >= count * price? Color.black : Color.red;
 
                //m_InputField.text = count.ToString();

                m_InputNum.SetData((uint)count);
            }

            private void SetLimiteCount(CSVShopItem.Data shopItemData, Packet.ShopItem sitem)
            {
                bool isSelfType = shopItemData.personal_limit != 0;
                if (isSelfType)
                {
                    uint lanId = (shopItemData.limit_type == 1 || shopItemData.limit_type == 3) ? (uint)2009805 : (uint)2009809;

                    //新加个人限购历史类型5
                    lanId = shopItemData.limit_type == 5 ? (uint)2009821 : lanId;

                    m_TextLimiteType.gameObject.SetActive(true);
                    m_TextLimiteType.text = LanguageHelper.GetTextContent(lanId);
                    m_TexCanBuy.text = string.Format("{0}/{1}", Sys_Mall.Instance.CalSelfLeftNum(shopItemData.id), shopItemData.personal_limit);
                }
                else
                {
                    m_TextLimiteType.gameObject.SetActive(false);
                }

            }
        }
        class UI_Shop : UIComponent
        {
            public InfinityGrid m_ShopGrid;

            public ShopItemDetail shopItemDetail = new ShopItemDetail();

            IListener m_Listener;
            protected override void Loaded()
            {
                m_ShopGrid = transform.Find("View_Left/Scroll_View").GetComponent<InfinityGrid>();

                shopItemDetail.Load(transform.Find("View_Middle"));

            }

            public void SetListener(IListener listener)
            {
                m_ShopGrid.onCreateCell = OnInfinityGridCreateCell;

                m_ShopGrid.onCellChange = listener.OnShopInfinityChange;

                shopItemDetail.SetListener(listener);

                m_Listener = listener;
            }

            private void OnInfinityGridCreateCell(InfinityGridCell cell)
            {
                ShopItem item = new ShopItem();

                item.Load(cell.mRootTransform);

                cell.BindUserData(item);

                item.Setlistener(m_Listener);
            }
        }

        public void SetShopItemCount(int count)
        {
            m_Shop.m_ShopGrid.CellCount = count;

            m_Shop.m_ShopGrid.ForceRefreshActiveCell();
            m_Shop.m_ShopGrid.MoveToIndex(0);

           // m_Shop.m_ShopGrid.GetItemByIndex(0);
        }

        public void SetShopItem(InfinityGridCell cell, CSVShopItem.Data data,bool isFocus)
        {
            ShopItem item = cell.mUserData as ShopItem;

            if (item == null)
                return;

            item.SetShopItem(data);

            item.m_Toggle.SetSelected(isFocus, false);

            bool isHadGet = false;

            if ((data.need_senior_BP && Sys_BattlePass.Instance.isVip == false)||
                (data.need_BP_lv > Sys_BattlePass.Instance.Info.Level))
                isHadGet = true;

            item.m_TransLimite.gameObject.SetActive(isHadGet);
        }

        public void SetSelectShopItem(CSVShopItem.Data data, Packet.ShopItem sitem)
        {
            m_Shop.shopItemDetail.SetShopItem(data, sitem);
        }

        public void SetSelectShopItemCount(int count, int price)
        {
            m_Shop.shopItemDetail.SetCount(count, price);
        }
    }
}
