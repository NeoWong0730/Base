using Lib.Core;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Table;
using UnityEngine.EventSystems;
using Packet;
using System;
using static Logic.Sys_Equip;

namespace Logic
{
    public class ItemSource
    {
        private Transform transform;

        private uint m_ItemId;
        private uint m_SourceUiID; //从哪个界面过来的，比如从背包界面点击 打开的tips 这个id就是背包界面
        private EUIID m_EOwnerTips; //节点归属于哪个tips   PropMessage Equip Jewelry ....
        private CSVItem.Data m_ItemData;

        private Transform infinityParent;
        private InfinityGridLayoutGroup infinity;
        private Dictionary<GameObject, SourceItemCeil> ceil1s = new Dictionary<GameObject, SourceItemCeil>();
        private List<uint> filterSource = new List<uint>();
        private bool bSourceActive;
        private List<uint> sourceUiIDs = new List<uint>();

        public void BindGameObject(GameObject gameObject)
        {
            transform = gameObject.transform;
            OnParseSourceLayout();

            InitData();
        }

        //设置完数据 会自动判断能不能显示 并返回flag 此flag可以用于不同tips特殊的需求 比如左侧按钮的显隐
        public bool SetData(uint itemId, uint sourceUiID, EUIID eOwnerTips)
        {
            this.m_ItemId = itemId;
            this.m_SourceUiID = sourceUiID;
            this.m_EOwnerTips = eOwnerTips;
            m_ItemData = CSVItem.Instance.GetConfData(itemId);

            filterSources();
            bSourceActive = filterSource.Count > 0 && sourceUiIDs.Contains(m_SourceUiID);

            return bSourceActive;
        }

        public void Show()
        {
            ShowSourceItems();
        }


        private void InitData()
        {
            string[] str = CSVParam.Instance.GetConfData(710).str_value.Split('|');
            for (int i = 0; i < str.Length; i++)
            {
                sourceUiIDs.Add(uint.Parse(str[i]));
            }
        }

        private void OnParseSourceLayout()
        {
            infinityParent = transform.Find("Scroll_View/Grid");
            infinity = infinityParent.gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            infinity.minAmount = 5;
            infinity.updateChildrenCallback = UpdateChildrenCallback1;
            for (int i = 0; i < infinityParent.childCount; i++)
            {
                GameObject go = infinityParent.GetChild(i).gameObject;
                SourceItemCeil sourceItemCeil = new SourceItemCeil();
                sourceItemCeil.BindGameObject(go);
                sourceItemCeil.AddClickListener(OnCeilSelected);
                ceil1s.Add(go, sourceItemCeil);
            }
        }

        private void ShowSourceItems()
        {
            transform.gameObject.SetActive(true);
            infinity.SetAmount(filterSource.Count);
        }

        private void filterSources()
        {
            filterSource.Clear();
            if (m_ItemData.Source == null)
                return;
            foreach (var item in m_ItemData.Source)
            {
                CSVItemSource.Data cSVItemSourceData = CSVItemSource.Instance.GetConfData(item[0]);
                if (cSVItemSourceData != null)
                {
                    if (Sys_FunctionOpen.Instance.IsOpen(cSVItemSourceData.Function_id) && Sys_Role.Instance.Role.Level >= cSVItemSourceData.Level[0]
                                                                                        && Sys_Role.Instance.Role.Level <= cSVItemSourceData.Level[1])
                    {
                        filterSource.Add(item[0]);
                    }
                }
            }
        }

        private void UpdateChildrenCallback1(int index, Transform trans)
        {
            SourceItemCeil sourceItemCeil = ceil1s[trans.gameObject];
            sourceItemCeil.SetData(filterSource[index]);
        }

        private void OnCeilSelected(SourceItemCeil sourceItemCeil)
        {
            //Sys_Bag.Instance.ExecuteItemSource(m_ItemId, sourceItemCeil.itemSourceId, m_EOwnerTips, (EUIID)sourceItemCeil.itemSourceId);
            CSVItemSource.Data cSVItemSourceData = CSVItemSource.Instance.GetConfData(sourceItemCeil.itemSourceId);
            if (cSVItemSourceData != null)
            {
                if (cSVItemSourceData.Type == 1) //商城
                {
                    UIManager.CloseUI(m_EOwnerTips);

                    MallPrama mallPrama = new MallPrama();
                    mallPrama.itemId = GetGoToItemId(m_ItemId, sourceItemCeil.itemSourceId);
                    mallPrama.mallId = cSVItemSourceData.Parameter[0];
                    mallPrama.shopId = cSVItemSourceData.Parameter[1];
                    EUIID uiId = (EUIID) cSVItemSourceData.UI_id;
                    UIManager.OpenUI(uiId, false, mallPrama);
                }
                else if (cSVItemSourceData.Type == 2) //日常界面
                {
                    UIManager.CloseUI(m_EOwnerTips);
                    if (cSVItemSourceData.Activity_id == 0)
                    {
                        UIManager.OpenUI(EUIID.UI_DailyActivites);
                    }
                    else
                    {
                        UIDailyActivitesParmas uIDailyActivitesParmas = new UIDailyActivitesParmas();
                        uIDailyActivitesParmas.SkipToID = cSVItemSourceData.Activity_id;
                        UIManager.OpenUI(EUIID.UI_DailyActivites, false, uIDailyActivitesParmas);
                    }
                }
                else if (cSVItemSourceData.Type == 3) //寻路
                {
                    UIManager.CloseUI(m_EOwnerTips);
                    UIManager.CloseUI((EUIID) m_SourceUiID);
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(cSVItemSourceData.NPC_id);
                }
                else if (cSVItemSourceData.Type == 4) //晶石界面
                {
                    UIManager.CloseUI(m_EOwnerTips);
                    Sys_StoneSkill.Instance.eventEmitter.Trigger(Sys_StoneSkill.EEvents.EnergysparSpecilEvent, m_ItemId);
                }
                else if (cSVItemSourceData.Type == 5) //道具合成 
                {
                    UIManager.CloseUI(m_EOwnerTips);
                    if (null != m_ItemData)
                    {
                        if (m_SourceUiID == cSVItemSourceData.UI_id) //在道具合成本界面
                        {
                            Sys_Bag.Instance.eventEmitter.Trigger(Sys_Bag.EEvents.ComposeSpecilEvent, m_ItemData.composed);
                        }
                        else //打开道具合成界面
                        {
                            UIManager.OpenUI(EUIID.UI_Compose, false, m_ItemData.composed);
                        }
                    }
                }
                else if (cSVItemSourceData.Type == 6)
                {
                    UIManager.CloseUI(m_EOwnerTips);
                    uint itemId = GetGoToItemId(m_ItemId, sourceItemCeil.itemSourceId);
                    Sys_Trade.Instance.TradeFind(itemId);
                }
                else if (cSVItemSourceData.Type == 7)
                {
                    UIManager.CloseUI(m_EOwnerTips);
                    uint lifeskillId = cSVItemSourceData.id - cSVItemSourceData.Type * 1000;
                    if (m_SourceUiID == (uint) EUIID.UI_LifeSkill_Message)
                    {
                        Sys_LivingSkill.Instance.eventEmitter.Trigger<uint, uint>(Sys_LivingSkill.EEvents.OnRefreshLifeSkillMessage, lifeskillId, m_ItemId);
                    }
                    else
                    {
                        LifeSkillOpenParm lifeSkillOpenParm = new LifeSkillOpenParm();
                        lifeSkillOpenParm.skillId = lifeskillId;
                        lifeSkillOpenParm.itemId = m_ItemId;
                        UIManager.OpenUI(EUIID.UI_LifeSkill_Message, false, lifeSkillOpenParm);
                    }

                    if ((EUIID) m_SourceUiID == EUIID.UI_FamilyWorkshop_entrust)
                    {
                        UIManager.CloseUI(EUIID.UI_FamilyWorkshop_entrust);
                        if (UIManager.IsOpen(EUIID.UI_FamilyWorkshop))
                        {
                            UIManager.CloseUI(EUIID.UI_FamilyWorkshop);
                        }

                        if (UIManager.IsOpen(EUIID.UI_Family))
                        {
                            UIManager.CloseUI(EUIID.UI_Family);
                        }
                    }
                }
                else if (cSVItemSourceData.Type == 8)
                {
                    UIManager.CloseUI(m_EOwnerTips);
                    UIManager.OpenUI(EUIID.UI_ClueTaskMain);
                }
                else if (cSVItemSourceData.Type == 9)
                {
                    UIManager.CloseUI(m_EOwnerTips);
                    UIManager.CloseUI((EUIID) m_SourceUiID, true);
                    UIManager.OpenUI((EUIID) cSVItemSourceData.UI_id);
                }
                else if (cSVItemSourceData.Type == 10)
                {
                    UIManager.CloseUI(m_EOwnerTips);
                    MallPrama mallPrama = new MallPrama();
                    mallPrama.itemId = GetGoToItemId(m_ItemId, sourceItemCeil.itemSourceId);
                    mallPrama.mallId = cSVItemSourceData.Parameter[0];
                    mallPrama.shopId = cSVItemSourceData.Parameter[1];
                    UIManager.OpenUI(EUIID.UI_PointMall, false, mallPrama);
                }
                else if (cSVItemSourceData.Type == 11)
                {
                    UIManager.CloseUI(m_EOwnerTips);
                    UIManager.OpenUI(EUIID.UI_Adventure, false, new AdventurePrama {page = cSVItemSourceData.Parameter[0]});
                }
                else if (cSVItemSourceData.Type == 12)
                {
                    UIManager.CloseUI(m_EOwnerTips);
                    Sys_Equip.UIEquipPrama data = new Sys_Equip.UIEquipPrama();
                    data.opType = (EquipmentOperations) cSVItemSourceData.Parameter[0];
                    UIManager.OpenUI(EUIID.UI_Equipment, false, data);
                }
                else if (cSVItemSourceData.Type == 13)
                {
                    UIManager.CloseUI(m_EOwnerTips);
                    UIManager.CloseUI((EUIID) m_SourceUiID);
                    Sys_Mall.Instance.skip2MallFromItemSource = new MallPrama();
                    Sys_Mall.Instance.skip2MallFromItemSource.mallId = cSVItemSourceData.Parameter[0];
                    Sys_Mall.Instance.skip2MallFromItemSource.shopId = cSVItemSourceData.Parameter[1];
                    Sys_Mall.Instance.skip2MallFromItemSource.itemId = m_ItemId;
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(cSVItemSourceData.NPC_id);
                }
                else if (cSVItemSourceData.Type == 14)
                {
                    UIManager.CloseUI(m_EOwnerTips);
                    UIManager.OpenUI(EUIID.UI_Fashion_Buy, false, m_ItemId);
                }
                else if (cSVItemSourceData.Type == 15)
                {
                    UIManager.CloseUI(m_EOwnerTips);
                    MallPrama param = new MallPrama();
                    param.mallId = 101u;
                    param.isCharge = true;
                    UIManager.OpenUI(EUIID.UI_Mall, false, param);
                }
                else if (cSVItemSourceData.Type == 16) //飘字
                {
                    uint lanId = cSVItemSourceData.Parameter[0];
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(lanId));
                }
                else if (cSVItemSourceData.Type == 17) //累充
                {
                    UIManager.CloseUI(m_EOwnerTips);
                    UIManager.OpenUI(EUIID.UI_OperationalActivity, false, cSVItemSourceData.Parameter[0]);
                }
                else if (cSVItemSourceData.Type == 18)
                {
                    UIManager.CloseUI(m_EOwnerTips);
                    UIManager.OpenUI(EUIID.UI_Knowledge_RecipeCooking, false, m_ItemId);
                }
                else if (cSVItemSourceData.Type == 19)
                {
                    UIManager.CloseUI(m_EOwnerTips);
                    UIManager.CloseUI((EUIID) m_SourceUiID);
                    UIManager.OpenUI(EUIID.UI_GoddessTrial);
                }
                else if (cSVItemSourceData.Type == 20)
                {
                    UIManager.CloseUI(m_EOwnerTips);
                    Sys_Trade.Instance.FindCategory(cSVItemSourceData.Parameter[0]);
                }
                else if (cSVItemSourceData.Type == 21) //家族
                {
                    if (!Sys_Family.Instance.familyData.isInFamily)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(6002201));
                        return;
                    }

                    MallPrama mallPrama = new MallPrama();
                    mallPrama.itemId = GetGoToItemId(m_ItemId, sourceItemCeil.itemSourceId);
                    mallPrama.mallId = cSVItemSourceData.Parameter[0];
                    mallPrama.shopId = cSVItemSourceData.Parameter[1];
                    EUIID uiId = (EUIID) cSVItemSourceData.UI_id;
                    UIManager.OpenUI(uiId, false, mallPrama);
                }
                else if (cSVItemSourceData.Type == 22) //时装抽奖活动
                {
                    bool open = CalFashionLuckyDraw(m_ItemId);

                    if (open)
                    {
                        UIManager.OpenUI(EUIID.UI_Fashion_LuckyDraw);
                    }
                    else
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(cSVItemSourceData.Parameter[0]));
                    }
                }
                else if (cSVItemSourceData.Type == 23) //战令活动
                {
                    bool open = CalBattlePass(m_ItemId);

                    if (open)
                    {
                        UIManager.OpenUI(EUIID.UI_BattlePass);
                    }
                    else
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(cSVItemSourceData.Parameter[0]));
                    }
                }
                else if (cSVItemSourceData.Type == 24) //成就界面跳转页签
                {
                    uint achId = GetAchId(m_ItemId, sourceItemCeil.itemSourceId);

                    Sys_Achievement.Instance.OpenAchievementMenu(achId);
                }
                else if (cSVItemSourceData.Type == 25)
                {
                    if (m_ItemData != null)
                    {
                        uint cookId = 0;
                        if(m_ItemData.Source.Count > 0)
                        {
                            if(m_ItemData.Source[0].Count > 0)
                            {
                                cookId = m_ItemData.Source[0][1];
                            }
                        }
                        UIManager.OpenUI(EUIID.UI_Knowledge_Cooking, false, cookId);
                    }
                }
                else if (cSVItemSourceData.Type == 26)
                {
                    if (m_ItemData != null)
                    {
                        UIManager.CloseUI(m_EOwnerTips);
                        if (UIManager.IsOpen(EUIID.UI_Bag))
                        {
                            UI_Bag uiBag = UIManager.GetUI((int)EUIID.UI_Bag) as UI_Bag;
                            if (uiBag != null)
                            {
                                uiBag.SetMainBagTabIndex(4);
                            }
                            //UIManager.CloseUI(EUIID.UI_Prop_Message);
                        }
                        else
                        {
                            UIManager.OpenUI(EUIID.UI_Bag,false,(4 << 16) | 1 );
                        }
                    }
                }
                else if (cSVItemSourceData.Type == 999) //单纯打开界面(不带参数)
                {
                    UIManager.CloseUI(m_EOwnerTips);
                    UIManager.OpenUI((EUIID) cSVItemSourceData.Parameter[0]);
                }
            }
        }

        // 当用于跳转到具体哪个道具的时候,策划第二个参数不配,表示跳转到他自己本身，配了则表示跳转到配置的道具id
        private uint GetGoToItemId(uint itemId, uint sourceId)
        {
            var datas = CSVItem.Instance.GetConfData(itemId).Source;
            uint resId = itemId;
            for (int i = 0; i < datas.Count; i++)
            {
                if (datas[i][0] == sourceId)
                {
                    resId = datas[i][1] == 0 ? itemId : datas[i][1];
                    break;
                }
            }

            return resId;
        }

        private uint GetAchId(uint itemId, uint sourceId)
        {
            var datas = CSVItem.Instance.GetConfData(itemId).Source;
            uint resId = itemId;
            for (int i = 0; i < datas.Count; i++)
            {
                if (datas[i][0] == sourceId)
                {
                    resId = datas[i][1] == 0 ? 0 : datas[i][1];
                    break;
                }
            }

            return resId;
        }

        private bool CalFashionLuckyDraw(uint itemId)
        {
            var datas = CSVItem.Instance.GetConfData(itemId).Source;

            for (int i = 0; i < datas.Count; i++)
            {
                uint activityId = datas[i][1];

                if (activityId == 0)
                {
                    continue;
                }

                if (activityId == Sys_Fashion.Instance.activeId)
                {
                    return true;
                }
            }

            return false;
        }

        private bool CalBattlePass(uint itemId)
        {
            var datas = CSVItem.Instance.GetConfData(itemId).Source;

            for (int i = 0; i < datas.Count; i++)
            {
                uint activityId = datas[i][1];

                if (!Sys_BattlePass.Instance.isActive)
                {
                    continue;
                }

                if (activityId == Sys_BattlePass.Instance.BranchID)
                {
                    return true;
                }
            }

            return false;
        }


        public class SourceItemCeil
        {
            private Transform transform;
            private Image icon;
            private Text name;
            public uint itemSourceId;
            private Action<SourceItemCeil> onClick;
            private Image eventBg;

            public void BindGameObject(GameObject go)
            {
                transform = go.transform;
                ParseComponent();
            }

            private void ParseComponent()
            {
                icon = transform.Find("PropItem/Btn_Item/Image_Icon").GetComponent<Image>();
                name = transform.Find("Text_Name").GetComponent<Text>();
                eventBg = transform.Find("EventBG").GetComponent<Image>();
                Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventBg);
                eventListener.AddEventListener(EventTriggerType.PointerClick, OnGridClicked);
            }

            public void AddClickListener(Action<SourceItemCeil> _onClick)
            {
                onClick = _onClick;
            }

            private void OnGridClicked(BaseEventData baseEventData)
            {
                onClick.Invoke(this);
            }


            public void SetData(uint _id)
            {
                this.itemSourceId = _id;
                Refresh();
            }

            private void Refresh()
            {
                icon.enabled = true;
                ImageHelper.SetIcon(icon, CSVItemSource.Instance.GetConfData(itemSourceId).Icon_id);
                TextHelper.SetText(name, CSVItemSource.Instance.GetConfData(itemSourceId).Name_id);
            }
        }
    }
}