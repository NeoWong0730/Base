using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Table;
using Packet;
using DG.Tweening;
using System.Linq;

namespace Logic
{
    public class RaceStudySubItem
    {
        public Transform transform;
        private Image icon;
        private Image smallIcon;
        private Text race;
        private Text level;
        private Text control;
        private GameObject selectGo;
        private GameObject contorlGo;
        private Button button;
        public Action<uint> action;

        public uint raceId;
        public uint raceStudyId;
        private CSVGenus.Data csvGenusData;
        private CSVRaceDepartmentResearch.Data csvRaceDepartmentResearchData;
        private  ClientShapeShiftPlan clientShapeShiftPlan;

        public void Init(Transform _transform)
        {
            transform = _transform;
            icon = transform.Find("Icon").GetComponent<Image>();
            smallIcon = transform.Find("Restrain/Image").GetComponent<Image>();
            race = transform.Find("Info/Text_Race").GetComponent<Text>();
            level = transform.Find("Info/Text_Level").GetComponent<Text>();
            control = transform.Find("Restrain/Text").GetComponent<Text>();
            selectGo = transform.Find("Image_Select").gameObject;
            contorlGo = transform.Find("Restrain").gameObject;
            button = transform.GetComponent<Button>();
            button.onClick.AddListener(OnClicked);
            selectGo.SetActive(false);
            contorlGo.SetActive(false);
        }

        public void SetData(uint  _raceId)
        {
            raceId = _raceId;
            csvGenusData = CSVGenus.Instance.GetConfData(raceId);
            if (csvGenusData!=null)
            {
                clientShapeShiftPlan = Sys_Transfiguration.Instance.GetCurUseShapeShiftData();
                if (clientShapeShiftPlan.shapeShiftRaceSubNodes.ContainsKey(raceId))
                {
                    raceStudyId= clientShapeShiftPlan.shapeShiftRaceSubNodes[raceId].Subnodeid;
                }
                else
                {
                    raceStudyId = csvGenusData.race_change_id;
                }
                CSVRaceDepartmentResearch.Instance.TryGetValue(raceStudyId, out csvRaceDepartmentResearchData);
                ImageHelper.SetIcon(icon, csvGenusData.rale_icon);
                ImageHelper.SetIcon(smallIcon, csvGenusData.rale_icon);
                TextHelper.SetText(race, csvGenusData.rale_name);        
                TextHelper.SetText(level, 11846, csvRaceDepartmentResearchData.rank.ToString(), csvRaceDepartmentResearchData.level.ToString());
            }
        }

        public void Refresh()
        {
            if (clientShapeShiftPlan.shapeShiftRaceSubNodes.ContainsKey(raceId))
            {
                raceStudyId = clientShapeShiftPlan.shapeShiftRaceSubNodes[raceId].Subnodeid;
            }
            else
            {
                raceStudyId = csvGenusData.race_change_id;
            }
            CSVRaceDepartmentResearch.Instance.TryGetValue(raceStudyId, out csvRaceDepartmentResearchData);
            TextHelper.SetText(level, 11846, csvRaceDepartmentResearchData.rank.ToString(), csvRaceDepartmentResearchData.level.ToString());
        }

        public void SetSelect(uint selectedRaceId)
        {
            selectGo.SetActive(selectedRaceId==raceId);
            ShowControlTip(selectedRaceId);
        }

        private void ShowControlTip(uint selectedRaceId)
        {
            contorlGo.SetActive(selectedRaceId!=raceId);
            CSVGenus.Data csvGenusData = CSVGenus.Instance.GetConfData(selectedRaceId);
            if (csvGenusData != null)
            {
                smallIcon.gameObject.SetActive(true);
                for (int i=0;i < csvGenusData.damageRate.Count; ++i)
                {
                    if(i==raceId)
                    {
                        uint damage = csvGenusData.damageRate[i];
                        if (damage == 12000) //全克
                        {
                            TextHelper.SetText(control, LanguageHelper.GetTextContent( 2013104),CSVWordStyle.Instance.GetConfData(162));
                            ImageHelper.SetIcon(smallIcon,994111);
                        }
                        else if (damage == 8000)//全被克
                        {
                            TextHelper.SetText(control, LanguageHelper.GetTextContent(2013126), CSVWordStyle.Instance.GetConfData(163));
                            ImageHelper.SetIcon(smallIcon, 994113);
                        }
                        else if (damage == 11000) //半克
                        {
                            TextHelper.SetText(control, LanguageHelper.GetTextContent(2013105), CSVWordStyle.Instance.GetConfData(162));
                            ImageHelper.SetIcon(smallIcon, 994112);
                        }
                        else if (damage == 9000)  //半被克
                        {
                            TextHelper.SetText(control, LanguageHelper.GetTextContent(2013127), CSVWordStyle.Instance.GetConfData(163));
                            ImageHelper.SetIcon(smallIcon, 994114);
                        }
                        else
                        {
                            contorlGo.SetActive(false);
                        }
                    }
                }         
            }
            else
            {
                control.text = string.Empty;
                smallIcon.gameObject.SetActive(false);
            }
        }

        private void OnClicked()
        {
            action?.Invoke(raceId);
        }
    }

    public class UI_Transfiguration_LevelUp : UIComponent
    {
        private Text mainLv;
        private Text curRaceName;
        private Text curRaceLv;
        private Text lvUpTip;
        private Text mainLvRight;
        private Text advanceTip;
        private Text lvUpBtnText;
        private Image advanceImage;

        private Button mainRaceBtn;
        private Button missBtn;
        private Button levelUpBtn;
        private Button advanceBtn;

        private GameObject subRaceRoot;
        private GameObject levelUpView;
        private GameObject advanceView;
        private GameObject restrainAttrItem;
        private GameObject skillItem;
        private GameObject levelUpCostItem;
        private GameObject advanceCostItem;
        private GameObject subCostView;
        private GameObject mainCostView;
        private GameObject mainSelectGo;

        private GameObject levelUpMoveFx;
        private GameObject levelUpFx;
        private Animator animator;
        private Timer timer;

        private PropItem propItem;
        private uint curRaceStudyId;
        private bool isMainShow = false;
        public CSVGenus.Data csvGenusData;
        private CSVRaceDepartmentResearch.Data csvRaceDepartmentResearchData;
        private CSVRaceDepartmentResearch.Data csvRaceDepartmentResearchDataNext;
        private CSVRaceChangeResearch.Data csvRaceChangeResearchData;
        private List<RaceStudySubItem> raceStudySubItems = new List<RaceStudySubItem>();
        protected override void Loaded()
        {
            mainLv = transform.Find("View_List/Image_bg/Item_Main/Text_Level").GetComponent<Text>();
            curRaceName = transform.Find("View_Right/View_LevelUp/Image_Title/Text_Name").GetComponent<Text>();
            curRaceLv = transform.Find("View_Right/View_LevelUp/Image_Title/Text_Level").GetComponent<Text>();
            lvUpTip = transform.Find("View_Right/View_LevelUp/View_3/Text").GetComponent<Text>();
            mainLvRight = transform.Find("View_Right/View_Advance/Image_bg/Image/Text_Level").GetComponent<Text>();
            advanceTip = transform.Find("View_Right/View_Advance/View_2/Text").GetComponent<Text>();
            lvUpBtnText = transform.Find("View_Right/View_LevelUp/View_3/Btn_LevelUp/Text_01").GetComponent<Text>();
            advanceImage = transform.Find("View_Right/View_Advance/View_1/Image/Image").GetComponent<Image>();

            mainRaceBtn = transform.Find("View_List/Image_bg/Item_Main").GetComponent<Button>();
            missBtn = transform.Find("View_Right/View_LevelUp/Image_Title/Btn_Forget").GetComponent<Button>();
            levelUpBtn = transform.Find("View_Right/View_LevelUp/View_3/Btn_LevelUp").GetComponent<Button>();
            advanceBtn = transform.Find("View_Right/View_Advance/View_2/Btn_Advance").GetComponent<Button>();
            mainRaceBtn.onClick.AddListener(OnMainRaceBtnClicked);
            missBtn.onClick.AddListener(OnMissBtnClicked);
            levelUpBtn.onClick.AddListener(OnLevelUpBtnClicked);
            advanceBtn.onClick.AddListener(OnAdvanceBtnClicked);

            subRaceRoot = transform.Find("View_List/Image_bg/Sub_Root").gameObject;
            levelUpView = transform.Find("View_Right/View_LevelUp").gameObject;
            advanceView = transform.Find("View_Right/View_Advance").gameObject;
            restrainAttrItem = transform.Find("View_Right/View_LevelUp/View_1/Scroll View/Viewport/Content/Text_Attr").gameObject;
            skillItem = transform.Find("View_Right/View_LevelUp/View_2/Scroll View/Viewport/Content/Skill").gameObject;
            levelUpCostItem = transform.Find("View_Right/View_LevelUp/View_3/Consume/PropItem").gameObject;
            advanceCostItem = transform.Find("View_Right/View_Advance/View_2/Consume/PropItem").gameObject;
            subCostView = transform.Find("View_Right/View_LevelUp/View_3").gameObject;
            mainCostView = transform.Find("View_Right/View_Advance/View_2").gameObject;
            mainSelectGo = transform.Find("View_List/Image_bg/Item_Main/Image_Select").gameObject;

            levelUpMoveFx = transform.Find("View_List/Image_bg/Item_Main/Fx_Ui_Transfiguration_Study_Trial").gameObject;
            levelUpFx = transform.Find("View_List/Image_bg/Item_Main/Fx_Ui_Transfiguration_Study_LevelUp").gameObject;
            animator = transform.GetComponent<Animator>();
        }

        public override void Show()
        {
            SetRaceStudySubData();
            csvRaceChangeResearchData = CSVRaceChangeResearch.Instance.GetConfData(Sys_Transfiguration.Instance.mainNodeId);
            TextHelper.SetText(mainLv, 11755, Sys_Transfiguration.Instance.mainNodeId.ToString());
            if (isMainShow)
            {
                SetMainView();
            }
            ProcessEventsForEnable(true);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Transfiguration.Instance.eventEmitter.Handle(Sys_Transfiguration.EEvents.OnForgetRaceStudy, OnForgetRaceStudy, toRegister);
            Sys_Transfiguration.Instance.eventEmitter.Handle<uint>(Sys_Transfiguration.EEvents.OnUpdateSubNode, OnUpdateSubNode, toRegister);
            Sys_Transfiguration.Instance.eventEmitter.Handle(Sys_Transfiguration.EEvents.OnUpdateMainNode, OnUpdateMainNode, toRegister);
        }

        public override void Hide()
        {
            ProcessEventsForEnable(false);
            raceStudySubItems.Clear();
            DefaultItem();
            timer?.Cancel();
            levelUpMoveFx.transform.position = mainRaceBtn.transform.position;
            levelUpMoveFx.SetActive(false);
            levelUpFx.SetActive(false);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            curRaceStudyId = 0;
            isMainShow = false;
        }

        private void SetRaceStudySubData()
        {
            raceStudySubItems.Clear();
            List<uint> races = Sys_Transfiguration.Instance.GetRacesIds();
            for(int i = 0; i < subRaceRoot.transform.childCount; ++i)
            {
                RaceStudySubItem item = new RaceStudySubItem();
                item.Init(subRaceRoot.transform.GetChild(i));
                item.SetData(races[i]);
                item.action = SelectRace;
                raceStudySubItems.Add(item);
            }
            if (!isMainShow)
            {
                if (csvGenusData == null)
                {
                    SelectRace(races[0]);
                }
                else
                {
                    SelectRace(csvGenusData.id);
                }
            }
        }

        private void SelectRace(uint raceId)
        {
            isMainShow = false;
            levelUpView.SetActive(true);
            advanceView.SetActive(false);
            mainSelectGo.SetActive(false);
            csvGenusData = CSVGenus.Instance.GetConfData(raceId);
            if (Sys_Transfiguration.Instance.GetCurUseShapeShiftData().shapeShiftRaceSubNodes.ContainsKey(raceId))
            {
                curRaceStudyId = Sys_Transfiguration.Instance.GetCurUseShapeShiftData().shapeShiftRaceSubNodes[raceId].Subnodeid;
                missBtn.gameObject.SetActive(true);
            }
            else
            {
                curRaceStudyId = csvGenusData.race_change_id;
                missBtn.gameObject.SetActive(false);
            }
            CSVRaceDepartmentResearch.Instance.TryGetValue(curRaceStudyId, out csvRaceDepartmentResearchData);
            CSVRaceDepartmentResearch.Instance.TryGetValue(csvRaceDepartmentResearchData.nextlevelid, out csvRaceDepartmentResearchDataNext);
            TextHelper.SetText(curRaceName, csvGenusData.rale_name);
            TextHelper.SetText(curRaceLv, 11846, csvRaceDepartmentResearchData.rank.ToString(), csvRaceDepartmentResearchData.level.ToString());
            for (int i = 0; i < raceStudySubItems.Count; ++i)
            {
                raceStudySubItems[i].SetSelect(raceId);
            }     
            if (csvRaceDepartmentResearchDataNext!=null)
            {
                subCostView.SetActive(true);
                SetLevelUpCost();
                if (csvRaceDepartmentResearchDataNext.rank != csvRaceDepartmentResearchData.rank)
                {
                    TextHelper.SetText(lvUpBtnText, 2013124);
                    uint resId = csvRaceDepartmentResearchData.upgrade_restrict;
                    if (Sys_Transfiguration.Instance.mainNodeId >= resId)
                    {
                        lvUpTip.text = string.Empty;
                    }
                    else
                    {
                        TextHelper.SetText(lvUpTip, 2013119, resId.ToString());
                    }
                }
                else
                {
                    TextHelper.SetText(lvUpBtnText, 2013120);
                    lvUpTip.text = string.Empty;
                }
            }
            else
            {
                subCostView.SetActive(false);
            }
            
            DefaultItem();
            SetRaceAttr();
            SetSkill();
        }

        private void SetRaceAttr()
        {
            if(csvRaceDepartmentResearchData.up_attr1==null|| csvRaceDepartmentResearchData.up_attr1.Count == 0)
            {
                restrainAttrItem.SetActive(false);
                return;
            }
            else
            {
                restrainAttrItem.SetActive(true);
            }
            FrameworkTool.CreateChildList(restrainAttrItem.transform.parent, csvRaceDepartmentResearchData.up_attr1.Count);
            for (int i=0;i< csvRaceDepartmentResearchData.up_attr1.Count; ++i)
            {
                Transform tran = restrainAttrItem.transform.parent.GetChild(i);
                if (i != 0)
                {
                    tran.name = csvRaceDepartmentResearchData.up_attr1[i][0].ToString();
                }
                Text name = tran.GetComponent<Text>();
                Text number = tran.Find("Text_Number").GetComponent<Text>();
                Text numberNext = tran.Find("Text_Number_Next").GetComponent<Text>();
                CSVAttr.Data data = CSVAttr.Instance.GetConfData(csvRaceDepartmentResearchData.up_attr1[i][0]);
                if (csvRaceDepartmentResearchDataNext == null)
                {
                    numberNext.text = string.Empty;
                }
                else
                {
                    CSVAttr.Data dataNext = CSVAttr.Instance.GetConfData(csvRaceDepartmentResearchDataNext.up_attr1[i][0]);
                    float num = csvRaceDepartmentResearchDataNext.up_attr1[i][1] - csvRaceDepartmentResearchData.up_attr1[i][1];
                    if (num == 0)
                    {
                        numberNext.text = string.Empty;
                    }
                    else
                    {
                        if (dataNext.show_type == 1)
                        {
                            TextHelper.SetText(numberNext, "+"+num.ToString(), CSVWordStyle.Instance.GetConfData(161));
                        }
                        else
                        {
                            TextHelper.SetText(numberNext, "+" + (num / 100).ToString() +"%", CSVWordStyle.Instance.GetConfData(161));
                        }
                    }
                }

                if (data != null)
                {
                    TextHelper.SetText(name, data.name);
                    if (data.show_type == 1)
                    {
                        TextHelper.SetText(number, "+"+csvRaceDepartmentResearchData.up_attr1[i][1].ToString());
                      
                    }
                    else
                    {
                        TextHelper.SetText(number, "+"+((float)csvRaceDepartmentResearchData.up_attr1[i][1]/100).ToString()+"%");
                    }
                }
            }
        }

        private void SetSkill()
        {
            Dictionary<uint, ShapeShiftSkillGrid> skillDataDic = Sys_Transfiguration.Instance.GetCurUseShapeShiftData().GetSkillGridByRaceIdByServer(csvGenusData.id);
            List<CSVRaceChangeSkill.Data> skillCsv = Sys_Transfiguration.Instance.GetSkillGridByRaceId(csvGenusData.id);
            FrameworkTool.CreateChildList(skillItem.transform.parent, skillCsv.Count);
            for(int i=0;i< skillCsv.Count; ++i)
            {
                Transform tran = skillItem.transform.parent.GetChild(i);
                if (i != 0)
                {
                    tran.name = skillCsv[i].id.ToString();
                }
                Image icon = tran.Find("Image_Icon").GetComponent<Image>();
                Image smallIcon = tran.Find("Image_Typebg/Image_Type").GetComponent<Image>();
                Text lockLv = tran.Find("Image_Lock/Text").GetComponent<Text>();
                Text lv = tran.Find("Image_Levelbg/Level").GetComponent<Text>();
                GameObject lockGo = tran.Find("Image_Lock").gameObject;
                GameObject tip = tran.Find("Image_Tip").gameObject;
                GameObject smallIconGo = tran.Find("Image_Typebg").gameObject;
                GameObject lvGo = tran.Find("Image_Levelbg").gameObject;

                if (csvRaceDepartmentResearchData.rank< skillCsv[i].rank)
                {
                    lockGo.SetActive(true);
                    TextHelper.SetText(lockLv, 2013202, skillCsv[i].rank.ToString());
                    tip.gameObject.SetActive(false);
                    smallIconGo.gameObject.SetActive(false);
                    lvGo.gameObject.SetActive(false);
                    icon.gameObject.SetActive(false);
                }
                else
                {
                    lockGo.SetActive(false);
                    if (skillDataDic.ContainsKey(skillCsv[i].id))
                    {
                        ShapeShiftSkillGrid skillData = skillDataDic[skillCsv[i].id];
                        CSVPassiveSkillInfo.Data info = CSVPassiveSkillInfo.Instance.GetConfData(skillData.Skillid);
                        CSVRaceSkillType.Data skillType = CSVRaceSkillType.Instance.GetConfData(skillData.Skillid);
                        if (info != null && skillType != null)
                        {
                            smallIconGo.gameObject.SetActive(true);
                            lvGo.gameObject.SetActive(true);
                            icon.gameObject.SetActive(true);
                            ImageHelper.SetIcon(icon, info.icon);
                            ImageHelper.SetIcon(smallIcon, skillType.skill_type);
                            TextHelper.SetText(lv,12462, info.level.ToString());
                            tip.SetActive(skillType.type == 1);
                        }
                        Button btn = tran.Find("Image_bg").GetComponent<Button>();
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(delegate () { this.OnClicked(skillData.Skillid); });
                    }
                }
            }
        }

        private void OnClicked(uint skillId)
        {
            if (skillId != 0)
            {
                UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(skillId, 0));
            }
        }

        private void SetLevelUpCost()
        {
            if (csvRaceDepartmentResearchData==null || csvRaceDepartmentResearchData.upgrade_cost ==null || csvRaceDepartmentResearchData.upgrade_cost.Count == 0)
            {
                subCostView.SetActive(false);
                return;
            }
            else
            {
                subCostView.SetActive(true);
            }
            int count = csvRaceDepartmentResearchData.upgrade_cost.Count;
            FrameworkTool.CreateChildList(levelUpCostItem.transform.parent, count);
            for (int i = 0; i < count; ++i)
            {
                GameObject go = levelUpCostItem.transform.parent.GetChild(i).gameObject;
                propItem = new PropItem();
                propItem.BindGameObject(go);
                PropIconLoader.ShowItemData showItemData = new PropIconLoader.ShowItemData(csvRaceDepartmentResearchData.upgrade_cost[i][0], csvRaceDepartmentResearchData.upgrade_cost[i][1], true, false, false, false, false,
                    _bShowCount: true, _bShowBagCount: true, _bUseClick: true, null, true, true);
                propItem.SetData(new MessageBoxEvt(EUIID.UI_Transfiguration_Study, showItemData));
            }
        }

        private void DefaultItem()
        {
            FrameworkTool.DestroyChildren(restrainAttrItem.transform.parent.gameObject, restrainAttrItem.transform.name);
            FrameworkTool.DestroyChildren(skillItem.transform.parent.gameObject, skillItem.transform.name);
            FrameworkTool.DestroyChildren(levelUpCostItem.transform.parent.gameObject, levelUpCostItem.transform.name);
        }

        private void SetMainView()
        {
            isMainShow = true;
            levelUpView.SetActive(false);
            advanceView.SetActive(true);
            mainSelectGo.SetActive(true);
            uint curMainNodeId = Sys_Transfiguration.Instance.mainNodeId;
            TextHelper.SetText(mainLvRight, 11755, curMainNodeId.ToString());
            TextHelper.SetText(advanceTip, 2013123, csvRaceChangeResearchData.world_level_restrict.ToString(), csvRaceChangeResearchData.upgrade_restrict[1].ToString(), csvRaceChangeResearchData.upgrade_restrict[0].ToString());
            if(CSVRaceChangeResearch.Instance.TryGetValue(curMainNodeId + 1,out CSVRaceChangeResearch.Data nextData))
            {
                SetAdvanceCost();
                mainCostView.SetActive(true);
            }
            else
            {
                mainCostView.SetActive(false);
            }
            uint maxId = CSVRaceChangeResearch.Instance.GetKeys().Max();
            if (curMainNodeId >= maxId)
            {
                advanceImage.material.SetFloat("_Amount", 1f);
            }
            else
            {
                float value = (float)curMainNodeId / maxId;
                advanceImage.material.SetFloat("_Amount", value);
            }

            for (int i = 0; i < raceStudySubItems.Count; ++i)
            {
                raceStudySubItems[i].SetSelect(0);
            }
        }

        private void SetAdvanceCost()
        {
            if (csvRaceChangeResearchData == null)
            {
                return;
            }
            int count = csvRaceChangeResearchData.upgrade_cost.Count;
            if (count == 0)
            {
                advanceCostItem.SetActive(false);
                return;
            }
            else
            {
                advanceCostItem.SetActive(true);
            }
        
            FrameworkTool.CreateChildList(advanceCostItem.transform.parent, count);
            for (int i = 0; i < count; ++i)
            {
                GameObject go = advanceCostItem.transform.parent.GetChild(i).gameObject;
                propItem = new PropItem();
                propItem.BindGameObject(go);
                PropIconLoader.ShowItemData showItemData = new PropIconLoader.ShowItemData(csvRaceChangeResearchData.upgrade_cost[i][0], csvRaceChangeResearchData.upgrade_cost[i][1], true, false, false, false, false,
                    _bShowCount: true, _bShowBagCount: true, _bUseClick: true, null, true, true);
                propItem.SetData(new MessageBoxEvt(EUIID.UI_Transfiguration_Study, showItemData));
            }
        }

        private void OnForgetRaceStudy()
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680001008,LanguageHelper.GetTextContent(csvGenusData.rale_name)));              
            SelectRace(csvGenusData.id);
            for (int i = 0; i < raceStudySubItems.Count; ++i)
            {
                raceStudySubItems[i].Refresh();
            }
        }

        private void OnUpdateSubNode(uint raceId)
        {
            levelUpMoveFx.SetActive(false);
            levelUpFx.SetActive(false);
            for (int i = 0; i < raceStudySubItems.Count; ++i)
            {
                if (raceStudySubItems[i].raceId == raceId)
                {
                    PlaySubNodeFx(raceStudySubItems[i].transform.position);
                }
                raceStudySubItems[i].Refresh();
            }
            SelectRace(csvGenusData.id);
        }

        private void OnUpdateMainNode()
        {
            csvRaceChangeResearchData = CSVRaceChangeResearch.Instance.GetConfData(Sys_Transfiguration.Instance.mainNodeId);
            TextHelper.SetText(mainLv, 11755, Sys_Transfiguration.Instance.mainNodeId.ToString());
            TextHelper.SetText(mainLvRight, 11755, Sys_Transfiguration.Instance.mainNodeId.ToString());
            PlayMainNodeFx();
            SetMainView();
        }

        private void PlaySubNodeFx(Vector3 endPos)
        {
            timer?.Cancel();
            animator.enabled = true;
            levelUpFx.SetActive(true);
            animator.Play("Up", -1, 0);
            timer = Timer.Register(1f,()=>
            {
                timer.Cancel();
                animator.enabled = false;
                levelUpFx.SetActive(false);
                levelUpMoveFx.SetActive(true);
                DOTween.To(() => levelUpMoveFx.transform.position, x => levelUpMoveFx.transform.position = x, endPos, 1).SetEase(Ease.Linear);
                timer = Timer.Register(1f, () =>
                {
                    timer.Cancel();
                    levelUpMoveFx.SetActive(false);
                    levelUpMoveFx.transform.position = mainRaceBtn.transform.position;
                }, null, false, true);
            }, null, false,true);
        }

        private void PlayMainNodeFx()
        {
            animator.enabled = true;
            animator.Play("Advance", -1, 0); 
        }


        private void OnMainRaceBtnClicked()
        {
            SetMainView();
        }

        private void OnMissBtnClicked()
        {
            string contentItem = null;
            if (csvRaceDepartmentResearchData.restitution == null)
            {
                return;
            }
            for(int i=0; i< csvRaceDepartmentResearchData.restitution.Count; ++i)
            {
                uint itemId = csvRaceDepartmentResearchData.restitution[i][0];
                uint count = csvRaceDepartmentResearchData.restitution[i][1];
                CSVItem.Data itemData = CSVItem.Instance.GetConfData(itemId);
                if (i == 0)
                {
                    contentItem += LanguageHelper.GetTextContent(itemData.name_id) + "x" + count.ToString();
                }
                else
                {
                    contentItem +=", "+ LanguageHelper.GetTextContent(itemData.name_id) + "x" + count.ToString();
                }
            }
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680001007, LanguageHelper.GetTextContent(csvGenusData.rale_name), contentItem);  
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                Sys_Transfiguration.Instance.ResetSubNodeReq(csvRaceDepartmentResearchData.type);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        private void OnLevelUpBtnClicked()
        {
            if (Sys_Transfiguration.Instance.mainNodeId >= csvRaceDepartmentResearchData.upgrade_restrict)
            {
                bool haveItem = true;
                for (int i = 0; i < csvRaceDepartmentResearchData.upgrade_cost.Count; ++i)
                {
                    uint itemId = csvRaceDepartmentResearchData.upgrade_cost[i][0];
                    uint count = csvRaceDepartmentResearchData.upgrade_cost[i][1];
                    if (Sys_Bag.Instance.GetItemCount(itemId) < count)
                    {
                        haveItem = false;
                        break;
                    }
                }
                if (haveItem)
                {
                    Sys_Transfiguration.Instance.UpdateSubNodeReq(csvRaceDepartmentResearchData.type);
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680001004, csvRaceDepartmentResearchData.upgrade_restrict.ToString()));
                }
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680001003, csvRaceDepartmentResearchData.upgrade_restrict.ToString()));
            }
        }

        private void OnAdvanceBtnClicked()
        {
            if (Sys_Role.Instance.GetWorldLv()<csvRaceChangeResearchData.world_level_restrict)
            {
                  Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680001009, csvRaceChangeResearchData.world_level_restrict.ToString()));
            }
            else
            {
                uint rank = csvRaceChangeResearchData.upgrade_restrict[0];
                uint count = csvRaceChangeResearchData.upgrade_restrict[1];
                int number = 0;
                for (int i=0;i< Sys_Transfiguration.Instance.GetCurUseShapeShiftData().shapeShiftSubNodeIds.Count;++i)
                {
                    uint id = Sys_Transfiguration.Instance.GetCurUseShapeShiftData().shapeShiftSubNodeIds[i];
                    if (CSVRaceDepartmentResearch.Instance.GetConfData(id).rank>= rank)
                    {
                        number++;
                    }                 
                }
                if(number>= count)
                {
                    bool haveItem = true;
                    for (int i = 0; i < csvRaceChangeResearchData.upgrade_cost.Count; ++i)
                    {
                        uint itemId = csvRaceChangeResearchData.upgrade_cost[i][0];
                        uint itemCount = csvRaceChangeResearchData.upgrade_cost[i][1];
                        if (Sys_Bag.Instance.GetItemCount(itemId) < itemCount)
                        {
                            haveItem = false;
                            break;
                        }
                    }
                    if (haveItem)
                    {
                        Sys_Transfiguration.Instance.UpdateMainNodeReq();
                    }
                    else
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680001004));
                    }
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680001010, count.ToString(), rank.ToString()));
                }
            }
        }
    }
}
