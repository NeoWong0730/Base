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
    public class UI_Pet_Advanced_Layout
    {
        public Transform transform;
        public Button closeButton;
        public GameObject leftViewGo;
        public GameObject rightViewGo;
        public GameObject petListViewGo;
        public void Init(Transform transform)
        {
            this.transform = transform;
            leftViewGo = transform.Find("Animator/View_Message/View_Left").gameObject;
            rightViewGo = transform.Find("Animator/View_Message/View_Right").gameObject;
            petListViewGo = transform.Find("Animator/Scroll_View_Pets").gameObject;
            closeButton = transform.Find("Animator/View_Title07/Btn_Close").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OncloseBtnClicked);

        }

        public interface IListener
        {
            void OncloseBtnClicked();
        }

    }

    public class UI_Pet_AdvancedLeftView : UIComponent
    {
        public ClientPet clientPet;
        public UI_PetLeftView_Common uI_PetLeftView_Common;

        public Button attrBBtn;

        public Button maxGradesEffect;
        protected override void Loaded()
        {
            uI_PetLeftView_Common = AddComponent<UI_PetLeftView_Common>(gameObject.transform);

            maxGradesEffect = transform.Find("Grid_FunctionBtn/Btn_Add").GetComponent<Button>();
            maxGradesEffect.onClick.AddListener(MaxGradeBtnClicked);
            attrBBtn = transform.Find("Button_Attribute").GetComponent<Button>();
            attrBBtn.onClick.AddListener(() => { UIManager.OpenUI(EUIID.UI_Element); });
        }

        public override void Hide()
        {
            base.Hide();
            clientPet = null;
            uI_PetLeftView_Common.Hide();
        }

        public override void Show()
        {
            base.Show();
            uI_PetLeftView_Common.Show();
        }

        private void OnReleaseBtnClick()
        {
            if(Sys_Pet.Instance.IsPetBeEffectWithSecureLock(clientPet.petUnit))
            {
                return;
            }
            else if(GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
            {
                Sys_Hint.Instance.PushForbidOprationInFight();  //战斗内提示：当前处于战斗中，无法进行该操作
                return;
            }
            else if (Sys_Pet.Instance.fightPet.IsSamePet(clientPet.GetPetUid()))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12203));
                return;
            }
            else if (Sys_Pet.Instance.IsLastPetEntExpiredTick(clientPet.petUnit.SimpleInfo.ExpiredTick > 0))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10927));
                return;
            }
            else if (clientPet.HasEquipDemonSpiritSphere())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002040));
                return;
            }
            bool hightScore = !clientPet.petUnit.SimpleInfo.Bind && Sys_Pet.Instance.GetPetIsHightScore(clientPet.petUnit);
            if (hightScore)
            {
                UIManager.OpenUI(EUIID.UI_Pet_Sale, false, clientPet.GetPetUid());
                
            }
            else
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.tipType = PromptBoxParameter.TipType.Text;
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(11969u, clientPet.abandonCoin.ToString());
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    if (clientPet != null)
                    {
                        Sys_Pet.Instance.OnPetAbandonPetReq(clientPet.petUnit.Uid);
                    }
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
        }

        public void SetValue(ClientPet _clientPet)
        {
            if (null != _clientPet)
            {
                clientPet = _clientPet;
            }
            else
            {
                clientPet = null;
            }

            uI_PetLeftView_Common.SetValue(_clientPet);

        }

       

        private void MaxGradeBtnClicked()
        {
            if (null != clientPet)
            {
                uint lowG = clientPet.GetPetMaxGradeCount() - clientPet.GetPetGradeCount();
                CSVPetNewParam.Data csv = CSVPetNewParam.Instance.GetConfData(29);
                if (null != csv)
                {
                    uint skillId = csv.value;
                    UIManager.OpenUI(EUIID.UI_PetSkill_Tips, false, new Tuple<uint, uint>(skillId, lowG));
                }
            }
        }
    }

    public class UI_Pet_AdvancedRightView : UIComponent
    {
        private ClientPet clientPet;
        private Image iconImage;
        private Button advancedBtn;
        private GameObject presentGo;

        private Button presentSkillBtn;
        private Image presentSkillImage;
        private Text presentNameText;
        private Text presentSkillLevelText;
        private Text presentPointText;
        private Text presentSkillCountText;
        
        private Button nextSkillBtn;
        private Image nextSkillImage;
        private Text nextNameText;
        private Text nextPointText;
        private Text nextSkillLevelText;
        private Text nextSkillCountText;

        private Text levelTipsText;
        private GameObject fullGo;
        private PropItem item;
        private Animator ani;
        protected override void Loaded()
        {
            ani = transform.GetComponent<Animator>();

            iconImage = transform.Find("Image_BG/Image_Icon").GetComponent<Image>();
            item = new PropItem();
            item.BindGameObject(transform.Find("Content/Present/PropItem").gameObject);
            advancedBtn = transform.Find("Content/Present/Button").GetComponent<Button>();
            advancedBtn.onClick.AddListener(AdvcancedBtnClicked);
            presentGo = transform.Find("Content/Present").gameObject;
            presentSkillBtn = transform.Find("Content/Present/Image1/Image_Icon").GetComponent<Button>();
            presentSkillBtn.onClick.AddListener(PresentSkillBtnClicked);
            presentSkillImage = transform.Find("Content/Present/Image1/Image_Icon").GetComponent<Image>();
            presentNameText = transform.Find("Content/Present/Text").GetComponent<Text>();
            presentSkillLevelText = transform.Find("Content/Present/Image1/Text").GetComponent<Text>();
            presentPointText = transform.Find("Content/Present/Text_Amount/Text").GetComponent<Text>();
            presentSkillCountText = transform.Find("Content/Present/Text_Skill/Text").GetComponent<Text>();

            nextSkillBtn = transform.Find("Content/Next/Image1/Image_Icon").GetComponent<Button>();
            nextSkillBtn.onClick.AddListener(NextSkillBtnClicked);
            nextSkillImage = transform.Find("Content/Next/Image1/Image_Icon").GetComponent<Image>();
            nextNameText = transform.Find("Content/Next/Text").GetComponent<Text>();
            nextSkillLevelText = transform.Find("Content/Next/Image1/Text").GetComponent<Text>();
            nextPointText = transform.Find("Content/Next/Text_Amount/Text").GetComponent<Text>();
            nextSkillCountText = transform.Find("Content/Next/Text_Skill/Text").GetComponent<Text>();

            levelTipsText = transform.Find("Content/Present/Text_Tips").GetComponent<Text>();
            fullGo = transform.Find("Content/Next/Text_full").gameObject;
        }

        private void PresentSkillBtnClicked()
        {
            uint advanceNum = clientPet.GetAdvancedNum();
            List<uint> advanceLevel = Sys_Pet.Instance.AdvancedLevel;
            bool isMax = advanceNum >= advanceLevel.Count;
            if (null != clientPet.petData.required_skills_up)
            {
                uint skillId = clientPet.petData.required_skills_up[(int)advanceNum];
                UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(skillId, 0));
            }
        }
    

        private void NextSkillBtnClicked()
        {
            uint advanceNum = clientPet.GetAdvancedNum();
            List<uint> advanceLevel = Sys_Pet.Instance.AdvancedLevel;
            bool isMax = advanceNum >= advanceLevel.Count;
            uint skillId = 0;
            if (isMax)
            {
                if (null != clientPet.petData.required_skills_up)
                {
                    skillId = clientPet.petData.required_skills_up[clientPet.petData.required_skills_up.Count - 1];
                }
            }
            else
            {
                if (null != clientPet.petData.required_skills_up)
                {
                     skillId = clientPet.petData.required_skills_up[(int)advanceNum + 1];
                }
            }
            UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(skillId, 0));
        }
    

        private void AdvcancedBtnClicked()
        {
            if (null != clientPet)
            {
                int currentAdvance = (int)clientPet.GetAdvancedNum();
                var advancelevels = Sys_Pet.Instance.AdvancedLevel;
                if (currentAdvance >= advancelevels.Count)
                {
                    return;//已满阶
                }
                uint nextAdvanceLevel = advancelevels[currentAdvance];
                if (nextAdvanceLevel > clientPet.petUnit.SimpleInfo.Level)
                {
                    uint langId = 12481;
                    if (currentAdvance == 1)
                    {
                        langId = 12482;
                    }
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(langId));
                    return;
                }
                if (null != clientPet.petData.required_skills_money)
                {
                    if (currentAdvance < clientPet.petData.required_skills_money.Count)
                    {
                        var itemAndId = clientPet.petData.required_skills_money[currentAdvance];
                        if (itemAndId.Count >= 2)
                        {
                            ItemIdCount itemIdCount = new ItemIdCount(itemAndId[0], itemAndId[1]);
                            
                            if(!itemIdCount.Enough)
                            {
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12483));
                                return;
                            }
                            else
                            {
                                PromptBoxParameter.Instance.Clear();
                                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(12485, itemIdCount.count.ToString(), LanguageHelper.GetTextContent(itemIdCount.CSV.name_id), clientPet.GetPetNmae());
                                PromptBoxParameter.Instance.SetConfirm(true, () =>
                                {
                                    ani.Play("Advanced", -1, 0f);
                                    Sys_Pet.Instance.PetUpStageReq(clientPet.GetPetUid());
                                    
                                });
                                PromptBoxParameter.Instance.SetCancel(true, null);
                                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                            }
                        }
                    }
                }
                
            }
        }

        public override void SetData(params object[] arg)
        {
            if(null != arg && arg.Length >= 1)
            {
                clientPet = arg[0] as ClientPet;
                Refresh();
            }
            else
            {
                Hide();
            }
        }

        protected override void Refresh()
        {
            if(null != clientPet)
            {
                ImageHelper.SetIcon(iconImage, clientPet.petData.icon_id);
                uint advanceNum = clientPet.GetAdvancedNum();
                List<uint> advanceLevel = Sys_Pet.Instance.AdvancedLevel;
                bool isMax = advanceNum >= advanceLevel.Count;
                presentGo.SetActive(!isMax);
                fullGo.SetActive(isMax);
                if (isMax)
                {
                    if(null != clientPet.petData.required_skills_up)
                    {
                        var skillId = clientPet.petData.required_skills_up[clientPet.petData.required_skills_up.Count - 1];
                        if (Sys_Skill.Instance.IsActiveSkill(skillId)) //主动技能
                        {
                            CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                            if (skillInfo != null)
                            {
                                ImageHelper.SetIcon(nextSkillImage, skillInfo.icon);
                                TextHelper.SetText(nextSkillLevelText, 10963, skillInfo.level.ToString());
                                //ImageHelper.GetPetSkillQuality_Frame(qualityImage, (int)skillInfo.quality);
                            }
                            else
                            {
                                Debug.LogErrorFormat("not found skillId={0} in  CSVActiveSkillInfoData", skillId);
                            }
                        }
                        else
                        {
                            CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                            if (skillInfo != null)
                            {
                                ImageHelper.SetIcon(nextSkillImage, skillInfo.icon);
                                //ImageHelper.GetPetSkillQuality_Frame(qualityImage, (int)skillInfo.quality);
                                TextHelper.SetText(nextSkillLevelText, 10963, skillInfo.level.ToString());
                            }
                            else
                            {
                                Debug.LogErrorFormat("not found skillId={0} in CSVPassiveSkillInfoData", skillId);
                            }
                        }
                    }
                    nextSkillImage.gameObject.SetActive(null != clientPet.petData.required_skills_up);
                    nextSkillLevelText.gameObject.SetActive(null != clientPet.petData.required_skills_up);
                    uint advanceNameId = 12478;
                    if (advanceNum == 2)
                    {
                        advanceNameId = 12478;
                    }
                    else if(advanceNum == 1)
                    {
                        advanceNameId = 12477;
                    }
                    else if (advanceNum == 0)
                    {
                        advanceNameId = 12476;
                    }
                    TextHelper.SetText(nextSkillCountText, (advanceNum * clientPet.petData.forward_adv_num).ToString());
                    TextHelper.SetText(nextNameText, advanceNameId);
                    if (null != clientPet.petData.required_skills_count)
                    {
                        uint count = 0;
                        for (int i = 0; i < clientPet.petData.required_skills_count.Count; i++)
                        {
                            if(i < advanceNum)
                            {
                                count += clientPet.petData.required_skills_count[i];
                            }
                        }
                        TextHelper.SetText(nextPointText, count.ToString());
                    }
                }
                else
                {
                    uint advanceNameId = 12478;
                    if (advanceNum == 2)
                    {
                        advanceNameId = 12478;
                    }
                    else if (advanceNum == 1)
                    {
                        advanceNameId = 12477;
                    }
                    else if (advanceNum == 0)
                    {
                        advanceNameId = 12476;
                    }
                    if(null != clientPet.petData.required_skills_money)
                    {
                        List<uint> itemIdCount = clientPet.petData.required_skills_money[(int)advanceNum];
                        PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(itemIdCount[0], itemIdCount[1], true, false, false, false, false, true, true, true, _bUseTips: true);
                        CSVItem.Data countItem = CSVItem.Instance.GetConfData(itemIdCount[0]);
                        TextHelper.SetText(item.txtName, countItem.name_id);
                        item.SetData(itemN, EUIID.UI_Pet_Advanced);
                    }
                    
                    TextHelper.SetText(presentSkillCountText, (advanceNum * clientPet.petData.forward_adv_num).ToString());
                    TextHelper.SetText(presentNameText, advanceNameId);
                    if (null != clientPet.petData.required_skills_count)
                    {
                        uint count = 0;
                        for (int i = 0; i < clientPet.petData.required_skills_count.Count; i++)
                        {
                            if (i < advanceNum)
                            {
                                count += clientPet.petData.required_skills_count[i];
                            }
                        }
                        TextHelper.SetText(presentPointText, count.ToString());
                    }

                    // ------------------nxet-------------------------
                    uint nextAdvance = advanceNum + 1;

                    int currentAdvance = (int)clientPet.GetAdvancedNum();
                    var advancelevels = Sys_Pet.Instance.AdvancedLevel;
                    uint nextAdvanceLevel = advancelevels[currentAdvance];
                    levelTipsText.gameObject.SetActive(true);
                    uint langId = 12481;
                    if (currentAdvance == 1)
                    {
                        langId = 12482;
                    }
                    uint styleID = 74;
                    if (nextAdvanceLevel > clientPet.petUnit.SimpleInfo.Level)
                    {
                        styleID = 75;
                    }
                    TextHelper.SetText(levelTipsText, LanguageHelper.GetTextContent(langId), CSVWordStyle.Instance.GetConfData(styleID));

                    if (nextAdvance == 2)
                    {
                        advanceNameId = 12478;
                    }
                    else if (nextAdvance == 1)
                    {
                        advanceNameId = 12477;
                    }
                    else if (nextAdvance == 0)
                    {
                        advanceNameId = 12476;
                    }

                    TextHelper.SetText(nextSkillCountText, (nextAdvance * clientPet.petData.forward_adv_num).ToString());
                    TextHelper.SetText(nextNameText, advanceNameId);
                    if (null != clientPet.petData.required_skills_count)
                    {
                        uint count = 0;
                        for (int i = 0; i < clientPet.petData.required_skills_count.Count; i++)
                        {
                            if (i < nextAdvance)
                            {
                                count += clientPet.petData.required_skills_count[i];
                            }
                        }
                        TextHelper.SetText(nextPointText, count.ToString());
                    }


                    //------------common------------------
                    if (null != clientPet.petData.required_skills_up)
                    {
                        // ------------------present--------------------------------
                        uint skillId = clientPet.petData.required_skills_up[(int)advanceNum];
                        if (Sys_Skill.Instance.IsActiveSkill(skillId)) //主动技能
                        {
                            CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                            if (skillInfo != null)
                            {
                                ImageHelper.SetIcon(presentSkillImage, skillInfo.icon);
                                TextHelper.SetText(presentSkillLevelText, 10963, skillInfo.level.ToString());
                                //ImageHelper.GetPetSkillQuality_Frame(qualityImage, (int)skillInfo.quality);
                            }
                            else
                            {
                                Debug.LogErrorFormat("not found skillId={0} in  CSVActiveSkillInfoData", skillId);
                            }
                        }
                        else
                        {
                            CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                            if (skillInfo != null)
                            {
                                ImageHelper.SetIcon(presentSkillImage, skillInfo.icon);
                                //ImageHelper.GetPetSkillQuality_Frame(qualityImage, (int)skillInfo.quality);
                                TextHelper.SetText(presentSkillLevelText, 10963, skillInfo.level.ToString());
                            }
                            else
                            {
                                Debug.LogErrorFormat("not found skillId={0} in CSVPassiveSkillInfoData", skillId);
                            }
                        }

                        skillId = clientPet.petData.required_skills_up[(int)nextAdvance];
                        if (Sys_Skill.Instance.IsActiveSkill(skillId)) //主动技能
                        {
                            CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                            if (skillInfo != null)
                            {
                                ImageHelper.SetIcon(nextSkillImage, skillInfo.icon);
                                TextHelper.SetText(nextSkillLevelText,10963, skillInfo.level.ToString());
                                //ImageHelper.GetPetSkillQuality_Frame(qualityImage, (int)skillInfo.quality);
                            }
                            else
                            {
                                Debug.LogErrorFormat("not found skillId={0} in  CSVActiveSkillInfoData", skillId);
                            }
                        }
                        else
                        {
                            CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                            if (skillInfo != null)
                            {
                                ImageHelper.SetIcon(nextSkillImage, skillInfo.icon);
                                //ImageHelper.GetPetSkillQuality_Frame(qualityImage, (int)skillInfo.quality);
                                TextHelper.SetText(nextSkillLevelText,10963, skillInfo.level.ToString());
                            }
                            else
                            {
                                Debug.LogErrorFormat("not found skillId={0} in CSVPassiveSkillInfoData", skillId);
                            }
                        }
                    }
                    nextSkillImage.gameObject.SetActive(null != clientPet.petData.required_skills_up);
                    presentSkillImage.gameObject.SetActive(null != clientPet.petData.required_skills_up);
                    nextSkillLevelText.gameObject.SetActive(null != clientPet.petData.required_skills_up);
                    presentSkillLevelText.gameObject.SetActive(null != clientPet.petData.required_skills_up);
                }
            }
            else
            {
                Hide();
            }
        }

        public override void Hide()
        {
            clientPet = null;
            base.Hide();
        }

        public override void Show()
        {
            base.Show();
        }
    }

    public class UI_Pet_AdvancedList : UI_Pet_ViewList
    {
        private List<int> petIndex;
        public override void InitGrid()
        {
            gridIndex = PetListCount();
            petIndex?.Clear();
            petIndex = Sys_Pet.Instance.GetAdvancedPetList();
            for (int i = 0; i < gridIndex; ++i)
            {
                if (petIndex.Contains(i))
                {
                    if (i < uI_Pet_Cells.Count)
                    {
                        uI_Pet_Cells[i].ReSetData();
                        uI_Pet_Cells[i].SetActive(true);
                    }
                    else
                    {
                        GameObject go = GameObject.Instantiate<GameObject>(petItemGo, petGridGo.transform);
                        InstanitatePetCell(go.transform, i, true);
                    }
                }
                else
                {
                    if (i < uI_Pet_Cells.Count)
                    {
                        uI_Pet_Cells[i].SetActive(false);
                    }
                    else
                    {
                        GameObject go = GameObject.Instantiate<GameObject>(petItemGo, petGridGo.transform);
                        InstanitatePetCell(go.transform, i, false);
                    }
                }
            }

            if (gridIndex < uI_Pet_Cells.Count)
            {
                for (int i = gridIndex; i < uI_Pet_Cells.Count; i++)
                {
                    uI_Pet_Cells[i].SetActive(false);
                }
            }
            scrollRect.StopMovement();
        }

        public override void Show()
        {
            InitGrid();
        }
    }

    public class UI_Pet_Advanced : UIBase, UI_Pet_Advanced_Layout.IListener
    {
        private UI_Pet_Advanced_Layout layout = new UI_Pet_Advanced_Layout();

        private UI_CurrencyTitle UI_CurrencyTitle;

        public UI_Pet_AdvancedLeftView leftview;
        public UI_Pet_AdvancedRightView rightview;
        private UI_Pet_AdvancedList pet_ViewList;

        private ClientPet currentChoosePet;
        #region ui_pet_message timeweekfun

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            UI_CurrencyTitle = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            rightview = AddComponent<UI_Pet_AdvancedRightView>(layout.rightViewGo.transform);
            leftview = AddComponent<UI_Pet_AdvancedLeftView>(layout.leftViewGo.transform);
            leftview.uI_PetLeftView_Common.assetDependencies = transform.GetComponent<AssetDependencies>();
            pet_ViewList = AddComponent<UI_Pet_AdvancedList>(layout.petListViewGo.transform);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle<UI_Pet_Cell>(Sys_Pet.EEvents.OnChoosePetCell, ChoosePetCell, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnChangeStatePet, SetFightState, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnReNamePet, ResetPetValue, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnNumberChangePet, AbandonPet, toRegister);

            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnUpdatePetInfo, OnUpdatePetInfo, toRegister);
            pet_ViewList.ProcessEvents(toRegister);
        }

        protected override void OnOpen(object arg = null)
        {
            if(null != arg)
            {
                currentChoosePet = Sys_Pet.Instance.GetPetByUId(Convert.ToUInt32(arg));
            }
            
        }

        protected override void OnShowEnd()
        {
            UI_CurrencyTitle.InitUi();
        }

        protected override void OnShow()
        {
            Sys_Pet.Instance.ChangeFightPetIndex();
            SetInitClient();
            LeftShow();
        }

        public void ChoosePetCell(UI_Pet_Cell uI_Pet_Cell)
        {
            if (!uI_Pet_Cell.longState)
            {
                if (uI_Pet_Cell.gridState == EPetGridState.Normal)
                {
                    currentChoosePet = uI_Pet_Cell.pet;
                    UpdateInfo();
                }
            }
        }

        private void LeftHide()
        {
            currentChoosePet = null;
            leftview.Hide();
            pet_ViewList.Hide();
        }

        private void LeftShow()
        {
            leftview.Show();
            pet_ViewList.Show();
            if (null != currentChoosePet)
            {
                int index = Sys_Pet.Instance.GetPetListIndexByUid(currentChoosePet.GetPetUid());
                if (index != -1)
                {
                    pet_ViewList?.SetPosView((uint)index);
                }
            }
            UpdateInfo();
        }

        private void SetFightState()
        {
            ReGetChoosePet();
            pet_ViewList.UpdateAllgrid();
            leftview.SetValue(currentChoosePet);
        }

        private void ResetPetValue()
        {
            ReGetChoosePet();
            rightview.SetData(currentChoosePet);
            leftview.SetValue(currentChoosePet);
        }

        private void OnUpdatePetInfo()
        {
            pet_ViewList.UpdateAllgrid();
            ResetPetValue();
        }

        private void ReGetChoosePet()
        {
            if (Sys_Pet.Instance.petsList.Count > 0)
            {
                int index = Sys_Pet.Instance.GetPetListIndexByUid(currentChoosePet.GetPetUid());
                if (index != -1)
                {
                    currentChoosePet = Sys_Pet.Instance.GetClientPet2Postion(index);
                }
                else
                {
                    currentChoosePet = Sys_Pet.Instance.GetPostion2ClientPet();
                }

            }
            else
            {
                currentChoosePet = null;
            }
        }

        public void AbandonPet()
        {
            ReGetChoosePet();
            UpdateInfo();
        }

        public void UpdateInfo()
        {
            rightview.SetData(currentChoosePet);
            leftview.SetValue(currentChoosePet);
            pet_ViewList.UpdateAllgrid();
            if (null != currentChoosePet)
            {
                int index = Sys_Pet.Instance.GetPetListIndexByUid(currentChoosePet.petUnit.Uid);
                if (index != -1)
                    pet_ViewList.SetSelect((uint)index);
            }
            else
            {
                leftview.Hide();
                pet_ViewList.SetSelect(0);
            }
        }

        private void SetInitClient()
        {
            if (currentChoosePet == null)
            {
                currentChoosePet = Sys_Pet.Instance.GetFirstAdvancedPet();
            }
            else
            {
                ClientPet tempPet = Sys_Pet.Instance.GetPetByUId(currentChoosePet.GetPetUid());
                if (null == tempPet)
                {
                    currentChoosePet = Sys_Pet.Instance.GetFirstAdvancedPet();
                }
            }
        }


        protected override void OnHide()
        {
            LeftHide();
            CameraManager.mCamera.gameObject.SetActive(true);
        }

        protected override void OnDestroy()
        {
            UI_CurrencyTitle?.Dispose();
        }

        protected override void OnUpdate()
        {
            leftview.uI_PetLeftView_Common?.ExecUpdate();
        }


        #endregion

        #region 点击事件
        public void OncloseBtnClicked()
        {
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnUnloadModel);
            UIManager.CloseUI(EUIID.UI_Pet_Advanced);
        }
        #endregion
    }
}