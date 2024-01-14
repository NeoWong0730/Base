using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;
using Packet;
using System;
using static Logic.Sys_Equip;

namespace Logic
{
    public class UI_SelectItemParam
    {
        public uint tittle_langId;
        public uint getAwayId;
        public List<ItemData> itemDatas;
        public uint petUid;
        public uint selectRemakePointIndex;// 选中的改造次数
    }

    public class UI_SelectItem_Layout 
    {
        public Transform transform;
        public Button closeBtn;
        public Transform itemScroll;
        public Button getAwayBtn;
        public Text tittleText;
        public void Init(Transform transform)
        {
            this.transform = transform;
            closeBtn = transform.Find("Blank (1)").GetComponent<Button>();
            itemScroll = transform.Find("Animator/Scroll_View/Grid");
            tittleText = transform.Find("Animator/Text").GetComponent<Text>();
            getAwayBtn = transform.Find("Animator/Add").GetComponent<Button>();
        }

        public void SetTittle(uint langId)
        {
            TextHelper.SetText(tittleText, langId);
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OncloseBtnClicked);
            getAwayBtn.onClick.AddListener(listener.OnGetAwayBtnClicked);
        }

        public interface IListener
        {
            void OncloseBtnClicked();
            void OnGetAwayBtnClicked();
        }
    }

    public class UI_Pet_UseItem : UIComponent
    {
        private PropItem propItem;
        public Text itemname;
        public Text itemmessage;
        public Button itemBtn;
        public Action<ItemData> action;
        private ItemData itemData;
        protected override void Loaded()
        {
            propItem = new PropItem();
            propItem.BindGameObject(transform.Find("PropItem").gameObject);           
            itemname = transform.Find("Text_Name").GetComponent<Text>();
            itemmessage = transform.Find("Text").GetComponent<Text>();            
            itemBtn = transform.GetComponent<Button>();
            itemBtn.onClick.AddListener(OnitemBtnClicked);
        }

        public void AddAction(Action<ItemData> action)
        {
            this.action = action;
        }

        public void RefreshItem(ItemData itemdata)
        {
            itemData = itemdata;
            propItem.Layout.imgIcon.gameObject.SetActive(true);
            propItem.SetData(new MessageBoxEvt(EUIID.UI_SelectItem, new PropIconLoader.ShowItemData(itemData.cSVItemData.id, itemdata.Count, true, false, false, false, false, false, false, true)));
            propItem.txtNumber.gameObject.SetActive(true);
            itemname.text = LanguageHelper.GetTextContent(itemdata.cSVItemData.name_id);
        }

        private void OnitemBtnClicked()
        {
            action?.Invoke(itemData);
        }
    }

    public class UI_SelectItem : UIBase,UI_SelectItem_Layout.IListener
    {
        private UI_SelectItem_Layout layout = new UI_SelectItem_Layout ();
        private List<UI_Pet_UseItem> itemlist = new List<UI_Pet_UseItem>();

        private UI_SelectItemParam param;

        private InfinityGridLayoutGroup infinity;
        private Dictionary<GameObject, UI_Pet_UseItem> itemCeilGrids = new Dictionary<GameObject, UI_Pet_UseItem>();
        private int infinityCount;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            infinity = layout.itemScroll.GetNeedComponent<InfinityGridLayoutGroup>();
            infinity.minAmount = 6;
            infinity.updateChildrenCallback = UpdateChildrenCallback;
            IntItem();
        }

        protected override void OnOpen(object arg)
        {
            param = arg as UI_SelectItemParam;
        }

        protected override void OnShow()
        {
            SetItemView();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnChangeItemCount, OnChangeItemCount, toRegister);
        }

        private void IntItem()
        {
            for (int i = 0; i < layout.itemScroll.childCount; i++)
            {
                GameObject go = layout.itemScroll.GetChild(i).gameObject;
                UI_Pet_UseItem itemCeil = new UI_Pet_UseItem();
                itemCeil.Init(go.transform);
                itemCeil.AddAction(OnItemClick);
                itemCeilGrids.Add(go, itemCeil);
            }
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= infinityCount)
                return;
            if (itemCeilGrids.ContainsKey(trans.gameObject))
            {
                UI_Pet_UseItem itemCeil = itemCeilGrids[trans.gameObject];
                itemCeil.RefreshItem(param.itemDatas[index]);
            }
        }

        private void OnChangeItemCount()
        {
            if(null != param)
            {
                SetItemView();
            }
        }

        protected override void OnClose()
        {
            param = null;
        }

        private void SetItemView()
        {
            if(param.tittle_langId != 0)
            {
                layout.SetTittle(param.tittle_langId);
            }
            List<ItemData> _itemDatas1 = new List<ItemData>(); 
            uint typeid = param.getAwayId;
            if (typeid == (uint)EItemType.PetRemakeSkillBook)
            {
                _itemDatas1 = Sys_Bag.Instance.GetItemDatasByItemType((uint)EItemType.PetRemakeSkillBook);
                if (_itemDatas1.Count > 1)
                {
                    _itemDatas1.Sort(SortSkillLevel);
                }
            }
            else if(typeid == (uint)EItemType.PetSkillBook)
            {
                _itemDatas1 = Sys_Bag.Instance.GetItemDatasByItemType((uint)EItemType.PetSkillBook);
                if (_itemDatas1.Count > 0)
                {
                    _itemDatas1.Sort(SkillItemComp);
                }
            }
            else if(typeid== (uint) EItemType.PetFruitItem)
            {
                _itemDatas1 = Sys_Bag.Instance.GetItemDatasByItemType((uint)EItemType.PetFruitItem);
            }
            else if(typeid == 3010)
            {
                _itemDatas1.AddRange(Sys_Bag.Instance.GetItemDatasByItemType(3027));
                _itemDatas1.AddRange(Sys_Bag.Instance.GetItemDatasByItemType(3010));
            }
            param.itemDatas = _itemDatas1;
            infinityCount = param.itemDatas.Count;
            infinity.SetAmount(infinityCount);            
        }

        private int SkillItemComp(ItemData a, ItemData b)
        {
            return (int)a.cSVItemData.lv - (int)b.cSVItemData.lv;
        }

        private int SortSkillLevel(ItemData a, ItemData b)
        {
            return (int)a.cSVItemData.lv - (int)b.cSVItemData.lv;
        }

        public void OnItemClick(ItemData itemData)
        {
            if (itemData.cSVItemData.type_id == 3015)
            {
                //1级书才可以学习
                if (itemData.cSVItemData.lv > 1)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11800));
                    return;
                }
                ClientPet clientPet = Sys_Pet.Instance.GetFightPetClient(param.petUid);
                if (null != clientPet && null != itemData.cSVItemData.fun_value && itemData.cSVItemData.fun_value.Count >= 2)
                {
                    if (clientPet.IsHaveSameSkill((uint)Math.Ceiling(itemData.cSVItemData.fun_value[1] / 1000.0f)))
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11625));
                        return;
                    }
                }
            }
            /*else if (itemData.cSVItemData.type_id == 3014)
            {
                ClientPet clientPet = Sys_Pet.Instance.GetFightPetClient(param.petUid);
                if (null != clientPet && null != itemData.cSVItemData.fun_value && itemData.cSVItemData.fun_value.Count >= 2)
                {
                    
                    if (clientPet.IsSameOrHighBuildSkill(itemData.cSVItemData.fun_value[1]))
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11625));
                        return;
                    }
                }
            }*/
            else if (itemData.cSVItemData.type_id == 3010)
            {
                ClientPet client = Sys_Pet.Instance.GetFightPetClient(param.petUid);
                if (null != client)
                {
                    int canRemakeCount = client.GetPetLevelCanRemakeTimes();
                    if(param.selectRemakePointIndex > canRemakeCount)
                    {
                        return;
                    }
                    int willBuildCount = (int)param.selectRemakePointIndex;
                    var items = Sys_Pet.Instance.GetCanUseLowsRemakeItems(willBuildCount);
                    bool isCanUse = false;
                    for (int i = 0; i < items.Count; i++)
                    {
                        if(items[i] == itemData.cSVItemData.id)
                        {
                            isCanUse = true;
                        }
                    }
                    if (!isCanUse)
                    {
                        if (items.Count > 0)
                        {
                            var lowsRemakItem = CSVItem.Instance.GetConfData(items[0]);
                            if (null != lowsRemakItem)
                            {
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12043, LanguageHelper.GetTextContent(lowsRemakItem.name_id)));
                                return;
                            }

                        }
                    }
                }
            }
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnSelectItem, itemData.cSVItemData.id);
            UIManager.CloseUI(EUIID.UI_SelectItem);
        }
        
        public void OncloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_SelectItem);
        }

        public void OnGetAwayBtnClicked()
        {
            if(null != param)
            {
                uint id = param.getAwayId;

                if (id == 3014)
                {
                    CSVPetNewParam.Data cSVPetParameterData = CSVPetNewParam.Instance.GetConfData(35u);
                    if (null != cSVPetParameterData)
                    {
                        Sys_Trade.Instance.FindCategory(cSVPetParameterData.value);
                        OncloseBtnClicked();
                    }
                }
                else
                {
                    if (Sys_FamilyResBattle.Instance.InFamilyBattle)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3230000059));
                        return;
                    }
                    else if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
                    {
                        Sys_Hint.Instance.PushForbidOprationInFight();  //战斗内提示：当前处于战斗中，无法进行该操作
                        return;
                    }

                    if (id == 3015)
                    {
                        PropIconLoader.ShowItemData iItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, false, false);

                        iItemData.id = 100400;

                        var boxEvt = new MessageBoxEvt(EUIID.UI_Pet_Message, iItemData);

                        boxEvt.b_ForceShowScource = true;
                        boxEvt.b_ShowItemInfo = false;
                        UIManager.OpenUI(EUIID.UI_Message_Box, false, boxEvt);
                    }
                    else if (id == 3010)
                    {
                        //uint petNewParamId = 31;
                        PropIconLoader.ShowItemData iItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, false, false);

                        iItemData.id = 100499;

                        var boxEvt = new MessageBoxEvt(EUIID.UI_Pet_Message, iItemData);

                        boxEvt.b_ForceShowScource = true;
                        boxEvt.b_ShowItemInfo = false;
                        UIManager.OpenUI(EUIID.UI_Message_Box, false, boxEvt);
                    }
                    else if (id == 3035)
                    {
                        //uint petNewParamId = 31;
                        PropIconLoader.ShowItemData iItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, false, false);

                        iItemData.id = 100500;

                        var boxEvt = new MessageBoxEvt(EUIID.UI_Pet_Message, iItemData);

                        boxEvt.b_ForceShowScource = true;
                        boxEvt.b_ShowItemInfo = false;
                        UIManager.OpenUI(EUIID.UI_Message_Box, false, boxEvt);
                    }
                }
                
            }
        }
    }
}
