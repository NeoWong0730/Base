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
    public class TransformSkillCeil
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
        private CSVRaceChangeSkill.Data csvRaceChangeSkillData;
        private ShapeShiftSkillGrid shapeShiftSkillGrid;

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
            if (_shapeShiftSkillGrid==null)
            {
                lockGo.SetActive(true);
                tipGo.gameObject.SetActive(false);
                icon.gameObject.SetActive(false);
                name.gameObject.SetActive(false);
                typeGo.gameObject.SetActive(false);
                skillLv.text = string.Empty;
                csvRaceChangeSkillData = CSVRaceChangeSkill.Instance.GetConfData(gridId);
                if (csvRaceChangeSkillData != null)
                {
                    TextHelper.SetText(openLv, 2013202, csvRaceChangeSkillData.rank.ToString());
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
                if (info != null&& skillType!=null)
                {
                    ImageHelper.SetIcon(icon, info.icon);
                    ImageHelper.SetIcon(type, skillType.skill_type);
                    TextHelper.SetText(name, info.name);
                    tipGo.gameObject.SetActive(skillType.type==1);
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
            selectGo.SetActive(selectId==gridId);
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

    public class UI_Tansform_Skill_ReStudy : UIComponent
    {
        private Text raceName;
        private Text lockTip;
        private Text curSkillName;
        private Text curSkillMessage;
        private Text curSkillLv;
        private Text reStudySkillName;
        private Text reStudySkillMessage;
        private Text reStudySkillLv;
        private Image curSkillIcon;
        private Image curSkillTypeIcon;
        private Image reStudySkillIcon;
        private Image reStudySkillTypeIcon;
        private GameObject curSkillTipGo;
        private GameObject reStudyTipGo;
        private GameObject infoGo;
        private GameObject noSkillGo;
        private GameObject costGo;
        private GameObject reStudyGo;
        private Button tipBtn;
        private Button reStudyBtn;
        private Button saveBtn;
        private Button giveUpBtn;
        private GameObject fxGo;
        private Timer timer;

        public InfinityGrid infinityGridMenu;
        public InfinityGrid infinityGridSkill;
        private Dictionary<GameObject, TransformSkillCeil> skillCeilDic = new Dictionary<GameObject, TransformSkillCeil>();
        private List<TransformSkillCeil> skillCellItems = new List<TransformSkillCeil>();
        private int infinityCountSkill;

        private Dictionary<GameObject, UI_Transfiguration_BookList_Menu> itemMenus = new Dictionary<GameObject, UI_Transfiguration_BookList_Menu>();
        private List<UI_Transfiguration_BookList_Menu> menuItems = new List<UI_Transfiguration_BookList_Menu>();
        private int infinityCountMenu;

        private List<uint> raceList = new List<uint>();
        private List<CSVRaceChangeSkill.Data> skillCsv = new List<CSVRaceChangeSkill.Data>();
        private  Dictionary<uint, ShapeShiftSkillGrid> skillDataDic = new Dictionary<uint, ShapeShiftSkillGrid>();

        public uint curRaceId;
        private uint curSkillGridId;
        private CSVRaceChangeSkill.Data csvRaceChangeSkillData;
        private PropItem propItem;

        protected override void Loaded()
        {
            raceName = transform.Find("View_Skill/Title/Text_Title").GetComponent<Text>();
            lockTip = transform.Find("Image_Nothing/Text").GetComponent<Text>();
            curSkillName = transform.Find("View_Rebuild/Skill_1/Text_Name").GetComponent<Text>();
            curSkillMessage = transform.Find("View_Rebuild/Skill_1/Text_Description").GetComponent<Text>();
            curSkillLv = transform.Find("View_Rebuild/Skill_1/Skillbg/Image_Levelbg/Level").GetComponent<Text>();
            reStudySkillName = transform.Find("View_Rebuild/Skill_2/Text_Name").GetComponent<Text>();
            reStudySkillMessage = transform.Find("View_Rebuild/Skill_2/Text_Description").GetComponent<Text>();
            reStudySkillLv = transform.Find("View_Rebuild/Skill_2/Skillbg/Image_Levelbg/Level").GetComponent<Text>();
            curSkillIcon = transform.Find("View_Rebuild/Skill_1/Skillbg/Icon").GetComponent<Image>();
            curSkillTypeIcon = transform.Find("View_Rebuild/Skill_1/Skillbg/Image_Typebg/Image_Type").GetComponent<Image>();
            reStudySkillIcon = transform.Find("View_Rebuild/Skill_2/Skillbg/Icon").GetComponent<Image>();
            reStudySkillTypeIcon = transform.Find("View_Rebuild/Skill_2/Skillbg/Image_Typebg/Image_Type").GetComponent<Image>();
            curSkillTipGo = transform.Find("View_Rebuild/Skill_1/Skillbg/Image_Tip").gameObject;
            reStudyTipGo = transform.Find("View_Rebuild/Skill_2/Skillbg/Image_Tip").gameObject;
            infoGo = transform.Find("View_Rebuild").gameObject;
            noSkillGo = transform.Find("Image_Nothing").gameObject;
            costGo = transform.Find("View_Rebuild/Cost/PropItem").gameObject;
            reStudyGo = transform.Find("View_Rebuild/Skill_2").gameObject;
            fxGo = transform.Find("View_Rebuild/Image/Fx_Ui_Transfiguration_Study_SkillRebuild").gameObject;

            tipBtn = transform.Find("View_Rebuild/Skill_1/Btn_Preview").GetComponent<Button>();
            tipBtn.onClick.AddListener(OnTipBtnClicked);
            reStudyBtn = transform.Find("View_Rebuild/Btn_Rebuild").GetComponent<Button>();
            reStudyBtn.onClick.AddListener(OnReStudyBtnClicked);
            saveBtn = transform.Find("View_Rebuild/Btn_Save").GetComponent<Button>();
            saveBtn.onClick.AddListener(OnSaveBtnClicked);
            giveUpBtn = transform.Find("View_Rebuild/Btn_Delete").GetComponent<Button>();
            giveUpBtn.onClick.AddListener(OnGiveUpBtnClicked);
            infinityGridMenu = transform.Find("View_List/Scroll View").GetComponent<InfinityGrid>();
            infinityGridSkill = transform.Find("View_Skill/Scroll View").GetComponent<InfinityGrid>();

            raceList = Sys_Transfiguration.Instance.GetRacesIds();
            infinityGridMenu.onCreateCell += OnCreateCellMenu;
            infinityGridMenu.onCellChange += OnCellChangeMenu;
            infinityGridSkill.onCreateCell += OnCreateCellSkill;
            infinityGridSkill.onCellChange += OnCellChangeSkill;
        }

        public override void Show()
        {
            if (curRaceId==0 && raceList.Count > 0)
            {
                curRaceId = raceList[0];
            }
            infinityCountMenu = raceList.Count;
            infinityGridMenu.CellCount = infinityCountMenu;
            infinityGridMenu.ForceRefreshActiveCell();
            ProcessEventsForEnable(true);
        }

        public override void Hide()
        {
            fxGo.SetActive(false);
            ProcessEventsForEnable(false);
            timer?.Cancel();
        }

        public override void OnDestroy()
        {
            curRaceId = 0;
            curSkillGridId = 0;
            csvRaceChangeSkillData = null;
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Transfiguration.Instance.eventEmitter.Handle<uint>(Sys_Transfiguration.EEvents.OnSelectRaceMenu, OnSelectRaceMenu, toRegister);
            Sys_Transfiguration.Instance.eventEmitter.Handle<uint>(Sys_Transfiguration.EEvents.OnRefreshSkill, OnRefreshSkill, toRegister);
            Sys_Transfiguration.Instance.eventEmitter.Handle<uint>(Sys_Transfiguration.EEvents.OnConfirmRefresh, OnConfirmRefresh, toRegister);
        }      

        private void OnSelectRaceMenu(uint raceId)
        {
            curRaceId = raceId;
            TextHelper.SetText(raceName,  2013201, LanguageHelper.GetTextContent(CSVGenus.Instance.GetConfData(curRaceId).rale_name));
            skillCsv = Sys_Transfiguration.Instance.GetSkillGridByRaceId(curRaceId);
            bool isSame = false;
            for (int i = 0; i < skillCsv.Count; ++i)
            {
                if(skillCsv[i].id== curSkillGridId)
                {
                    isSame = true;
                    break;
                }
            }
            if (!isSame)
            {
                curSkillGridId = skillCsv[0].id;
            }
            skillDataDic = Sys_Transfiguration.Instance.GetCurUseShapeShiftData().GetSkillGridByRaceIdByServer(curRaceId);
            skillCellItems.Clear();        
            infinityCountSkill = skillCsv.Count;
            infinityGridSkill.CellCount = infinityCountSkill;
            infinityGridSkill.ForceRefreshActiveCell();
            OnSelectItem(curSkillGridId);           
        }

        private void OnConfirmRefresh(uint gridId)
        {
            OnSelectItem(curSkillGridId);
            skillDataDic = Sys_Transfiguration.Instance.GetCurUseShapeShiftData().GetSkillGridByRaceIdByServer(curRaceId);
            for (int i=0;i< skillCellItems.Count; ++i)
            {
                if (skillCellItems[i].gridId == gridId)
                {
                    skillCellItems[i].Refresh(skillDataDic[gridId]);
                }                    
            }
        }

        private void OnRefreshSkill(uint gridId)
        {
            OnSelectItem(curSkillGridId);
            skillDataDic = Sys_Transfiguration.Instance.GetCurUseShapeShiftData().GetSkillGridByRaceIdByServer(curRaceId);
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

        private void OnCreateCellMenu(InfinityGridCell cell)
        {
            itemMenus.Clear();
            UI_Transfiguration_BookList_Menu entry = new UI_Transfiguration_BookList_Menu();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            cell.BindUserData(entry);
            itemMenus.Add(go, entry);
        }

        private void OnCellChangeMenu(InfinityGridCell cell, int index)
        {
            uint id = raceList[index];
            UI_Transfiguration_BookList_Menu item = cell.mUserData as UI_Transfiguration_BookList_Menu;
            item.SetData(id);
            menuItems.Add(item);
            if (id == curRaceId)
            {
                item.SetToggleOn();
            }
        }

        private void OnCreateCellSkill(InfinityGridCell cell)
        {
            skillCeilDic.Clear();
            TransformSkillCeil entry = new TransformSkillCeil();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            entry.AddAction(OnSelectItem);
            cell.BindUserData(entry);
            skillCeilDic.Add(go, entry);
        }

        private void OnCellChangeSkill(InfinityGridCell cell, int index)
        {
            uint id = skillCsv[index].id;
            TransformSkillCeil item = cell.mUserData as TransformSkillCeil;
            if (skillDataDic.ContainsKey(id))
            {
                item.SetData(id, skillDataDic[id]);
            }
            else
            {
                item.SetData(id, null);
            }        
            skillCellItems.Add(item);
            //if (curSkillGridId == id)
            //{
            //    OnSelectItem(curSkillGridId);
            //}
            item.SetSelect(curSkillGridId);
        }
         
        private void OnSelectItem(uint skillGridId)
        {
            curSkillGridId = skillGridId;
            csvRaceChangeSkillData = CSVRaceChangeSkill.Instance.GetConfData(curSkillGridId);
            if (skillDataDic.ContainsKey(curSkillGridId))
            {
                infoGo.SetActive(true);
                noSkillGo.SetActive(false);

                uint curSkillId = skillDataDic[curSkillGridId].Skillid;
                uint reSkillId = skillDataDic[curSkillGridId].Replaceid;
                CSVPassiveSkillInfo.Data curInfo = CSVPassiveSkillInfo.Instance.GetConfData(curSkillId);
                CSVRaceSkillType.Data curSkillType = CSVRaceSkillType.Instance.GetConfData(curSkillId);
                if (curInfo != null && curSkillType != null)
                {
                    ImageHelper.SetIcon(curSkillIcon, curInfo.icon);
                    TextHelper.SetText(curSkillName, curInfo.name);
                    TextHelper.SetText(curSkillMessage, curInfo.desc);
                    TextHelper.SetText(curSkillLv, 12462, curInfo.level.ToString());
                    ImageHelper.SetIcon(curSkillTypeIcon, curSkillType.skill_type);
                    curSkillTipGo.gameObject.SetActive(curSkillType.type == 1);
                }
                if (reSkillId == 0)
                {
                    reStudyGo.SetActive(false);
                    saveBtn.gameObject.SetActive(false);
                    giveUpBtn.gameObject.SetActive(false);
                    reStudySkillLv.text = string.Empty;
                }
                else
                {
                    reStudyGo.SetActive(true);
                    saveBtn.gameObject.SetActive(true);
                    giveUpBtn.gameObject.SetActive(true);
                    CSVPassiveSkillInfo.Data reInfo = CSVPassiveSkillInfo.Instance.GetConfData(reSkillId);
                    CSVRaceSkillType.Data skillType = CSVRaceSkillType.Instance.GetConfData(reSkillId);
                    if (reInfo != null && skillType != null)
                    {
                        ImageHelper.SetIcon(reStudySkillIcon, reInfo.icon);
                        TextHelper.SetText(reStudySkillName, reInfo.name);
                        TextHelper.SetText(reStudySkillMessage, reInfo.desc);
                        TextHelper.SetText(reStudySkillLv, 12462, reInfo.level.ToString());
                        ImageHelper.SetIcon(reStudySkillTypeIcon, skillType.skill_type);     
                        reStudyTipGo.gameObject.SetActive(skillType.type == 1);
                    }
                }
                FrameworkTool.CreateChildList(costGo.transform.parent, csvRaceChangeSkillData.relearn_cost.Count);
                for (int i = 0; i < csvRaceChangeSkillData.relearn_cost.Count; ++i)
                {
                    Transform trans = costGo.transform.parent.GetChild(i);
                    propItem = new PropItem();
                    propItem.BindGameObject(trans.gameObject);
                    PropIconLoader.ShowItemData showItemData = new PropIconLoader.ShowItemData(csvRaceChangeSkillData.relearn_cost[0][0], csvRaceChangeSkillData.relearn_cost[0][1], true, false, false, false, false,
                        _bShowCount: true, _bShowBagCount: true, _bUseClick: true, null, true, true);
                    propItem.SetData(new MessageBoxEvt(EUIID.UI_Transfiguration_Study, showItemData));
                }
            }
            else
            {
                noSkillGo.SetActive(true);
                infoGo.SetActive(false);
                CSVGenus.Data csvGenusData = CSVGenus.Instance.GetConfData(csvRaceChangeSkillData.type);
                TextHelper.SetText(lockTip, 2013209,LanguageHelper.GetTextContent(csvGenusData.rale_name), csvRaceChangeSkillData.rank.ToString());
            }

            for(int i = 0; i < skillCellItems.Count; ++i)
            {
                skillCellItems[i].SetSelect(curSkillGridId);
            }
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
                Sys_Transfiguration.Instance.OnConfirmRefreshReq(curSkillGridId, false,Sys_Transfiguration.Instance.curIndex);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
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
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680001016,LanguageHelper.GetTextContent(reInfo.name), reInfo.level.ToString(), LanguageHelper.GetTextContent(curInfo.name), curInfo.level.ToString());
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                Sys_Transfiguration.Instance.OnConfirmRefreshReq(curSkillGridId,true,Sys_Transfiguration.Instance.curIndex);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        private void OnReStudyBtnClicked()
        {
            for (int i = 0; i < csvRaceChangeSkillData.relearn_cost.Count; ++i)
            {
                uint itemId = csvRaceChangeSkillData.relearn_cost[i][0];
                long count = Sys_Bag.Instance.GetItemCount(itemId);
                if (count< csvRaceChangeSkillData.relearn_cost[i][1])
                {
                    CSVItem.Data csvItemData = CSVItem.Instance.GetConfData(itemId);
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680001021, LanguageHelper.GetTextContent( csvItemData.name_id)));
                    return;
                }
            }
            ShapeShiftSkillGrid shapeShiftSkillGrid = Sys_Transfiguration.Instance.GetCurUseShapeShiftData().GetReplaceSkillGridByRaceId(curRaceId);
            if (shapeShiftSkillGrid == null)
            {
                Sys_Transfiguration.Instance.RefreshSkillReq(curSkillGridId,Sys_Transfiguration.Instance.curIndex);
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
                                Sys_Transfiguration.Instance.RefreshSkillReq(curSkillGridId,Sys_Transfiguration.Instance.curIndex);
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
                    CSVGenus.Data csvGenusData = CSVGenus.Instance.GetConfData(curRaceId);
                    CSVPassiveSkillInfo.Data infoRe = CSVPassiveSkillInfo.Instance.GetConfData(shapeShiftSkillGrid.Replaceid);
                    CSVPassiveSkillInfo.Data info = CSVPassiveSkillInfo.Instance.GetConfData(shapeShiftSkillGrid.Skillid);

                    if (info == null || infoRe==null)
                    {
                        return;
                    }
                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680001012, LanguageHelper.GetTextContent(csvGenusData.rale_name), LanguageHelper.GetTextContent(info.name), LanguageHelper.GetTextContent(infoRe.name));
                    PromptBoxParameter.Instance.SetConfirm(true, () =>
                    {
                        Sys_Transfiguration.Instance.OnConfirmRefreshReq(shapeShiftSkillGrid.Gridid,false, Sys_Transfiguration.Instance.curIndex);
                    });
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                }
            }
        }

        private void OnTipBtnClicked()
        {
            ChangeSkillInfo info = new ChangeSkillInfo();
            info.gridId = curSkillGridId;
            info.raceId = curRaceId;
            UIManager.OpenUI(EUIID.UI_Transfiguration_SkillTips,false, info);
        }
    }
}
