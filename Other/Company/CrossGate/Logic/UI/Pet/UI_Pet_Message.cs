using Framework;
using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Logic
{
    /// <summary>
    ///宠物卡片类型///
    /// </summary>
    public enum EPetCard
    {
        Normal = 1,
        Silver = 2,
        Gold = 3,
        King = 4,
    }

    public enum EPetMessageViewState
    {
        None = 0,
        Attribute = 1,
        Book = 2,
        Foster = 3,
        Skill = 4,
        Build = 5,
        Mount = 6,
        DemonSpirit = 7,
    }
    public class PetPrama
    {
        public EPetMessageViewState page;
    }
    public class MessageEx
    {
        public EPetMessageViewState messageState;
        public int subPage;
        public uint petUid;
        public uint itemID;
    }

    public class UI_Pet_Message_Layout
    {
        public Transform transform;
        
        public Button closeButton;
        public Button HelpButton;
        public Button FirstButton;
        public CP_Toggle attrToggle;
        public CP_Toggle bookToggle;
        public CP_Toggle fosterToggle;
        public CP_Toggle skillToggle;
        public CP_Toggle buildToggle;
        public CP_Toggle mountToggle;
        public CP_Toggle demonSpiritToggle;

        public GameObject bookRedGo;
        public GameObject attrRedGo;
        public GameObject messageGo;
        public GameObject leftViewGo;
        public GameObject petListViewGo;
        public GameObject noneGo;

        public GameObject attrViewGo;
        public GameObject bookViewGo;
        public GameObject fosterViewGo;
        public GameObject skillViewGo;
        public GameObject buildViewGo;
        public GameObject mountViewGo;
        public GameObject demonSpiritViewGo;
        public GameObject scorePopViewGo;
        public GameObject petSkillFx;
        public void Init(Transform transform)
        {
            this.transform = transform;
            leftViewGo = transform.Find("Animator/View_Message/View_Messag/View_Left").gameObject;
            petListViewGo = transform.Find("Animator/Scroll_View_Pets").gameObject;
            attrViewGo = transform.Find("Animator/View_Message/View_Messag").gameObject;
            bookViewGo = transform.Find("Animator/View_BookList").gameObject;
            messageGo = transform.Find("Animator/View_Message").gameObject;
            fosterViewGo = transform.Find("Animator/View_Message/View_Messag/View_Right_Foster").gameObject;
            skillViewGo = transform.Find("Animator/View_Message/View_Messag/View_Right_Skill").gameObject;
            buildViewGo = transform.Find("Animator/View_Remake").gameObject;
            mountViewGo = transform.Find("Animator/View_Mount").gameObject;
            demonSpiritViewGo = transform.Find("Animator/View_DemonSpirit").gameObject;

            scorePopViewGo = transform.Find("Animator/View_Message/View_Messag/View_RarePopup").gameObject;

            noneGo = transform.Find("Animator/View_Message/View_None").gameObject;

            attrToggle = transform.Find("Animator/Toggle_Tab/Toggle_Attribute").GetComponent<CP_Toggle>();
            bookToggle = transform.Find("Animator/Toggle_Tab/Toggle_Book").GetComponent<CP_Toggle>();
            fosterToggle = transform.Find("Animator/Toggle_Tab/Toggle_Foster").GetComponent<CP_Toggle>();
            skillToggle = transform.Find("Animator/Toggle_Tab/Toggle_Skill").GetComponent<CP_Toggle>();
            buildToggle = transform.Find("Animator/Toggle_Tab/Toggle_Remake").GetComponent<CP_Toggle>();
            mountToggle = transform.Find("Animator/Toggle_Tab/Toggle_Mount").GetComponent<CP_Toggle>();
            demonSpiritToggle = transform.Find("Animator/Toggle_Tab/Toggle_Demon").GetComponent<CP_Toggle>();

            bookRedGo = transform.Find("Animator/Toggle_Tab/Toggle_Book/Image_Dot").gameObject;
            attrRedGo = transform.Find("Animator/Toggle_Tab/Toggle_Attribute/Image_Dot").gameObject;

            closeButton = transform.Find("Animator/View_Title07/Btn_Close").GetComponent<Button>();
            HelpButton = transform.Find("Animator/Btn_Help").GetComponent<Button>();
            HelpButton.gameObject.AddComponent<ButtonCtrl>();
            FirstButton = transform.Find("Animator/Btn_FirstStart").GetComponent<Button>();
            FirstButton.gameObject.AddComponent<ButtonCtrl>();
            petSkillFx = transform.Find("Animator/Image_bg01/Fx_ui_Pet_Select").gameObject;
            petSkillFx.SetActive(false);
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OncloseBtnClicked);
            attrToggle.onValueChanged.AddListener(listener.OnAttrValueChange);
            bookToggle.onValueChanged.AddListener(listener.OnBookValueChange);
            fosterToggle.onValueChanged.AddListener(listener.OnFosterValueChange);
            skillToggle.onValueChanged.AddListener(listener.OnSkillValueChange);
            buildToggle.onValueChanged.AddListener(listener.OnBuildValueChange);
            mountToggle.onValueChanged.AddListener(listener.OnMountValueChange);
            demonSpiritToggle.onValueChanged.AddListener(listener.OnDemonSpiritValueChange);
            HelpButton.onClick.AddListener(listener.OnHelpButtonClicked);
            FirstButton.onClick.AddListener(listener.OnFirstButtonClicked);
        }

        public interface IListener
        {
            void OncloseBtnClicked();
            void OnAttrValueChange(bool isOn);
            void OnBookValueChange(bool isOn);
            void OnFosterValueChange(bool isOn);
            void OnSkillValueChange(bool isOn);
            void OnBuildValueChange(bool isOn);
            void OnMountValueChange(bool isOn);
            void OnDemonSpiritValueChange(bool isOn);
            void OnHelpButtonClicked();
            void OnFirstButtonClicked();
        }

    }

    public class UI_Pet_LeftView : UIComponent
    {
        public ClientPet clientPet;
        public UI_PetEquipSlot petEquipSlot;
        public UI_PetLeftView_Common uI_PetLeftView_Common;
        //改名
        private Button changeNameBtn;

        public Button releaseBtn;
        
        public Button peiyangButton;
        public Button attrBBtn;
        public Button scoreBtn;

        public Button rideBtn;
        public GameObject rideLightGo;
        public GameObject rideDisGo;
        public Text petLimit;

        public Button followBtn;
        public GameObject followLightGo;
        public GameObject followDisGo;

        public Button maxGradesEffect;
        public Text degreeText;
        public Button goToBankBtn;
        public Text goToBankBtnText;

        public Button openBookReviewBtn;

        public Button godPetAdvancedBtn;
        public Button levelBackBtn;
        public GameObject redPointGo;

        public CP_Toggle toggleLock;

        public Button petAppearanceBtn;
        
        private IListener listener;
        protected override void Loaded()
        {
            petEquipSlot = AddComponent<UI_PetEquipSlot>(transform.Find("Grid_PetMagicCore"));
            uI_PetLeftView_Common = AddComponent<UI_PetLeftView_Common>(gameObject.transform);         

            releaseBtn = transform.Find("Btn_01").GetComponent<Button>();
            releaseBtn.onClick.AddListener(OnReleaseBtnClick);

            changeNameBtn = transform.Find("Button_ChangeName").GetComponent<Button>();
            changeNameBtn.onClick.AddListener(OnChangeNameClick);

            maxGradesEffect = transform.Find("Grid_FunctionBtn/Btn_Add").GetComponent<Button>();
            maxGradesEffect.onClick.AddListener(MaxGradeBtnClicked);            
            attrBBtn = transform.Find("Button_Attribute").GetComponent<Button>();
            attrBBtn.onClick.AddListener(()=> { UIManager.OpenUI(EUIID.UI_Element); });

            degreeText = transform.Find("Text_Degree").GetComponent<Text>();

            rideBtn = transform.Find("Grid_FunctionBtn/Btn_Ride").GetComponent<Button>();
            rideLightGo = transform.Find("Grid_FunctionBtn/Btn_Ride/Image_Ride").gameObject;
            rideDisGo = transform.Find("Grid_FunctionBtn/Btn_Ride/Image_Rest").gameObject;
            rideBtn.onClick.AddListener(OnRideButtonClicked);
            petLimit = transform.Find("Grid_FunctionBtn/Btn_Ride/Limit").GetComponent<Text>();

            followBtn = transform.Find("Grid_FunctionBtn/Btn_Follow").GetComponent<Button>();
            followLightGo = transform.Find("Grid_FunctionBtn/Btn_Follow/Image_Follow").gameObject;
            followDisGo = transform.Find("Grid_FunctionBtn/Btn_Follow/Image_Cancel").gameObject;
            followBtn.onClick.AddListener(OnFollowBtnClicked);

            goToBankBtn = transform.Find("Btn_Go").GetComponent<Button>();
            goToBankBtnText = transform.Find("Btn_Go/Text_01").GetComponent<Text>();
            scoreBtn = transform.Find("Image_Rare").GetComponent<Button>();
            scoreBtn.onClick.AddListener(ScoreBtnClicked);
            goToBankBtn.onClick.AddListener(()=> 
            {
                if (Sys_OperationalActivity.Instance.CheckSpecialCardWithBankIsUnlock())
                {
                    UIManager.OpenUI(EUIID.UI_SafeBox);
                }
                else
                {
                    if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
                    {
                        Sys_Hint.Instance.PushForbidOprationInFight();  //战斗内提示：当前处于战斗中，无法进行该操作
                        return;
                    }
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(Sys_Pet.Instance.BankNpcId);
                    UIManager.CloseUI(EUIID.UI_Pet_Message, false, false);
                }
            });

            openBookReviewBtn = transform.Find("Btn_Book").GetComponent<Button>();
            openBookReviewBtn.onClick.AddListener(OpenBookReviewBtnClicked);
            godPetAdvancedBtn = transform.Find("Btn_Advance").GetComponent<Button>();
            godPetAdvancedBtn.onClick.AddListener(GodPetAdvancedBtnClicked);

            levelBackBtn = transform.Find("Btn_LevelDown").GetComponent<Button>();
            levelBackBtn.onClick.AddListener(LevelBackBtnClicked);
            redPointGo = transform.Find("Btn_Advance/Image_Dot").gameObject;

            toggleLock = transform.Find("Toggle_Lock").GetComponent<CP_Toggle>();
            toggleLock.onValueChanged.AddListener(OnClickLock);

            petAppearanceBtn = transform.Find("Grid_FunctionBtn/Btn_PetFashion").GetComponent<Button>();
            petAppearanceBtn.onClick.AddListener(OnPetAppearanceBtnClicked);
        }

        public void RegisterListener(IListener _listener)
        {
            listener = _listener;
        }

        private void ScoreBtnClicked()
        {
            listener?.OpenScoreView();
        }

        private void OpenBookReviewBtnClicked()
        {
            if(null != clientPet)
            {
                PetBookListPar petBookListPar = new PetBookListPar();
                petBookListPar.petId = clientPet.petData.id;
                petBookListPar.showChangeBtn = true;
                petBookListPar.ePetReviewPageType = EPetBookPageType.Friend;
                UIManager.OpenUI(EUIID.UI_Pet_BookReview, false, petBookListPar);
            }
        }

        private void GodPetAdvancedBtnClicked()
        {
            if (null != clientPet)
            {
                UIManager.OpenUI(EUIID.UI_Pet_Advanced, false, clientPet.GetPetUid());
            }
        }

        private void LevelBackBtnClicked()
        {
            if (null != clientPet)
            {
                UIManager.OpenUI(EUIID.UI_Pet_LevelDown, false, clientPet.GetPetUid());
            }
        }

        private void OnClickLock(bool isOn)
        {
            if (null != clientPet)
            {
                if (clientPet.petUnit.Islocked == isOn)
                    Sys_Pet.Instance.OnPetLockReq(clientPet.petUnit.Uid, !isOn);
            }
        }

        private void OnPetAppearanceBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_Pet_Appearance);
        }

        #region more

        public override void Hide()
        {
            base.Hide();
            petLimitTimer?.Cancel();
            clientPet = null;
            uI_PetLeftView_Common.Hide();
        }

        public override void Show()
        {
            base.Show();
            uI_PetLeftView_Common.Show();
        }
        #endregion

        #region Release
        private void OnReleaseBtnClick()
        {
            if(Sys_Pet.Instance.IsUniquePet(clientPet.petData.id))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12467));
                return;
            }
            else if (Sys_Pet.Instance.IsPetBeEffectWithSecureLock(clientPet.petUnit))
            {
                return;
            }
            else if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
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
        #endregion

        private bool isRide;
        private void OnRideButtonClicked()
        {
            if (null != clientPet)
            {
                if (clientPet.petData.mount)
                {
                    uint mountPetUid = Sys_Pet.Instance.mountPetUid;
                    uint currentPetUid = clientPet.GetPetUid();
                    if (mountPetUid != currentPetUid)
                    {
                        if (!clientPet.GetPetIsDomestication())
                        {
                            PromptBoxParameter.Instance.Clear();
                            PromptBoxParameter.Instance.tipType = PromptBoxParameter.TipType.Text;
                            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680000660);//"驯化后可以骑宠";
                            PromptBoxParameter.Instance.SetConfirm(true, () =>
                            {
                                if (clientPet != null)
                                {
                                    CSVPetNewParam.Data cSVPetParameterData = CSVPetNewParam.Instance.GetConfData(37u);
                                    if (null != cSVPetParameterData)
                                    {
                                        ActionCtrl.Instance.MoveToTargetNPCAndInteractive(cSVPetParameterData.value);
                                        UIManager.CloseUI(EUIID.UI_Pet_Message, needDestroy: false);
                                    }
                                }

                            });
                            PromptBoxParameter.Instance.SetCancel(true, null);
                            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                        }
                        else
                        {
                            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
                            {
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000664));
                                return;
                            }
                            Sys_Pet.Instance.OnPetSetCurrentMountReq(currentPetUid);
                        }
                    }
                    else
                    {
                        Sys_Pet.Instance.OnPetSetCurrentMountReq(0);// 下骑
                    }                    
                }
            }
        }

        private void OnFollowBtnClicked()
        {
            if (null != clientPet)
            {
                uint followUid = Sys_Pet.Instance.followPetUid;
                uint currentPetUid = clientPet.GetPetUid();
                if (followUid != currentPetUid)
                {
                    Sys_Pet.Instance.OnPetSetFollowPetReq(currentPetUid);
                }
                else
                {
                    Sys_Pet.Instance.OnPetSetFollowPetReq(0);// 取消跟随
                }
            }
        }

        public void SetValue(ClientPet _clientPet)
        {
            if(null != _clientPet)
            {
                if (clientPet == null)
                {
                    clientPet = _clientPet;
                }
                else
                {
                    uint curUid = clientPet.petUnit.Uid;
                    clientPet = _clientPet;
                }
                bool showAdvance = clientPet.petData.is_gold_adv && Sys_FunctionOpen.Instance.IsOpen(10553);
                godPetAdvancedBtn.gameObject.SetActive(showAdvance);
                if(showAdvance)
                {
                    redPointGo.SetActive(Sys_Pet.Instance.PetCanAdvanced(clientPet));
                }

                openBookReviewBtn.gameObject.SetActive(clientPet.petData.show_pet);
            }
            else
            {
                clientPet = null;
            }
            petEquipSlot.SetPetEquip(_clientPet);
            uI_PetLeftView_Common.SetValue(_clientPet);
            SetGradesState();
            SetRideState();
            SetFollowState();
            SetLevelDownBtntate();
            OnUpdateLockState();
            goToBankBtnText.text = LanguageHelper.GetTextContent(!Sys_OperationalActivity.Instance.CheckSpecialCardWithBankIsUnlock() ? 2007400u : 11899u);
        }

        private void SetGradesState()
        {
            if(null != clientPet)
            {
                uint lowG = clientPet.GetPetMaxGradeCount() - clientPet.GetPetGradeCount();
                bool isMax = lowG == 0;
                if (isMax)
                {
                    TextHelper.SetText(degreeText, 11802);
                }
                else
                {
                    TextHelper.SetText(degreeText, LanguageHelper.GetTextContent(11801, lowG.ToString()));
                }
                
            }            
        }

        private void SetRideState()
        {
            bool isMount = null != clientPet && clientPet.petData.mount;
            rideBtn.gameObject.SetActive(isMount && Sys_FunctionOpen.Instance.IsOpen(10512));
            if(isMount)
            {
                SetRideLiOrDis();
            }
        }

        Timer petLimitTimer = null;

        private void LimitPetTimer(float t)
        {
            long time = (long)clientPet.petUnit.SimpleInfo.ExpiredTick - (long)Sys_Time.Instance.GetServerTime();
            if (time >= 0)
            {
                petLimit.gameObject.SetActive(true);
                petLimit.text = LanguageHelper.GetTextContent(680000600u, LanguageHelper.TimeToString((uint)time, LanguageHelper.TimeFormat.Type_4));
            }
            else
            {
                HidePetLimtText();
            }
        }

        private void HidePetLimtText()
        {
            petLimitTimer?.Cancel();
            petLimit.gameObject.SetActive(false);
        }

        private void SetRideLiOrDis()
        {
            uint uid = clientPet.GetPetUid();
            bool isExpiredTick = clientPet.petUnit.SimpleInfo.ExpiredTick != 0;
            petLimitTimer?.Cancel();
            if (isExpiredTick)
            {
                LimitPetTimer(0);
                petLimitTimer = Timer.Register(1, null, LimitPetTimer, true);
            }
            else
            {
                HidePetLimtText();
            }
            bool isRide = Sys_Pet.Instance.mountPetUid == uid;
            rideLightGo.SetActive(!isRide);
            rideDisGo.SetActive(isRide);
        }

        private void SetFollowState()
        {
            if(null != clientPet)
            {
                bool isfollow = Sys_Pet.Instance.followPetUid == clientPet.GetPetUid();
                followLightGo.SetActive(!isfollow);
                followDisGo.SetActive(isfollow);
            }
        }

        private void SetLevelDownBtntate()
        {
            if (null != clientPet)
            {
                var limitLevle = Sys_Pet.Instance.LevelBackLimit;
                if(limitLevle.Length >= 1)
                {
                    levelBackBtn.gameObject.SetActive(clientPet.petUnit.SimpleInfo.Level > limitLevle[0]);
                }
            }
            else
            {
                levelBackBtn.gameObject.SetActive(false);
            }
        }


        private void OnChangeNameClick()
        {
            UIManager.OpenUI(EUIID.UI_Modification_Name, false, clientPet);
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

        public void OnUpdateLockState()
        {
            if (null != clientPet)
            {
                ClientPet data = Sys_Pet.Instance.GetPetByUId(clientPet.petUnit.Uid);
                toggleLock.SetSelected(!data.petUnit.Islocked, true);
            }
        }

        public interface IListener
        {
            void OpenScoreView();
        }
    }

    public class UI_Pet_MsgSkillView
    {
        private ClientPet clientPet;
        private Dictionary<GameObject, PetSkillCeil> skillCeilGrids = new Dictionary<GameObject, PetSkillCeil>();
        private List<PetSkillCeil> skillCeilList = new List<PetSkillCeil>();
        private List<uint> skillIdList = new List<uint>();
        private int infinityCount;

        private InfinityGrid _infinityGrid;
        public void Init(Transform transform)
        {
            _infinityGrid = transform.Find("Scroll View").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            PetSkillCeil entry = new PetSkillCeil();
            GameObject go = cell.mRootTransform.gameObject;
            entry.BingGameObject(go);
            entry.AddClickListener(OnSkillSelect);
            skillCeilGrids.Add(go, entry);
            skillCeilList.Add(entry);
            cell.BindUserData(entry);

            //dicEquipments.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            PetSkillCeil entry = cell.mUserData as PetSkillCeil;
            if (index < skillIdList.Count)
            {
                uint skillId = skillIdList[index];
                entry.SetData(skillId, clientPet.IsUniqueSkill(skillId), clientPet.IsBuildSkill(skillId));
            }
            else
            {
                entry.SetData(0, false, false);
            }
            //entry.SetData(hornType == 0 ? Sys_Chat.Instance.mSingleServerHornDatas[index] : Sys_Chat.Instance.mFullServerHornDatas[index]);
        }

        public void SetView(ClientPet clientPet)
        {
            this.clientPet = clientPet;
            skillIdList.Clear();
            infinityCount = 0;
            if (clientPet != null)
            {
                skillIdList = clientPet.GetPetSkillList();
                infinityCount = clientPet.GetPetSkillGridsCount();
                _infinityGrid.CellCount = infinityCount;
                _infinityGrid.ForceRefreshActiveCell();
            }
            else
            {
                _infinityGrid.CellCount = infinityCount;
                _infinityGrid.ForceRefreshActiveCell();
            }            
        }

        private void OnSkillSelect(PetSkillCeil petSkillCeil)
        {
            uint skillId = petSkillCeil.petSkillBase.skillId;
            if (0 != petSkillCeil.petSkillBase.skillId)
            {
                UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(skillId, 0));
            }
            else
            {
                Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnSkillToChange);                
            }            
        }
    }

    public class UI_Pet_MsgAttrView
    {
        public class MsgAttrParent : UIComponent
        {
            private COWVd<UI_PetAttr> attrVd = new COWVd<UI_PetAttr>();
            private GameObject attrGo;
            private Text tipsText;
            private Transform attrsParent;
            private Dictionary<uint, int> idValues;
            private List<uint> ids;
            private ClientPet pet;
            protected override void Loaded()
            {
                attrGo = transform.Find("Text_Attr01").gameObject;
                tipsText = transform.Find("Title_Tips01/Text_Title").GetComponent<Text>();
                attrsParent = transform.Find("Grid_Attr");
            }

            public void SetAttrView(ClientPet pet,uint type, Dictionary<uint, int> dic, List<uint> list)
            {
                this.pet = pet;
                idValues = dic;
                TextHelper.SetText(tipsText, 10966 + type);
                ids = list;
                if(ids.Count > 1)
                {
                    ids.Sort(ComPetAttr);
                }
                attrVd.TryBuildOrRefresh(attrGo, attrsParent, list.Count, BaseAttrRefresh);
            }

            public int ComPetAttr(uint a, uint b)
            {
                CSVAttr.Data aAttr = CSVAttr.Instance.GetConfData(a);
                CSVAttr.Data bAttr = CSVAttr.Instance.GetConfData(b);
                if(aAttr.pet_show_sor > bAttr.pet_show_sor)
                {
                    return 1;
                }
                else if(aAttr.pet_show_sor < bAttr.pet_show_sor)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }

            private void BaseAttrRefresh(UI_PetAttr uI_PetAttr, int index)
            {
                int value = idValues[ids[index]];
                uI_PetAttr.RefreshItem((EPkAttr)value, pet);
            }
        }

        private Transform transform;

        private GameObject attrGridGo;
        private Transform attrGridParent;

        Dictionary<uint, Dictionary<uint, int>> dataDics = new Dictionary<uint, Dictionary<uint, int>>();
        Dictionary<uint, List<uint>> dataIdDics = new Dictionary<uint, List<uint>>();
        List<uint> types = new List<uint>();

        private COWVd<MsgAttrParent> attrParentVd = new COWVd<MsgAttrParent>();

        private ClientPet pet;


        public void Init(Transform transform)
        {
            this.transform = transform;

            attrGridParent = transform.Find("Scroll View/Viewport/Content");
            attrGridGo = transform.Find("Scroll View/Viewport/Content/Attr_Grid").gameObject;
        }

        public void SetValue(ClientPet pet)
        {
            transform.gameObject.SetActive(true);
            if (null != pet)
            {
                this.pet = pet;
                dataDics.Clear();
                dataIdDics.Clear();
                types.Clear();
                List<int> keyList = new List<int>(pet.pkAttrs.Keys);
                for (int i = 0; i < keyList.Count; i++)
                {
                    int key = keyList[i];
                    uint value = (uint)key;
                    CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(value);
                    if (null != attrData && attrData.attr_type != 3 && attrData.pet_show_type != 0)
                    {
                        if (dataDics.TryGetValue(attrData.pet_show_type, out Dictionary<uint, int> dic) && dataIdDics.TryGetValue(attrData.pet_show_type, out List<uint> list))
                        {
                            dic.Add(value, key);
                            list.Add(value);
                        }
                        else
                        {
                            dic = new Dictionary<uint, int>();
                            dic.Add(value, key);
                            dataDics.Add(attrData.pet_show_type, dic);

                            list = new List<uint>();
                            list.Add(value);
                            dataIdDics.Add(attrData.pet_show_type, list);
                        }

                        if (!types.Contains(attrData.pet_show_type))
                        {
                            types.Add(attrData.pet_show_type);
                        }
                    }
                }
                types.Sort();
                List<uint> typeKey = new List<uint>(dataIdDics.Keys);
                attrParentVd.TryBuildOrRefresh(attrGridGo, attrGridParent, typeKey.Count, MsgAttrRefresh);
                FrameworkTool.ForceRebuildLayout(attrGridParent.gameObject);
            }
        }

        private void MsgAttrRefresh(MsgAttrParent msgAttrParent, int index)
        {
            uint showType = types[index];
            msgAttrParent.SetAttrView(pet, showType, dataDics[showType], dataIdDics[showType]);
        }
    }

    public class UI_Pet_RightView : UIComponent
    {
        public enum EPetAttrType
        {
            General,
            Attr,
            Skill
        }

        //成长toggle
        private CP_ToggleEx generalToggle;
        //属性toggle
        private CP_ToggleEx attrToggle;
        //技能toggle
        //private CP_ToggleEx skillToggle;

        private Button addButton;
        private Button battleButton;
        private Text battleStateText;

        private GameObject generalGo;
        private GameObject attrGo;
        private GameObject skillGo;

        private GameObject hpGo;
        private Text hp;
        private Slider hpSlider;

        private GameObject mpGo;
        private Text mp;
        private Slider mpSlider;

        private GameObject expGo;
        private Text exp;
        private Slider expSlider;
       

        private GameObject loyaltyGo;
        private Text loyalty;
        private Slider loyaltySlider;
        
        //技能数量
        private Text skill_CountText;
        //档位总数
        private Text grade_Countext;
        //改造次数
        private Text build_CountText;
        //改造技能数量
        private Text build_SkillCountText;
        //改造总档位加成
        private Text build_AllGradeCountText;

        //进阶信息
        private Text advanceNumText;

        private UI_Pet_AdvanceView mUI_Pet_AdvcanceView;
        private UI_Pet_MsgAttrView uI_Pet_MsgAttrView;
        private UI_Pet_MsgSkillView uI_Pet_MsgSkillView;
        private ClientPet clientPet;
        private EPetAttrType ePetAttrType;
        protected override void Loaded()
        {
            generalToggle = transform.Find("Menu/ListItem").GetComponent<CP_ToggleEx>();
            generalToggle.onValueChanged.AddListener(OngeneralToggleChange);

            attrToggle = transform.Find("Menu/ListItem (1)").GetComponent<CP_ToggleEx>();
            attrToggle.onValueChanged.AddListener(OnAttrToggleChange);

            //skillToggle = transform.Find("Menu/ListItem (2)").GetComponent<CP_ToggleEx>();
            //skillToggle.onValueChanged.AddListener(OnSkillToggleChange);


            addButton = transform.Find("Btn_Add").GetComponent<Button>();
            addButton.onClick.AddListener(OnaddButtonClicked);

            battleButton = transform.Find("Btn_Battle").GetComponent<Button>();
            battleButton.onClick.AddListener(OnbattleButtonClicked);
            battleStateText = transform.Find("Btn_Battle/Text_01").GetComponent<Text>();
            generalGo = transform.Find(" Page01").gameObject;

            hpGo = transform.Find(" Page01/Scroll_View/Grid/Hp").gameObject;
            hp = transform.Find(" Page01/Scroll_View/Grid/Hp/Text_Hp/Text_Percent").GetComponent<Text>();
            hpSlider = transform.Find(" Page01/Scroll_View/Grid/Hp/Text_Hp/Slider_Hp").GetComponent<Slider>();

            mpGo = transform.Find(" Page01/Scroll_View/Grid/Mp").gameObject;
            mp = transform.Find(" Page01/Scroll_View/Grid/Mp/Text_Mp/Text_Percent").GetComponent<Text>();
            mpSlider = transform.Find(" Page01/Scroll_View/Grid/Mp/Text_Mp/Slider_Mp").GetComponent<Slider>();

            expGo = transform.Find(" Page01/Scroll_View/Grid/EXP").gameObject;
            exp = transform.Find(" Page01/Scroll_View/Grid/EXP/Text_Exp/Text_Percent").GetComponent<Text>();
            expSlider = transform.Find(" Page01/Scroll_View/Grid/EXP/Text_Exp/Slider_Exp").GetComponent<Slider>();
           
            loyaltyGo = transform.Find(" Page01/Scroll_View/Grid/Loyalty").gameObject;
            loyalty = transform.Find(" Page01/Scroll_View/Grid/Loyalty/Text_Exp/Text_Percent").GetComponent<Text>();
            loyaltySlider = transform.Find(" Page01/Scroll_View/Grid/Loyalty/Text_Exp/Slider_Exp").GetComponent<Slider>();
           

            skill_CountText = transform.Find(" Page01/Scroll_View/Grid/Field/Text_Amount").GetComponent<Text>();
            grade_Countext = transform.Find(" Page01/Scroll_View/Grid/Number/Text_Amount").GetComponent<Text>();
            build_CountText = transform.Find(" Page01/Scroll_View/Grid/Reform_Number/Text_Amount").GetComponent<Text>();
            build_SkillCountText = transform.Find(" Page01/Scroll_View/Grid/Reform_Skill/Text_Amount").GetComponent<Text>();
            build_AllGradeCountText = transform.Find(" Page01/Scroll_View/Grid/Reform_Add/Text_Amount").GetComponent<Text>();
            advanceNumText = transform.Find(" Page01/Scroll_View/Grid/Number/Text_Amount").GetComponent<Text>();

            mUI_Pet_AdvcanceView = new UI_Pet_AdvanceView();
            mUI_Pet_AdvcanceView.Init(transform.Find(" Page01/Scroll_View/Grid"));


            attrGo = transform.Find(" Page02").gameObject;
            uI_Pet_MsgAttrView = new UI_Pet_MsgAttrView();
            uI_Pet_MsgAttrView.Init(attrGo.transform);

            skillGo = transform.Find(" Page03").gameObject;
            uI_Pet_MsgSkillView = new UI_Pet_MsgSkillView();
            uI_Pet_MsgSkillView.Init(skillGo.transform);

            UI_LongPressButton hp_LongPressButton = hpGo.gameObject.AddComponent<UI_LongPressButton>();
            UI_LongPressButton mp_LongPressButton = mpGo.gameObject.AddComponent<UI_LongPressButton>();
            UI_LongPressButton exp_LongPressButton = expGo.gameObject.AddComponent<UI_LongPressButton>();

            hp_LongPressButton.onStartPress.AddListener(OnHpLongPressed);
            mp_LongPressButton.onStartPress.AddListener(OnMpLongPressed);
            exp_LongPressButton.onStartPress.AddListener(OnExpLongPressed);

            hp_LongPressButton.onRelease.AddListener(OnPointerUp);
            mp_LongPressButton.onRelease.AddListener(OnPointerUp);
            exp_LongPressButton.onRelease.AddListener(OnPointerUp);
            
        }

        private void OnHpLongPressed()
        {
            UIManager.OpenUI(EUIID.UI_Pet_Tips01, false, 2001100);
        }

        private void OnMpLongPressed()
        {
            UIManager.OpenUI(EUIID.UI_Pet_Tips01, false, 2001101);
        }

        private void OnExpLongPressed()
        {
            UIManager.OpenUI(EUIID.UI_Pet_Tips01, false, 2001102);
        }

        private void OnPointerUp()
        {
            UIManager.CloseUI(EUIID.UI_Pet_Tips01);
        }

        private void ViewControl()
        {
            switch(ePetAttrType)
            {
                case EPetAttrType.General:
                    if(generalToggle.IsOn)
                    {
                        SetPetGeneralValue();
                        ViewState();
                    }
                    else
                    {
                        generalToggle.SetSelected(true, true); 
                    }
                    break;
                case EPetAttrType.Attr:
                    if (attrToggle.IsOn)
                    {
                        uI_Pet_MsgAttrView.SetValue(clientPet);
                        ViewState();
                    }
                    else
                    {
                        attrToggle.SetSelected(true, true);
                    }
                    break;
                /*case EPetAttrType.Skill:
                    if (skillToggle.IsOn)
                    {
                        ViewState();
                        uI_Pet_MsgSkillView.SetView(clientPet);                        
                    }
                    else
                    {
                        skillToggle.SetSelected(true, true);
                    }                    
                    break;*/
            }
        }

        public void UpdateData(ClientPet pet)
        {
            clientPet = pet;
            ViewControl();
            BattleButtonState();
        }

        private void SetPetGeneralValue()
        {
            if(null != clientPet)
            {
                long curHp = clientPet.GetAttrValueByAttrId((int)EPkAttr.CurHp);
                long maxHp = clientPet.GetAttrValueByAttrId((int)EPkAttr.MaxHp);
                long curMp = clientPet.GetAttrValueByAttrId((int)EPkAttr.CurMp);
                long maxMp = clientPet.GetAttrValueByAttrId((int)EPkAttr.MaxMp);
                hp.text = string.Format("{0}/{1}", curHp.ToString(), maxHp.ToString());
                hpSlider.value = (curHp + 0f) / maxHp;
                mp.text = string.Format("{0}/{1}", curMp.ToString(), maxMp.ToString());
                mpSlider.value = (curMp + 0f) / maxMp;
                loyalty.text = clientPet.petUnit.SimpleInfo.Loyalty.ToString() + "/" + Sys_Pet.Instance.MaxLoyalty.ToString();
                loyaltySlider.value = clientPet.petUnit.SimpleInfo.Loyalty / (Sys_Pet.Instance.MaxLoyalty + 0f);
                if (CSVPetNewlv.Instance.GetConfData(clientPet.petUnit.SimpleInfo.Level + 1) != null)
                {
                    expSlider.value = (clientPet.petUnit.SimpleInfo.Exp + 0f) / CSVPetNewlv.Instance.GetConfData(clientPet.petUnit.SimpleInfo.Level + 1).exp;
                    exp.text = clientPet.petUnit.SimpleInfo.Exp.ToString() + "/" + CSVPetNewlv.Instance.GetConfData(clientPet.petUnit.SimpleInfo.Level + 1).exp.ToString();
                }
                else
                {
                    string str_value = CSVParam.Instance.GetConfData(561).str_value;
                    expSlider.value = (clientPet.petUnit.SimpleInfo.Exp + 0f) / float.Parse(str_value);
                    exp.text = clientPet.petUnit.SimpleInfo.Exp.ToString() + "/" + str_value;
                }
                grade_Countext.text = clientPet.GetPetCurrentGradeCount().ToString() + "/" + clientPet.GetPetBuildMaxGradeCount().ToString();
                skill_CountText.text = clientPet.GetPetSkillGridsCount().ToString();
                build_CountText.text = clientPet.GetPeBuildCount().ToString() + "/" + clientPet.GetPetLevelCanRemakeTimes().ToString();
                build_SkillCountText.text = clientPet.GetPeBuildtSkillNotZeroCount().ToString() + "/" + clientPet.GetPeBuildtSkillGridsCount().ToString();
                build_AllGradeCountText.text = clientPet.GetPeBuildGradeCount().ToString();
                bool canAdvanced = Sys_FunctionOpen.Instance.IsOpen(10553);
                advanceNumText.transform.parent.gameObject.SetActive(canAdvanced);
                if(canAdvanced)
                {
                    uint langId = 12476;
                    if(clientPet.petData.is_gold_adv)
                    {
                        uint advanceNum = clientPet.GetAdvancedNum();
                        if(advanceNum == 1)
                        {
                            langId = 12477;
                        }
                        else if(advanceNum == 2)
                        {
                            langId = 12478;
                        }
                    }
                    else
                    {
                        langId = 12479;
                    }
                    TextHelper.SetText(advanceNumText, langId);
                }
            }
            else
            {
                advanceNumText.transform.parent.gameObject.SetActive(false);
                hp.text = "";
                hpSlider.value = 0;
                mp.text = "";
                mpSlider.value = 0;
                expSlider.value = 0;
                exp.text = "";
                loyalty.text = "";
                loyaltySlider.value = 0;
                skill_CountText.text = "";
                build_CountText.text = "";
                build_SkillCountText.text = "";
                build_AllGradeCountText.text = "";
                grade_Countext.text = "";
            }
            mUI_Pet_AdvcanceView.RefreshView(clientPet);
        }

        private void OngeneralToggleChange(bool isOn)
        {
            if(isOn)
            {
                ePetAttrType = EPetAttrType.General;
                SetPetGeneralValue();
                ViewState();
            }            
        }

        private void ViewState()
        {
            generalGo.SetActive(ePetAttrType == EPetAttrType.General);
            attrGo.SetActive(ePetAttrType == EPetAttrType.Attr);
            skillGo.SetActive(ePetAttrType == EPetAttrType.Skill);
        }

        private void OnAttrToggleChange(bool isOn)
        {
            if (isOn)
            {
                ePetAttrType = EPetAttrType.Attr;
                uI_Pet_MsgAttrView.SetValue(clientPet);
                ViewState();
            }
        }

        /*private void OnSkillToggleChange(bool isOn)
        {
            if (isOn)
            {
                ePetAttrType = EPetAttrType.Skill;
                ViewState();
                uI_Pet_MsgSkillView.SetView(clientPet);                
            }
        }*/

        private void OnaddButtonClicked()
        {
            if (null == clientPet)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10693));
                return;
            }
            
            if (!Sys_FunctionOpen.Instance.IsOpen(10502, false))
                return;
            UIManager.OpenUI(EUIID.UI_Pet_AddPoint, false, clientPet);
        }

        private void BattleButtonState()
        {            
            if(null != clientPet)
            {
                if (Sys_Pet.Instance.fightPet.IsSamePet(clientPet.petUnit))
                {
                    battleStateText.text = LanguageHelper.GetTextContent(2009322);
                }
                else
                {
                    battleStateText.text = LanguageHelper.GetTextContent(2009323);
                }
            }  
            else
            {
                battleStateText.text = LanguageHelper.GetTextContent(2009323);
            }
        }

        private void OnbattleButtonClicked()
        {
            if (null == clientPet)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10695));
                return;
            }
            CSVPetNew.Data sVPetData = CSVPetNew.Instance.GetConfData(clientPet.petUnit.SimpleInfo.PetId);
            uint petHlevel = CSVPetNewParam.Instance.GetConfData(7).value;
            if (!Sys_Pet.Instance.fightPet.IsSamePet(clientPet.petUnit))
            {
                if (Sys_Role.Instance.Role.Level < sVPetData.participation_lv)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009445));
                    return;
                }
                else if (Sys_Role.Instance.Role.Level + petHlevel < clientPet.petUnit.SimpleInfo.Level)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009446, petHlevel.ToString()));
                    return;
                }
                else if(clientPet.petUnit.SimpleInfo.ExpiredTick != 0)
                {
                    //时效坐骑，无法出战
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000650));
                    return;
                }

                /*if(clientPet.GetPetUid() == Sys_Pet.Instance.mountPetUid)
                {
                    //当前宠物处于骑乘中，是否出战宠物并取消骑乘？
                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.tipType = PromptBoxParameter.TipType.Text;
                    PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680000659);
                    PromptBoxParameter.Instance.SetConfirm(true, () =>
                    {
                        if (clientPet != null)
                        {
                            Sys_Pet.Instance.OnChangeStateReq(clientPet.petUnit.Uid);
                            Sys_Hint.Instance.PushEffectInNextFight();  //战斗内提示：当前战斗不生效，将在下一场战斗生效
                        }

                    });
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                }
                else
                {*/
                    Sys_Pet.Instance.OnChangeStateReq(clientPet.petUnit.Uid);
                    Sys_Hint.Instance.PushEffectInNextFight();  //战斗内提示：当前战斗不生效，将在下一场战斗生效
                //}
            }
            else
            {
                Sys_Pet.Instance.OnChangeStateReq(0);
                Sys_Hint.Instance.PushEffectInNextFight();  //战斗内提示：当前战斗不生效，将在下一场战斗生效
            }
        }

        public void OnClose()
        {
            ePetAttrType = EPetAttrType.General;
        }
    }

    public class UI_PetAttr : UIComponent
    {
        private Text attrname;
        private Text number;
        private Button attrDescBtn;
        private uint id;
        protected override void Loaded()
        {
            attrname = transform.GetComponent<Text>();
            number = transform.Find("Text_Number").GetComponent<Text>();
            attrDescBtn = attrname.GetComponent<Button>();
            attrDescBtn?.onClick.AddListener(OnDescBtnClick);
        }

        public void RefreshItem(EPkAttr attr, ClientPet clientPet)
        {
            this.id = (uint)attr;
            CSVAttr.Data attrInfo = CSVAttr.Instance.GetConfData(id);
            TextHelper.SetText(attrname, attrInfo.name);
            if (clientPet == null)
            {
                number.text = "";
            }
            else
            {
                long _value = clientPet.GetAttrValueByAttrId((int)attr);
                if(id == 101)
                {
                    long roleSpeed = Sys_Attr.Instance.GetRoleBaseSpeed();
                    if (_value >= 0)
                    {
                        TextHelper.SetText(number, LanguageHelper.GetTextContent(2006142u, ((_value + 0f) / roleSpeed * 100.0f).ToString() + "%"));
                    }
                    else
                    {
                        TextHelper.SetText(number, ((_value + 0f) / roleSpeed * 100.0f).ToString() + "%");
                    }
                }
                else
                {
                    if(_value >= 0)
                    {
                        TextHelper.SetText(number, LanguageHelper.GetTextContent(2006142u, (attrInfo.show_type == 1 ? _value.ToString() : (_value / 100.0f).ToString() + "%")));
                    }
                    else
                    {
                        TextHelper.SetText(number, (attrInfo.show_type == 1 ? _value.ToString() : (_value / 100.0f).ToString() + "%"));
                    }
                    
                }  
            }           
        }

        private void OnDescBtnClick()
        {
            CSVAttr.Data csvData = CSVAttr.Instance.GetConfData(id);
            if (null != csvData)
            {
                AttributeTip tip = new AttributeTip();
                tip.tipLan = csvData.desc;
                UIManager.OpenUI(EUIID.UI_Attribute_Tips, false, tip);
            }
        }
    }

    public class UI_Pet_Message_AttrView : UIComponent
    {
        private UI_Pet_RightView rightview;
        private ClientPet currentChoosePet;
        private Button loyaltyAdd;
        private Button expAdd;

        private GameObject redPointGo;
        private uint nextPostion;
        private IListener listener;

        protected override void Loaded()
        {
            rightview = AddComponent<UI_Pet_RightView>(transform.Find("View_Right"));
            expAdd = transform.Find("View_Right/ Page01/Scroll_View/Grid/EXP/Text_Exp/Butto_Add").GetComponent<Button>();
            expAdd.onClick.AddListener(OnAddexpClicked);
            loyaltyAdd = transform.Find("View_Right/ Page01/Scroll_View/Grid/Loyalty/Text_Exp/Butto_Add").GetComponent<Button>();
            loyaltyAdd.onClick.AddListener(OnAddLoyaltyClicked);
            redPointGo = transform.Find("View_Right/Btn_Add/Image_Dot").gameObject;
        }

        private void OnAddexpClicked()
        {
            listener?.AddExpBtnClick();
        }

        private void OnAddLoyaltyClicked()
        {
            listener?.AddLoyaltyBtnClick();
        }

        public override void Hide()
        {
            rightview.Hide();
        }

        public override void Show()
        {
            rightview.Show();
        }

        public void OnClose()
        {
            rightview.OnClose();
        }

        public void SetPetData(ClientPet client)
        {
            currentChoosePet = client;                
            UpdateInfo();
        }        

        public void ChangeState()
        {
            UpdateInfo();
        }


        public void UpdateInfo()
        {
            rightview.UpdateData(currentChoosePet);
            redPointGo.SetActive(null != currentChoosePet && Sys_Pet.Instance.fightPet.IsSamePet(currentChoosePet.petUnit) && Sys_Pet.Instance.IsHasFightPetPointNotUse());
        }
        public void Register(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void OnPetCeilClick(uint magicDeedsId);
            void AddExpBtnClick();
            void AddLoyaltyBtnClick();
        }
    }

    public class UI_Pet_ScorePopView : UIComponent
    {
        private Button closeBtn;
        private Text petNameText;
        private List<Text> descTexts = new List<Text>(5);
        protected override void Loaded()
        {
            closeBtn = transform.Find("Btn_Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(()=> { base.Hide(); });
            petNameText = transform.Find("Describe/Text").GetComponent<Text>();
            for (int i = 4; i >= 0; i--)
            {
                descTexts.Add(transform.Find($"Rare{i}/Text").GetComponent<Text>());
            }
        }

        public void SetValue(ClientPet clientPet)
        {
            if(null != clientPet)
            {
                TextHelper.SetText(petNameText, 12054, LanguageHelper.GetTextContent(clientPet.petData.name));
                for (int i = 0; i < descTexts.Count; i++)
                {
                    if(i == 0)//0
                    {
                        //0 1
                        TextHelper.SetText(descTexts[i], 12051, clientPet.petData.quality_score[i].ToString());
                        TextHelper.SetText(descTexts[i + 1], 12052, clientPet.petData.quality_score[i].ToString(), clientPet.petData.quality_score[i + 1].ToString());
                    }
                    else if( i == descTexts.Count - 1)//4
                    {
                        //3
                        TextHelper.SetText(descTexts[i], 12053, clientPet.petData.quality_score[clientPet.petData.quality_score.Count - 1].ToString());
                    }
                    else
                    {
                        if(i < clientPet.petData.quality_score.Count)
                        {
                            TextHelper.SetText(descTexts[i], 12052, clientPet.petData.quality_score[i - 1].ToString(), clientPet.petData.quality_score[i].ToString());
                        }
                    }
                }
                base.Show();
            }
        }
    }

    public class UI_Pet_Message : UIBase, UI_Pet_Message_Layout.IListener, UI_Pet_Message_AttrView.IListener, UI_Pet_LeftView.IListener
    {
        private UI_Pet_Message_Layout layout = new UI_Pet_Message_Layout();
        public UI_Pet_LeftView leftview;
        private UI_Pet_ViewList pet_ViewList;
        private UI_Pet_Message_AttrView attrView;
        private UI_Pet_BookList bookView;
        private UI_Pet_BookList BookView
        {
            get
            {
                if(null == bookView)
                {
                    bookView = AddComponent<UI_Pet_BookList>(layout.bookViewGo.transform);
                }

                return bookView;
            }
        }
        private UI_Pet_Develop developView;
        /// <summary>
        /// 技能页签
        /// </summary>
        private UI_Pet_Paractice_Skill leamSkillView;
        private UI_Pet_Paractice_Skill LeamSkillView
        {
            get
            {
                if (null == leamSkillView)
                {
                    leamSkillView = new UI_Pet_Paractice_Skill();
                    leamSkillView.Init(layout.skillViewGo.transform);
                }
                return leamSkillView;
            }
        }
        /// <summary>
        /// 改造页签
        /// </summary>
        private UI_Pet_Remake remakeView;
        private UI_Pet_Remake RemakeView
        {
            get
            {
                if (null == remakeView)
                {
                    remakeView = new UI_Pet_Remake();
                    remakeView.Init(layout.buildViewGo.transform);
                }
                return remakeView;
            }
        }

        private UI_Pet_Mount mountView;
        private UI_Pet_Mount MountView
        {
            get
            {
                if (null == mountView)
                {
                    mountView = new UI_Pet_Mount();
                    mountView.Init(layout.mountViewGo.transform);
                }
                return mountView;
            }
        }

        private UI_Pet_DemonSpirit demonSpirit;
        private UI_Pet_DemonSpirit DemonSpirit
        {
            get
            {
                if (null == demonSpirit)
                {
                    demonSpirit = new UI_Pet_DemonSpirit();
                    demonSpirit.Init(layout.demonSpiritViewGo.transform);
                }
                return demonSpirit;
            }
        }

        private UI_Pet_ScorePopView scorePopView;
        private UI_Pet_ScorePopView ScorePopView
        {
            get
            {
                if (null == scorePopView)
                {
                    scorePopView = AddComponent<UI_Pet_ScorePopView>(layout.scorePopViewGo.transform);
                }
                return scorePopView;
            }
        }
        private UI_CurrencyTitle UI_CurrencyTitle;
        private MessageEx messageEx;
        //private uint currentPos = 0;
        private uint currentItemId = 0;
        private EPetMessageViewState currentView = EPetMessageViewState.None;
        private UI_Pet_RedPoint redPoint;
        private ClientPet currentChoosePet;
        #region ui_pet_message timeweekfun
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            attrView = AddComponent<UI_Pet_Message_AttrView>(layout.attrViewGo.transform);
            attrView.Register(this);
            UI_CurrencyTitle = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);

            leftview = AddComponent<UI_Pet_LeftView>(layout.leftViewGo.transform);
            leftview.RegisterListener(this);
            leftview.uI_PetLeftView_Common.assetDependencies = transform.GetComponent<AssetDependencies>();
            pet_ViewList = AddComponent<UI_Pet_ViewList>(layout.petListViewGo.transform);            

            developView = new UI_Pet_Develop();
            developView.Init(layout.fosterViewGo.transform);
            redPoint = gameObject.AddComponent<UI_Pet_RedPoint>();
            redPoint?.Init(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle<UI_Pet_Cell>(Sys_Pet.EEvents.OnChoosePetCell, ChoosePetCell, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnUpdateAttr, ResetPetValue, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnChangeStatePet, SetFightState, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnReNamePet, ResetPetValue, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnNumberChangePet, AbandonPet, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnChangePostion, ResetPetList, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnCancelPostion, ResetPetList, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnUpdateExp, ResetPetValue, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnPetActivate, OnPetActivate, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnUpdatePetInfo, OnUpdatePetInfo, toRegister);
            
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnSelectItem, OnSelectItem, toRegister);

            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnSkillToChange, OnSkillToChange, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnGetAutoBlinkDataEnd, OnGetAutoBlinkDataEnd, toRegister);

            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnUpdateLoyalty, ResetPetValue, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnLearnSkill, LearnSkillEffect, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnGuidChangeToggle, OnGuidChangeToggle, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnPetRemakeRecastTipsEntry, OnRemakeTipsEntry, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle<bool>(Sys_Pet.EEvents.OnPetRemakeSkillEnd, OnRemakeSkillEnd, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, ItemRefresh, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnContractChange, ResetMount, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnEnergyChargeEnd, MountEnergyChange, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnMountScreemChange, OnMoutScreemChange, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle<List<uint>>(Sys_Pet.EEvents.OnEatFruitEnd, OnEatFruitEnd, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateOperatinalActivityShowOrHide, OnUpdateOperatinalActivityShowOrHide, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnPetLoveExpUp, OnPetActivate, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnPetLockChange, OnPetLockChange, toRegister);
            pet_ViewList?.ProcessEvents(toRegister);
            developView.ProcessEvents(toRegister);
        }
        private Timer timer;
        private void LearnSkillEffect()
        {
            layout.petSkillFx.SetActive(true);
            if (currentView == EPetMessageViewState.Skill)
            {
                LeamSkillView.SetLearnSkillEffect();
            }
            timer?.Cancel();
            timer = Timer.Register(0.5f, SkillEffect);
        }

        private void SkillEffect()
        {
            layout.petSkillFx.SetActive(false);            
        }

        protected override void OnOpen(object arg=null)
        {
            if (null != arg)
            {
                //TODO 后面统一下外部调用-H
                if (arg is Tuple<uint, object>)
                {
                    Tuple<uint, object> tuple = arg as Tuple<uint, object>;
                    uint param = Convert.ToUInt32(tuple.Item2);
                    SetMessageEx(param);
                }
                else if (arg is uint)
                {
                    uint param = Convert.ToUInt32(arg);
                    SetMessageEx(param);
                }
                else if (arg is MessageEx)
                {
                    messageEx = arg as MessageEx;
                }
                else if (arg is PetPrama)
                {
                    PetPrama petParam =arg as PetPrama;
                    SetMessageEx(petParam.page);
                }
                
                if (messageEx != null)
                {
                    currentView = messageEx.messageState;
                    //currentPos = messageEx.currentPos;
                    currentItemId = messageEx.itemID;
                }
            }
        }

        private void SetMessageEx(uint param)
        {
            uint partnerId = 0;
            uint subId = 0;
            if (param > 10)
            {
                partnerId = param / 10;
                subId = param % 10;
            }
            else
            {
                partnerId = param;
            }
            messageEx = new MessageEx
            {
                messageState = (EPetMessageViewState)partnerId,
                subPage = (int)subId
            };
        }

        private void SetMessageEx(EPetMessageViewState param)
        {
            SetMessageEx((uint)param);
        }

        protected override void OnShow()
        {
            Sys_Pet.Instance.ChangeFightPetIndex();
            UI_CurrencyTitle.InitUi();
            InitView();
            layout.bookRedGo.SetActive(Sys_Pet.Instance.BookAllCanActive());
            layout.petSkillFx.SetActive(false);
            SetDemonSpiritToggleActiveState();
            timer?.Cancel();
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
                else if (uI_Pet_Cell.gridState == EPetGridState.Emyty)
                {
                }
                else if (uI_Pet_Cell.gridState == EPetGridState.Unlock)
                {
                    uint po = uI_Pet_Cell.currentPos;
                    OnPetCeilClick(po);
                }
            }
        }

        private void LeftHide()
        {
            currentChoosePet = null;
            leftview.Hide();
            pet_ViewList?.Hide();
            layout.petSkillFx.SetActive(false);
            timer?.Cancel();
        }
        private void LeftButtonHideSet(EPetMessageViewState etype)
        {
            if (etype==EPetMessageViewState.Book|| etype == EPetMessageViewState.Mount)
            {
                layout.HelpButton.gameObject.SetActive(false);
                layout.FirstButton.gameObject.SetActive(false);
            }
            else
            {
                layout.HelpButton.gameObject.SetActive(Sys_FunctionOpen.Instance.IsOpen(10503));
                layout.FirstButton.gameObject.SetActive(true);
            }
        }
        private void LeftShow()
        {
            if (currentView == EPetMessageViewState.Attribute ||
                currentView == EPetMessageViewState.Skill ||
                currentView == EPetMessageViewState.Foster)
            {
                leftview.Show();
            }
            
            pet_ViewList?.Show();
            pet_ViewList?.CheckUnlcokRedState();
            UpdateInfo();            
        }

        private void ResetPetList()
        {
            ReGetChoosePet();
            pet_ViewList?.UpdateAllgrid();
            pet_ViewList?.CheckUnlcokRedState();
            UpdateInfo();
            //SetListPos();
        }

        private void SetFightState()
        {
            ReGetChoosePet();
            pet_ViewList?.UpdateAllgrid();
            pet_ViewList?.CheckUnlcokRedState();

            if (currentView == EPetMessageViewState.Attribute ||
                currentView == EPetMessageViewState.Skill ||
                currentView == EPetMessageViewState.Foster)
            {
                leftview.SetValue(currentChoosePet);
            }

            
            if (currentView == EPetMessageViewState.Attribute)
            {
                attrView.SetPetData(currentChoosePet);
            }
            else if (currentView == EPetMessageViewState.Skill)
            {
                LeamSkillView.SetView(currentChoosePet);
            }
            else if(currentView == EPetMessageViewState.Build)
            {
                RemakeView.RefeshView(currentChoosePet);
            }
        }

        private void OnSkillToChange()
        {

            layout.skillToggle.SetSelected(true, true);
        }

        private void OnGuidChangeToggle(uint page)
        {
            EPetMessageViewState type = (EPetMessageViewState)page;
            switch (type)
            {
                case EPetMessageViewState.None:
                    {
                        layout.attrToggle.SetSelected(true, true);                    
                    }
                    break;
                case EPetMessageViewState.Attribute:
                    {
                        layout.attrToggle.SetSelected(true, true);                      
                    }
                    break;
                case EPetMessageViewState.Book:
                    {
                        layout.bookToggle.SetSelected(true, true);                     
                    }
                    break;
                case EPetMessageViewState.Foster: // 培养
                    {
                        layout.fosterToggle.SetSelected(true, true);
                    }
                    break;
                case EPetMessageViewState.Skill: // 技能
                    {
                        layout.skillToggle.SetSelected(true, true);
                    }
                    break;
                case EPetMessageViewState.Build: // 改造
                    {
                        layout.buildToggle.SetSelected(true, true);
                    }
                    break;
                case EPetMessageViewState.Mount: // 骑术
                    {
                        layout.mountToggle.SetSelected(true, true);
                    }
                    break;
                case EPetMessageViewState.DemonSpirit: // 魔魂
                    {
                        layout.demonSpiritToggle.SetSelected(true, true);
                    }
                    break;
                default: break;
            }
        }

        private void ResetPetValue()
        {
            ReGetChoosePet();
            if (currentView == EPetMessageViewState.Attribute)
            {
                leftview.SetValue(currentChoosePet);
                attrView.SetPetData(currentChoosePet);
            }
            else if (currentView == EPetMessageViewState.Skill)
            {
                LeamSkillView.SetView(currentChoosePet);
            }
            else if (currentView == EPetMessageViewState.Build)
            {
                RemakeView.RefeshView(currentChoosePet);
            }
            else if (currentView == EPetMessageViewState.Mount)
            {
                MountView.Show();
            }
            else if(currentView == EPetMessageViewState.DemonSpirit)
            {
                DemonSpirit.Show();
            }
        }

        private void ResetMount()
        {
            if (currentView == EPetMessageViewState.Mount)
            {
                MountView.Show();
            }
        }

        private void OnMoutScreemChange()
        {
            if (currentView == EPetMessageViewState.Mount)
            {
                MountView.OnMountScreemChange();
            }
        }

        private void MountEnergyChange()
        {
            if (currentView == EPetMessageViewState.Mount)
            {
                MountView.SetEnergyInfo();
            }
        }

        private void OnRemakeTipsEntry(uint type)
        {
            if (currentView == EPetMessageViewState.Build)
            {
                RemakeView.OnRemakeTipsEntry(type);
            }
        }

        private void OnRemakeSkillEnd(bool isSuccess)
        {
            if (currentView == EPetMessageViewState.Build)
            {
                RemakeView.OnRemakeSkillEnd(isSuccess);
                OnUpdatePetInfo();
            }
        }

        private void OnEatFruitEnd(List<uint> list)
        {
            if (currentView == EPetMessageViewState.Build)
            {
                RemakeView.OnEatFruitEnd(list);
            }
        }

        private void OnUpdateOperatinalActivityShowOrHide()
        {
            SetDemonSpiritToggleActiveState();
        }

        private void SetDemonSpiritToggleActiveState()
        {
            layout.demonSpiritToggle.gameObject.SetActive(Sys_FunctionOpen.Instance.IsOpen(10595) && Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(222));
        }

        private void OnUpdatePetInfo()
        {
            ResetPetValue();
            LeftShow();
        }

        private void ItemRefresh(int changeType, int curBoxId)
        {
            ReGetChoosePet();
            if (currentView == EPetMessageViewState.Attribute)
            {
                leftview.SetValue(currentChoosePet);
                attrView.SetPetData(currentChoosePet);
            }
            else if (currentView == EPetMessageViewState.Skill)
            {
                LeamSkillView.SetView(currentChoosePet);
            }
            else if (currentView == EPetMessageViewState.Build)
            {
                RemakeView.RefeshView(currentChoosePet, true);
            }
            else if (currentView == EPetMessageViewState.Mount)
            {
                MountView.Show();
            }
            else if (currentView == EPetMessageViewState.DemonSpirit)
            {
                DemonSpirit.Show();
            }
            //LeftShow();
        }

        private void ReGetChoosePet()
        {
            if (Sys_Pet.Instance.petsList.Count > 0)
            {
                if(null != currentChoosePet)
                {
                    int index = Sys_Pet.Instance.GetPetListIndexByUid(currentChoosePet.GetPetUid());
                    if (index != -1)
                    {
                        currentChoosePet = Sys_Pet.Instance.GetClientPet2Postion(index);
                        pet_ViewList?.SetSelect((uint)index);
                    }
                    else
                    {
                        currentChoosePet = Sys_Pet.Instance.GetPostion2ClientPet();
                    }
                }
            }
            else
            {
                //CloseSelf();
                currentChoosePet = null;
            }
        }

        public void AbandonPet()
        {
            ReGetChoosePet();
            pet_ViewList?.UpdateAllgrid();
            UpdateInfo();
        }

        private void OnGetAutoBlinkDataEnd()
        {
            if(!UIManager.IsOpen(EUIID.UI_Pet_Help))
                UIManager.OpenUI(EUIID.UI_Pet_Help);
        }

        public void UpdateInfo()
        {
            if (currentView != EPetMessageViewState.Mount)
            {
                if (currentView == EPetMessageViewState.Attribute)
                {
                    attrView.SetPetData(currentChoosePet);
                    leftview.SetValue(currentChoosePet);
                }
                else if (currentView == EPetMessageViewState.Skill)
                {
                    LeamSkillView.SetView(currentChoosePet);
                    leftview.SetValue(currentChoosePet);
                }
                else if (currentView == EPetMessageViewState.Build)
                {
                    if (null != messageEx)
                    {
                        RemakeView.InitView2MessageEx(currentChoosePet, messageEx.subPage);
                        messageEx = null;
                    }
                    else
                    {
                        RemakeView.SetView(currentChoosePet);
                    }

                }
                else if (currentView == EPetMessageViewState.Foster)
                {
                    if (null != messageEx)
                    {
                        developView.SetPetData(currentChoosePet, messageEx.subPage, messageEx.itemID);
                        messageEx = null;
                    }
                    else
                    {
                        developView.SetPetData(currentChoosePet, 0, 0);
                    }
                    leftview.SetValue(currentChoosePet);
                }
                else if (currentView == EPetMessageViewState.DemonSpirit)
                {
                    demonSpirit.SetValue(currentChoosePet);
                }

                if (null != currentChoosePet)
                {
                    int index = Sys_Pet.Instance.GetPetListIndexByUid(currentChoosePet.petUnit.Uid);
                    if (index != -1)
                        pet_ViewList?.SetSelect((uint)index);
                }
                else
                {
                    leftview.Hide();
                    pet_ViewList?.SetSelect(0);
                }
                layout.noneGo.SetActive(currentChoosePet == null);
                if (currentChoosePet != null)
                {
                    if (layout.scorePopViewGo.activeSelf)
                    {
                        ScorePopView.SetValue(currentChoosePet);
                    }
                }
            }
            else
            {
                MountView.Reset();
            }
            if (null != messageEx)
            {
                messageEx = null;
            }
        }

        private void SetInitClient()
        {
            if (currentView != EPetMessageViewState.Book)
            {
                //currentChoosePet = Sys_Pet.Instance.DebutPet.GetClientPet();
                if(null == currentChoosePet)
                {
                    if(null != messageEx)
                    {
                        currentChoosePet = Sys_Pet.Instance.GetPetByUId(messageEx.petUid);
                    }
                    if(null == currentChoosePet)
                        currentChoosePet = Sys_Pet.Instance.GetPostion2ClientPet();
                } 
                else
                {
                    if(null == Sys_Pet.Instance.GetPetByUId(currentChoosePet.GetPetUid()))
                    {
                        currentChoosePet = Sys_Pet.Instance.GetPostion2ClientPet();
                    }
                }
            }
            else
            {
                currentChoosePet = null;
            }
        }

        private void InitView()
        {
            SetInitClient();
            if (null != currentChoosePet)
            {
                int index = Sys_Pet.Instance.GetPetListIndexByUid(currentChoosePet.GetPetUid());
                if (index != -1)
                {
                    pet_ViewList?.SetPosView((uint)index);
                }
            }
            
            void TryRegistAndTrigger(CP_Toggle tg, bool trigger = true){
                if (tg.ownerRegistry) {
                    tg.ownerRegistry.RegisterToggle(tg);
                }

                if (trigger) {
                    tg.SetSelected(true, true);
                }   
            }
            switch (currentView)
            {
                case EPetMessageViewState.None:
                case EPetMessageViewState.Attribute: {
                        // 先提前注册到ownerRegistry中
                        TryRegistAndTrigger(this.layout.attrToggle);
                    }
                    break;
                case EPetMessageViewState.Book:  
                    {
                        layout.bookToggle.SetSelected(true, false);
                        LeftHide();
                        attrView.Hide();
                        HideScoreView();
                        SkillHide();
                        BuildHide();
                        developView.Hide();
                        CheckOpenPage(EPetMessageViewState.Book);
                        if (null != messageEx)
                        {
                            BookView.SeInitType(messageEx.subPage);
                            BookView.Show();
                            BookView.ShowEx();
                            messageEx = null;
                        }
                        else
                        {
                            BookView.ShowEx();
                        }
                    }
                    break;
                case EPetMessageViewState.Foster: // 培养
                    {
                        TryRegistAndTrigger(this.layout.fosterToggle);
                    }
                    break;
                case EPetMessageViewState.Skill: // 技能
                    {
                        TryRegistAndTrigger(this.layout.skillToggle);
                    }
                    break;
                case EPetMessageViewState.Build: // 改造
                    {
                        TryRegistAndTrigger(this.layout.buildToggle);
                    }
                    break;
                case EPetMessageViewState.Mount: //坐骑
                    {
                        // 先提前注册到ownerRegistry中
                        TryRegistAndTrigger(this.layout.mountToggle);
                    }
                    break;
                case EPetMessageViewState.DemonSpirit:
                    {
                        // 先提前注册到ownerRegistry中
                        TryRegistAndTrigger(this.layout.demonSpiritToggle);
                    }
                    break;
                default:break;
            }
        }

        private void OnSelectItem(uint itemId)
        {
            if(currentView == EPetMessageViewState.Skill)
            {
                LeamSkillView.OnSelectItem(itemId);
            }
            else if(currentView == EPetMessageViewState.Build)
            {
                RemakeView.OnSelectItem(itemId);
            }
        }
       
        protected override void OnHide()
        {
            if (UIManager.IsOpen(EUIID.UI_Pet_RemakeTips))
                UIManager.CloseUI(EUIID.UI_Pet_RemakeTips);
            //attrView.Hide();
            HideScoreView();
            MountHide();
            DemonSpiritHide();
            leftview?.Hide();
            pet_ViewList?.Hide();
            layout.petSkillFx.SetActive(false);
            timer?.Cancel();
            CameraManager.mCamera.gameObject.SetActive(true);
        }
       
        protected override void OnDestroy()
        {
            if(null != bookView)
            {
                BookView.OnDestroy();
            }
            developView.Hide();
            UI_CurrencyTitle?.Dispose();
        }

        protected override void OnUpdate()
        {
            if (currentView == EPetMessageViewState.Attribute || currentView == EPetMessageViewState.Foster
                || currentView == EPetMessageViewState.Skill)
            {
                leftview.uI_PetLeftView_Common?.ExecUpdate();
                //pet_ViewList??.ExecUpdate();
            }
            else if (currentView == EPetMessageViewState.Book)
            {
                BookView.ExecUpdate();
            }
            else if (currentView == EPetMessageViewState.DemonSpirit)
            {
                DemonSpirit.ExecUpdate();
            }
        }
        #endregion

        #region 点击事件
        public void OncloseBtnClicked()
        {
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnUnloadModel);
            UIManager.CloseUI(EUIID.UI_Pet_Message, needDestroy: false);
        }

        private void BookHide()
        {
            if (null != bookView)
            {
                BookView.Hide();
            }
        }

        private void BookToggleChange()
        {
            if (null != bookView)
            {
                BookView.ChangeToggle();
            }
        }

        private void SkillHide()
        {
            if (null != leamSkillView)
            {
                LeamSkillView.Hide();
            }
        }

        private void BuildHide()
        {
            if(null != remakeView)
            {
                RemakeView.Hide();
            }
        }

        private void MountHide()
        {
            if (null != mountView)
            {
                MountView.Hide();
            }
        }

        private void DemonSpiritHide()
        {
            if (null != demonSpirit)
            {
                DemonSpirit.Hide();
            }

        }

        uint pageShowTime;
        private void CheckOpenPage(EPetMessageViewState state)
        {
            if (currentView != state)
            {
                if(pageShowTime != 0)
                {
                    UIManager.HitPointHide(EUIID.UI_Pet_Message, pageShowTime, currentView.ToString());
                }
                pageShowTime = Sys_Time.Instance.GetServerTime();
                UIManager.HitPointShow(EUIID.UI_Pet_Message, state.ToString());
            }
        }

        public void OnAttrValueChange(bool isOn)
        {
            if(isOn)
            {
                BookToggleChange();
                BuildHide();
                SkillHide();
                developView.Hide();
                MountHide();
                DemonSpiritHide();
                EPetMessageViewState preState = currentView;
                CheckOpenPage(EPetMessageViewState.Attribute);
                currentView = EPetMessageViewState.Attribute;
                bool needPos = preState == EPetMessageViewState.Book || preState == EPetMessageViewState.Mount;
                if (needPos)
                {
                    SetInitClient();
                }
                attrView.Show();
                LeftShow();
                LeftButtonHideSet(currentView);
            }            
        }

        public void OnBookValueChange(bool isOn)
        {
            if(isOn)
            {
                LeftHide();
                attrView.Hide();
                HideScoreView();
                SkillHide();
                BuildHide();
                developView.Hide();
                MountHide();
                DemonSpiritHide();
                CheckOpenPage(EPetMessageViewState.Book);
                currentView = EPetMessageViewState.Book;
                BookView.Show();
                LeftButtonHideSet(currentView);
            }
        }       

        public void OnFosterValueChange(bool isOn)
        {
            if (isOn)
            {
                BookToggleChange();
                attrView.Hide();
                HideScoreView();
                SkillHide();
                BuildHide();
                MountHide();
                DemonSpiritHide();
                EPetMessageViewState preState = currentView;
                CheckOpenPage(EPetMessageViewState.Foster);
                currentView = EPetMessageViewState.Foster;
                bool needPos = preState == EPetMessageViewState.Book || preState == EPetMessageViewState.Mount;
                if (needPos)
                {
                    SetInitClient();
                }
                developView.Show();
                LeftShow();
                LeftButtonHideSet(currentView);
            }
        }       

        public void OnSkillValueChange(bool isOn)
        {
            if (isOn)
            {
                BookToggleChange();
                attrView.Hide();
                HideScoreView();
                BuildHide();
                developView.Hide();
                MountHide();
                DemonSpiritHide();
                EPetMessageViewState preState = currentView;
                CheckOpenPage(EPetMessageViewState.Skill);
                currentView = EPetMessageViewState.Skill;
                bool needPos = preState == EPetMessageViewState.Book || preState == EPetMessageViewState.Mount;
                if (needPos)
                {
                    SetInitClient();                    
                }
                layout.messageGo.SetActive(true);
                LeamSkillView.Show();
                LeftShow();
                LeftButtonHideSet(currentView);
            }
        }       

        public void OnBuildValueChange(bool isOn)
        {
            if (isOn)
            {
                BookToggleChange();
                attrView.Hide();
                HideScoreView();
                SkillHide();
                developView.Hide();
                leftview.Hide();
                MountHide();
                DemonSpiritHide();
                EPetMessageViewState preState = currentView;
                CheckOpenPage(EPetMessageViewState.Build);
                currentView = EPetMessageViewState.Build;
                bool needPos = preState == EPetMessageViewState.Book || preState == EPetMessageViewState.Mount;
                if (needPos)
                {
                    SetInitClient();
                }                
                RemakeView.Show();
                pet_ViewList?.Show();
                pet_ViewList?.CheckUnlcokRedState();
                UpdateInfo();
                LeftButtonHideSet(currentView);
            }
        }

        public void OnMountValueChange(bool isOn)
        {
            if (isOn)
            {
                LeftHide();
                attrView.Hide();
                HideScoreView();
                SkillHide();
                BuildHide();
                developView.Hide();
                DemonSpiritHide();
                BookToggleChange();
                CheckOpenPage(EPetMessageViewState.Mount);
                currentView = EPetMessageViewState.Mount;
                MountView.Show();
                LeftButtonHideSet(currentView);
            }
        }

        public void OnDemonSpiritValueChange(bool isOn)
        {
            if (isOn)
            {
                leftview.Hide();
                attrView.Hide();
                HideScoreView();
                SkillHide();
                BuildHide();
                developView.Hide();
                MountHide();
                BookToggleChange();
                EPetMessageViewState preState = currentView;
                CheckOpenPage(EPetMessageViewState.DemonSpirit);
                currentView = EPetMessageViewState.DemonSpirit;
                bool needPos = preState == EPetMessageViewState.Book || preState == EPetMessageViewState.Mount;
                if (needPos)
                {
                    SetInitClient();
                }
                DemonSpirit.Show();
                pet_ViewList?.Show();
                pet_ViewList?.CheckUnlcokRedState();
                UpdateInfo();
                LeftButtonHideSet(currentView);
            }
        }

        public void OnPetCeilClick(uint magicDeedsId)
        {
            uint clickeIndex = magicDeedsId + 1;
            if (clickeIndex == Sys_Pet.Instance.devicesCount + 1) // 如果点的是第一个锁
            {
                if (Sys_Pet.Instance.costBagNum < Sys_Pet.Instance.MaxCostListCout) // 第一个锁如果等级解锁未完
                {
                    //资源解锁
                    UnLockingCost();
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10928, Sys_Pet.Instance.GetBagLevel((int)Sys_Pet.Instance.bagNum).ToString()));
                }
                
            }
            else
            {
                if (Sys_Pet.Instance.MaxLevelListCount > Sys_Pet.Instance.bagNum) // 第一个锁如果等级解锁未完
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10928, Sys_Pet.Instance.GetBagLevel((int)Sys_Pet.Instance.bagNum).ToString()));
                }
            }
        }

        private void UnLockingCost()
        {
            List<uint> costData = Sys_Pet.Instance.GetBagCountUnLockData();
            if (null != costData)
            {
                // 第一个锁如果等级解锁已经完
                //锁未资源解锁

                if (costData.Count >= 3)
                {
                    uint level = costData[0];
                    uint resId = costData[1];
                    uint resCout = costData[2];
                    CSVItem.Data csvItem = CSVItem.Instance.GetConfData(resId);
                    string text = LanguageHelper.GetTextContent(10940, level.ToString(), LanguageHelper.GetTextContent(csvItem.name_id), resCout.ToString());
                        //LanguageHelper.GetTextContent(7777777);
                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.content = text;
                    PromptBoxParameter.Instance.SetConfirm(true, () =>
                    {
                        bool isEngout = Sys_Bag.Instance.GetItemCount(resId) >= resCout;
                        if(level > Sys_Role.Instance.Role.Level)
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10929));
                        }
                        else if(!isEngout)
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10930, LanguageHelper.GetTextContent(csvItem.name_id)));
                        }
                        else
                        {
                            Sys_Pet.Instance.OnPetBagUnlockReq();
                        }
                       

                    });
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                }
                
            }
            else
            {
                DebugUtil.Log(ELogType.eNone, "data is null");
            }
        }

        private void OnPetActivate()
        {
            BookView.Active();
            layout.bookRedGo.SetActive(Sys_Pet.Instance.BookAllCanActive());
        }

        private void OnPetLockChange()
        {
            if (leftview != null)
                leftview.OnUpdateLockState();
        }

        public void OnHelpButtonClicked()
        {
            Sys_Pet.Instance.OnAutoBlinkInfoReq();
        }
        public void OnFirstButtonClicked()
        {
            Sys_Pet.Instance.PetFirstChoiceOpen();
        }
        public void AddExpBtnClick()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(10561, true))
                return;
            developView.currentItemId = Sys_Pet.Instance.expIdData[0];
            layout.fosterToggle.SetSelected(true, true);
          
        }

        public void AddLoyaltyBtnClick()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(10561, true))
                return;
            developView.currentItemId = Sys_Pet.Instance.loyatyIdData[0];
            layout.fosterToggle.SetSelected(true, true);
        }

        protected override void OnClose()
        {
            BookHide();
            currentChoosePet = null;
            if (pageShowTime != 0)
            {
                UIManager.HitPointHide(EUIID.UI_Pet_Message, pageShowTime, currentView.ToString());
            }
            pageShowTime = 0;
            currentView = EPetMessageViewState.None;
            attrView.OnClose();
            if(null != mountView)
            {
                MountView.Close();
            }
        }

        public void OpenScoreView()
        {
            if(null != currentChoosePet)
            {
                ScorePopView.SetValue(currentChoosePet);
            }
        }

        public void HideScoreView()
        {
            layout.scorePopViewGo.SetActive(false);
        }
        #endregion
    }
}