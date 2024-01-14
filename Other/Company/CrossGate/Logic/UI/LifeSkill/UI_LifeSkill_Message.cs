using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using UnityEngine.UI;
using Lib.Core;
using System;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

namespace Logic
{

    public class LifeSkillOpenParm
    {
        public uint skillId;
        public uint itemId;
    }

    public partial class UI_LifeSkill_Message : UIBase
    {
        private LivingSkill livingSkill;
        private Text skillStage;
        private Text skillName;
        private Text unlearnedTitle;
        private GameObject exp_Npcroot;
        private GameObject expRoot;
        private GameObject text_Exp;
        private Slider expSlider;
        private Text sliderVal;
        private Button addExp;
        private Button closeBtn;
        private Button learnSkill;
        private Button btnUp;
        private InfinityGrid infinity0;
        private Transform infinityParent0;
        private GameObject view1;
        private Dictionary<GameObject, LifeSkillCeil0> ceil0s = new Dictionary<GameObject, LifeSkillCeil0>();
        private int curLifeSkillSelectIndex0;
        private List<uint> unlockSkillItems = new List<uint>();
        private GameObject collect;
        private GameObject make;
        private Button forgetSkillButton;
        private GameObject medicine;
        private Button buttonRule_2;
        private Button buttonRule_3;
        private Button buttonRule_1;
        private GameObject fx_skillLevelUp;
        private GameObject fx_SkillCanLevelUp;
        private Timer timer;
        private Transform npcParent_notlearned;

        private AsyncOperationHandle<GameObject> requestRefNpc;
        private AsyncOperationHandle<GameObject> requestRefMake;
        private GameObject npcObj;

        private Animator animator;

        protected override void OnInit()
        {
            m_CurSkillId = 7;
            UpdateSkillData();
        }

        private void UpdateSkillData()
        {
            livingSkill = Sys_LivingSkill.Instance.livingSkills[m_CurSkillId];
            if (livingSkill.category == 2)
            {
                curView3MenuParentLable = (int)livingSkill.Level;
            }
            else if (livingSkill.category == 1)
            {
                curView2MenuParentLable = (int)livingSkill.Level;
            }
            curLifeSkillSelectIndex_make = 0;
            curLifeSkillSelectIndex_collect = 0;
        }

        protected override void OnOpen(object arg)
        {
            m_LifeSkillOpenParm = arg as LifeSkillOpenParm;
            if (m_LifeSkillOpenParm != null)
            {
                m_CurSkillId = m_LifeSkillOpenParm.skillId;
                m_ItemId = m_LifeSkillOpenParm.itemId;
            }
        }

        protected override void OnLoaded()
        {
            ParceLeftCp();
            ParseView1Component();
            ParseView3Component();
            ParseView2Component();
            RegisterEvent();
            RegistCollectEvent();
            RegistMakeEvent();
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_LivingSkill.Instance.eventEmitter.Handle<LifeSkillExpChangeEvt>(Sys_LivingSkill.EEvents.OnUpdateExp, OnUpdateExp, toRegister);
            Sys_LivingSkill.Instance.eventEmitter.Handle<uint, bool>(Sys_LivingSkill.EEvents.OnUpdateLevelUpButtonState, UpdateLevelUpButton, toRegister);
            Sys_LivingSkill.Instance.eventEmitter.Handle<uint>(Sys_LivingSkill.EEvents.OnLevelUp, OnLevelUp, toRegister);
            Sys_LivingSkill.Instance.eventEmitter.Handle(Sys_LivingSkill.EEvents.OnRefreshUnfixFormulaSelectItems, RefreshUnFixedFormulaGrids, toRegister);
            Sys_LivingSkill.Instance.eventEmitter.Handle(Sys_LivingSkill.EEvents.OnSetHardenItem, OnSetHardenItem, toRegister);
            Sys_LivingSkill.Instance.eventEmitter.Handle<ulong>(Sys_LivingSkill.EEvents.OnMakeSuccess, OnMakeSuccess, toRegister);
            Sys_LivingSkill.Instance.eventEmitter.Handle(Sys_LivingSkill.EEvents.OnPlayLevelUpFx, OnPlayLevelUpFx, toRegister);
            Sys_LivingSkill.Instance.eventEmitter.Handle<uint, uint>(Sys_LivingSkill.EEvents.OnRefreshLifeSkillMessage, OnRefreshCollect, toRegister);
            Sys_LivingSkill.Instance.eventEmitter.Handle<uint>(Sys_LivingSkill.EEvents.OnSkipToFormula, OnSkipToItem, toRegister);
            //Sys_LivingSkill.Instance.eventEmitter.Handle(Sys_LivingSkill.EEvents.OnUpdateGrade, OnUpdateGrade, toRegister);
            //Sys_LivingSkill.Instance.eventEmitter.Handle(Sys_LivingSkill.EEvents.OnPlayGradeUpFx, OnPlayGradeUpFx, toRegister);
            Sys_Equip.Instance.eventEmitter.Handle<ulong>(Sys_Equip.EEvents.OnNtfDecomposeItem, OnNtfDecomposeItem, toRegister);
            Sys_LivingSkill.Instance.eventEmitter.Handle<ulong>(Sys_LivingSkill.EEvents.OnEquip, OnNtfDecomposeItem, toRegister);
            Sys_Equip.Instance.eventEmitter.Handle<ulong>(Sys_Equip.EEvents.OnJewelNtfInlay, OnNtfDecomposeItem, toRegister);
            Sys_Trade.Instance.eventEmitter.Handle<ulong>(Sys_Trade.EEvents.OnSaleSuccessNtf, OnNtfDecomposeItem, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<ulong>(Sys_Bag.EEvents.OnDeleteItem, OnNtfDecomposeItem, toRegister);
            Sys_LivingSkill.Instance.eventEmitter.Handle<ulong>(Sys_LivingSkill.EEvents.OnCloseEquipTips, OnCloseEquipTips, toRegister);
            Sys_LivingSkill.Instance.eventEmitter.Handle(Sys_LivingSkill.EEvents.OnUpdateCanLearn, OnUpdateCanLearn, toRegister);
            Sys_LivingSkill.Instance.eventEmitter.Handle(Sys_LivingSkill.EEvents.OnForgetSkill, OnForgetSkill, toRegister);
            Sys_LivingSkill.Instance.eventEmitter.Handle(Sys_LivingSkill.EEvents.OnLearnedSkill, OnLearnedSkill, toRegister);
            Sys_LivingSkill.Instance.eventEmitter.Handle<uint, uint>(Sys_LivingSkill.EEvents.OnUpdateLuckyValue, OnUpdateLuckyValue, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, OnRefreshChangeData, toRegister);
        }

        private void ParseView1Component()
        {
            npcParent_notlearned = transform.Find("Animator/View01/View_Npc");
            fx_skillLevelUp = transform.Find("Animator/Fx_ui_LifeSkillup").gameObject;
            fx_SkillCanLevelUp = transform.Find("Animator/View_Title/Btn_Up/Image/Fx_ui_LifeSkill_01").gameObject;
            unlearnedTitle = transform.Find("Animator/View01/Title_Tips03/Text_Title").GetComponent<Text>();
            expSlider = transform.Find("Animator/View_Title/Text_Exp/Slider_Exp").GetComponent<Slider>();
            sliderVal = transform.Find("Animator/View_Title/Text_Exp/Text_Percent").GetComponent<Text>();
            skillStage = transform.Find("Animator/View_Title/Text_Type/Text_Name/Text_Stage").GetComponent<Text>();
            skillName = transform.Find("Animator/View_Title/Text_Type").GetComponent<Text>();
            exp_Npcroot = transform.Find("Animator/View_Title").gameObject;
            expRoot = transform.Find("Animator/View_Title").gameObject;
            text_Exp = expRoot.transform.Find("Text_Exp").gameObject;
            medicine = transform.Find("Animator/BG/View_BG/Image_Medicine").gameObject;
            make = transform.Find("Animator/BG/View_BG/Image_Make").gameObject;
            collect = transform.Find("Animator/BG/View_BG/Image_Collect").gameObject;
            btnUp = transform.Find("Animator/View_Title/Btn_Up").GetComponent<Button>();
            view1 = transform.Find("Animator/View01").gameObject;
            addExp = expRoot.transform.Find("Text_Exp/Btn_Add").GetComponent<Button>();
            learnSkill = view1.transform.Find("Btn_01").GetComponent<Button>();
            closeBtn = transform.Find("Animator/View_Title08/Btn_Close").GetComponent<Button>();
            buttonRule_2 = transform.Find("Animator/View02/Center/Btn_Rule").GetComponent<Button>();
            buttonRule_3 = transform.Find("Animator/View03/Center/Btn_Rule").GetComponent<Button>();
            buttonRule_1 = transform.Find("Animator/View01/Button_Detail").GetComponent<Button>();
            infinityParent0 = view1.transform.Find("Scroll View01");
            infinity0 = infinityParent0.gameObject.GetComponent<InfinityGrid>();
            infinity0.onCreateCell += OnCreateCellUnLerned;
            infinity0.onCellChange += OnCellChangeUnLerned;
        }

        private void OnCreateCellUnLerned(InfinityGridCell cell)
        {
            LifeSkillCeil0 entry = new LifeSkillCeil0();
            entry.BindGameObject(cell.mRootTransform.gameObject);
            entry.AddClickListener(OnCeil0Selected);
            cell.BindUserData(entry);
            ceil0s.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChangeUnLerned(InfinityGridCell cell, int index)
        {
            LifeSkillCeil0 lifeSkillCeil0 = cell.mUserData as LifeSkillCeil0;
            lifeSkillCeil0.SetData(unlockSkillItems[index], livingSkill.category, index);
            if (index != curLifeSkillSelectIndex0)
            {
                lifeSkillCeil0.Release();
            }
            else
            {
                lifeSkillCeil0.Select();
            }
        }

        private void RegisterEvent()
        {
            learnSkill.onClick.AddListener(OnLearnSkillClicked);
            addExp.onClick.AddListener(OnAddExpClicked);
            btnUp.onClick.AddListener(OnLevelUpClicked);
            buttonRule_2.onClick.AddListener(OnButtonRuleClicked);
            buttonRule_1.onClick.AddListener(OnButtonRuleClicked);
            buttonRule_3.onClick.AddListener(OnButtonRuleClicked);
            closeBtn.onClick.AddListener(() =>
            {
                UIManager.CloseUI(EUIID.UI_LifeSkill_Message, false, false);
            });
        }

        protected override void OnShow()
        {
            m_UI_CurrencyTitle.InitUi();
            UpdateCost();
            UpdateLearnState();
            m_CP_ToggleRegisterLeft.SwitchTo((int)m_CurSkillId);
            RefreshView();
            //OnUpdateGrade();
            OnUpdateCanLearn();
            if (m_ItemId != 0)
            {
                OnRefreshCollect(m_CurSkillId, m_ItemId);
            }
        }

        protected override void OnHide()
        {
            //m_GradeTimer?.Cancel();
            //fx_skillGradeUp.SetActive(false);
            npcObj = null;
            timer?.Cancel();
            fx_skillLevelUp.SetActive(false);
            canMake = false;
            A?.Kill();
            makeFun = true;
            DestroyMakeAnim();
            AddressablesUtil.ReleaseInstance(ref requestRefNpc, OnNpcAssetsLoaded);
        }

        protected override void OnClose()
        {

        }

        private void RefreshView()
        {
            curLifeSkillSelectIndex0 = -1;
            unlockSkillItems.Clear();
            //未学习
            if (!livingSkill.Unlock)
            {
                if (livingSkill.category == 2)
                {
                    unlockSkillItems = Sys_LivingSkill.Instance.GetLockedSkillItems(m_CurSkillId);
                }
                else if (livingSkill.category == 1)
                {
                    unlockSkillItems = Sys_LivingSkill.Instance.GetLockedSkillFormulas(m_CurSkillId);
                }
                ChangeToView(1);
                //infinity0.SetAmount(unlockSkillItems.Count);
                infinity0.CellCount = unlockSkillItems.Count;
                infinity0.ForceRefreshActiveCell();
                infinity0.MoveToIndex(0);
                SetExp_NpcRoot(false);
                if (livingSkill.cSVLifeSkillData.type == 1)
                    TextHelper.SetText(unlearnedTitle, 2010025);
                else if (livingSkill.cSVLifeSkillData.type == 2)
                    TextHelper.SetText(unlearnedTitle, 2010019);
                TextHelper.SetText(skillStage, 2010014);
                if (npcObj != null)
                {
                    GameObject.Destroy(npcObj);
                }
                LoadNpcIconAssetAsyn(livingSkill.cSVLifeSkillData.npc_image);
                UpdateBG(false);
                forgetSkillButton.gameObject.SetActive(false);
            }
            //已学习
            else
            {
                //采集
                if (livingSkill.category == 2)
                {
                    ChangeToView(3);
                    ConstructView3DropDownRoot();
                    forgetSkillButton.gameObject.SetActive(false);
                }
                //制造
                else if (livingSkill.category == 1)
                {
                    ChangeToView(2);
                    ConstructView2DropDownRoot();
                    forgetSkillButton.gameObject.SetActive(true);
                }
                SetExp_NpcRoot(true);
                TextHelper.SetText(skillStage, livingSkill.LevelState);
                LifeSkillExpChangeEvt lifeSkillExpChangeEvt = new LifeSkillExpChangeEvt();
                lifeSkillExpChangeEvt.skillId = livingSkill.SkillId;
                lifeSkillExpChangeEvt.cur = livingSkill.Proficiency;
                lifeSkillExpChangeEvt.max = livingSkill._MaxProficiency;
                OnUpdateExp(lifeSkillExpChangeEvt);
                UpdateLevelUpButton(livingSkill.SkillId, livingSkill.bExpFull);
                UpdateBG();
            }
            TextHelper.SetText(skillName, livingSkill.name);
        }



        private void UpdateBG(bool active = true)
        {
            if (!active)
            {
                collect.SetActive(false);
                make.SetActive(false);
                medicine.SetActive(false);
            }
            else
            {
                if (livingSkill.cSVLifeSkillData.picture == 1)
                {
                    collect.SetActive(false);
                    make.SetActive(true);
                    medicine.SetActive(false);
                }
                else if (livingSkill.cSVLifeSkillData.picture == 2)
                {
                    collect.SetActive(false);
                    make.SetActive(false);
                    medicine.SetActive(true);
                }
                else if (livingSkill.cSVLifeSkillData.picture == 3)
                {
                    collect.SetActive(true);
                    make.SetActive(false);
                    medicine.SetActive(false);
                }
            }
        }

        private void ChangeToView(int index)
        {
            if (index == 1)
            {
                view1.SetActive(true);
                view3.SetActive(false);
                view2.SetActive(false);
            }
            else if (index == 2)
            {
                view1.SetActive(false);
                view3.SetActive(false);
                view2.SetActive(true);
            }
            else if (index == 3)
            {
                view1.SetActive(false);
                view3.SetActive(true);
                view2.SetActive(false);
            }
        }

        private void UpdateChildrenCallback0(int index, Transform trans)
        {
            LifeSkillCeil0 lifeSkillCeil0 = ceil0s[trans.gameObject];
            lifeSkillCeil0.SetData(unlockSkillItems[index], livingSkill.category, index);
            if (index != curLifeSkillSelectIndex0)
            {
                lifeSkillCeil0.Release();
            }
            else
            {
                lifeSkillCeil0.Select();
            }
        }

        private void OnCeil0Selected(LifeSkillCeil0 lifeSkillCeil0)
        {
            curLifeSkillSelectIndex0 = lifeSkillCeil0.dataIndex;
            foreach (var item in ceil0s)
            {
                if (item.Value.dataIndex == curLifeSkillSelectIndex0)
                {
                    item.Value.Select();
                }
                else
                {
                    item.Value.Release();
                }
            }
            if (lifeSkillCeil0.category == 1)
            {
                CSVFormula.Data cSVFormulaData = CSVFormula.Instance.GetConfData(lifeSkillCeil0.id);
                UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_LifeSkill_Message,
                   new PropIconLoader.ShowItemData(cSVFormulaData.view_item, 0, true, false, false, false, false)));
            }
            else if (lifeSkillCeil0.category == 2)
            {
                long itemCount = Sys_Bag.Instance.GetItemCount(lifeSkillCeil0.id);
                UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_LifeSkill_Message,
                    new PropIconLoader.ShowItemData(lifeSkillCeil0.id, itemCount, true, false, false, false, false)));
            }
        }

        private void OnLearnSkillClicked_1()
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
            {
                Sys_Hint.Instance.PushForbidOprationInFight();  //战斗内提示：当前处于战斗中，无法进行该操作
                return;
            }
            uint npcId = livingSkill.cSVLifeSkillData.learn_npc;
            if (npcId == 0)
            {
                DebugUtil.LogErrorFormat("配置npcId为0");
                return;
            }
            PromptBoxParameter.Instance.Clear();
            string content = CSVLanguage.Instance.GetConfData(2010106).words;
            PromptBoxParameter.Instance.content = string.Format(content, CSVNpcLanguage.Instance.GetConfData(CSVNpc.Instance.GetConfData(npcId).name).words);
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                ActionCtrl.Instance.MoveToTargetNPCAndInteractive(npcId);
                UIManager.CloseUI(EUIID.UI_LifeSkill_Message);
                //UIManager.CloseUI(EUIID.UI_LifeSkill);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        private void OnLearnSkillClicked()
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
            {
                Sys_Hint.Instance.PushForbidOprationInFight();  //战斗内提示：当前处于战斗中，无法进行该操作
                return;
            }
            if (livingSkill.category == 1)
            {
                if (Sys_LivingSkill.Instance.GetLearnedSkillLifeNum() >= Sys_LivingSkill.Instance.freeLearnNum + Sys_LivingSkill.Instance.canLearnNum)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010150));
                }
                else
                {
                    PromptBoxParameter.Instance.OpenPromptBox(2010151, 0, () =>
                    {
                        Sys_LivingSkill.Instance.LifeSkillLearnSkillReq(livingSkill.SkillId);
                    });
                }
            }
            else if (livingSkill.category == 2)
            {
                Sys_LivingSkill.Instance.LifeSkillLearnSkillReq(livingSkill.SkillId);
            }
        }

        private void OnAddExpClicked()
        {
            if (livingSkill.bMaxLevel)
            {
                string content = CSVLanguage.Instance.GetConfData(2010111).words;
                Sys_Hint.Instance.PushContent_Normal(content);
                return;
            }
            List<uint> items = livingSkill.cSVLifeSkillData.add_proficiency_item;
            foreach (var item in items)
            {
                CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(item);
                if (cSVItemData.fun_parameter != "addproficiency")
                {
                    DebugUtil.LogErrorFormat($"道具 {item}功能参数配置错误");
                    continue;
                }
            }
            UIManager.OpenUI(EUIID.UI_ExpUp_SelectItem, false, livingSkill.SkillId);
        }

        private void OnButtonRuleClicked()
        {
            UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(livingSkill.cSVLifeSkillData.desc_id) });
        }

        private void OnLevelUpClicked()
        {
            if (Sys_Role.Instance.Role.Level < livingSkill.cSVLifeSkillLevelData.role_level)
            {
                string content = string.Format(CSVLanguage.Instance.GetConfData(2010107).words, livingSkill.cSVLifeSkillLevelData.role_level);
                Sys_Hint.Instance.PushContent_Normal(content);
                //玩家等级不够
            }
            else
            {
                if (livingSkill.cSVLifeSkillLevelData.cost_item == null)
                {
                    //该技能已经满级
                }
                else
                {
                    PromptItemData promptItemData = new PromptItemData();
                    List<uint> needitems = new List<uint>();
                    List<uint> needcounts = new List<uint>();
                    foreach (var item in livingSkill.cSVLifeSkillLevelData.cost_item)
                    {
                        needitems.Add(item[0]);
                        needcounts.Add(item[1]);
                    }
                    promptItemData.onConfire = () =>
                    {
                        Sys_LivingSkill.Instance.SkillLevelUpReq(livingSkill.SkillId);
                        UIManager.CloseUI(EUIID.UI_PromptItemBox);
                    };
                    promptItemData.notEnough = CSVLanguage.Instance.GetConfData(2010108).words;   //需要设置道具不足的语言表
                    promptItemData.content = string.Format(CSVLanguage.Instance.GetConfData(2010053).words, livingSkill.Level + 1);
                    promptItemData.needCount = needcounts;
                    promptItemData.items = needitems;
                    promptItemData.titleId = 2010135u;
                    promptItemData.type = 1;
                    UIManager.OpenUI(EUIID.UI_PromptItemBox, false, promptItemData);
                }
            }
        }

        private void OnUpdateExp(LifeSkillExpChangeEvt lifeSkillExpChangeEvt)
        {
            if (lifeSkillExpChangeEvt.skillId != livingSkill.SkillId)
            {
                return;
            }
            if (lifeSkillExpChangeEvt.max == 0)
            {
                expSlider.value = 0;
            }
            else
            {
                expSlider.value = lifeSkillExpChangeEvt.cur / lifeSkillExpChangeEvt.max;
            }
           
            if (livingSkill.bMaxLevel)
            {
                btnUp.gameObject.SetActive(false);
                sliderVal.text = string.Format("{0}/--", livingSkill.Proficiency);
            }
            else
            {
                sliderVal.text = string.Format("{0}/{1}", lifeSkillExpChangeEvt.cur, lifeSkillExpChangeEvt.max);
            }
        }



        private void SetExp_NpcRoot(bool active)
        {
            exp_Npcroot.SetActive(active);
        }

        private void UpdateLevelUpButton(uint skillId, bool active)
        {
            if (livingSkill.SkillId != skillId)
            {
                return;
            }
            if (livingSkill.bMaxLevel)
            {
                btnUp.gameObject.SetActive(false);
                sliderVal.text = string.Format("{0}/--", livingSkill.Proficiency);
                return;
            }
            btnUp.gameObject.SetActive(active);

            bool fx_can = true;
            if (Sys_Role.Instance.Role.Level < livingSkill.cSVLifeSkillLevelData.role_level)
            {
                fx_can = false;
            }
            if (livingSkill.cSVLifeSkillLevelData.cost_item == null)
            {
                fx_can = false;
            }
            else
            {
                List<uint> needitems = new List<uint>();
                List<uint> needcounts = new List<uint>();
                foreach (var item in livingSkill.cSVLifeSkillLevelData.cost_item)
                {
                    needitems.Add(item[0]);
                    needcounts.Add(item[1]);
                }
                for (int i = 0; i < needitems.Count; i++)
                {
                    uint itemCount = (uint)Sys_Bag.Instance.GetItemCount(needitems[i]);
                    if (itemCount < needcounts[i])
                    {
                        fx_can = false;
                        continue;
                    }
                }
            }
            fx_SkillCanLevelUp.SetActive(active & fx_can);
        }

        private void OnLevelUp(uint skillId)
        {
            OnUpdateLeftLevelUp(skillId);
            TextHelper.SetText(skillStage, livingSkill.LevelState);
            if (skillId != livingSkill.SkillId)
                return;
            if (livingSkill.category == 1)
            {
                foreach (var item in ceil1s_make)
                {
                    item.Value.SetGray(Sys_LivingSkill.Instance.IsSkillFormulaUnlock(item.Value.id));
                }
            }
            else if (livingSkill.category == 2)
            {
                foreach (var item in ceil1s_collect)
                {
                    item.Value.SetGray(CurView3MenuParentLable <= livingSkill.Level);
                }
            }
        }

        private void OnPlayLevelUpFx()
        {
            fx_skillLevelUp.SetActive(false);
            timer?.Cancel();
            fx_skillLevelUp.SetActive(true);
            timer = Timer.Register(5, () =>
            {
                fx_skillLevelUp.SetActive(false);
            });
        }

        private void LoadNpcIconAssetAsyn(string path)
        {
            AddressablesUtil.InstantiateAsync(ref requestRefNpc, path, OnNpcAssetsLoaded);
        }

        private void OnNpcAssetsLoaded(AsyncOperationHandle<GameObject> handle)
        {
            npcObj = handle.Result;
            if (null != npcObj)
            {
                npcObj.transform.SetParent(npcParent_notlearned);
                RectTransform rectTransform = npcObj.transform as RectTransform;
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localEulerAngles = Vector3.zero;
                rectTransform.localScale = Vector3.one;
            }
            animator = npcObj.GetComponent<Animator>();
        }


        public class LifeSkillCeil0
        {
            private Transform transform;
            private Image icon;
            private Image eventBg;
            private Image quailty;
            private GameObject select;
            public uint id;
            public uint category;
            public int dataIndex;
            private Action<LifeSkillCeil0> onClick;

            public void BindGameObject(GameObject go)
            {
                transform = go.transform;
                ParseComponent();
            }

            private void ParseComponent()
            {
                icon = transform.Find("Btn_Item/Image_Icon").GetComponent<Image>();
                eventBg = transform.Find("Btn_Item/Image_BG").GetComponent<Image>();
                quailty = transform.Find("Btn_Item/Image_Quality").GetComponent<Image>();
                select = transform.Find("Image_Select").gameObject;
                Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventBg);
                eventListener.AddEventListener(EventTriggerType.PointerClick, OnGridClicked);
            }

            public void AddClickListener(Action<LifeSkillCeil0> _onClick)
            {
                onClick = _onClick;
            }

            private void OnGridClicked(BaseEventData baseEventData)
            {
                onClick.Invoke(this);
            }

            public void SetData(uint _id, uint _category, int _dataIndex)
            {
                this.id = _id;
                this.category = _category;
                this.dataIndex = _dataIndex;
                Refresh();
            }

            private void Refresh()
            {
                if (category == 1)
                {
                    quailty.gameObject.SetActive(false);
                    CSVFormula.Data cSVFormulaData = CSVFormula.Instance.GetConfData(id);
                    if (cSVFormulaData != null)
                    {
                        ImageHelper.SetIcon(icon, CSVItem.Instance.GetConfData(cSVFormulaData.view_item).icon_id);
                        //ImageHelper.GetQualityColor_Frame(quailty, (int)CSVItem.Instance.GetConfData(cSVFormulaData.view_item).quality);
                    }
                }
                else if (category == 2)
                {
                    quailty.gameObject.SetActive(true);
                    ImageHelper.SetIcon(icon, CSVItem.Instance.GetConfData(id).icon_id);
                    ImageHelper.GetQualityColor_Frame(quailty, (int)CSVItem.Instance.GetConfData(id).quality);
                }
            }

            public void Release()
            {
                select.SetActive(false);
            }

            public void Select()
            {
                select.SetActive(true);
            }
        }
    }
}

