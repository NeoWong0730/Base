using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using Table;
using System.Collections.Generic;
using Lib.Core;
using System.Text;
using System;

namespace Logic
{
    public class UI_Pet_SkillStudyParam
    {
        public uint petUid;
        public uint skillId;
    }

    public class UI_Pet_SkillStudy_Layout
    {
        private Button closeBtn;
        public Transform messageView;
        public void Init(Transform transform)
        {
            closeBtn = transform.Find("Animator/GameObject/View_TipsBgNew07/Btn_Close").GetComponent<Button>();
            messageView = transform.Find("Animator");
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OncloseBtnClicked);
        }

        public interface IListener
        {
            void OncloseBtnClicked();
        }

    }

    public class UI_Pet_SkillStudyMessage
    {
        // 技能信息
        private Image skillImage;
        private Text skillNameText;
        private Text skillCostText;
        private Text expPercentText;
        private Slider expSlider;
        private RectTransform sliderRect;
        private Text skillLevelText;
        private Text addExpText;
        //技能描述
        private Text cdText;
        private Text skillCurrentDescText;
        private Text skillNextDescText;
        private GameObject skillNextDescParentGo;
        //强化类型相关
        private GameObject fullGo; // 满级
        private GameObject UniqueSkillUpTipsGo;
        private Transform upGradeitem; // 升级需求道具
        private Text levelExpText;
        private Text upGradeTipText;
        private Transform moreItem;
        private Dropdown dropdown;
        private Button quickAddBtn;
        private Button forgetBtn;
        private Button upBtn;
        private Text upText;
        private Button useSilverButton;
        private GameObject useSilverCheckMark;
        private Animator ani;

        private List<uint> petSkillLevels;
        private List<ulong> _itemUids = new List<ulong>();
        private List<long> _itemNums = new List<long>();
        private List<ulong> _grid2Dic = new List<ulong>();
        private List<ulong> itemUids = new List<ulong>();
        private List<long> itemNums = new List<long>();
        private List<ulong> grid2Dic = new List<ulong>();
        private List<PropItem> upExpItemDic = new List<PropItem>();
        private InfinityGridLayoutGroup infinity;
        private Dictionary<GameObject, PropItem> propItemGrids = new Dictionary<GameObject, PropItem>();
        private Dictionary<GameObject, PropItem> upGradeitemDic = new Dictionary<GameObject, PropItem>();
        private List<PropItem> propItems = new List<PropItem>();
        private List<ItemData> chooseItemDatas = new List<ItemData>();
        private List<ulong> hSelectItem = new List<ulong>();
        private List<uint> petBookExp;
        private List<uint> useSliverPer = new List<uint>(2);
        private int chooseType = -1;
        private uint skillId;
        private uint petUid;
        private bool isEnoughItem = false;
        private int type = 1; // 1满级  2升级 3 经验添加
        private bool itemViewShow;
        private float maxSliderWidth;
        private readonly uint maxItemGrid = 5;
        private bool _isUseSliver;
        private bool isUseSliver
        {
            get
            {
                return _isUseSliver;
            }
            set
            {
                _isUseSliver = value;
                useSilverCheckMark.SetActive(_isUseSliver);
            }
        }
        private long useSilverNum;
        public void Init(Transform transform)
        {
            ani = transform.GetComponent<Animator>();
            skillImage = transform.Find("GameObject/View_Message/Message/Skill_Icon").GetComponent<Image>();
            skillNameText = transform.Find("GameObject/View_Message/Message/Text_SkillName").GetComponent<Text>();
            skillCostText = transform.Find("GameObject/View_Message/Message/Text_Mp/Text_Mp_Num").GetComponent<Text>();
            expPercentText = transform.Find("GameObject/View_Message/Message/Text_Percent").GetComponent<Text>();
            expSlider = transform.Find("GameObject/View_Message/Message/Slider_Lv").GetComponent<Slider>();
            sliderRect = transform.Find("GameObject/View_Message/Message/Slider_Lv/Image1").GetComponent<RectTransform>();
            maxSliderWidth = sliderRect.rect.width;
            skillLevelText = transform.Find("GameObject/View_Message/Message/Image_Lv/Text").GetComponent<Text>();
            addExpText = transform.Find("GameObject/View_Message/Message/Text_Up/Text_Mp_Num").GetComponent<Text>();
            cdText = transform.Find("GameObject/View_Message/Skill_Des/Des1/Text1/Text_Tips02").GetComponent<Text>();
            skillCurrentDescText = transform.Find("GameObject/View_Message/Skill_Des/Des2/Text2/Scroll/Text_Tips02").GetComponent<Text>();
            skillNextDescText = transform.Find("GameObject/View_Message/Skill_Des/Des3/Text2 (1)/Scroll/Text_Tips02").GetComponent<Text>();
            skillNextDescParentGo = transform.Find("GameObject/View_Message/Skill_Des/Des3").gameObject;

            // 满
            fullGo = transform.Find("GameObject/View_Message/Up_Grade/Full").gameObject;
            UniqueSkillUpTipsGo = transform.Find("GameObject/View_Message/Up_Grade/Text_Tips").gameObject;

            // 升级
            upGradeitem = transform.Find("GameObject/View_Message/Up_Grade/One/items");
            levelExpText = transform.Find("GameObject/View_Message/Up_Grade/One/Text_Tips1").GetComponent<Text>();
            upGradeTipText = transform.Find("GameObject/View_Message/Up_Grade/One/Text_Tips").GetComponent<Text>();

            //添加
            quickAddBtn = transform.Find("GameObject/View_Message/Up_Grade/More/Button_Add").GetComponent<Button>();
            quickAddBtn.onClick.AddListener(QuickAddBtn);

            //银币代替按钮
            useSilverButton = transform.Find("GameObject/View_Message/Up_Grade/More/Button").GetComponent<Button>();
            useSilverCheckMark = transform.Find("GameObject/View_Message/Up_Grade/More/Button/Checkmark").gameObject;
            useSilverButton.onClick.AddListener(OnUseSilverButtonClicked);

            CSVPetNewParam.Data perParam = CSVPetNewParam.Instance.GetConfData(42u);
            if (null != perParam)
            {
                useSliverPer = ReadHelper.ReadArray_ReadUInt(perParam.str_value, '|');
            }
            moreItem = transform.Find("GameObject/View_Message/Up_Grade/More/ItemGroup");
            grid2Dic.Clear();
            for (int i = 0; i < moreItem.childCount; i++)
            {
                GameObject go = moreItem.GetChild(i).gameObject;
                PropItem item = new PropItem();
                item.BindGameObject(go);
                PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(0, 0, false, false, false, false, false, true, false, true, ItemGridBeClicked, _bUseTips: false);
                itemN.guid = 0;
                item.SetData(itemN, EUIID.UI_Pet_SkillStudy);
                item.btnNone.gameObject.SetActive(true);
                upExpItemDic.Add( item);
                grid2Dic.Add(0);
                itemUids.Add(0);
                itemNums.Add(0);
                _grid2Dic.Add(0);
                _itemUids.Add(0);
                _itemNums.Add(0);
            }
            
            int max = 10;
            CSVPetNewParam.Data param = CSVPetNewParam.Instance.GetConfData(24);
            if (null != param)
            {
                max = (int)param.value;
            }
            petSkillLevels = new List<uint>();
            for (int i = 0; i < max; i++)
            {
                petSkillLevels.Add((uint)i);
            }
            dropdown = transform.Find("GameObject/View_Message/Up_Grade/More/View_PopupList/PopupList/DropDownList").GetComponent<Dropdown>();
            
            PopdownListBuild();
            //遗忘
            forgetBtn = transform.Find("GameObject/View_Message/Up_Grade/Button/Button_Forget").GetComponent<Button>();
            forgetBtn.onClick.AddListener(ForgetBtnClicked);
            //添加 升级
            upBtn = transform.Find("GameObject/View_Message/Up_Grade/Button/Button_Up").GetComponent<Button>();
            upBtn.onClick.AddListener(UpBtnClicked);
            upText = transform.Find("GameObject/View_Message/Up_Grade/Button/Button_Up/Text_01").GetComponent<Text>();

            infinity = transform.Find("View_Choose/Animator/Scroll_View_Bag/TabList").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            infinity.minAmount = 24;
            infinity.updateChildrenCallback = UpdateChildrenCallback;

            for (int i = 0; i < infinity.transform.childCount; i++)
            {
                GameObject go = infinity.transform.GetChild(i).gameObject;
                PropItem item = new PropItem();
                item.BindGameObject(go);
                propItemGrids.Add(go, item);
                propItems.Add(item);
            }
        }

        private void OnUseSilverButtonClicked()
        {
            if(isUseSliver)
            {
                isUseSliver = false;
                SetAddExpValueView();
            }
            else
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(11838); //勾选后点击添加经验将消耗会使用等价银币作为替换。请问是否确认？
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    isUseSliver = true;
                    SetAddExpValueView();
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
        }

        public void OpenItemView()
        {
            if (!itemViewShow)
            {
                ani.Play("Choose_Open", -1, 0f);
                itemViewShow = true;
            }
        }

        public void CloseItemView()
        {
            if (itemViewShow)
            {
                ani.Play("Choose_Close", -1, 0f);
                itemViewShow = false;
            }
        }

        public ulong selectItemUid;
        private PropItem selectItem;
        private void RigitItemClicked(PropItem item)
        {
            if(!itemViewShow)
            {
                return;
            }
            ulong uid = item.ItemData.guid;
            if(uid == 0)
            {

                PropIconLoader.ShowItemData iItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, false, false);

                iItemData.id = 100400;

                var boxEvt = new MessageBoxEvt(EUIID.UI_Pet_Message, iItemData);
                boxEvt.b_changeSourcePos = true;
                boxEvt.pos = item.transform.position;
                boxEvt.b_ForceShowScource = true;
                boxEvt.b_ShowItemInfo = false;
                UIManager.OpenUI(EUIID.UI_Message_Box, false, boxEvt);
                /*CSVPetNewParam.Data cSVPetParameterData = CSVPetNewParam.Instance.GetConfData(33u);
                if (null != cSVPetParameterData)
                {
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(cSVPetParameterData.value);
                    UIManager.CloseUI(EUIID.UI_Pet_SkillStudy);
                    UIManager.CloseUI(EUIID.UI_Pet_Message, needDestroy: false);
                }*/
                return;
            }
            if (hSelectItem.Contains(uid))
            {
                for (int i = 0; i < itemUids.Count; i++)
                {
                    ulong _uid = itemUids[i];
                    if (uid == _uid)
                    {
                        itemUids[i] = 0;
                        itemNums[i] = 0;
                        grid2Dic[i] = 0;
                        hSelectItem.Remove(uid);
                        item.SetGot(false);
                    }
                }
                SetItemGrids();
            }
            else
            {
                if (!IsMaxSelect())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10912));
                }
                else
                {
                    selectItem = item;
                    selectItemUid = uid;
                    UIManager.OpenUI(EUIID.UI_Pet_Usage, false, new UI_Pet_Usage.UI_Pet_UsageParam { itemUid = uid, action = SelectItem });
                }
            }
        }    

        private void SelectItem(long itemNum)
        {
            uint curentLV = GetSkillLevel();
            if (curentLV < selectItem.ItemData.CSV.lv)
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(11016);
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    RightItemDataClick(selectItem, itemNum);

                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
            else
            {
                RightItemDataClick(selectItem, itemNum);
            }
        }
        
        private void RightItemDataClick(PropItem item, long itemCount = 0)
        {
            ulong uid = item.ItemData.guid;
            if (itemUids.Contains(uid))
            {
                for (int i = 0; i < itemUids.Count; i++)
                {
                    ulong _uid = itemUids[i];
                    if (uid == _uid)
                    {
                        itemNums[i] = itemCount;
                        hSelectItem.Add(uid);
                        item.SetGot(true);
                    }
                }
            }
            else
            {
                if (!IsMaxSelect())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10912));
                }
                else
                {
                    for (int i = 0; i < itemUids.Count; i++)
                    {
                        if (itemUids[i] == 0)
                        {
                            itemUids[i] = item.ItemData.guid;
                            itemNums[i] = itemCount;
                            grid2Dic[i] = item.ItemData.guid;
                            hSelectItem.Add(item.ItemData.guid);
                            item.SetGot(true);
                            break;
                        }
                    }
                }
            }
           
            SetItemGrids();
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= chooseItemDatas.Count)
                return;
            if (propItemGrids.ContainsKey(trans.gameObject))
            {
                PropItem item = propItemGrids[trans.gameObject];
                ItemData data = chooseItemDatas[index];
                PropIconLoader.ShowItemData itemN;
                if (data.Id == 0)
                {
                    itemN = new PropIconLoader.ShowItemData(0, 0, false, false, false, false, false, true, false, true, RigitItemClicked, _bUseTips: false);
                    itemN.guid = data.Uuid;
                    item.SetData(itemN, EUIID.UI_Pet_SkillStudy);
                    item.btnNone.gameObject.SetActive(true);
                    item.Layout.imgIcon.enabled = false;
                    item.Layout.imgQuality.gameObject.SetActive(false);
                    item.txtName.gameObject.SetActive(false);
                    item.txtNumber.gameObject.SetActive(false);
                    item.SetGot(false);
                }
                else
                {
                    itemN = new PropIconLoader.ShowItemData(data.Id, data.Count, true, false, false, false, false, true, false, true, RigitItemClicked, _bUseTips: false);
                    itemN.guid = data.Uuid;
                    item.SetData(itemN, EUIID.UI_Pet_SkillStudy);
                    TextHelper.SetText(item.txtName, data.cSVItemData.name_id);
                    item.txtNumber.gameObject.SetActive(true);
                    item.txtName.gameObject.SetActive(true);
                    item.SetGot(hSelectItem.Contains(data.Uuid));
                }
            }
        }

        public string GetSkillForgetRetrunItem(uint exp)
        {
            int num = 0;
            CSVItem.Data item = CSVItem.Instance.GetConfData(Sys_Pet.Instance.PetSkillExpItemId);
            
            if (item.fun_parameter == "PetskillBook" && item.fun_value != null && item.fun_value.Count >= 2)
            {
                CSVPetNewParam.Data csvNewParam = CSVPetNewParam.Instance.GetConfData(39u);
                if(null != csvNewParam)
                {
                    num = (int)(exp * (csvNewParam.value + 0f) / 10000.0f / (item.fun_value[1] + 0f));
                }
                return LanguageHelper.GetTextContent(11665, LanguageHelper.GetLanguageColorWordsFormat(CSVLanguage.Instance.GetConfData(item.name_id).words, TextHelper.GetQuailtyLangId(item.quality)), num.ToString());
            }
            return "";
        }

        private void ForgetBtnClicked()
        {
            ClientPet currentPet = Sys_Pet.Instance.GetPetByUId(petUid);
            bool isUni = currentPet.IsUniqueSkill(skillId);
            uint type = isUni ? 1u : 2u;
            PromptBoxParameter.Instance.Clear();
            if (isUni)
            {
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(10964);
            }
            else
            {
                CSVPetNewSkillsLv.Data csv = CSVPetNewSkillsLv.Instance.GetConfData(level);
                uint totalExp = 0;
                if(null != csv)
                {
                    totalExp = csv.total_exp;
                }
                PromptBoxParameter.Instance.content = GetSkillForgetRetrunItem(currentPet.GetPetSkillExp(skillId) + totalExp);
            }
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                if (currentPet != null)
                {
                    Sys_Pet.Instance.OnCmdPetRemoveSkillReq(petUid, isUni ? currentPet.GetBaseUniqueSkillIdByupGradeSkillId(skillId) : skillId, type);
                    UIManager.CloseUI(EUIID.UI_Pet_SkillStudy);
                }
            }, 8000);
            PromptBoxParameter.Instance.SetCancel(true, null);
            PromptBoxParameter.Instance.SetCountdown(3f, PromptBoxParameter.ECountdown.Confirm, PromptBoxParameter.ECountDownType.SetEnable);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        private ItemData CreateItemData()
        {
            return new ItemData();
        }

        private void ItemGridBeClicked(PropItem item)
        {
            if(item.ItemData.id == 0)
            {
                if(!itemViewShow)
                {
                    OpenItemView();
                    chooseItemDatas = Sys_Bag.Instance.GetItemDatasByItemType(3023);
                    chooseItemDatas.AddRange(Sys_Bag.Instance.GetItemDatasByItemType((uint)EItemType.PetSkillBook,
                    new List<Func<ItemData, bool>>
                        {
                            (_item) => {return !Sys_Pet.Instance.DontBeUseUpGradePetSkillExpList.Contains(_item.Id); },
                        }));
                    chooseItemDatas.Insert(0, CreateItemData());
                    hSelectItem.Clear();
                    hSelectItem.AddRange(itemUids);
                    infinity.SetAmount(chooseItemDatas.Count);
                }
            }
            else
            {
                for (int i = 0; i < grid2Dic.Count; i++)
                {
                    ItemData itemData =  Sys_Bag.Instance.GetItemDataByUuid(grid2Dic[i]);
                    if(null != itemData && itemData.Id == item.ItemData.id)
                    {
                        hSelectItem.Remove(itemData.Uuid);
                        grid2Dic[i] = 0;
                        itemUids[i] = 0;
                        itemNums[i] = 0;
                        PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(0, 0, false, false, false, false, false, true, false, true, ItemGridBeClicked, _bUseTips: false);
                        itemN.guid = 0;
                        item.SetData(itemN, EUIID.UI_Pet_SkillStudy);
                        item.btnNone.gameObject.SetActive(true);
                        item.txtNumber.gameObject.SetActive(false);
                        item.Layout.imgIcon.enabled = false;
                        item.Layout.imgQuality.gameObject.SetActive(false);
                        SetAddExpValueView();
                    }
                }
            }
            if (itemViewShow)
                ResetItemGridState();
        }

        private void ResetItemGridState()
        {
            for (int i = 0; i < propItems.Count; i++)
            {
                PropItem item = propItems[i];
                if(null != item.ItemData)
                {
                    if(item.ItemData.id == 0)
                    {
                        item.SetGot(false);
                    }
                    else
                    {
                        item.SetGot(hSelectItem.Contains(item.ItemData.guid));
                    }
                    
                }
            }            
        }

        private void UpBtnClicked()
        {
            if(type == 2) // 升级判断
            {
                CSVPetNewSkillsLv.Data csv = CSVPetNewSkillsLv.Instance.GetConfData(level);
                ClientPet currentPet = Sys_Pet.Instance.GetPetByUId(petUid);
                if (null!= currentPet && null != csv)
                {
                    var petLevel = currentPet.petUnit.SimpleInfo.Level;
                    if(petLevel < csv.pet_level)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11784, csv.pet_level.ToString()));
                        return;
                    }
                }

                if (isEnoughItem)
                {
                    Sys_Pet.Instance.OnPetSkillLevelUpReq(petUid, skillId);
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10913));
                }
            }
            else if(type == 3) // 经验判断
            {
                if(isUseSliver)
                {
                    bool hasItem = HasItems();
                    bool hasSilver = useSilverNum > 0;
                    if (hasItem || hasSilver)
                    {
                        if(hasSilver)
                        {
                            PromptBoxParameter.Instance.Clear();
                            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(11839, expAdd.ToString(), useSilverNum.ToString()); //勾选后点击添加经验将消耗会使用等价银币作为替换。请问是否确认？
                            PromptBoxParameter.Instance.SetConfirm(true, () =>
                            {
                                Sys_Pet.Instance.OnCmdPetSkillAddExpReq(petUid, skillId, itemUids, itemNums, (uint)useSilverNum);
                            });
                            PromptBoxParameter.Instance.SetCancel(true, null);
                            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                        }
                        else
                        {
                            Sys_Pet.Instance.OnCmdPetSkillAddExpReq(petUid, skillId, itemUids, itemNums, (uint)useSilverNum);
                        }
                    }
                    else
                    {
                        if(!hasSilver)
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12104));
                        }
                        else if(!hasItem)
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10914));
                        }
                    }
                }
                else
                {
                    if (HasItems())
                    {
                        Sys_Pet.Instance.OnCmdPetSkillAddExpReq(petUid, skillId, itemUids, itemNums, 0);
                    }
                    else
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10914));
                    }
                }
                
            }
        }

        private bool HasItems()
        {
            for (int i = 0; i < itemUids.Count; i++)
            {
                if(itemUids[i] != 0)
                {
                    return true;
                }
            }

            return false;
        }

        uint level = 0;

        //TODO
        public void SetSkillData(uint _skillId , uint _petUid)
        {
            SetItemByNeedExp(0);
            if (itemViewShow)
            {
                chooseItemDatas = Sys_Bag.Instance.GetItemDatasByItemType(3023);
                chooseItemDatas.AddRange(Sys_Bag.Instance.GetItemDatasByItemType((uint)EItemType.PetSkillBook,
                    new List<Func<ItemData, bool>>
                        {
                            (_item) => {return !Sys_Pet.Instance.DontBeUseUpGradePetSkillExpList.Contains(_item.Id); },
                        }));
                chooseItemDatas.Insert(0, CreateItemData());
                infinity.SetAmount(chooseItemDatas.Count);
            }
            petUid = _petUid;
            ClientPet currentPet = Sys_Pet.Instance.GetPetByUId(petUid);
            skillId = currentPet.GetPetSkill(_skillId);            
           
            List<uint> _itemIds = new List<uint>();
            List<uint> _itemNum = new List<uint>();
            
            //ClientPet currentPet = Sys_Pet.Instance.DebutPet.GetClientPet();
            bool isUnique = currentPet.IsUniqueSkill(skillId);

            bool isActiveSkill = Sys_Skill.Instance.IsActiveSkill(skillId);
            skillCostText.transform.parent.gameObject.SetActive(isActiveSkill);

            bool isShowMax = false;
            bool upNextIsMax = false;
            if (isActiveSkill) //主动技能
            {
                CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);                
                if (skillInfo != null)
                {
                    ImageHelper.SetIcon(skillImage, skillInfo.icon);
                    skillNameText.text = LanguageHelper.GetTextContent(skillInfo.name);
                    TextHelper.SetText(skillCurrentDescText, Sys_Skill.Instance.GetSkillDesc(skillId));
                    CSVActiveSkill.Data cSVActiveSkillData = CSVActiveSkill.Instance.GetConfData(skillId);
                    level = skillInfo.level;                    
                    if (null != cSVActiveSkillData)
                    {
                        if(cSVActiveSkillData.cold_time == 0)
                        {
                            cdText.text = LanguageHelper.GetTextContent(10915);
                        }
                        else
                        {
                            uint baseLangId = 10915;
                            cdText.text = LanguageHelper.GetTextContent(10915, (cSVActiveSkillData.cold_time + baseLangId).ToString());
                        }                        
                        skillCostText.text = cSVActiveSkillData.mana_cost.ToString();
                    }
                    else
                    {
                        DebugUtil.LogFormat(ELogType.eNone, "not found skillId={0} in activeSkill", skillId);
                    }
                }
                else
                {
                    DebugUtil.LogFormat(ELogType.eNone, "not found skillId={0} in skillInfo", skillId);
                }

                uint nextSkillId = skillId + 1u;
                CSVActiveSkillInfo.Data nextSkillInfo = CSVActiveSkillInfo.Instance.GetConfData(nextSkillId);
                if (null != nextSkillInfo && null != nextSkillInfo.upgrade_cost)
                {
                    for (int i = 0; i < nextSkillInfo.upgrade_cost.Count; i++)
                    {
                        _itemIds.Add(nextSkillInfo.upgrade_cost[i][0]);
                        _itemNum.Add(nextSkillInfo.upgrade_cost[i][1]);
                    }
                }

                isShowMax = nextSkillInfo == null || skillInfo.skill_type == 1;
                if (!isShowMax)
                {
                    upNextIsMax = CSVActiveSkillInfo.Instance.GetConfData(nextSkillId + 1u) != null;
                    TextHelper.SetText(skillNextDescText, Sys_Skill.Instance.GetSkillDesc(nextSkillId));
                }
                skillNextDescParentGo.gameObject.SetActive(!isShowMax /*&& !isUnique*/);
            }
            else
            {
                CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                if (skillInfo != null)
                {
                    ImageHelper.SetIcon(skillImage, skillInfo.icon);
                    skillNameText.text = LanguageHelper.GetTextContent(skillInfo.name);
                    skillCurrentDescText.text = LanguageHelper.GetTextContent(skillInfo.desc);
                    level = skillInfo.level;                    
                }
                else
                {
                    DebugUtil.LogFormat(ELogType.eNone, "not found skillId={0}", skillId);
                }
                cdText.text = LanguageHelper.GetTextContent(10915);
                uint nextSkillId = skillId + 1u;
                CSVPassiveSkillInfo.Data nextSkillInfo = CSVPassiveSkillInfo.Instance.GetConfData(nextSkillId);

                if (null != nextSkillInfo && null != nextSkillInfo.upgrade_cost)
                {
                    for (int i = 0; i < nextSkillInfo.upgrade_cost.Count; i++)
                    {
                        _itemIds.Add(nextSkillInfo.upgrade_cost[i][0]);
                        _itemNum.Add(nextSkillInfo.upgrade_cost[i][1]);
                    }
                }

                isShowMax = nextSkillInfo == null;
                if (!isShowMax)
                {
                    upNextIsMax = CSVPassiveSkillInfo.Instance.GetConfData(nextSkillId + 1) != null;
                    TextHelper.SetText(skillNextDescText, LanguageHelper.GetTextContent(nextSkillInfo.desc));
                }
                skillNextDescParentGo.gameObject.SetActive(!isShowMax /*&& !isUnique*/);
            }
            skillLevelText.text = level.ToString();
            fullGo.SetActive(isShowMax /*|| isUnique*/);
            UniqueSkillUpTipsGo.gameObject.SetActive(isUnique && !isShowMax);
            upBtn.gameObject.SetActive(!isShowMax && !isUnique);            

            CSVPetNewSkillsLv.Data petSkillLevelData = CSVPetNewSkillsLv.Instance.GetConfData(level);

            float sliderValue = 0f;
            string percent = "";
            uint exp = 0u;
            uint configExp = 0u;
            if (null != petSkillLevelData)
            {
                //isShowMax &= (petSkillLevelData.exp == 0);
                if (isShowMax || isUnique) // 满级
                {                    
                    sliderValue = 1f;
                    percent = "";                   
                }
                else
                {
                    exp = currentPet.GetPetSkillExp(skillId);
                    configExp = petSkillLevelData.exp;
                    sliderValue = (exp + 0f) / configExp;
                    percent = string.Format("{0}/{1}", exp.ToString(), configExp.ToString());
                }
            }
            expSlider.value = sliderValue;
            expPercentText.text = percent;
            bool isCanUpGrade = sliderValue >= 1f;
            bool UpGrade = !isShowMax && isCanUpGrade;
            levelExpText.transform.parent.gameObject.SetActive(UpGrade && !isUnique);
            bool addExp = !isShowMax && !isCanUpGrade;
            quickAddBtn.transform.parent.gameObject.SetActive(addExp && !isUnique);

            if (isShowMax || isUnique)
            {
                type = 1;
            }
            else if (UpGrade)
            {
                type = 2;
            }
            else if (addExp)
            {
                type = 3;
            }
            if (type != 3)
                CloseItemView();

            if (UpGrade)
            {
                if(!upNextIsMax)
                {
                    levelExpText.text = LanguageHelper.GetTextContent(10921);
                }
                else
                {
                    levelExpText.text = LanguageHelper.GetTextContent(10922, (exp - configExp).ToString());
                }
                
                SetAddExpItemView(_itemIds, _itemNum);
            }
            SetAddExpValueView();
            SetButtonNameString();
            //if (itemViewShow)
            //    ResetItemGridState();
        }

        private void SetButtonNameString()
        {
            uint langId = 11017u;
            if (type == 2)
            {
                langId = 11018u;
            }
            TextHelper.SetText(upText, langId);
        }

        /// <summary>
        /// 设置升级界面提示
        /// </summary>
        /// <param name="itemIds"></param>
        /// <param name="itemNum"></param>
        private void SetAddExpItemView(List<uint> itemIds, List<uint> itemNum)
        {
            isEnoughItem = false;
            int count = itemIds.Count;
            StringBuilder stringBuilder = StringBuilderPool.GetTemporary();
            FrameworkTool.CreateChildList(upGradeitem, itemIds.Count);
            for (int i = 0; i < count; i++)
            {
                uint _itemId = itemIds[i];
               
                uint _itemNum = itemNum[i];
                CSVItem.Data data = CSVItem.Instance.GetConfData(_itemId);
                if (null != data)
                {
                    stringBuilder.Append(LanguageHelper.GetTextContent(10923, _itemNum.ToString()));
                    stringBuilder.Append(LanguageHelper.GetTextContent(data.name_id));
                    if(i < count - 1)
                    {
                        stringBuilder.Append(",");
                    }
                }
                long _itemBagNum = Sys_Bag.Instance.GetItemCount(_itemId);
                if (_itemBagNum >= _itemNum)
                {
                    isEnoughItem = true;
                }
                Transform transform = upGradeitem.GetChild(i);
                if (upGradeitemDic.ContainsKey(transform.gameObject))
                {
                    PropItem item = upGradeitemDic[transform.gameObject];
                    item.SetData(new PropIconLoader.ShowItemData(_itemId, itemNum[i],
                       true, false, false, false, false, _bShowCount: true, _bShowBagCount: true), EUIID.UI_Pet_SkillStudy);
                }
                else
                {
                    PropItem item = new PropItem();
                    item.BindGameObject(transform.gameObject);
                    upGradeitemDic.Add(transform.gameObject, item);
                    item.SetData(new PropIconLoader.ShowItemData(_itemId, itemNum[i],
                       true, false, false, false, false, _bShowCount: true, _bShowBagCount: true), EUIID.UI_Pet_SkillStudy);
                }
            }
           // TextHelper.SetText(skillCostText, LanguageHelper.GetTextContent(9999, StringBuilderPool.ReleaseTemporaryAndToString(stringBuilder)));
            TextHelper.SetText(upGradeTipText, LanguageHelper.GetTextContent(10924, StringBuilderPool.ReleaseTemporaryAndToString(stringBuilder)));

        }

        private void QuickAddBtn()
        {
            int bookLevel = chooseType + 1;
            ClientPet currentPet = Sys_Pet.Instance.GetPetByUId(petUid);
            CSVPetNewSkillsLv.Data petSkillLevelData = CSVPetNewSkillsLv.Instance.GetConfData(level);
            uint exp = 0u;
            uint configExp = 0u;
            if (null != petSkillLevelData)
            {
                exp = currentPet.GetPetSkillExp(skillId);
                configExp = petSkillLevelData.exp;
            }

            long needExp = configExp - exp;
            SetItemByNeedExp(needExp);
        }

        private void ResetGrid2Dic()
        {
            for (int i = 0; i < grid2Dic.Count; i++)
            {
                grid2Dic[i] = 0;
            }

            for (int i = 0; i < itemUids.Count; i++)
            {
                itemUids[i] = 0;
            }

            for (int i = 0; i < itemNums.Count; i++)
            {
                itemNums[i] = 0;
            }

            hSelectItem.Clear();
        }

        private void ResetTempGrid2Dic()
        {
            for (int i = 0; i < _grid2Dic.Count; i++)
            {
                _grid2Dic[i] = 0;
            }

            for (int i = 0; i < _itemUids.Count; i++)
            {
                _itemUids[i] = 0;
            }

            for (int i = 0; i < _itemNums.Count; i++)
            {
                _itemNums[i] = 0;
            }
        }

        private bool IsMaxSelect()
        {
            int count = 0;
            for (int i = 0; i < itemUids.Count; i++)
            {
                if(itemUids[i] != 0)
                {
                    count += 1;
                }
            }

            return maxItemGrid > count;
        }

        private bool IsMaxSelect2()
        {
            int count = 0;
            for (int i = 0; i < _itemUids.Count; i++)
            {
                if (_itemUids[i] != 0)
                {
                    count += 1;
                }
            }

            return maxItemGrid > count;
        }

        private void SetItemByNeedExp(long exp)
        {
            ResetTempGrid2Dic();
            List<ItemData> itemDatas = Sys_Bag.Instance.GetItemDatasByItemType(3023);
            itemDatas.AddRange(Sys_Bag.Instance.GetItemDatasByItemType((uint)EItemType.PetSkillBook,
                new List<Func<ItemData, bool>>
                        {
                            (item) => {return (item.cSVItemData.lv <= (chooseType +1)) && !Sys_Pet.Instance.DontBeUseUpGradePetSkillExpList.Contains(item.Id);},
                        }));
            if (itemDatas.Count > 1)
            {
                itemDatas.Sort(CompItem);
            }
            bool hasLvCond = false;
            uint currentLv = GetSkillLevel();
            for (int i = 0; i < itemDatas.Count; i++)
            {
                if (exp > 0 && IsMaxSelect2())
                {
                    ItemData data = itemDatas[i];
                    int lv = (int)data.cSVItemData.lv;
                    long _itemExp = 0;
                    if (data.cSVItemData.type_id != (uint)EItemType.PetSkillBook)
                    {
                        if (data.cSVItemData.id == Sys_Pet.Instance.PetSkillExpItemId)
                        {
                            if (data.cSVItemData.fun_parameter == "PetskillBook" && data.cSVItemData.fun_value != null && data.cSVItemData.fun_value.Count >= 2)
                            {
                                _itemExp = data.cSVItemData.fun_value[1];
                            }
                        }
                    }
                    else
                    {
                        _itemExp = GetBookExp(lv - 1);
                    }
                    long count = data.Count;
                    if (!hasLvCond)
                    {
                        hasLvCond = currentLv < lv;
                    }
                    long _needCout = (long)Math.Ceiling((exp + 0f) / _itemExp);
                    ulong uid = data.Uuid;
                    _itemUids[i] = uid;
                    _grid2Dic[i] = uid;
                    if (_needCout > count)
                    {
                        _itemNums[i] = count;
                        exp -= count * _itemExp;
                    }
                    else
                    {
                        _itemNums[i] = _needCout;
                        exp -= _needCout * _itemExp;
                    }
                }
                else
                {
                    break;
                }
            }

            if (hasLvCond)
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(11016);
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    ResetGrid2Dic();
                    ReSetItemData();
                    SetItemGrids();
                    if (itemViewShow)
                        ResetItemGridState();

                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
            else
            {
                ResetGrid2Dic();
                ReSetItemData();
                SetItemGrids();
                if (itemViewShow)
                    ResetItemGridState();
            }
        }

        private void ReSetItemData()
        {
            hSelectItem.Clear();
            for (int i = 0; i < _grid2Dic.Count; i++)
            {
                grid2Dic[i] = _grid2Dic[i];
            }

            for (int i = 0; i < _itemUids.Count; i++)
            {
                itemUids[i] = _itemUids[i];
                hSelectItem.Add(_itemUids[i]);
            }

            for (int i = 0; i < _itemNums.Count; i++)
            {
                itemNums[i] = _itemNums[i];
            }
        }

        private uint GetSkillLevel()
        {
            if(Sys_Skill.Instance.IsActiveSkill(skillId))
            {
                CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                if (skillInfo != null)
                {
                    return skillInfo.level;
                }
            }
            else
            {
                CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                if (skillInfo != null)
                {
                    return skillInfo.level;
                }
            }
            return 0;
        }

        private void SetItemGrids()
        {
            for (int i = 0; i < grid2Dic.Count; i++)
            {
                ulong uid = itemUids[i];
                bool hasItem = uid != 0;
                uint itemId = 0;
                long itemNum = 0;
                if (hasItem)
                {
                    itemId = Sys_Bag.Instance.GetItemDataByUuid(itemUids[i]).Id;
                    itemNum = itemNums[i];
                }
                PropItem _itemP = upExpItemDic[i];

                PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(itemId, itemNum, hasItem, false, false, false, false, true, false, true, ItemGridBeClicked, _bUseTips: false);
                itemN.guid = uid;

                _itemP.SetData(itemN, EUIID.UI_Pet_SkillStudy);
                _itemP.txtNumber.gameObject.SetActive(hasItem);
                _itemP.Layout.imgIcon.enabled = hasItem;
                _itemP.Layout.imgQuality.gameObject.SetActive(hasItem);
                _itemP.btnNone.gameObject.SetActive(!hasItem);
            }

            SetAddExpValueView();
        }

        long expAdd = 0L;
        long needAdd = 0L;
        /// <summary>
        /// 刷新添加经验
        /// </summary>
        private void SetAddExpValueView()
        {
            expAdd = 0L;
            useSilverNum = 0L;
            for (int i = 0; i < itemUids.Count; i++)
            {
                ulong uid = itemUids[i];
                if (uid != 0)
                {
                    CSVItem.Data item = CSVItem.Instance.GetConfData(Sys_Bag.Instance.GetItemDataByUuid(uid).Id);
                    if (null != item && item.id == Sys_Pet.Instance.PetSkillExpItemId)
                    {
                        if (item.fun_parameter == "PetskillBook" && item.fun_value != null && item.fun_value.Count >= 2)
                        {
                            expAdd += item.fun_value[1] * itemNums[i];
                        }
                    }
                    else if (null != item)
                    {
                        expAdd += GetBookExp((int)item.lv - 1) * itemNums[i];
                    }
                }                
            }

            CSVPetNewSkillsLv.Data petSkillLevelData = CSVPetNewSkillsLv.Instance.GetConfData(level);
            if (null != petSkillLevelData && isUseSliver)
            {
                ClientPet currentPet = Sys_Pet.Instance.GetPetByUId(petUid);
                uint currentExp = currentPet.GetPetSkillExp(skillId);
                if (currentExp + expAdd < petSkillLevelData.exp)
                {
                    useSilverNum = GetNeedSliver(petSkillLevelData.exp - (currentExp + expAdd));
                }
            }

            bool showAdd = expAdd != 0;
            addExpText.transform.parent.gameObject.SetActive(showAdd);
            sliderRect.transform.gameObject.SetActive(showAdd);
            addExpText.text = expAdd.ToString();
            if(showAdd)
            {
                long exp = 0L;
                uint configExp = 0u;
                if (null != petSkillLevelData)
                {
                    ClientPet currentPet = Sys_Pet.Instance.GetPetByUId(petUid);
                    exp = currentPet.GetPetSkillExp(skillId) + expAdd;
                    configExp = petSkillLevelData.exp;
                    float sliderValue = ((exp + 0f) / configExp);
                    sliderValue = sliderValue > 1 ? 1f: sliderValue;
                    sliderRect.sizeDelta = new Vector2(maxSliderWidth * sliderValue, sliderRect.rect.height);
                }
            }

        }

        /// <summary>
        /// 获取对应书籍的经验
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private long GetBookExp(int index)
        {
            if (null == petBookExp)
            {
                petBookExp = new List<uint>();
                CSVPetNewParam.Data petBookExpParam = CSVPetNewParam.Instance.GetConfData(21u);
                string[] strValue = petBookExpParam.str_value.Split('|');
                for (int i = 0; i < strValue.Length; i++)
                {
                    petBookExp.Add(uint.Parse(strValue[i]));
                }
            }

            if(0 <= index && index < petBookExp.Count)
            {
                return petBookExp[index];
            }
            return 0;
            
        }

        /// <summary>
        /// 获取需要的银币
        /// </summary>
        /// <param name="nendExp"></param>
        /// <returns></returns>
        private long GetNeedSliver(long nendExp)
        {
            float per = nendExp / (float)useSliverPer[0];
            var needSliver = (long)per * useSliverPer[1];
            var bagCount = Sys_Bag.Instance.GetItemCount(3);
            if(bagCount >= needSliver)
            {
                expAdd += nendExp;
                return needSliver;
            }
            else
            {
                int bagPer = (int)bagCount / (int)useSliverPer[1];
                expAdd += (useSliverPer[0] * bagPer);
                return bagPer * useSliverPer[1];
            }
        }

        public int CompItem(ItemData a, ItemData b)
        {
            if (a.cSVItemData.lv == b.cSVItemData.lv)
            {
                return (int)b.cSVItemData.type_id - (int)a.cSVItemData.type_id;
            }
            else
            {
                return (int)a.cSVItemData.lv - (int)b.cSVItemData.lv;
            }
        }

        private void PopdownListBuild()
        {
            dropdown.ClearOptions();

            dropdown.options.Clear();
            for (int i = 0; i < petSkillLevels.Count; ++i)
            {
                Dropdown.OptionData op = new Dropdown.OptionData();
                op.text = LanguageHelper.GetTextContent(10819 + petSkillLevels[i]);
                dropdown.options.Add(op);
            }
            chooseType = 0;
            dropdown.onValueChanged.AddListener(OnValueChanged); 
        }

        private void OnValueChanged(int index)
        {
            chooseType = (int)petSkillLevels[index];
        }
    }

    public class UI_Pet_SkillStudy: UIBase, UI_Pet_SkillStudy_Layout.IListener
    {
        private UI_Pet_SkillStudy_Layout layout;
        private UI_Pet_SkillStudyMessage messageView;
        private UI_Pet_SkillStudyParam uiParam;
        public uint skillId;
        public uint petUid;
        protected override void OnLoaded()
        {
            layout = new UI_Pet_SkillStudy_Layout();
            layout.Init(transform);
            layout.RegisterEvents(this);
            messageView = new UI_Pet_SkillStudyMessage();
            messageView.Init(layout.messageView);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnUpdatePetInfo, OnUpdatePetInfo, toRegister);
        }

        protected override void OnOpen(object arg = null)
        {
            uiParam = arg as UI_Pet_SkillStudyParam;
        }

        protected override void OnShow()
        {
            if(null != uiParam)
            {
                this.skillId = uiParam.skillId;
                this.petUid = uiParam.petUid;
                messageView.SetSkillData(uiParam.skillId, uiParam.petUid);
                //uiParam = null;
            }
            
        }

        private void OnUpdatePetInfo()
        {
            messageView.SetSkillData(skillId, petUid);
        }

        protected override void OnClose()
        {
            uiParam = null;
        }

        public void OncloseBtnClicked()
        {
            CloseSelf();
        }

        
    }
}
