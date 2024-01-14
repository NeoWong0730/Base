using Logic.Core;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Table;
using Packet;
using Lib.Core;

namespace Logic
{
    public class TransformAddCeil
    {
        public Transform transform;
        private Image icon;
        private Image type;
        private Text openLv;
        private Text name;
        private Text skillLv;
        private GameObject typeGo;
        private GameObject lockGo;
        private GameObject tipGo;
        private GameObject selectGo;
        private Button button;
        public Action<uint> action;

        public uint gridId;
        private CSVAllRaceBonus.Data csvAllRaceBonusData;
        private ShapeShiftSkillGrid shapeShiftSkillGrid;
        private ClientShapeShiftPlan clientShapeShiftPlan;

        public void Init(Transform _transform)
        {
            transform = _transform;
            icon = transform.Find("PetSkillItem01/Image_Skill").GetComponent<Image>();
            type = transform.Find("PetSkillItem01/Image_Typebg/Image_Type").GetComponent<Image>();
            openLv = transform.Find("View_Lock_Transfiguration/Image_BG/Text").GetComponent<Text>();
            name = transform.Find("Text_Name").GetComponent<Text>();
            skillLv = transform.Find("PetSkillItem01/Level").GetComponent<Text>();
            typeGo = transform.Find("PetSkillItem01/Image_Typebg").gameObject;
            lockGo = transform.Find("View_Lock_Transfiguration").gameObject;
            tipGo = transform.Find("PetSkillItem01/Image_Tip").gameObject;
            selectGo = transform.Find("Image_Select").gameObject;

            button = transform.GetComponent<Button>();
            button.onClick.AddListener(OnClicked);
        }

        public void SetData(uint _id, ShapeShiftSkillGrid _shapeShiftSkillGrid)
        {
            gridId = _id;
            shapeShiftSkillGrid = _shapeShiftSkillGrid;
            if (_shapeShiftSkillGrid == null)
            {
                clientShapeShiftPlan = Sys_Transfiguration.Instance.GetCurUseShapeShiftData();
                lockGo.SetActive(true);
                tipGo.gameObject.SetActive(false);
                icon.gameObject.SetActive(false);
                name.gameObject.SetActive(false);
                typeGo.gameObject.SetActive(false);
                skillLv.text = string.Empty;
                csvAllRaceBonusData = CSVAllRaceBonus.Instance.GetConfData(gridId);
                if (csvAllRaceBonusData!=null)
                {
                    if (clientShapeShiftPlan.CanUnLockAllStudy(csvAllRaceBonusData.race_rank))
                    {
                        TextHelper.SetText(openLv, 2013503);
                    }
                    else
                    {
                        TextHelper.SetText(openLv, 2013504, csvAllRaceBonusData.race_rank.ToString());
                    }
                }
            }
            else
            {
                lockGo.SetActive(false);
                icon.gameObject.SetActive(true);
                name.gameObject.SetActive(true);
                typeGo.gameObject.SetActive(true);
                CSVPassiveSkillInfo.Data info = CSVPassiveSkillInfo.Instance.GetConfData(shapeShiftSkillGrid.Skillid);
                CSVRaceSkillType.Data skillType = CSVRaceSkillType.Instance.GetConfData(shapeShiftSkillGrid.Skillid);
                if (info != null && skillType != null)
                {
                    ImageHelper.SetIcon(icon, info.icon);
                    ImageHelper.SetIcon(type, skillType.skill_type);
                    TextHelper.SetText(name, info.name);
                    tipGo.gameObject.SetActive(skillType.type == 1);
                    TextHelper.SetText(skillLv, 12462, info.level.ToString());
                }
            }
        }

        public void Refresh(ShapeShiftSkillGrid _shapeShiftSkillGrid)
        {
            SetData(gridId, _shapeShiftSkillGrid);
        }

        public void SetSelect(uint selectId)
        {
            selectGo.SetActive(selectId == gridId);
        }

        public void AddAction(Action<uint> action)
        {
            this.action = action;
        }

        private void OnClicked()
        {
            action?.Invoke(gridId);
        }
    }

    public class UI_Transfiguration_Add : UIComponent
    {
        private Text unlockTip;
        private Text unlockLevel;
        private GameObject unlockRaceItem;
        private GameObject lockView;

        private GameObject costItem;
        private Button unlockBtn;
        private GameObject unlockView;

        private Image skillBeforeIcon;
        private Image typeBeforeIcon;
        private GameObject tipBeforeGo;
        private Text levelBefore;
        private Text skillBeforeName;
        private Text skillBeforeDes;

        private Image skillAfterIcon;
        private Image typeAfterIcon;
        private GameObject tipAfterGo;
        private Text levelAfter;
        private Text skillAfterName;
        private Text skillAfterDes;

        private Button eyeBtn;
        private GameObject rebuildCostItem;
        private Button rebuildBtn;
        private Button saveBtn;
        private Button giveUpBtn;
        private GameObject rebuildView;
        private GameObject afterSkillGo;
        private GameObject fxGo;
        private Timer timer;

        private InfinityGrid infinityGridSkill;
        private Dictionary<GameObject, TransformAddCeil> skillCeilDic = new Dictionary<GameObject, TransformAddCeil>();
        private List<TransformAddCeil> skillCellItems = new List<TransformAddCeil>();
        private int infinityCountSkill;

        private uint curRaceId = 0;
        private uint curSkillGridId;
        private PropItem propItem;
        private CSVAllRaceBonus.Data csvAllRaceBonusData;
        private CSVRaceChangeSkill.Data csvRaceChangeSkillData;
        private List<CSVAllRaceBonus.Data> skillCsv = new List<CSVAllRaceBonus.Data>();
        private Dictionary<uint, ShapeShiftSkillGrid> skillDataDic = new Dictionary<uint, ShapeShiftSkillGrid>();
        private ClientShapeShiftPlan clientShapeShiftPlan;

        protected override void Loaded()
        {
            unlockTip = transform.Find("View_Lock/Level/Image_Title/Text_Condition").GetComponent<Text>();
            unlockLevel = transform.Find("View_Lock/Level/Image_Nothing/Text").GetComponent<Text>();
            unlockRaceItem = transform.Find("View_Lock/Level/Grid/Item").gameObject;
            lockView = transform.Find("View_Lock").gameObject;

            costItem = transform.Find("View_Unlock/Cost/PropItem").gameObject;
            unlockBtn = transform.Find("View_Unlock/Btn_Unlock").GetComponent<Button>();
            unlockBtn.onClick.AddListener(OnUnlockBtnClicked);
            unlockView = transform.Find("View_Unlock").gameObject;

            skillBeforeIcon = transform.Find("View_Rebuild/Skill_1/Skillbg/Icon").GetComponent<Image>();
            typeBeforeIcon = transform.Find("View_Rebuild/Skill_1/Skillbg/Image_Typebg/Image_Type").GetComponent<Image>();
            levelBefore = transform.Find("View_Rebuild/Skill_1/Skillbg/Image_Levelbg/Level").GetComponent<Text>();
            tipBeforeGo = transform.Find("View_Rebuild/Skill_1/Skillbg/Image_Tip").gameObject;
            skillBeforeName = transform.Find("View_Rebuild/Skill_1/Text_Name").GetComponent<Text>();
            skillBeforeDes = transform.Find("View_Rebuild/Skill_1/Text_Description").GetComponent<Text>();

            skillAfterIcon = transform.Find("View_Rebuild/Skill_2/Skillbg/Icon").GetComponent<Image>();
            typeAfterIcon = transform.Find("View_Rebuild/Skill_2/Skillbg/Image_Typebg/Image_Type").GetComponent<Image>();
            levelAfter = transform.Find("View_Rebuild/Skill_2/Skillbg/Image_Levelbg/Level").GetComponent<Text>();
            tipAfterGo = transform.Find("View_Rebuild/Skill_2/Skillbg/Image_Tip").gameObject;
            skillAfterName = transform.Find("View_Rebuild/Skill_2/Text_Name").GetComponent<Text>();
            skillAfterDes = transform.Find("View_Rebuild/Skill_2/Text_Description").GetComponent<Text>();

            eyeBtn = transform.Find("View_Rebuild/Skill_1/Btn_Preview").GetComponent<Button>();
            eyeBtn.onClick.AddListener(OnEyeBtnClicked);
            rebuildBtn = transform.Find("View_Rebuild/Btn_Rebuild").GetComponent<Button>();
            rebuildBtn.onClick.AddListener(OnRebuildBtnClicked);
            saveBtn = transform.Find("View_Rebuild/Btn_Save").GetComponent<Button>();
            saveBtn.onClick.AddListener(OnSaveBtnClicked);
            giveUpBtn = transform.Find("View_Rebuild/Btn_Delete").GetComponent<Button>();
            giveUpBtn.onClick.AddListener(OnGiveUpBtnClicked);
            rebuildCostItem = transform.Find("View_Rebuild/Cost/PropItem").gameObject;
            rebuildView = transform.Find("View_Rebuild").gameObject;
            afterSkillGo = transform.Find("View_Rebuild/Skill_2").gameObject;
            fxGo = transform.Find("View_Rebuild/Image/Fx_Ui_Transfiguration_Study_SkillRebuild").gameObject;

            infinityGridSkill = transform.Find("View_Skill/Scroll View").GetComponent<InfinityGrid>();
            infinityGridSkill.onCreateCell += OnCreateCellSkill;
            infinityGridSkill.onCellChange += OnCellChangeSkill;
        }

        public override void Show()
        {
            clientShapeShiftPlan = Sys_Transfiguration.Instance.GetCurUseShapeShiftData();
            ProcessEventsForEnable(true);
            skillCsv = Sys_Transfiguration.Instance.GetAllRaceSkillGridData();
            curSkillGridId = skillCsv[0].id;
            skillDataDic = clientShapeShiftPlan.allRaceSkillGrids;
            skillCellItems.Clear();
            infinityCountSkill = skillCsv.Count;
            infinityGridSkill.CellCount = infinityCountSkill;
            infinityGridSkill.ForceRefreshActiveCell();
            OnSelectItem(curSkillGridId);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Transfiguration.Instance.eventEmitter.Handle<uint>(Sys_Transfiguration.EEvents.OnRefreshSkill, OnRefreshSkill, toRegister);
            Sys_Transfiguration.Instance.eventEmitter.Handle<uint>(Sys_Transfiguration.EEvents.OnConfirmRefresh, OnConfirmRefresh, toRegister);
        }

        public override void Hide()
        {
            fxGo.SetActive(false);
            ProcessEventsForEnable(false);
            timer?.Cancel();
            curSkillGridId = 0;
            csvRaceChangeSkillData = null;
            csvAllRaceBonusData = null;
        }

        public override void OnDestroy()
        {
            curSkillGridId = 0;
            csvRaceChangeSkillData = null;
            csvAllRaceBonusData = null;
        }

        private void OnConfirmRefresh(uint gridId)
        {
            OnSelectItem(curSkillGridId);
            skillDataDic = clientShapeShiftPlan.allRaceSkillGrids;
            for (int i = 0; i < skillCellItems.Count; ++i)
            {
                if (skillCellItems[i].gridId == gridId)
                {
                    skillCellItems[i].Refresh(skillDataDic[gridId]);
                }
            }
        }

        private void OnRefreshSkill(uint gridId)
        {
            if (curSkillGridId != gridId)
            {
                return;
            }
            OnSelectItem(curSkillGridId);
            skillDataDic = clientShapeShiftPlan.allRaceSkillGrids;
            for (int i = 0; i < skillCellItems.Count; ++i)
            {
                if (skillCellItems[i].gridId == gridId)
                {
                    skillCellItems[i].Refresh(skillDataDic[gridId]);
                }
            }
            fxGo.SetActive(true);
            timer?.Cancel();
            timer = Timer.Register(0.6f, () =>
            {
                timer.Cancel();
                fxGo.SetActive(false);
            }, null, false, true);
        }

        private void OnCreateCellSkill(InfinityGridCell cell)
        {
            skillCeilDic.Clear();
            TransformAddCeil entry = new TransformAddCeil();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            entry.AddAction(OnSelectItem);
            cell.BindUserData(entry);
            skillCeilDic.Add(go, entry);
        }

        private void OnCellChangeSkill(InfinityGridCell cell, int index)
        {
            uint id = skillCsv[index].id;
            TransformAddCeil item = cell.mUserData as TransformAddCeil;
            if (skillDataDic.ContainsKey(id))
            {
                item.SetData(id, skillDataDic[id]);
            }
            else
            {
                item.SetData(id, null);
            }
            skillCellItems.Add(item);
            item.SetSelect(curSkillGridId);
        }

        private void OnSelectItem(uint skillGridId)
        {
            curSkillGridId = skillGridId;
            csvAllRaceBonusData = CSVAllRaceBonus.Instance.GetConfData(curSkillGridId);
            csvRaceChangeSkillData = CSVRaceChangeSkill.Instance.GetConfData(curSkillGridId);
            if (skillDataDic.ContainsKey(curSkillGridId)) //已解锁
            {
                rebuildView.SetActive(true);
                unlockView.SetActive(false);
                lockView.SetActive(false);
                uint curSkillId = skillDataDic[curSkillGridId].Skillid;
                uint reSkillId = skillDataDic[curSkillGridId].Replaceid;
                CSVPassiveSkillInfo.Data curInfo = CSVPassiveSkillInfo.Instance.GetConfData(curSkillId);
                CSVRaceSkillType.Data curSkillType = CSVRaceSkillType.Instance.GetConfData(curSkillId);
                if (curInfo != null && curSkillType != null)
                {
                    ImageHelper.SetIcon(skillBeforeIcon, curInfo.icon);
                    TextHelper.SetText(skillBeforeName, curInfo.name);
                    TextHelper.SetText(skillBeforeDes, curInfo.desc);
                    TextHelper.SetText(levelBefore, 12462, curInfo.level.ToString());
                    ImageHelper.SetIcon(typeBeforeIcon, curSkillType.skill_type);
                    tipBeforeGo.gameObject.SetActive(curSkillType.type == 1);
                }
                if (reSkillId == 0)
                {
                    afterSkillGo.SetActive(false);
                    saveBtn.gameObject.SetActive(false);
                    giveUpBtn.gameObject.SetActive(false);
                    levelAfter.text = string.Empty;
                }
                else
                {
                    afterSkillGo.SetActive(true);
                    saveBtn.gameObject.SetActive(true);
                    giveUpBtn.gameObject.SetActive(true);
                    CSVPassiveSkillInfo.Data reInfo = CSVPassiveSkillInfo.Instance.GetConfData(reSkillId);
                    CSVRaceSkillType.Data skillType = CSVRaceSkillType.Instance.GetConfData(reSkillId);
                    if (reInfo != null && skillType != null)
                    {
                        ImageHelper.SetIcon(skillAfterIcon, reInfo.icon);
                        TextHelper.SetText(skillAfterName, reInfo.name);
                        TextHelper.SetText(skillAfterDes, reInfo.desc);
                        TextHelper.SetText(levelAfter, 12462, reInfo.level.ToString());
                        ImageHelper.SetIcon(typeAfterIcon, skillType.skill_type);
                        tipAfterGo.gameObject.SetActive(skillType.type == 1);
                    }
                }
                FrameworkTool.CreateChildList(rebuildCostItem.transform.parent, csvRaceChangeSkillData.relearn_cost.Count);
                for (int i = 0; i < csvRaceChangeSkillData.relearn_cost.Count; ++i)
                {
                    Transform trans = rebuildCostItem.transform.parent.GetChild(i);
                    propItem = new PropItem();
                    propItem.BindGameObject(trans.gameObject);
                    PropIconLoader.ShowItemData showItemData = new PropIconLoader.ShowItemData(csvRaceChangeSkillData.relearn_cost[0][0], csvRaceChangeSkillData.relearn_cost[0][1], true, false, false, false, false,
                        _bShowCount: true, _bShowBagCount: true, _bUseClick: true, null, true, true);
                    propItem.SetData(new MessageBoxEvt(EUIID.UI_Transfiguration_Study, showItemData));
                }
            }
            else
            {
                if (clientShapeShiftPlan.CanUnLockAllStudy(csvAllRaceBonusData.race_rank))
                {
                    rebuildView.SetActive(false);
                    unlockView.SetActive(true);
                    lockView.SetActive(false);
                    FrameworkTool.CreateChildList(costItem.transform.parent, csvAllRaceBonusData.upgrade_cost.Count);
                    for (int i = 0; i < csvAllRaceBonusData.upgrade_cost.Count; ++i)
                    {
                        Transform trans = costItem.transform.parent.GetChild(i);
                        propItem = new PropItem();
                        propItem.BindGameObject(trans.gameObject);
                        PropIconLoader.ShowItemData showItemData = new PropIconLoader.ShowItemData(csvAllRaceBonusData.upgrade_cost[i][0], csvAllRaceBonusData.upgrade_cost[i][1], true, false, false, false, false,
                            _bShowCount: true, _bShowBagCount: true, _bUseClick: true, null, true, true);
                        propItem.SetData(new MessageBoxEvt(EUIID.UI_Transfiguration_Study, showItemData));
                    }
                }
                else
                {
                    rebuildView.SetActive(false);
                    unlockView.SetActive(false);
                    lockView.SetActive(true);
                    TextHelper.SetText(unlockTip, 2013505, csvAllRaceBonusData.race_rank.ToString());
                    TextHelper.SetText(unlockLevel, 2013507, csvAllRaceBonusData.race_rank.ToString());
                    int raceCount = Sys_Transfiguration.Instance.GetRacesIds().Count;
                    FrameworkTool.CreateChildList(unlockRaceItem.transform.parent, raceCount);
                    for (int i = 0; i < raceCount; ++i)
                    {
                        Text text = unlockRaceItem.transform.parent.GetChild(i).Find("Text").GetComponent<Text>();
                        uint genusId = Sys_Transfiguration.Instance.GetRacesIds()[i];
                        CSVGenus.Data csvGenusData = CSVGenus.Instance.GetConfData(genusId);
                        string content = null;
                        if (clientShapeShiftPlan.shapeShiftRaceSubNodes.ContainsKey(csvGenusData.id))
                        {
                            ShapeShiftSubNode node = clientShapeShiftPlan.shapeShiftRaceSubNodes[csvGenusData.id];
                            CSVRaceDepartmentResearch.Data csvRaceDepartmentResearchData = CSVRaceDepartmentResearch.Instance.GetConfData(node.Subnodeid);
                            content =LanguageHelper.GetTextContent( 2013506, LanguageHelper.GetTextContent(csvGenusData.rale_name), csvRaceDepartmentResearchData.rank.ToString(), csvRaceDepartmentResearchData.level.ToString());
                            if(csvRaceDepartmentResearchData.rank>= csvAllRaceBonusData.race_rank)
                            {
                                TextHelper.SetText(text, content,CSVWordStyle.Instance.GetConfData(178));
                            }
                            else
                            {
                                TextHelper.SetText(text, content, CSVWordStyle.Instance.GetConfData(179));
                            }
                        }
                        else
                        {
                            content = LanguageHelper.GetTextContent(2013506, LanguageHelper.GetTextContent(csvGenusData.rale_name), 0.ToString(), 1.ToString());
                            if (0>=csvAllRaceBonusData.race_rank)
                            {
                                TextHelper.SetText(text, content, CSVWordStyle.Instance.GetConfData(178));
                            }
                            else
                            {
                                TextHelper.SetText(text, content, CSVWordStyle.Instance.GetConfData(179));
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < skillCellItems.Count; ++i)
            {
                skillCellItems[i].SetSelect(curSkillGridId);
            }
        }

        private void OnUnlockBtnClicked()
        {
            for (int i = 0; i < csvAllRaceBonusData.upgrade_cost.Count; ++i)
            {
                uint itemId = csvAllRaceBonusData.upgrade_cost[i][0];
                long count = Sys_Bag.Instance.GetItemCount(itemId);
                if (count < csvAllRaceBonusData.upgrade_cost[i][1])
                {
                    CSVItem.Data csvItemData = CSVItem.Instance.GetConfData(itemId);
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680001021, LanguageHelper.GetTextContent(csvItemData.name_id)));
                    return;
                }
            }
            Sys_Transfiguration.Instance.UnlockMasterNodeReq(curSkillGridId);
        }

        private void OnEyeBtnClicked()
        {
            ChangeSkillInfo info = new ChangeSkillInfo();
            info.gridId = curSkillGridId;
            info.raceId = 99;
            UIManager.OpenUI(EUIID.UI_Transfiguration_SkillTips, false, info);
        }

        private void OnRebuildBtnClicked()
        {
            for (int i = 0; i < csvRaceChangeSkillData.relearn_cost.Count; ++i)
            {
                uint itemId = csvRaceChangeSkillData.relearn_cost[i][0];
                long count = Sys_Bag.Instance.GetItemCount(itemId);
                if (count < csvRaceChangeSkillData.relearn_cost[i][1])
                {
                    CSVItem.Data csvItemData = CSVItem.Instance.GetConfData(itemId);
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680001021, LanguageHelper.GetTextContent(csvItemData.name_id)));
                    return;
                }
            }
            ShapeShiftSkillGrid shapeShiftSkillGrid = clientShapeShiftPlan.GetReplaceSkillGridByAllRace();
            if (shapeShiftSkillGrid == null)
            {
                Sys_Transfiguration.Instance.RefreshSkillReq(curSkillGridId, Sys_Transfiguration.Instance.curIndex);
            }
            else
            {
                if (shapeShiftSkillGrid.Gridid == curSkillGridId)
                {
                    CSVPassiveSkillInfo.Data info = CSVPassiveSkillInfo.Instance.GetConfData(shapeShiftSkillGrid.Replaceid);
                    if (info.score >= 50)  //极品技能
                    {
                        if (Sys_Transfiguration.Instance.checkHightQualityReStudy)
                        {
                            PromptBoxParameter.Instance.Clear();
                            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680001015);
                            PromptBoxParameter.Instance.useToggle = Sys_Transfiguration.Instance.checkHightQualityReStudy;
                            PromptBoxParameter.Instance.SetConfirm(true, () =>
                            {
                                Sys_Transfiguration.Instance.RefreshSkillReq(curSkillGridId, Sys_Transfiguration.Instance.curIndex);
                            });
                            PromptBoxParameter.Instance.SetToggleChanged(true, (bool value) =>
                            {
                                Sys_Transfiguration.Instance.checkHightQualityReStudy = !value;
                            });
                            PromptBoxParameter.Instance.SetToggleChecked(false);
                            PromptBoxParameter.Instance.SetCancel(true, null);
                            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                        }
                        else
                        {
                            Sys_Transfiguration.Instance.RefreshSkillReq(curSkillGridId, Sys_Transfiguration.Instance.curIndex);
                        }
                    }
                    else
                    {
                        if (Sys_Transfiguration.Instance.checkReplaceSkillReStudy)
                        {
                            PromptBoxParameter.Instance.Clear();
                            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680001014);
                            PromptBoxParameter.Instance.useToggle = Sys_Transfiguration.Instance.checkReplaceSkillReStudy;
                            PromptBoxParameter.Instance.SetConfirm(true, () =>
                            {
                                Sys_Transfiguration.Instance.RefreshSkillReq(curSkillGridId, Sys_Transfiguration.Instance.curIndex);
                            });
                            PromptBoxParameter.Instance.SetCancel(true, null);
                            PromptBoxParameter.Instance.SetToggleChanged(true, (bool value) =>
                            {
                                Sys_Transfiguration.Instance.checkReplaceSkillReStudy = !value;
                            });
                            PromptBoxParameter.Instance.SetToggleChecked(false);
                            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                        }
                        else
                        {
                            Sys_Transfiguration.Instance.RefreshSkillReq(curSkillGridId, Sys_Transfiguration.Instance.curIndex);
                        }
                    }
                }
                else
                {
                    CSVPassiveSkillInfo.Data infoRe = CSVPassiveSkillInfo.Instance.GetConfData(shapeShiftSkillGrid.Replaceid);
                    CSVPassiveSkillInfo.Data info = CSVPassiveSkillInfo.Instance.GetConfData(shapeShiftSkillGrid.Skillid);
                    if (info == null || infoRe == null)
                    {
                        return;
                    }
                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680001012, string.Empty, LanguageHelper.GetTextContent(info.name), LanguageHelper.GetTextContent(infoRe.name));
                    PromptBoxParameter.Instance.SetConfirm(true, () =>
                    {
                        Sys_Transfiguration.Instance.OnConfirmRefreshReq(shapeShiftSkillGrid.Gridid, false, Sys_Transfiguration.Instance.curIndex);
                    });
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                }
            }
        }

        private void OnSaveBtnClicked()
        {
            CSVPassiveSkillInfo.Data curInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillDataDic[curSkillGridId].Skillid);
            CSVPassiveSkillInfo.Data reInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillDataDic[curSkillGridId].Replaceid);
            if (reInfo == null)
            {
                return;
            }
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680001016, LanguageHelper.GetTextContent(reInfo.name), reInfo.level.ToString(), LanguageHelper.GetTextContent(curInfo.name), curInfo.level.ToString());
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                Sys_Transfiguration.Instance.OnConfirmRefreshReq(curSkillGridId, true, Sys_Transfiguration.Instance.curIndex);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        private void OnGiveUpBtnClicked()
        {
            CSVPassiveSkillInfo.Data reInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillDataDic[curSkillGridId].Replaceid);
            if (reInfo == null)
            {
                return;
            }
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680001018, LanguageHelper.GetTextContent(reInfo.name));
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                Sys_Transfiguration.Instance.OnConfirmRefreshReq(curSkillGridId, false, Sys_Transfiguration.Instance.curIndex);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }
    }
}
