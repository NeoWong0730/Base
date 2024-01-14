using Framework;
using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Pet_RemakeSkill
    {
        public class RemakeSkillItemCost : UI_CostItem
        {
            public override void SetGameObject(GameObject go)
            {
                this.gameObject = go;
                content = go.transform.Find("Value").GetComponent<Text>();
                icon = go.transform.Find("Icon").GetComponent<Image>();
            }

            public void Refresh(ItemIdCount idCount)
            {
                this.idCount = idCount;

                if (idCount != null)
                {
                    if (idCount.CSV != null)
                    {
                        ImageHelper.SetIcon(this.icon, idCount.CSV.small_icon_id );
                    }
                    uint worldStyleId = 0;
                    string content = string.Empty;
                    if (!idCount.Enough)
                    {
                        worldStyleId = 125;
                    }
                    else
                    {
                        worldStyleId = 124;
                    }
                    content = Sys_Bag.Instance.GetValueFormat(idCount.count);
                    TextHelper.SetText(this.content, content, CSVWordStyle.Instance.GetConfData(worldStyleId));
                }
            }
        }
        public class PetRemakeLearnCeil
        {
            public Transform transform;
            public PetSkillItem02 skillItem;
            private Action<PetRemakeLearnCeil> onClick;
            private Action<PetRemakeLearnCeil> onSelect;

            public Button selectBtn; //改造锁
            public GameObject hasSkillGo; //已领悟
            public uint skillId;
            public void BingGameObject(GameObject go)
            {
                transform = go.transform;
                skillItem = new PetSkillItem02();
                skillItem.Bind(transform.Find("PetSkillItem01").gameObject);
                selectBtn = transform.Find("Btn_Select")?.GetComponent<Button>();
                hasSkillGo = transform.Find("Image_Comprehend").gameObject;
                selectBtn?.onClick.AddListener(OnSelect);
                skillItem.EnableLongPress(false);
                skillItem.AddClickListener(OnClicked);
            }

            public void AddClickListener(Action<PetRemakeLearnCeil> onClick = null, Action<PetRemakeLearnCeil> onSelect = null)
            {
                this.onClick = onClick;
                this.onSelect = onSelect;
            }

            private void OnSelect()
            {
                onSelect?.Invoke(this);
            }

            private void OnClicked()
            {
                onClick?.Invoke(this);
            }

            /// <summary>
            /// index 只有在是改造技能是有效
            /// </summary>
            /// <param name="skillId"></param>
            /// <param name="isUnique"></param>
            /// <param name="isBuild"></param>
            /// <param name="itemId"></param>
            /// <param name="index"></param>
            public void SetData(uint skillId, bool has)
            {
                this.skillId = skillId;
                SetPetSkillInfo(has);
            }

            private void SetPetSkillInfo(bool has)
            {
                bool hasSkill = skillId != 0;
                skillItem.SetDate(skillId);
                skillItem.skillImage.gameObject.SetActive(true);
                skillItem.transform.gameObject.SetActive(hasSkill);
                selectBtn.gameObject.SetActive(hasSkill && !has);
                hasSkillGo.SetActive(hasSkill && has);
            }
        }
        public class ToggleItem
        {
            private Toggle toggle;
            private Image itemIcon;
            private Text itemCost;
            private Action<uint> action;
            private uint type;
            public void Init(Transform transform, Action<uint> action, uint type)
            {
                toggle = transform.GetComponent<Toggle>();
                itemIcon = transform.Find("Background").GetComponent<Image>();
                itemCost = transform.Find("Consume").GetComponent<Text>();

                this.action = action;
                this.type = type;
                toggle.onValueChanged.AddListener((b) =>
                {
                    if(b)
                    {
                        this.action?.Invoke(type);
                    }
                });
            }
            public void SetIsOnWithoutNotify(bool bState)
            {
                toggle.SetIsOnWithoutNotify(bState);
            }

            public void TriggerEvent()
            {
                if(toggle.isOn)
                {
                    toggle.onValueChanged.Invoke(true);
                }
                else
                {
                    toggle.isOn = true;
                }
                
            }

            public void SetItemInfo(uint itemId, long NeedCount)
            {
                long right = NeedCount;
                long left = Sys_Bag.Instance.GetItemCount(itemId);
                uint worldStyleId = 0;
                string content = string.Empty;
                if (left < right)
                {
                    worldStyleId = 125;
                }
                else
                {
                    worldStyleId = 124;
                }
                content = LanguageHelper.GetTextContent(12048, left.ToString(), right.ToString());
                TextHelper.SetText(itemCost, content, CSVWordStyle.Instance.GetConfData(worldStyleId));
            }
        }
        private Transform transform;
        private ClientPet client;
        private Button remakeSkillBtn;
        private Button againBtn;
        private Button cancelBtn;
        public Button skillListBtn;

        private List<PetSkillCeil> skillCeilList = new List<PetSkillCeil>();
        private List<uint> leftSkillIdsList = new List<uint>();

        public IListener listener;
        public int selectIndex = -1;

        private InfinityGrid leftInfinityGrid;
        private PropItem bulidItem;
        private uint itemId;
        private uint selectType = 0; // 0 普通 1 高级
        //private ToggleItem toggleItem1;
        //private ToggleItem toggleItem2;
        private PropItem toggleItem1;
        private PropItem toggleItem2;
        private PetSkillCeil learnSkillCeil;
        private GameObject noLearnTipsGo;
        private GameObject noLearnSelectTipsGo;
        private GameObject noSkillTipsGo;
        private GameObject resultGo;

        private Text successText;
        private GameObject costGo;
        private RemakeSkillItemCost itemCost;
        private GameObject selectGo;
        List<PetRemakeLearnCeil> petRemakeLearnCeils = new List<PetRemakeLearnCeil>(3);
        private Animator skillAni;
        private Animator skillSelectAni;
        /// <summary>
        /// 选中的技能改造次数
        /// </summary>
        public uint PetSkillBuildCount
        {
            get
            { 
                var lst = Sys_Pet.Instance.BuildSkillNumByIndex;
                if (selectIndex >= 0 && selectIndex < lst.Count)
                {
                    return lst[selectIndex];
                }
                return 0;
            }
        }

        public void Init(Transform transform)
        {
            this.transform = transform;
            leftInfinityGrid = transform.Find("View_Left/GameObject/Skill/Scroll View").GetComponent<InfinityGrid>();
            leftInfinityGrid.onCreateCell += OnLeftCreateCell;
            leftInfinityGrid.onCellChange += OnLeftCellChange;

            bulidItem = new PropItem();
            bulidItem.BindGameObject(transform.Find("View_Right/GameObject/Select/PropItem").gameObject);
            PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(0, 0, false, false, false, false, false, false, false, true, ItemGridBeClicked, _bUseTips: false);
            bulidItem.SetData(itemN, EUIID.UI_Pet_Message);
            bulidItem.btnNone.gameObject.SetActive(true);

            remakeSkillBtn = transform.Find("View_Bottom/Btn_Comprehend").GetComponent<Button>();
            remakeSkillBtn.onClick.AddListener(OnRemakeSkillBtnClicked);

            cancelBtn = transform.Find("View_Bottom/Btn_Delete").GetComponent<Button>();
            cancelBtn.onClick.AddListener(CancelBtnClicked);

            againBtn = transform.Find("View_Bottom/Btn_Again").GetComponent<Button>();
            againBtn.onClick.AddListener(OnAgainBtnClicked);

            skillListBtn = transform.Find("View_Right/Button_Preview").GetComponent<Button>();
            skillListBtn.onClick.AddListener(OnOpenSkillListBtnClicked);

            costGo = transform.Find("View_Bottom/Consume").gameObject;
            successText = transform.Find("View_Bottom/SuccessRate/Value").GetComponent<Text>();

            learnSkillCeil = new PetSkillCeil();
            learnSkillCeil.BingGameObject(transform.Find("View_Right/GameObject/Image_Bottom").gameObject);
            learnSkillCeil.AddClickListener(OnLearnSkillBeClikcked);
            noLearnTipsGo = transform.Find("View_Right/GameObject/ComprehendTheResult/Text_Result_Tips").gameObject;
            noLearnSelectTipsGo = transform.Find("View_Right/GameObject/Select/Text_Select_Tips").gameObject;
            noSkillTipsGo = transform.Find("View_Left/GameObject/Text_Tips").gameObject;
            resultGo = transform.Find("View_Right/GameObject/ComprehendTheResult/Result").gameObject;

            selectGo = transform.Find("View_Right/GameObject/Select").gameObject;
            skillAni = transform.Find("View_Right/GameObject/ComprehendTheResult/Fx").GetComponent<Animator>();
            skillSelectAni = transform.Find("View_Right/GameObject/ComprehendTheResult/Fx3_2").GetComponent<Animator>();

            itemCost = new RemakeSkillItemCost();
            itemCost.SetGameObject(transform.Find("View_Bottom/Consume").gameObject);
            toggleItem1 = new PropItem();
            toggleItem1.BindGameObject(transform.Find("View_Right/GameObject/Select/Toggle_1/PropItem").gameObject);
            toggleItem2 = new PropItem();
            toggleItem2.BindGameObject(transform.Find("View_Right/GameObject/Select/Toggle_2/PropItem").gameObject);
            selectType = 0;
            PetRemakeLearnCeil ceil1 = new PetRemakeLearnCeil();
            ceil1.BingGameObject(transform.Find("View_Right/GameObject/ComprehendTheResult/Result/Image_Bottom").gameObject);
            ceil1.AddClickListener(OnSkillBeClicked, OnSkillSelectBeClicked);
            petRemakeLearnCeils.Add(ceil1);

            PetRemakeLearnCeil ceil2 = new PetRemakeLearnCeil();
            ceil2.BingGameObject(transform.Find("View_Right/GameObject/ComprehendTheResult/Result/Image_Bottom (1)").gameObject);
            ceil2.AddClickListener(OnSkillBeClicked, OnSkillSelectBeClicked);
            petRemakeLearnCeils.Add(ceil2);

            PetRemakeLearnCeil ceil3 = new PetRemakeLearnCeil();
            ceil3.BingGameObject(transform.Find("View_Right/GameObject/ComprehendTheResult/Result/Image_Bottom (2)").gameObject);
            ceil3.AddClickListener(OnSkillBeClicked, OnSkillSelectBeClicked);
            petRemakeLearnCeils.Add(ceil3);
        }

        private void OnTypeSelectToggle(uint type)
        {
            selectType = type;
            SetSelectTypeState();
        }

        private void OnSkillBeClicked(PetRemakeLearnCeil skillCeil)
        {
            UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(skillCeil.skillId, 0));
        }
        bool showEffect = false;
        private void OnSkillSelectBeClicked(PetRemakeLearnCeil skillCeil)
        {
            var nowSkill = client.GetPetBuildSkillList();
            var currentSkillId = nowSkill[selectIndex];
            PromptBoxParameter.Instance.OpenPromptBox(LanguageHelper.GetTextContent(12030, GetSkillName(skillCeil.skillId), GetSkillName(currentSkillId)),
                              0, () =>
                              {
                                  skillAni.gameObject.SetActive(true);
                                  skillSelectAni.gameObject.SetActive(true);
                                  skillAni.Play("Close", -1, 0f);
                                  skillSelectAni.Play("Open", -1, 0f);
                                  showEffect = true;
                                  Sys_Pet.Instance.PetRemakeSelectSkillReq(client.GetPetUid(), (uint)selectIndex, skillCeil.skillId);
                              });
        }

        private string GetSkillName(uint skillId)
        {
            if (Sys_Skill.Instance.IsActiveSkill(skillId))
            {
                CSVActiveSkillInfo.Data skill = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                if (null != skill)
                {
                    return LanguageHelper.GetTextContent(skill.name);
                }
            }
            else
            {
                CSVPassiveSkillInfo.Data skill = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                if (null != skill)
                {
                    return LanguageHelper.GetTextContent(skill.name);
                }
            }
            return "";
        }

        /// <summary>
        /// 取消改造结果
        /// </summary>
        private void CancelBtnClicked()
        {
            PromptBoxParameter.Instance.OpenPromptBox(LanguageHelper.GetTextContent(12041),
                           0, () =>
                           {
                               Sys_Pet.Instance.PetRemakeSelectSkillReq(client.GetPetUid(), (uint)selectIndex, 0);
                           });
        }

        /// <summary> 重置按钮事件 </summary>
        private void OnAgainBtnClicked()
        {
            if (!Sys_Pet.Instance.isShowRemakeSkillTips)
            {
                UIManager.OpenUI(EUIID.UI_Pet_RemakeTips, false, 1u);
            }
            else
            {
                OnRemakeSkillBtnClicked();
                //Sys_Pet.Instance.PetRemakeLearnSkillReq(client.GetPetUid(), (uint)selectIndex, selectType, itemId);
            }
        }

        private void OnRemakeSkillBtnClicked()
        {
            if (Sys_Pet.Instance.IsPetBeEffectWithSecureLock(client.petUnit))
            {
                return;
            }

            if (client.petUnit.SimpleInfo.ExpiredTick > 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000656));
                return;
            }

            if (selectIndex < 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12025));
                return;
            }

            if (client.GetBuildNotSave())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12305, LanguageHelper.GetTextContent(12306), LanguageHelper.GetTextContent(12307)));
                return;
            }
            else if (client.GetBuildRecastNotSave())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12305, LanguageHelper.GetTextContent(12308), LanguageHelper.GetTextContent(12307)));
                return;
            }

            if (selectType == 0)
            {
                CSVPetNewReBuild.Data cSVPetRemakeData = CSVPetNewReBuild.Instance.GetConfData(PetSkillBuildCount);
                if (null != cSVPetRemakeData && null != cSVPetRemakeData.skill_cost && cSVPetRemakeData.skill_cost.Count >= 2)
                {
                    for (int i = 0; i < cSVPetRemakeData.skill_cost.Count; i++)
                    {
                        if(null != cSVPetRemakeData.skill_cost[i] && cSVPetRemakeData.skill_cost[i].Count >= 2)
                        {
                            ItemIdCount itemIdCount = new ItemIdCount(cSVPetRemakeData.skill_cost[i][0], cSVPetRemakeData.skill_cost[i][1]);
                            if(!itemIdCount.Enough)
                            {
                                if(itemIdCount.id < 500)
                                {
                                    Sys_Bag.Instance.TryOpenExchangeCoinUI((ECurrencyType)itemIdCount.id, itemIdCount.count);
                                }
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12021, LanguageHelper.GetTextContent(itemIdCount.CSV.name_id)));
                                return;
                            }
                        }
                    }
                }
            }
            else
            {
                CSVPetNewReBuild.Data cSVPetRemakeData = CSVPetNewReBuild.Instance.GetConfData(PetSkillBuildCount);
                if (null != cSVPetRemakeData && null != cSVPetRemakeData.senior_skill_cost && cSVPetRemakeData.senior_skill_cost.Count >= 2)
                {
                    for (int i = 0; i < cSVPetRemakeData.senior_skill_cost.Count; i++)
                    {
                        if (null != cSVPetRemakeData.senior_skill_cost[i] && cSVPetRemakeData.senior_skill_cost[i].Count >= 2)
                        {
                            ItemIdCount itemIdCount = new ItemIdCount(cSVPetRemakeData.senior_skill_cost[i][0], cSVPetRemakeData.senior_skill_cost[i][1]);
                            if (!itemIdCount.Enough)
                            {
                                if (itemIdCount.id < 500)
                                {
                                    Sys_Bag.Instance.TryOpenExchangeCoinUI((ECurrencyType)itemIdCount.id, itemIdCount.count);
                                }
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12021, LanguageHelper.GetTextContent(itemIdCount.CSV.name_id)));
                                return;
                            }
                        }
                    }
                }
            }
            skillSelectAni.gameObject.SetActive(true);
            skillSelectAni.Play("Close", -1, 0f);
            Sys_Pet.Instance.PetRemakeLearnSkillReq(client.GetPetUid(), (uint)selectIndex, selectType, itemId);
        }

        /// <summary> 打开技能列表 </summary>
        private void OnOpenSkillListBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_Pet_SkillTips);
        }

        private void OnLeftCreateCell(InfinityGridCell cell)
        {
            PetSkillCeil entry = new PetSkillCeil();
            GameObject go = cell.mRootTransform.gameObject;
            entry.BingGameObject(go);
            entry.AddClickListener(OnSkillSelect);
            cell.BindUserData(entry);
        }

        private void OnLeftCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= leftSkillIdsList.Count)
                return;
            PetSkillCeil entry = cell.mUserData as PetSkillCeil;
            if (index < leftSkillIdsList.Count)
            {
                uint skillId = leftSkillIdsList[index];
                entry.SetData(skillId, false, true, index: index, hasHight: skillId == 0 ? false : client.IsHasHighBuildSkill(skillId));
                entry.LockSelect(selectIndex == index);
            }
        }

        private void OnSkillSelect(PetSkillCeil petSkillCeil)
        {
            if (client.petUnit.SimpleInfo.ExpiredTick > 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000655));
                return;
            }
            uint skillId = petSkillCeil.petSkillBase.skillId;
            if (skillId != 0)
            {
                bool isNotSaveSkill = client.GetBuildSkillNotSave();
                if (isNotSaveSkill)
                {
                    UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(skillId, 0));
                }
                else
                {
                    selectIndex = client.GetPetBuildSkillList().IndexOf(skillId);

                    CSVPetNewReBuild.Data cSVPetRemakeData = CSVPetNewReBuild.Instance.GetConfData(PetSkillBuildCount);
                    if (null != cSVPetRemakeData &&  null != cSVPetRemakeData.skill_cost && cSVPetRemakeData.skill_cost.Count >= 2)
                    {
                        
                        ItemIdCount itemIdCount = new ItemIdCount(cSVPetRemakeData.skill_cost[0][0], cSVPetRemakeData.skill_cost[0][1]);
                        if(itemIdCount.Enough)
                        {
                            selectType = 0;
                        }
                    }
                    if (null != cSVPetRemakeData &&  null != cSVPetRemakeData.senior_skill_cost && cSVPetRemakeData.senior_skill_cost.Count >= 2)
                    {
                        ItemIdCount itemIdCount = new ItemIdCount(cSVPetRemakeData.senior_skill_cost[0][0], cSVPetRemakeData.senior_skill_cost[0][1]);
                        if (itemIdCount.Enough)
                        {
                            selectType = 1;
                        }
                    }
                    SetSelectTypeState();
                    SetLearnSkillInfo();
                    LearnStateView(isNotSaveSkill);
                    leftInfinityGrid.ForceRefreshActiveCell();
                    UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(skillId, 0));
                }
            }
        }

        private void OnLearnSkillBeClikcked(PetSkillCeil petSkillCeil)
        {
            uint skillId = petSkillCeil.petSkillBase.skillId;
            if (skillId != 0)
            {
                UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(skillId, 0));
            }
        }

        public void Show()
        {
            if (!transform.gameObject.activeSelf)
            {
                skillAni.gameObject.SetActive(false);
                skillSelectAni.gameObject.SetActive(false);
                transform.gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            itemId = 0;
            selectIndex = -1;
            selectType = 0;
            skillAni.gameObject.SetActive(false);
            skillSelectAni.gameObject.SetActive(false);
            transform.gameObject.SetActive(false);
        }

        public void OnAgainRight()
        {
            OnRemakeSkillBtnClicked();
        }

        public void OnRemakeSkillEnd(bool isSuccess)
        {
            if(isSuccess)
            {
                skillAni.gameObject.SetActive(true);
                skillAni.Play("Open", -1, 0f);
            }
        }

        private void ItemGridBeClicked(PropItem bulidItem)
        {
            if (selectType == 0)
                return;
            if (itemId == 0)
            {
                UIManager.OpenUI(EUIID.UI_SelectItem, false,
                       new UI_SelectItemParam
                       {
                           tittle_langId = 10946,
                           getAwayId = (uint)EItemType.PetRemakeSkillBook,
                           petUid = client.GetPetUid()
                       }, EUIID.UI_Pet_Message);
            }
            else
            {
                SetNoneItem();
                listener?.OnItemClear();
            }
        }

        private void SetNoneItem()
        {
            PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(0, 0, false, false, false, false, false, false, false, true, ItemGridBeClicked, _bUseTips: false);
            bulidItem.SetData(itemN, EUIID.UI_Pet_Message);
            bulidItem.txtNumber.gameObject.SetActive(false);
            bulidItem.btnNone.gameObject.SetActive(true);
            bulidItem.Layout.imgIcon.enabled = false;
            bulidItem.Layout.imgQuality.gameObject.SetActive(false);
        }

        public void ReSetView(uint _itemId)
        {
            itemId = _itemId;
            bool isSelectItem = itemId != 0;
            if (!isSelectItem)
            {
                SetNoneItem();
            }
            else
            {
                PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(itemId, 1, true, false, false, false, false, true, true, true, ItemGridBeClicked, _bUseTips: false);
                bulidItem.SetData(itemN, EUIID.UI_Pet_Message);
                bulidItem.btnNone.gameObject.SetActive(false);
                bulidItem.Layout.imgIcon.enabled = true;
            }
        }

        private void LearnStateView(bool hasNotSave)
        {
            noLearnTipsGo.gameObject.SetActive(!hasNotSave);
            
            resultGo.gameObject.SetActive(hasNotSave);
            cancelBtn.gameObject.SetActive(hasNotSave);
            againBtn.gameObject.SetActive(hasNotSave);
            remakeSkillBtn.gameObject.SetActive(!hasNotSave);
        }
        private void SetLearnSkillInfo()
        {
            if(selectIndex >= 0 && selectIndex < leftSkillIdsList.Count)
            {
                learnSkillCeil.SetData(leftSkillIdsList[selectIndex], false, true, index: selectIndex);
            }
        }

        private void ChangeCostView()
        {
            CSVPetNewReBuild.Data cSVPetRemakeData = CSVPetNewReBuild.Instance.GetConfData(PetSkillBuildCount);
            if (selectType == 0)
            {
                if (null != cSVPetRemakeData &&  null != cSVPetRemakeData.skill_cost && cSVPetRemakeData.skill_cost.Count >= 2)
                {
                    ItemIdCount itemIdCount = new ItemIdCount(cSVPetRemakeData.skill_cost[1][0], cSVPetRemakeData.skill_cost[1][1]);
                    itemCost.Refresh(itemIdCount);
                }
            }
            else
            {
                if (null != cSVPetRemakeData &&  null != cSVPetRemakeData.senior_skill_cost && cSVPetRemakeData.senior_skill_cost.Count >= 2)
                {
                    ItemIdCount itemIdCount = new ItemIdCount(cSVPetRemakeData.senior_skill_cost[1][0], cSVPetRemakeData.senior_skill_cost[1][1]);
                    itemCost.Refresh(itemIdCount);
                }
            }
        }

        public void SetSelectTypeState()
        {
            bool hasSelectSkillItem = selectIndex != -1;
            if (hasSelectSkillItem)
            {


                CSVPetNewReBuild.Data cSVPetRemakeData = CSVPetNewReBuild.Instance.GetConfData(PetSkillBuildCount);
                ChangeCostView();
                if (null != cSVPetRemakeData)
                {
                    if (null != cSVPetRemakeData.skill_cost && cSVPetRemakeData.skill_cost.Count >= 2)
                    {
                        PropIconLoader.ShowItemData showItemData = new PropIconLoader.ShowItemData(cSVPetRemakeData.skill_cost[0][0], cSVPetRemakeData.skill_cost[0][1], true, false, false, false, false,
                        _bShowCount: true, _bShowBagCount: true, _bUseClick: true, (propItem) => { SelectShow(propItem, cSVPetRemakeData.skill_cost[0][0]); }, true, true);

                        toggleItem1.SetData(showItemData, EUIID.UI_Pet_Message);
                        toggleItem1.imgSelect.gameObject.SetActive(selectType == 0);
                        //toggleItem1.SetItemInfo(cSVPetRemakeData.skill_cost[0][0], cSVPetRemakeData.skill_cost[0][1]);
                        ItemIdCount itemIdCount = new ItemIdCount(cSVPetRemakeData.skill_cost[1][0], cSVPetRemakeData.skill_cost[1][1]);
                        itemCost.Refresh(itemIdCount);
                    }

                    if (null != cSVPetRemakeData.senior_skill_cost && cSVPetRemakeData.senior_skill_cost.Count >= 2)
                    {
                        PropIconLoader.ShowItemData showItemData = new PropIconLoader.ShowItemData(cSVPetRemakeData.senior_skill_cost[0][0], cSVPetRemakeData.senior_skill_cost[0][1], true, false, false, false, false,
                          _bShowCount: true, _bShowBagCount: true, _bUseClick: true, (propItem) => { SelectShow(propItem, cSVPetRemakeData.senior_skill_cost[0][0]); }, true, true);
                        toggleItem2.SetData(showItemData, EUIID.UI_Pet_Message);
                        toggleItem2.imgSelect.gameObject.SetActive(selectType != 0);
                        //toggleItem2.SetItemInfo(cSVPetRemakeData.senior_skill_cost[0][0], cSVPetRemakeData.senior_skill_cost[0][1]);
                        ItemIdCount itemIdCount = new ItemIdCount(cSVPetRemakeData.senior_skill_cost[1][0], cSVPetRemakeData.senior_skill_cost[1][1]);
                        itemCost.Refresh(itemIdCount);
                    }
                    successText.text = (cSVPetRemakeData.skill_success_rate / 100.0f).ToString("0.#") + "%";
                }
               
                ImageHelper.SetImageGray(bulidItem.transform, selectType == 0, true);
                
            }
            for (int i = 0; i < noLearnSelectTipsGo.transform.parent.childCount; i++)
            {
                noLearnSelectTipsGo.transform.parent.GetChild(i).gameObject.SetActive(hasSelectSkillItem);
            }
            noLearnSelectTipsGo.gameObject.SetActive(!hasSelectSkillItem);
            learnSkillCeil.transform.gameObject.SetActive(hasSelectSkillItem);
            //selectGo.SetActive(hasSelectSkillItem);
            successText.transform.parent.gameObject.SetActive(hasSelectSkillItem);
            costGo.SetActive(hasSelectSkillItem);
        }

        private void SelectShow(PropItem clickItem, uint id)
        {
            bool hasSelectSkillItem = selectIndex != -1;
            if (hasSelectSkillItem)
            {
                CSVPetNewReBuild.Data cSVPetRemakeData = CSVPetNewReBuild.Instance.GetConfData(PetSkillBuildCount);
                if (null != cSVPetRemakeData &&  null != cSVPetRemakeData.skill_cost && cSVPetRemakeData.skill_cost.Count >= 2)
                {
                    if (id == cSVPetRemakeData.skill_cost[0][0])
                    {
                        toggleItem1.imgSelect.gameObject.SetActive(true);
                        toggleItem2.imgSelect.gameObject.SetActive(false);
                        selectType = 0;
                        SetSelectTypeState();
                    }
                }


                if (null != cSVPetRemakeData &&  null != cSVPetRemakeData.senior_skill_cost && cSVPetRemakeData.senior_skill_cost.Count >= 2)
                {
                    if (id == cSVPetRemakeData.senior_skill_cost[0][0])
                    {
                        toggleItem1.imgSelect.gameObject.SetActive(false);
                        toggleItem2.imgSelect.gameObject.SetActive(true);
                        selectType = 1;
                        SetSelectTypeState();
                    }
                }
            }
        }

        public void SetView(ClientPet _client)
        {
            if(null == _client && null != client)
            {
                return;
            }
            if (null != client && client.GetPetUid() != _client.GetPetUid())
            {
                itemId = 0;
                selectIndex = -1;
                selectType = 0;
            }
            this.client = _client;
            bool isNotSaveSkill = client.GetBuildSkillNotSave();
            leftSkillIdsList.Clear();
            leftSkillIdsList.AddRange(client.petUnit.BuildInfo.BuildSkills);
            if (isNotSaveSkill)
            {
                selectIndex = (int)client.petUnit.BuildInfo.SkillPosition;
                for (int i = 0; i < client.petUnit.BuildInfo.SkillTemp.Count; i++)
                {
                    if(i < petRemakeLearnCeils.Count)
                    {
                        var id = client.petUnit.BuildInfo.SkillTemp[i];
                        petRemakeLearnCeils[i].SetData(id, client.IsSameOrHighBuildSkill(id));
                    }
                }
            }
            else
            {
                if(selectIndex == -1)
                {
                    selectIndex = client.GetPeBuildtSkillNotZeroFirstIndex();

                    CSVPetNewReBuild.Data cSVPetRemakeData = CSVPetNewReBuild.Instance.GetConfData(PetSkillBuildCount);
                    if (null != cSVPetRemakeData &&  null != cSVPetRemakeData.skill_cost && cSVPetRemakeData.skill_cost.Count >= 2)
                    {

                        ItemIdCount itemIdCount = new ItemIdCount(cSVPetRemakeData.skill_cost[0][0], cSVPetRemakeData.skill_cost[0][1]);
                        if (itemIdCount.Enough)
                        {
                            selectType = 0;
                        }
                    }
                    if (null != cSVPetRemakeData &&  null != cSVPetRemakeData.senior_skill_cost && cSVPetRemakeData.senior_skill_cost.Count >= 2)
                    {
                        ItemIdCount itemIdCount = new ItemIdCount(cSVPetRemakeData.senior_skill_cost[0][0], cSVPetRemakeData.senior_skill_cost[0][1]);
                        if (itemIdCount.Enough)
                        {
                            selectType = 1;
                        }
                    }
                }
                
            }
            //设置选中类型
            SetSelectTypeState();
            //设置学习技能信息
            SetLearnSkillInfo();
            LearnStateView(isNotSaveSkill);
            
            leftInfinityGrid.CellCount = leftSkillIdsList.Count;
            leftInfinityGrid.ForceRefreshActiveCell();
            noSkillTipsGo.gameObject.SetActive(leftSkillIdsList.Count == 0);
            ItemIdCount idCount = new ItemIdCount(itemId, 1);
            if (!idCount.Enough)
            {
                itemId = 0;
                ReSetView(itemId);
            }
            else
            {
                ReSetView(itemId);
            }
        }

        public void Register(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void OnItemClear();
        }
    }

    public class UI_Pet_RemakeAttr
    {
        public class UI_PetRemakePointBtn : IDisposable
        {
            public enum ERemakePointBtnType
            {
                Lock,//锁定灰态
                Unlock,//解锁态
                Remake,// 解锁并改造完毕
            }
            public GameObject selectGo;
            public Image iconImage;
            public Text levelText;
            public Button selectBtn;
            public Action<int> action;
            public int index;
            public void Init(Transform transform)
            {
                levelText = transform.Find("Image_Level/Text").GetComponent<Text>();
                iconImage = transform.Find("Image_bg/Image_Icon").GetComponent<Image>();
                selectGo = transform.Find("Image_Select").gameObject;

                selectBtn = transform.GetComponent<Button>();
                selectBtn.onClick.AddListener(OnSelectBtnClicked);
            }

            public void RemoveEvent()
            {
                if (null != selectBtn)
                {
                    selectBtn.onClick.RemoveListener(OnSelectBtnClicked);
                }
                action = null;
            }

            public void SetData(int index, ClientPet client, int selectIndex)
            {
                this.index = index;
                TextHelper.SetText(levelText, 10799 + (uint)index);
                //ImageHelper.SetIcon(iconImage, Sys_Pet.Instance.GetGradeStampStarImageId((uint)index + 1, client.GetBuildPointByIndex(index)), true);
                
                ERemakePointBtnType currentCeilType = ERemakePointBtnType.Lock;
                int remakeCount = client.GetPeBuildCount();
                if(index >= remakeCount + 1)
                {
                    currentCeilType = ERemakePointBtnType.Lock;
                }
                else if(index >= remakeCount)
                {
                    currentCeilType = ERemakePointBtnType.Unlock;
                }
                else
                {
                    currentCeilType = ERemakePointBtnType.Remake;
                }

                iconImage.gameObject.SetActive(currentCeilType == ERemakePointBtnType.Remake);
                ButtonHelper.Enable(selectBtn, currentCeilType != ERemakePointBtnType.Lock);
                ImageHelper.SetIcon(iconImage, Sys_Pet.Instance.GetGradeStampImageId((uint)index + 1, client.GetBuildPointByIndex(index)), true);
                SetSelectState(selectIndex == index);
            }

            public void AddSelectBtnListener(Action<int> action)
            {
                if (null != action)
                    this.action = action;
            }

            public void SetSelectState(bool select)
            {
                selectGo.SetActive(select);
            }

            private void OnSelectBtnClicked()
            {
                action?.Invoke(index);
            }

            public void Dispose()
            {

            }
        }
        private List<UI_PetRemakePointBtn> ceilList;
        
        private Transform transform;
        private UI_Pet_BuildAdvanceView leftRemakeAttrView;
        private UI_Pet_BuildAdvanceView rightRemakeAttrView;

        private InfinityGrid leftInfinityGrid;
        private InfinityGrid rightInfinityGrid;
        private InfinityGrid rightRemakeInfinityGrid;

        private List<uint> leftSkillIdsList = new List<uint>();
        private List<uint> rightSkillIdsList = new List<uint>();

        private PropItem bulidItem;

        /// <summary>
        /// 成功率界面按钮
        /// </summary>
        private Button successRuleBtn;
        /// <summary>
        /// 评级界面按钮
        /// </summary>
        private Button pointRuleBtn;
        /// <summary>
        /// 改造按钮
        /// </summary>
        private Button buildBtn;
        /// <summary>
        /// 重置评级按钮
        /// </summary>
        //private Button rePointBtn;
        /// <summary>
        /// 重新改造
        /// </summary>
        private Button reBuildBtn;
        /// <summary>
        /// 保存按钮
        /// </summary>
        private Button saveBtn;
        /// <summary>
        /// 取消改造
        /// </summary>
        private Button cancelBtn;
        private GameObject pointRuleGo;
        private Button closePointRuleBtn;
        private Transform starParent;
        private Text buildCountText;
        private Text buildNextText;
        private Text bulidSelectTipsText;
        private Image pointImage;
        private Text buildTitleText;
        private Text buildGradeTittleText;
        private Text buildSkillTittleText;
        private GameObject attrFullGo;
        private GameObject skillFullGo;
        private GameObject attrGo;
        private GameObject skillGo;
        private GameObject rightPreViewGo;
        private GameObject rightRe_RemakePreViewGo;
        private ClientPet client;
        private Animator saveAni;
        private Animator openAndRenovateAni;

        private Text reRemakePointText;
        private Text remakeAddGradeText;
        private Image remakePreviewImage;

        private uint itemId;
        public IListener listener;
        private int selectRemkePointIndex = -1;
        private bool SelectIsNew
        {
            get
            {
                if(null != client)
                {
                    return selectRemkePointIndex >= client.GetPeBuildCount();
                }
                return false;
            }
        }
        public void Init(Transform transform)
        {
            this.transform = transform;
            leftRemakeAttrView = new UI_Pet_BuildAdvanceView();
            leftRemakeAttrView.Init(transform.Find("View_Left/GameObject/Atrr"));
            rightRemakeAttrView = new UI_Pet_BuildAdvanceView();
            rightRemakeAttrView.Init(transform.Find("View_Right/GameObject/Atrr"));

            leftInfinityGrid = transform.Find("View_Left/GameObject/Skill/Scroll View").GetComponent<InfinityGrid>();
            leftInfinityGrid.onCreateCell += OnLeftCreateCell;
            leftInfinityGrid.onCellChange += OnLeftCellChange;

            rightInfinityGrid = transform.Find("View_Right/GameObject/Skill/Scroll View").GetComponent<InfinityGrid>();
            rightInfinityGrid.onCreateCell += OnRightCreateCell;
            rightInfinityGrid.onCellChange += OnRightCellChange;

            rightRemakeInfinityGrid = transform.Find("View_Right/View_Details/Skill/Scroll View").GetComponent<InfinityGrid>();
            rightRemakeInfinityGrid.onCreateCell += OnRightPreviewCreateCell;
            rightRemakeInfinityGrid.onCellChange += OnRightPreviewCellChange;

            rightPreViewGo = transform.Find("View_Right/GameObject").gameObject;
            rightRe_RemakePreViewGo = transform.Find("View_Right/View_Details").gameObject;

            skillFullGo = transform.Find("View_Right/GameObject/Text_Skill_Full").gameObject;
            attrFullGo = transform.Find("View_Right/GameObject/Text_Atrr_Full").gameObject;
            skillGo = transform.Find("View_Right/GameObject/Skill").gameObject;
            attrGo = transform.Find("View_Right/GameObject/Atrr").gameObject;

            bulidItem = new PropItem();
            bulidItem.BindGameObject(transform.Find("View_Bottom/PropItem").gameObject);
            PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(0, 0, false, false, false, false, false, false, false, true, ItemGridBeClicked, _bUseTips: false);
            bulidItem.SetData(itemN, EUIID.UI_Pet_Message);
            bulidItem.btnNone.gameObject.SetActive(true);
            bulidItem.Layout.imgQuality.gameObject.SetActive(false);
            starParent = transform.Find("View_Bottom/Scroll View/Viewport/Content");

            buildCountText = transform.Find("View_Bottom/Amount/Text_Amount/Text_Value").GetComponent<Text>();
            buildNextText = transform.Find("View_Bottom/Amount/Text_Tips").GetComponent<Text>();
            bulidSelectTipsText = transform.Find("View_Left/Text_Tips").GetComponent<Text>();
            buildTitleText = transform.Find("View_Right/GameObject/Image_Title/Text").GetComponent<Text>();
            buildGradeTittleText = transform.Find("View_Right/GameObject/Title_Atrr/Text_Title").GetComponent<Text>();
            buildSkillTittleText = transform.Find("View_Right/GameObject/Title_Skill/Text_Title").GetComponent<Text>();

            successRuleBtn = transform.Find("View_Bottom/Amount/Btn_Preview").GetComponent<Button>();
            successRuleBtn.onClick.AddListener(SuccessRuleBtn);

            buildBtn = transform.Find("View_Bottom/Btn_Remould").GetComponent<Button>();
            buildBtn.onClick.AddListener(BuildBtnClicked);

            pointRuleBtn = transform.Find("View_Bottom/Btn_Details").GetComponent<Button>();
            pointRuleBtn.onClick.AddListener(PointRuleBtnClicked);

            /*rePointBtn = transform.Find("View_Bottom/Btn_Reset").GetComponent<Button>();
            rePointBtn.onClick.AddListener(RePointBtnClicked);*/

            reBuildBtn = transform.Find("View_Bottom/Btn_Reinvent").GetComponent<Button>();
            reBuildBtn.onClick.AddListener(ReBuildBtnClicked);

            saveBtn = transform.Find("View_Bottom/Btn_Save").GetComponent<Button>();
            saveBtn.onClick.AddListener(SaveBtnClicked);

            cancelBtn = transform.Find("View_Bottom/Btn_Delete").GetComponent<Button>();
            cancelBtn.onClick.AddListener(CancelBtnClicked);

            pointRuleGo = transform.Find("View_GradePopup").gameObject;
            pointImage = transform.Find("View_Bottom/Stamp/Image_Stamp").GetComponent<Image>();

            reRemakePointText = transform.Find("View_Right/View_Details/Title_1/Text_Title").GetComponent<Text>();
            remakeAddGradeText = transform.Find("View_Right/View_Details/Content_2/Text").GetComponent<Text>();
            remakePreviewImage = transform.Find("View_Right/View_Details/Content_1/Image_Stamp").GetComponent<Image>();


            closePointRuleBtn = transform.Find("View_GradePopup/Btn_Close").GetComponent<Button>();
            closePointRuleBtn.onClick.AddListener(ClosePointRuleBtnClicked);
            openAndRenovateAni = transform.Find("View_Right/GameObject").GetComponent<Animator>();
            saveAni = transform.GetComponent<Animator>();

            ceilList = new List<UI_PetRemakePointBtn>(CSVPetNewReBuild.Instance.Count);
        }

        /// <summary>
        /// 关闭评级界面按钮
        /// </summary>
        private void ClosePointRuleBtnClicked()
        {
            pointRuleGo.SetActive(false);
        }

        /// <summary>
        /// 取消改造结果
        /// </summary>
        private void CancelBtnClicked()
        {
            PromptBoxParameter.Instance.OpenPromptBox(LanguageHelper.GetTextContent(12040),
                           0, () =>
                           {
                               Sys_Pet.Instance.PetRemakeSelectReq(client.GetPetUid(), 0);
                           });
        }

        /// <summary>
        /// 保存改造结构
        /// </summary>
        private void SaveBtnClicked()
        {
            if(client.IsOnReRemake())
            {

                LanguageHelper.GetTextContent(12318, LanguageHelper.GetTextContent(10781 + (uint)selectRemkePointIndex), client.GetAddGradeCountByReamkeTime(selectRemkePointIndex).ToString());
                PromptBoxParameter.Instance.OpenPromptBox(
                    LanguageHelper.GetTextContent(12303,
                    LanguageHelper.GetTextContent(client.petData.name),
                    LanguageHelper.GetTextContent(10781 + (uint)selectRemkePointIndex),
                    client.GetAddGradeCountByReamkeTime(selectRemkePointIndex).ToString(),
                    client.GetPreviewSkillNotZeroCountByRemakeTimes(selectRemkePointIndex).ToString(),
                    client.GetPetTempGradeCount().ToString(),
                    client.GetBuildPreviewSkillNotZeroCount().ToString()
                    ),
                          0, () =>
                          {
                              OnSave();
                          });
            }
            else
            {
                OnSave();
            }
        }

        private void OnSave()
        {
            saveAni.Play("Open", -1, 0f);
            Sys_Hint.Instance.PushEffectInNextFight();  //战斗内提示：当前战斗不生效，将在下一场战斗生效

            Sys_Pet.Instance.PetRemakeSelectReq(client.GetPetUid(), 1);
        }

        /// <summary>
        /// 重置评级按钮
        /// </summary>
        /*private void RePointBtnClicked()
        {
            uint buildCount = (uint)client.GetPeBuildCount();
            if (buildCount > 0)
            {
                bool isRemakeNotSave = client.GetBuildNotSave();
                bool isSkillNotSave = client.GetBuildSkillNotSave();
                bool isRecastNotSave = client.GetBuildRecastNotSave();
                CSVPetNewReBuild.Data rebuildData = CSVPetNewReBuild.Instance.GetConfData(buildCount);
                if (null != rebuildData)
                {
                    if (null != rebuildData.reset_cost && rebuildData.reset_cost.Count >= 1 && rebuildData.reset_cost[0].Count >= 2)
                    {
                        ItemIdCount item = new ItemIdCount(rebuildData.reset_cost[0][0], rebuildData.reset_cost[0][1]);

                        PromptBoxParameter.Instance.OpenPromptBox(LanguageHelper.GetTextContent(12006, LanguageHelper.GetLanguageColorWordsFormat(LanguageHelper.GetTextContent(item.CSV.name_id), TextHelper.GetQuailtyLangId(item.CSV.quality)), item.count.ToString()),
                            0, () =>
                            {
                                if (item.Enough)
                                {
                                    Sys_Pet.Instance.OnPetReRemakeReq(client.GetPetUid());
                                }
                                else
                                {
                                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12044));
                                }
                            });
                    }
                }
                else
                {
                    DebugUtil.LogError($"Not find id = {buildCount} in CSVPetNewReBuild");
                }

            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12045));
            }
        }*/

        /// <summary>
        /// 评级规则打开按钮
        /// </summary>
        private void PointRuleBtnClicked()
        {
            pointRuleGo.SetActive(true);
        }

        /// <summary>改造按钮事件 </summary>
        private void BuildBtnClicked()
        {
            if (null != client)
            {
                if (Sys_Pet.Instance.isPerfectRemakePoint((uint)selectRemkePointIndex + 1, client.GetBuildPointByIndex(selectRemkePointIndex)))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12302));
                }
                else if (Sys_Pet.Instance.IsPetBeEffectWithSecureLock(client.petUnit))
                {
                    return;
                }
                else if(client.petUnit.SimpleInfo.ExpiredTick > 0)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000656));
                }
                else if(client.GetBuildSkillNotSave())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12305, LanguageHelper.GetTextContent(12307), LanguageHelper.GetTextContent(12306)));
                }
                else if(client.GetBuildRecastNotSave())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12305, LanguageHelper.GetTextContent(12308), LanguageHelper.GetTextContent(12306)));
                }
                else if (itemId == 0)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10908));
                }
                else
                {
                    /*bool isCurrentPetMax = client.GetPeBuildCount() >= client.petData.max_remake_num;
                    var nextLevel = Sys_Pet.Instance.GetNextUnlockRemakeData((uint)client.GetPeBuildCount() + 1);
                    bool isCurrentMax = client.GetPetLevelCanRemakeTimes() <= client.GetPeBuildCount();
                    if (isCurrentMax && null == nextLevel)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11761));
                        return;
                    }
                    else if(isCurrentPetMax)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12018));
                        return;
                    }
                    else if (isCurrentMax)
                    {
                        if(client.petData.card_type == (uint)EPetCard.Gold)
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(nextLevel.gold_remake_hint));
                        }
                        else
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(nextLevel.remake_hint));
                        }
                        return;
                    }*/

                    if (null != client)
                    {
                        int willBuildCount = selectRemkePointIndex + 1;
                        var items = Sys_Pet.Instance.GetCanUseLowsRemakeItems(willBuildCount);
                        bool isCanUse = false;
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (items[i] == itemId)
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
                    openAndRenovateAni.Play("Renovate", -1, 0f);
                    Sys_Pet.Instance.OnPetRemakeReq(client.GetPetUid(), itemId, GetSeverRemakeData());
                }
            }
        }

        /// <summary> 重置按钮事件 </summary>
        private void ReBuildBtnClicked()
        {
            if (itemId == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12009));
            }
            else
            {
                if (!Sys_Pet.Instance.isShowRemakePerfectTips)
                {
                    if (Sys_Pet.Instance.isPerfectRemakePoint((uint)selectRemkePointIndex + 1, client.petUnit.BuildInfo.GradeScoreTemp))
                    {
                        UIManager.OpenUI(EUIID.UI_Pet_RemakeTips, false, 3u);
                    }
                    else
                    {
                        openAndRenovateAni.Play("Renovate", -1, 0f);
                        Sys_Pet.Instance.OnPetRemakeReq(client.GetPetUid(), itemId, GetSeverRemakeData());
                    }
                }
                else
                {
                    openAndRenovateAni.Play("Renovate", -1, 0f);
                    
                    Sys_Pet.Instance.OnPetRemakeReq(client.GetPetUid(), itemId, GetSeverRemakeData());
                }
                
            }
        }

        /// <summary>
        /// 成功率展示界面
        /// </summary>
        private void SuccessRuleBtn()
        {
            UIManager.OpenUI(EUIID.UI_Pet_List);
        }

        public void OnAgainRight()
        {
            openAndRenovateAni.Play("Renovate", -1, 0f);
            
            Sys_Pet.Instance.OnPetRemakeReq(client.GetPetUid(), itemId, GetSeverRemakeData());
        }

        private uint GetSeverRemakeData()
        {
            uint remakeTime = 0;
            if (selectRemkePointIndex < client.GetPeBuildCount())
            {
                remakeTime = (uint)selectRemkePointIndex + 1;
            }
            return remakeTime;
        }

        private void ItemGridBeClicked(PropItem bulidItem)
        {
            if (itemId == 0)
            {
                UIManager.OpenUI(EUIID.UI_SelectItem, false,
                    new UI_SelectItemParam
                    {
                        tittle_langId = 10947,
                        getAwayId = 3010,
                        petUid = client.GetPetUid(),
                        selectRemakePointIndex = (uint)selectRemkePointIndex + 1
                    });
            }
            else
            {
                SetNoneItem();
                listener?.OnItemClear();
            }
        }

        private void SetNoneItem()
        {
            PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(0, 0, false, false, false, false, false, false, false, true, ItemGridBeClicked, _bUseTips: false);
            bulidItem.SetData(itemN, EUIID.UI_Pet_Message);
            bulidItem.txtNumber.gameObject.SetActive(false);
            bulidItem.btnNone.gameObject.SetActive(true);
            bulidItem.Layout.imgIcon.enabled = false;
            bulidItem.Layout.imgQuality.gameObject.SetActive(false);
        }

        private void OnLeftCreateCell(InfinityGridCell cell)
        {
            PetSkillCeil entry = new PetSkillCeil();
            GameObject go = cell.mRootTransform.gameObject;
            entry.BingGameObject(go);
            entry.AddClickListener(OnLeftSkillSelect);
            cell.BindUserData(entry);
        }

        private void OnLeftCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= leftSkillIdsList.Count)
                return;
            PetSkillCeil entry = cell.mUserData as PetSkillCeil;
            if (index < leftSkillIdsList.Count)
            {
                uint skillId = leftSkillIdsList[index];
                entry.SetData(skillId, false, true, index: index, hasHight: skillId == 0 ? false : client.IsHasHighBuildSkill(skillId));
            }
        }

        private void OnRightCreateCell(InfinityGridCell cell)
        {
            PetSkillCeil entry = new PetSkillCeil();
            GameObject go = cell.mRootTransform.gameObject;
            entry.BingGameObject(go);
            entry.AddClickListener(OnRightSkillSelect);
            cell.BindUserData(entry);
        }

        private void OnRightCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= rightSkillIdsList.Count)
                return;
            PetSkillCeil entry = cell.mUserData as PetSkillCeil;
            if (index < rightSkillIdsList.Count)
            {
                uint skillId = rightSkillIdsList[index];
                entry.SetData(skillId, false, true, index: index, hasHight: skillId == 0 ? false : client.IsHasHighBuildSkill(skillId), 
                    isPreview:(!client.GetBuildNotSave() && index >= client.GetPetBuildSkillList().Count));
            }
        }

        private void OnRightPreviewCreateCell(InfinityGridCell cell)
        {
            PetSkillCeil entry = new PetSkillCeil();
            GameObject go = cell.mRootTransform.gameObject;
            entry.BingGameObject(go);
            entry.AddClickListener(OnRightSkillSelect);
            cell.BindUserData(entry);
        }

        private void OnRightPreviewCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= rightSkillIdsList.Count)
                return;
            PetSkillCeil entry = cell.mUserData as PetSkillCeil;
            if (index < rightSkillIdsList.Count)
            {
                uint skillId = rightSkillIdsList[index];
                int _index = currentSelectSkillIndex + index;
                entry.SetData(skillId, false, true, index: _index, hasHight: skillId == 0 ? false : client.IsHasHighBuildSkill(skillId),
                    isPreview: (!client.GetBuildNotSave() && index >= client.GetPetBuildSkillList().Count));
            }
        }

        public int currentSelectSkillIndex
        {
            get
            {
                int total = 0;
                if (selectRemkePointIndex > 0)
                {
                    var lst = Sys_Pet.Instance.BuildSkillNum;
                    int maxNum = (int)lst[(int)selectRemkePointIndex];
                    for (int i = selectRemkePointIndex; i >= 0; i--)
                    {
                        total += (int)lst[i];
                    }
                    total -= maxNum;
                }
                return total;
            }
            
        }

        private void OnRightSkillSelect(PetSkillCeil petSkillCeil)
        {
            uint skillId = petSkillCeil.petSkillBase.skillId;
            if (skillId != 0)
            {
                UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(skillId, 0));
            }
            else
            {
                if(petSkillCeil.petSkillBase.isPreview) // 预览
                {
                    var lst = Sys_Pet.Instance.BuildSkillNumByIndex;
                    var lst1 = Sys_Pet.Instance.BuildSkillNum;
                    int skillMax = (int)lst1[(int)lst[petSkillCeil.petSkillBase.index] - 1];
                    
                    UIManager.OpenUI(EUIID.UI_PetAttributeTips_Right, false, LanguageHelper.GetTextContent(12301 , LanguageHelper.GetTextContent(10781 + lst[petSkillCeil.petSkillBase.index] - 1), skillMax.ToString()));
                }
                else//锁
                {
                    UIManager.OpenUI(EUIID.UI_PetAttributeTips_Right, false, LanguageHelper.GetTextContent(12309));
                }
            }
        }

        private void OnLeftSkillSelect(PetSkillCeil petSkillCeil)
        {
            uint skillId = petSkillCeil.petSkillBase.skillId;
            if (skillId != 0)
            {
                UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(skillId, 0));
            }
            else
            {
                if (petSkillCeil.petSkillBase.isPreview) // 预览
                {
                    var lst = Sys_Pet.Instance.BuildSkillNumByIndex;
                    var lst1 = Sys_Pet.Instance.BuildSkillNum;
                    int skillMax = (int)lst1[(int)lst[petSkillCeil.petSkillBase.index] - 1];

                    UIManager.OpenUI(EUIID.UI_PetAttributeTips_Left, false, LanguageHelper.GetTextContent(12301, LanguageHelper.GetTextContent(10781 + lst[petSkillCeil.petSkillBase.index] - 1), skillMax.ToString()));
                }
                else//锁
                {
                    UIManager.OpenUI(EUIID.UI_PetAttributeTips_Left, false, LanguageHelper.GetTextContent(12309));
                }
            }
        }

        public void Show()
        {
            if(!transform.gameObject.activeSelf)
            {
                transform.gameObject.SetActive(true);
                saveAni.Play("Close", -1, 0f);
            }
            ClosePointRuleBtnClicked();
        }

        public void Hide()
        {
            itemId = 0;
            transform.gameObject.SetActive(false);
            ClosePointRuleBtnClicked();
        }

        private void SetRemakePreviewBtn(bool hasNotSave)
        {
            buildBtn.gameObject.SetActive(!hasNotSave);
            reBuildBtn.gameObject.SetActive(hasNotSave);
            saveBtn.gameObject.SetActive(hasNotSave);
            cancelBtn.gameObject.SetActive(hasNotSave);
        }

        public void SetBuildFullState(bool state)
        {
            attrGo.SetActive(!state);
            attrFullGo.SetActive(state);
            skillGo.SetActive(!state);
            skillFullGo.SetActive(state);
        }

        public void ReSetView(uint _itemId)
        {
            itemId = _itemId;
            bool isSelectItem = itemId != 0;
            SetTipsTextState();
            if (!isSelectItem)
            {
                SetNoneItem();
            }
            else
            {
                PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(itemId, 1, true, false, false, false, false, true, true, true, ItemGridBeClicked, _bUseTips: false);
                bulidItem.SetData(itemN, EUIID.UI_Pet_Message);
                bulidItem.btnNone.gameObject.SetActive(false);
                bulidItem.Layout.imgIcon.enabled = true;
            }
        }

        private void SetTipsTextState()
        {
            bool isSelectItem = itemId != 0;
            /*bool isMax = client.GetIsBuildMax();
            if (isMax)
            {
                bulidSelectTipsText.gameObject.SetActive(false);
            }
            else*/ if(!isSelectItem)
            {
                uint willBuildCount = (uint)selectRemkePointIndex + 1;
                CSVPetNewReBuild.Data cSVPetNewReBuildData = CSVPetNewReBuild.Instance.GetConfData(willBuildCount);
                if (null != cSVPetNewReBuildData)
                {
                    TextHelper.SetText(bulidSelectTipsText, cSVPetNewReBuildData.remake_lan);
                    bulidSelectTipsText.gameObject.SetActive(true);
                }
                else
                {
                    bulidSelectTipsText.gameObject.SetActive(false);
                }
            }
            else
            {
                uint buildCount = (uint)selectRemkePointIndex + 1;
                CSVPetRemake.Data csvRemakeData = CSVPetRemake.Instance.GetConfData(itemId * 100 + buildCount);
                if (null != csvRemakeData)
                {
                    int index = selectRemkePointIndex;
                    var tempData = Sys_Pet.Instance.BuildSkillNum;
                    CSVItem.Data remakeItem = CSVItem.Instance.GetConfData(itemId);
                    if(null != remakeItem)
                    {
                        bulidSelectTipsText.gameObject.SetActive(true);
                        bulidSelectTipsText.text = LanguageHelper.GetTextContent(11999, LanguageHelper.GetLanguageColorWordsFormat(CSVLanguage.Instance.GetConfData(remakeItem.name_id).words, TextHelper.GetQuailtyLangId(remakeItem.quality)), (csvRemakeData.add_attr_rate / 100.0f * 100).ToString("0.#"), tempData[index].ToString());
                    }
                    else
                    {
                        bulidSelectTipsText.gameObject.SetActive(false);
                    }
                   
                }
                else
                {
                    uint willBuildCount = (uint)selectRemkePointIndex + 1;
                    CSVPetNewReBuild.Data cSVPetNewReBuildData = CSVPetNewReBuild.Instance.GetConfData(willBuildCount);
                    if (null != cSVPetNewReBuildData)
                    {
                        TextHelper.SetText(bulidSelectTipsText, cSVPetNewReBuildData.remake_lan);
                        bulidSelectTipsText.gameObject.SetActive(true);
                    }
                    else
                    {
                        bulidSelectTipsText.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void ReSetClientView()
        {
            if (null != client)
            {
                //是否有未保存数据
                bool hasRemakeNotSave = client.GetBuildNotSave();
                //是否改造满次数
                //bool isMax = client.GetIsBuildMax();
                SetTipsTextState();

                buildTitleText.text = LanguageHelper.GetTextContent(hasRemakeNotSave ? 60131u: 60109u);
                buildGradeTittleText.text = LanguageHelper.GetTextContent(hasRemakeNotSave ? 60103u : 60110u);
                buildSkillTittleText.text = LanguageHelper.GetTextContent(hasRemakeNotSave ? 60105u : 60111u);
                bool isOnReemake = client.IsOnReRemake(); // 是否正在重置
                leftRemakeAttrView.RefreshView(client, 0);
                //SetBuildFullState(isMax);
                pointImage.transform.parent.gameObject.SetActive(hasRemakeNotSave);

                if (!isOnReemake) // 不是在重置 - 两个状态 点中已经改造过- 点中未改造
                {
                    var remakeCount = client.GetPeBuildCount();
                    if (selectRemkePointIndex == remakeCount) //选中未改造完成的-即 选中当前可以改造格子
                    {
                        rightSkillIdsList.Clear();
                        uint showType = 1;
                        if(hasRemakeNotSave) // 有未保存
                        {
                            rightSkillIdsList.AddRange(client.petUnit.BuildInfo.BuildSkills);
                            ImageHelper.SetIcon(pointImage, Sys_Pet.Instance.GetGradeStampImageId((uint)selectRemkePointIndex + 1, client.petUnit.BuildInfo.GradeScoreTemp), true);
                            rightSkillIdsList.AddRange(client.petUnit.BuildInfo.SkillTemp);

                            showType = 2;
                        }
                        else // 没有未保存
                        {
                            rightSkillIdsList.AddRange(client.petUnit.BuildInfo.BuildSkills);
                            var lst = Sys_Pet.Instance.BuildSkillNum;
                            int previewCount = (int)lst[(int)client.petUnit.BuildInfo.BuildCount];
                            for (int i = 0; i < previewCount; i++)
                            {
                                rightSkillIdsList.Add(0);
                            }
                            showType = 1;
                        }
                        rightInfinityGrid.CellCount = rightSkillIdsList.Count;
                        rightInfinityGrid.ForceRefreshActiveCell();
                        rightRemakeAttrView.RefreshView(client, showType);
                        rightRe_RemakePreViewGo.SetActive(false);
                        rightPreViewGo.SetActive(true);
                    }
                    else//选中已改造的
                    {
                        // 选中已改造完成，没有未保存
                       
                        reRemakePointText.text = LanguageHelper.GetTextContent(12317, LanguageHelper.GetTextContent( 10781 + (uint)selectRemkePointIndex), LanguageHelper.GetTextContent(Sys_Pet.Instance.GetGradeStampLangId((uint)selectRemkePointIndex + 1, client.GetBuildPointByIndex(selectRemkePointIndex))) );
                        remakeAddGradeText.text = LanguageHelper.GetTextContent(12318, LanguageHelper.GetTextContent(10781 + (uint)selectRemkePointIndex), client.GetAddGradeCountByReamkeTime(selectRemkePointIndex).ToString());
                        ImageHelper.SetIcon(pointImage, Sys_Pet.Instance.GetGradeStampImageId((uint)selectRemkePointIndex + 1, client.petUnit.BuildInfo.GradeScoreTemp), true);
                        ImageHelper.SetIcon(remakePreviewImage, Sys_Pet.Instance.GetGradeStampImageId((uint)selectRemkePointIndex + 1, client.GetBuildPointByIndex(selectRemkePointIndex)), true);
                        rightSkillIdsList.Clear();
                        rightSkillIdsList = client.GetPetRemakeSkilByRemakeTimes(selectRemkePointIndex);
                        rightRemakeInfinityGrid.CellCount = rightSkillIdsList.Count;
                        rightRemakeInfinityGrid.ForceRefreshActiveCell();
                        rightRe_RemakePreViewGo.SetActive(true);
                        rightPreViewGo.SetActive(false);
                    }
                }
                else // 在重置
                {
                    if (hasRemakeNotSave) // 有未保存
                    {
                        ImageHelper.SetIcon(pointImage, Sys_Pet.Instance.GetGradeStampImageId((uint)selectRemkePointIndex + 1, client.petUnit.BuildInfo.GradeScoreTemp), true);
                    }
                    rightSkillIdsList.Clear();
                    rightSkillIdsList = client.GetReRemakePreviewSkill();

                    rightInfinityGrid.CellCount = rightSkillIdsList.Count;
                    rightInfinityGrid.ForceRefreshActiveCell();

                    rightRemakeAttrView.RefreshView(client, 5);
                    rightRe_RemakePreViewGo.SetActive(false);
                    rightPreViewGo.SetActive(true);
                }
                

                leftSkillIdsList.Clear();
                leftSkillIdsList.AddRange(client.petUnit.BuildInfo.BuildSkills);
                leftInfinityGrid.CellCount = leftSkillIdsList.Count;
                leftInfinityGrid.ForceRefreshActiveCell();
                SetRemakePreviewBtn(hasRemakeNotSave);

                var currentRemakeLevel = client.GetPetLevelCanRemakeTimes();
               
                bool isMaxLevel = client.petData.max_remake_num <= currentRemakeLevel- client.GetPetDemonSoulRemakeTimes();// 等级解锁次数是否最高

                TextHelper.SetText(buildCountText, 12007, client.GetPeCanBuildCount().ToString(), currentRemakeLevel.ToString());
                if (!isMaxLevel) //全满
                {
                    var nextLevel = Sys_Pet.Instance.GetNextUnlockRemakeData((uint)currentRemakeLevel + 1);
                    if (client.petData.card_type == (uint)EPetCard.Gold)
                    {
                        TextHelper.SetText(buildNextText, nextLevel.gold_remake_hint);
                    }
                    else
                    {
                        TextHelper.SetText(buildNextText, nextLevel.remake_hint);
                    }
                    
                }
                buildNextText.gameObject.SetActive(!isMaxLevel);

                //印章显示灰态会加载一个灰态材质，当首次创建时耗时严重（只在第一次） 如需优化可提前加载材质，或改变灰态显示方法
                for (int i = 0; i < ceilList.Count; i++)
                {
                    UI_PetRemakePointBtn ceil = ceilList[i];
                    ceil.RemoveEvent();
                    PoolManager.Recycle(ceil);
                }
                ceilList.Clear();

                FrameworkTool.CreateChildList(starParent, client.GetPetLevelCanRemakeTimes());
                for (int i = 0; i < starParent.childCount; i++)
                {
                    UI_PetRemakePointBtn ceil = PoolManager.Fetch<UI_PetRemakePointBtn>();
                    ceil.Init(starParent.GetChild(i));
                    ceil.AddSelectBtnListener(OnSelectRemakeSelect);
                    ceil.SetData(i, client, selectRemkePointIndex);
                    ceilList.Add(ceil);
                }
            }
        }

        private void OnSelectRemakeSelect(int index)
        {
            int remakeCount = client.GetPeBuildCount();
            if (index >= remakeCount + 1 )
            {
                return;
            }
            else if(client.GetBuildNotSave() && index != selectRemkePointIndex)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12336));
                return;
            }
            selectRemkePointIndex = index;
            SetBuildBtnState();
            SetTipsTextState();
            ReSetClientView();
            for (int i = 0; i < ceilList.Count; i++)
            {
                var _ceil = ceilList[i];
                _ceil.SetSelectState(selectRemkePointIndex == _ceil.index);
            }
        }

        public void SetView(ClientPet _client, bool isItemRefesh = false)
        {
            if(null != client && null != _client && client.GetPetUid() != _client.GetPetUid())
            {
                selectRemkePointIndex = -1;
                itemId = 0;
            }
            this.client = _client;
            if (!isItemRefesh)
                InitSelectIndex();
            SetBuildBtnState();
            ReSetClientView();
            ItemIdCount idCount = new ItemIdCount(itemId, 1);
            if (!idCount.Enough)
            {
                itemId = 0;
                ReSetView(itemId);
            }
            else
            {
                ReSetView(itemId);
            }
        }

        private void InitSelectIndex()
        {
            if (selectRemkePointIndex == -1)
                selectRemkePointIndex = 0;

            if (client.GetBuildNotSave())
            {
                if (client.petUnit.BuildInfo.Index > 0)
                {
                    selectRemkePointIndex = (int)client.petUnit.BuildInfo.Index - 1;
                }
                else
                {
                    if(null == client)
                        return;
                
                    selectRemkePointIndex = client.GetPeBuildCount();
                }
            }
            

                /*if(null == client)
                {
                    return;
                }
                int maxLevelTimes = client.GetPetLevelCanRemakeTimes();
                int currentRemakeTimes = client.GetPeBuildCount();
                if(maxLevelTimes == 0)
                {
                    selectRemkePointIndex = -1;
                }
                else if (client.GetBuildNotSave() && client.petUnit.BuildInfo.Index > 0)
                {
                    if(needChange)
                    {
                        needChange = false;
                        selectRemkePointIndex = (int)client.petUnit.BuildInfo.Index - 1;
                    }
                }
                else if(currentRemakeTimes == maxLevelTimes)
                {
                    if (needChange)
                    {
                        selectRemkePointIndex = 0;
                    }
                }
                else
                {
                    if (needChange)
                    {
                        selectRemkePointIndex = currentRemakeTimes;
                    }
                }*/
                }

                private void SetBuildBtnState()
        {
            ImageHelper.SetImageGray(buildBtn, Sys_Pet.Instance.isPerfectRemakePoint((uint)selectRemkePointIndex + 1, client.GetBuildPointByIndex(selectRemkePointIndex)), true);
        }

        public void Register(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void OnItemClear();
        }
    }

    public class UI_Pet_Recast
    {
        private Transform transform;
        private UI_Pet_BuildAdvanceView leftRemakeAttrView;
        private UI_Pet_BuildAdvanceView rightRemakeAttrView;

        private Button recastBtn;
        private Button againBtn;
        private Button saveBtn;
        private Button cancelBtn;
        private Button eatFruitBtn;
        private Text gradeTips;
        private Text fruitTips;
        private PropItem buildItem;

        private PropItem bulidItem2;

        private ClientPet client;

        private ItemIdCount itemIdCount = null;
        private Animator saveAni;
        private Animator openAndRenovateAni;

        private List<Transform> fxTrans = new List<Transform>(5);
        private List<GameObject> fxGo = new List<GameObject>(2);

        public IListener listener;
        public void Init(Transform transform)
        {
            this.transform = transform;
            leftRemakeAttrView = new UI_Pet_BuildAdvanceView();
            leftRemakeAttrView.Init(transform.Find("View_Left/GameObject/Atrr"));
            rightRemakeAttrView = new UI_Pet_BuildAdvanceView();
            rightRemakeAttrView.Init(transform.Find("View_Right/GameObject/Atrr"));

            recastBtn = transform.Find("View_Bottom/Btn_Remodeling").GetComponent<Button>();
            recastBtn.onClick.AddListener(ReCastBtnClicked);

            againBtn = transform.Find("View_Bottom/Btn_Again").GetComponent<Button>();
            againBtn.onClick.AddListener(AgainBtnClicked);

            saveBtn = transform.Find("View_Bottom/Btn_Save").GetComponent<Button>();
            saveBtn.onClick.AddListener(SaveBtnClicked);

            cancelBtn = transform.Find("View_Bottom/Btn_Delete").GetComponent<Button>();
            cancelBtn.onClick.AddListener(CancelBtnClicked);

            eatFruitBtn = transform.Find("View_Fruit/Btn01").GetComponent<Button>();
            eatFruitBtn.onClick.AddListener(EatFruitBtnClicked);

            gradeTips = transform.Find("View_Right/Text_Tips").GetComponent<Text>();
            fruitTips = transform.Find("View_Fruit/Text_Tips").GetComponent<Text>();


            buildItem = new PropItem();
            buildItem.BindGameObject(transform.Find("View_Bottom/PropItem").gameObject);

            bulidItem2 = new PropItem();
            bulidItem2.BindGameObject(transform.Find("View_Fruit/PropItem").gameObject);
            PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(0, 0, false, false, false, false, false, false, false, true, ItemGridBeClicked, _bUseTips: false);
            bulidItem2.SetData(itemN, EUIID.UI_Pet_Message);
            bulidItem2.btnNone.gameObject.SetActive(true);
            bulidItem2.Layout.imgQuality.gameObject.SetActive(false);

            saveAni = transform.GetComponent<Animator>();
            openAndRenovateAni = transform.Find("View_Right/GameObject").GetComponent<Animator>();
            var itemIdAndCount = Sys_Pet.Instance.RecastCostItemAndIds;
            if (null != itemIdAndCount)
            {
                for (int i = 0; i < itemIdAndCount.Count; i++)
                {
                    if (null != itemIdAndCount[i] && itemIdAndCount[i].Count >= 2)
                    {
                        itemIdCount = new ItemIdCount(itemIdAndCount[i][0], itemIdAndCount[i][1]);
                        break;
                    }
                }
            }

            fxTrans.Add(transform.Find("View_Left/GameObject/Atrr/Vit"));
            fxTrans.Add(transform.Find("View_Left/GameObject/Atrr/Pow"));
            fxTrans.Add(transform.Find("View_Left/GameObject/Atrr/Str"));
            fxTrans.Add(transform.Find("View_Left/GameObject/Atrr/Spe"));
            fxTrans.Add(transform.Find("View_Left/GameObject/Atrr/Ma"));
            fxGo.Add(transform.Find("Fx_Ui_Pet_Message3_View_Remake_Fruit").gameObject);
        }

        /// <summary>重塑按钮事件 </summary>
        private void ReCastBtnClicked()
        {
            if (null != client)
            {
                var count = client.GetPeBuildCount();
                if (client.GetBuildNotSave())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12305, LanguageHelper.GetTextContent(12306), LanguageHelper.GetTextContent(12308)));
                    return;
                }
                else if (client.GetBuildSkillNotSave())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12305, LanguageHelper.GetTextContent(12307), LanguageHelper.GetTextContent(12308)));
                    return;
                }
                if (count >= Sys_Pet.Instance.RecastNeedBuildCount)
                {
                    var itemIdAndCount = Sys_Pet.Instance.RecastCostItemAndIds;
                    if (null != itemIdAndCount)
                    {
                        for (int i = 0; i < itemIdAndCount.Count; i++)
                        {
                            if (null != itemIdAndCount[i] && itemIdAndCount[i].Count >= 2)
                            {
                                ItemIdCount itemIdCount = new ItemIdCount(itemIdAndCount[i][0], itemIdAndCount[i][1]);
                                if (!itemIdCount.Enough)
                                {
                                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12033));
                                    return;
                                }
                            }
                        }
                    }
                    openAndRenovateAni.Play("Renovate", -1, 0f);
                    Sys_Pet.Instance.PetRemakeReGradeReq(client.GetPetUid());
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12034, Sys_Pet.Instance.RecastNeedBuildCount.ToString()));
                }
            }
        }

        public void OnAgainRight()
        {
            openAndRenovateAni.Play("Renovate", -1, 0f);
            Sys_Pet.Instance.PetRemakeReGradeReq(client.GetPetUid());
        }

        /// <summary>重塑按钮事件 </summary>
        public void AgainBtnClicked()
        {
            if (null != client)
            {
                var count = client.GetPeBuildCount();
                if (client.GetBuildNotSave())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12035));
                    return;
                }
                if (count >= Sys_Pet.Instance.RecastNeedBuildCount)
                {
                    var itemIdAndCount = Sys_Pet.Instance.RecastCostItemAndIds;
                    if (null != itemIdAndCount)
                    {
                        for (int i = 0; i < itemIdAndCount.Count; i++)
                        {
                            if (null != itemIdAndCount[i] && itemIdAndCount[i].Count >= 2)
                            {
                                ItemIdCount itemIdCount = new ItemIdCount(itemIdAndCount[i][0], itemIdAndCount[i][1]);
                                if (!itemIdCount.Enough)
                                {
                                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12033));
                                    return;
                                }
                            }
                        }
                    }
                    if(!Sys_Pet.Instance.isShowRemakeTips)
                    {
                        UIManager.OpenUI(EUIID.UI_Pet_RemakeTips, false, 2u);
                    }
                    else
                    {
                        openAndRenovateAni.Play("Renovate", -1, 0f);
                        Sys_Pet.Instance.PetRemakeReGradeReq(client.GetPetUid());
                    }
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12034, Sys_Pet.Instance.RecastNeedBuildCount.ToString()));
                }
            }
        }

        /// <summary>
        /// 取消改造结果
        /// </summary>
        private void CancelBtnClicked()
        {
            PromptBoxParameter.Instance.OpenPromptBox(LanguageHelper.GetTextContent(12042),
                           0, () =>
                           {
                               Sys_Pet.Instance.PetRemakeSelectGradeReq(client.GetPetUid(), 0);
                           });
        }

        /// <summary>
        /// 保存改造结构
        /// </summary>
        private void SaveBtnClicked()
        {
            saveAni.Play("Open", -1, 0f);
            Sys_Pet.Instance.PetRemakeSelectGradeReq(client.GetPetUid(), 1);
        }

        private void EatFruitBtnClicked()
        {

            var count = client.GetPeBuildCount();
            if (count < Sys_Pet.Instance.RecastNeedBuildCount)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(15105));
                return;
            }
            else if (client.GetBuildNotSave())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(15102));
                return;
            }
            else if (client.GetBuildSkillNotSave())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(15103));
                return;
            }
            else if (client.GetBuildRecastNotSave())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(15104));
                return;
            }
            else if (itemId == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(15100));
                return;
            }
            EBaseAttr selectItem2BaseAttr = Sys_Pet.Instance.GetPetRemakeGradeTypeByItemId(itemId);
            uint attrCount = client.GetPetBuildGradeAttr(selectItem2BaseAttr);
            if(attrCount < 1)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(15108));
                return;
            }

            Sys_Pet.Instance.PetEatFruitReq(client.GetPetUid(), itemId);
        }

        public void Show()
        {
            if(!transform.gameObject.activeSelf)
            {
                saveAni.Play("Close", -1, 0f);
                transform.gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            itemId = 0;
            for (int i = 0; i < fxGo.Count; i++)
            {
                fxGo[i].gameObject.SetActive(false);
            }
            transform.gameObject.SetActive(false);
        }

        private void SetRemakePreviewBtn(bool hasNotSave)
        {
            recastBtn.gameObject.SetActive(!hasNotSave);
            againBtn.gameObject.SetActive(hasNotSave);
            saveBtn.gameObject.SetActive(hasNotSave);
            cancelBtn.gameObject.SetActive(hasNotSave);
        }

        public void ReSetView()
        {
            
            if (null != client)
            {
                //是否有重塑未保存数据
                bool hasRemakeNotSave = client.GetBuildRecastNotSave();
                //是否改造满次数
                bool isMax = client.GetIsBuildMax();
                leftRemakeAttrView.RefreshView(client, 0);
                if (hasRemakeNotSave)
                {
                    rightRemakeAttrView.RefreshView(client, 3);
                }
                else
                {
                    rightRemakeAttrView.RefreshView(client, 4);
                }
                TextHelper.SetText(gradeTips, 12115, client.GetPeBuildGradeCount().ToString());
                SetRemakePreviewBtn(hasRemakeNotSave);
            }
            ResetRecastItem();
        }

        public void ReSetView(uint _itemId)
        {
            itemId = _itemId;
            bool isSelectItem = itemId != 0;
            if (!isSelectItem)
            {
                SetNoneItem();
            }
            else
            {
                PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(itemId, 1, true, false, false, false, false, true, true, true, ItemGridBeClicked, _bUseTips: false);
                bulidItem2.SetData(itemN, EUIID.UI_Pet_Message);
                bulidItem2.btnNone.gameObject.SetActive(false);
                bulidItem2.Layout.imgIcon.enabled = true;

                if(null != client)
                {
                    CSVItem.Data itemData = CSVItem.Instance.GetConfData(itemId);
                    if(null != itemData)
                    {
                        EBaseAttr selectItem2BaseAttr = Sys_Pet.Instance.GetPetRemakeGradeTypeByItemId(itemId);
                        TextHelper.SetText(fruitTips, LanguageHelper.GetTextContent(15101, LanguageHelper.GetTextContent(itemData.name_id), LanguageHelper.GetTextContent(GetGradeNameId(selectItem2BaseAttr))));
                    }
                }
            }
        }

        public uint GetGradeNameId(EBaseAttr baseAttr)
        {
           switch(baseAttr)
            {
                case EBaseAttr.Vit:
                    return 2011135;
                case EBaseAttr.Snh:
                    return 2011136;
                case EBaseAttr.Inten:
                    return 2011137;
                case EBaseAttr.Speed:
                    return 2011138;
                case EBaseAttr.Magic:
                    return 2011139;
                default:
                    return 2011135; 
            }
        }

        public void SetView(ClientPet _client)
        {
            if (null == _client)
            {
                return;
            }
            this.client = _client;
            if (null != client && null != _client && client.GetPetUid() != _client.GetPetUid())
            {
                itemId = 0;
            }

            ItemIdCount idCount = new ItemIdCount(itemId, 1);
            if (!idCount.Enough)
            {
                itemId = 0;
                ReSetView(itemId);
            }
            else
            {
                ReSetView(itemId);
            }
            ReSetView();
        }

        private void SetNoneItem()
        {
            TextHelper.SetText(fruitTips, 15100);
            PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(0, 0, false, false, false, false, false, false, false, true, ItemGridBeClicked, _bUseTips: false);
            bulidItem2.SetData(itemN, EUIID.UI_Pet_Message);
            bulidItem2.txtNumber.gameObject.SetActive(false);
            bulidItem2.btnNone.gameObject.SetActive(true);
            bulidItem2.Layout.imgIcon.enabled = false;
            bulidItem2.Layout.imgQuality.gameObject.SetActive(false);
        }
        uint itemId = 0;
        private void ItemGridBeClicked(PropItem bulidItem)
        {
            if (itemId == 0)
            {
                UIManager.OpenUI(EUIID.UI_SelectItem, false,
                    new UI_SelectItemParam
                    {
                        tittle_langId = 15100,
                        getAwayId = 3035,
                        petUid = client.GetPetUid()
                    });
            }
            else
            {
                
                SetNoneItem();
                listener?.OnItemClear();
            }
        }

        private void ResetRecastItem()
        {
            if(null != itemIdCount)
            {
                PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(itemIdCount.id, itemIdCount.count, true, false, false, false, false, true, true, true, _bshowBtnNo : !itemIdCount.Enough, _bUseTips: true);
                buildItem.SetData(itemN, EUIID.UI_Pet_Message);
            }
        }

        public void OnEatFruitEnd(List<uint> list)
        {
            for (int i = 0; i < fxGo.Count; i++)
            {
                fxGo[i].gameObject.SetActive(false);
            }

            int needFxCount = list.Count;
            if (fxGo.Count < needFxCount)
            {
                GameObject go = GameObject.Instantiate(fxGo[0]);
                fxGo.Add(go);
            }

            for (int i = 0; i < needFxCount; i++)
            {
                fxGo[i].transform.SetParent(fxTrans[(int)list[i]].transform, false);
                fxGo[i].SetActive(true);
            }
        }

        public void Register(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void OnItemClear();
        }
    }

    public class UI_PetRemakeInfo
    {
        private Text scoreText;
        private Text gradeText;
        private Text addGradeText;

        public void Init(Transform transform)
        {
            scoreText = transform.Find("Grade/Text_Value").GetComponent<Text>();
            gradeText = transform.Find("Gears/Text_Value").GetComponent<Text>();
            addGradeText = transform.Find("Remake/Text_Value").GetComponent<Text>();
        }

        public void SetView(ClientPet pet)
        {
            scoreText.text = pet.petUnit.SimpleInfo.Score.ToString();
            uint lowG = pet.GetPetMaxGradeCount() - pet.GetPetGradeCount();
            bool isMax = lowG == 0;
            if (isMax)
            {
                TextHelper.SetText(gradeText, LanguageHelper.GetTextContent(60007, pet.GetPetCurrentGradeCount().ToString(), pet.GetPetBuildMaxGradeCount().ToString(), LanguageHelper.GetTextContent(11802)));
            }
            else
            {
                TextHelper.SetText(gradeText, LanguageHelper.GetTextContent(60007, pet.GetPetCurrentGradeCount().ToString(), pet.GetPetBuildMaxGradeCount().ToString(), LanguageHelper.GetTextContent(11801, lowG.ToString())));
            }
            addGradeText.text = pet.GetPeBuildGradeCount().ToString();
        }
    }

    public class UI_Pet_Remake : UI_Pet_RemakeAttr.IListener, UI_Pet_RemakeSkill.IListener, UI_Pet_Recast.IListener
    {
        /// <summary>
        /// Remake = 改造
        /// skill = 领悟
        /// Recast = 重塑
        /// </summary>
        public enum EPetBuildAttrType
        {
            Remake,
            Skill,
            Recast,
        }
        private Transform transform;
        private Toggle attrToggle;
        //技能toggle
        private Toggle skillToggle;
        //技能toggle
        private Toggle reBuildToggle;
        private UI_PetRemakeInfo baseInfo;
        private UI_Pet_RemakeAttr attrView;
        private UI_Pet_RemakeSkill skillView;
        private UI_Pet_Recast recastView;
        private ClientPet clientPet;
        private EPetBuildAttrType ePetBuildAttr;

        public void Init(Transform transform)
        {
            this.transform = transform;
            attrToggle = transform.Find("Menu/ListItem").GetComponent<Toggle>();
            attrToggle.onValueChanged.AddListener(OnAttrToggleChange);

            skillToggle = transform.Find("Menu/ListItem (1)").GetComponent<Toggle>();
            skillToggle.onValueChanged.AddListener(OnSkillToggleChange);

            reBuildToggle = transform.Find("Menu/ListItem (2)").GetComponent<Toggle>();
            reBuildToggle.onValueChanged.AddListener(OnRuBuildToggleChange);

            baseInfo = new UI_PetRemakeInfo();
            baseInfo.Init(transform.Find("Title"));

            attrView = new UI_Pet_RemakeAttr();
            attrView.Init(transform.Find("View_Remake"));
            attrView.Register(this);

            skillView = new UI_Pet_RemakeSkill();
            skillView.Init(transform.Find("View_Skill"));
            skillView.Register(this);

            recastView = new UI_Pet_Recast();
            recastView.Init(transform.Find("View_Gears"));
            recastView.Register(this);
        }

        public void Show()
        {
            if(!transform.gameObject.activeSelf)
                transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            ePetBuildAttr = EPetBuildAttrType.Remake;
            attrView?.Hide();
            skillView?.Hide();
            recastView?.Hide();
            transform.gameObject.SetActive(false);
        }

        public void OnRemakeTipsEntry(uint type)
        {
            if(type == 1 && ePetBuildAttr == EPetBuildAttrType.Skill)
            {
                skillView.OnAgainRight();
            }
            else if (type == 2 && ePetBuildAttr == EPetBuildAttrType.Recast)
            {
                recastView.OnAgainRight();
            }
            else if (type == 3 && ePetBuildAttr == EPetBuildAttrType.Remake)
            {
                attrView.OnAgainRight();
            }
        }

        public void OnRemakeSkillEnd(bool isSuccess)
        {
            if (ePetBuildAttr == EPetBuildAttrType.Skill)
            {
                skillView.OnRemakeSkillEnd(isSuccess);
            }
        }

        public void OnEatFruitEnd(List<uint> list)
        {
            if (ePetBuildAttr == EPetBuildAttrType.Recast)
            {
                recastView.OnEatFruitEnd(list);
            }
        }

        public void SetView(ClientPet client)
        {
            clientPet = client;
            baseInfo.SetView(clientPet);
            ViewControl();
        }

        public void RefeshView(ClientPet client, bool isItemRefesh = false)
        { 
            clientPet = client;
            baseInfo.SetView(clientPet);
            switch (ePetBuildAttr)
            {
                case EPetBuildAttrType.Remake:
                    if (attrToggle.isOn)
                    {
                        attrView.SetView(clientPet, isItemRefesh);
                    }
                    else
                    {
                        attrToggle.isOn = true;
                    }
                    break;
                case EPetBuildAttrType.Skill:
                    if (skillToggle.isOn)
                    {
                        skillView.SetView(clientPet);
                    }
                    else
                    {
                        skillToggle.isOn = true;
                    }
                    break;
                case EPetBuildAttrType.Recast:
                    if (reBuildToggle.isOn)
                    {
                        recastView.SetView(clientPet);
                    }
                    else
                    {
                        reBuildToggle.isOn = true;
                    }
                    break;
            }
        }

        public void InitView2MessageEx(ClientPet client, int currentPage)
        {
            ePetBuildAttr = (EPetBuildAttrType)currentPage;
            SetView(client);
        }

        public void OnSelectItem(uint itemId)
        {
            if (ePetBuildAttr == EPetBuildAttrType.Remake)
            {
                attrView.ReSetView(itemId);
            }
            else if (ePetBuildAttr == EPetBuildAttrType.Skill)
            {
                skillView.ReSetView(itemId);
            }
            else if (ePetBuildAttr == EPetBuildAttrType.Recast)
            {
                recastView.ReSetView(itemId);
            }
        }


        public void OnItemClear()
        {
            OnSelectItem(0);
        }

        public void ViewControl()
        {
            switch (ePetBuildAttr)
            {
                case EPetBuildAttrType.Remake:
                    if (attrToggle.isOn)
                    {
                        attrView.Show();
                        attrView.SetView(clientPet);
                    }
                    else
                    {
                        attrToggle.isOn = true;
                    }
                    break;
                case EPetBuildAttrType.Skill:
                    if (skillToggle.isOn)
                    {
                        skillView.Show();
                        skillView.SetView(clientPet);
                    }
                    else
                    {
                        skillToggle.isOn = true;
                    }
                    break;
                case EPetBuildAttrType.Recast:
                    if (reBuildToggle.isOn)
                    {
                        recastView.Show();
                        recastView.SetView(clientPet);
                    }
                    else
                    {
                        reBuildToggle.isOn = true;
                    }
                    break;
            }
        }

        private void OnAttrToggleChange(bool isOn)
        {
            if (isOn)
            {
                ePetBuildAttr = EPetBuildAttrType.Remake;
                skillView.Hide();
                recastView.Hide();
                attrView.Show();
                attrView.SetView(clientPet);
            }
        }

        private void OnSkillToggleChange(bool isOn)
        {
            if (isOn)
            {
                ePetBuildAttr = EPetBuildAttrType.Skill;
                attrView.Hide();
                recastView.Hide();
                skillView.Show();
                skillView.SetView(clientPet);
            }
        }

        private void OnRuBuildToggleChange(bool isOn)
        {
            if (isOn)
            {
                ePetBuildAttr = EPetBuildAttrType.Recast;
                attrView.Hide();
                skillView.Hide();
                recastView.Show();
                recastView.SetView(clientPet);
            }
        }
    }
}