using Framework;
using Google.Protobuf;
using Lib.Core;
using Logic.Core;
using Packet;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Logic
{
    #region Icon
    public class UI_MainBattle_Icon : UIComponent
    {
        public Image roleIcon;
        public Text roleLv;
        public Text roleName;
        public Slider roleLife;
        public Slider roleMagic;
        public Image petIcon;
        private Text petLv;
        private Text petName;
        public Slider petLife;
        public Slider petMagic;
        public Transform petShow;
        private GameObject nopetGo;
        private GameObject petLvGo;
        private GameObject petLifeGo;
        private GameObject petMagicGo;
        private Button roleBtn;
        private Button petBtn;

        protected override void Loaded()
        {
            roleIcon = transform.Find("Image_Role/Head").GetComponent<Image>();
            roleLv = transform.Find("Image_Role/Text_Level").GetComponent<Text>();
            roleName = transform.Find("Image_Role/Text_Name").GetComponent<Text>();
            roleLife = transform.Find("Image_Role/Text_Life/Image_Life").GetComponent<Slider>();
            roleMagic = transform.Find("Image_Role/Text_Magic/Image_Magic").GetComponent<Slider>();
            petIcon = transform.Find("Image_Icon_Pet").GetComponent<Image>();
            petLv = transform.Find("Image_Pet/Text_Level").GetComponent<Text>();
            petName = transform.Find("Image_Pet/Text_Name").GetComponent<Text>();
            petLife = transform.Find("Image_Pet/Text_Life/Image_Life").GetComponent<Slider>();
            petMagic = transform.Find("Image_Pet/Text_Magic/Image_Magic").GetComponent<Slider>();
            petShow = transform.Find("Image_Pet");
            nopetGo = transform.Find("Image_Pet/Image_Empty").gameObject;
            petLvGo = transform.Find("Image_Pet/Image_Level_BG").gameObject;
            petLifeGo = transform.Find("Image_Pet/Text_Life/Image_Life").gameObject;
            petMagicGo = transform.Find("Image_Pet/Text_Magic/Image_Magic").gameObject;
            roleBtn = transform.Find("Image_Role").GetComponent<Button>();
            roleBtn.onClick.AddListener(OnroleBtnClicked);
            petBtn = transform.Find("Image_Pet").GetComponent<Button>();
            petBtn.onClick.AddListener(OnpetBtnClicked);
        }

        private void OnpetBtnClicked()
        {
            Sys_Pet.Instance.OnGetPetInfoReq(null, EPetUiType.UI_Message);
        }

        private void OnroleBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_Attribute);
        }

        public void SetValue()
        {
            //功能开启判断
            if (!Sys_FunctionOpen.Instance.IsOpen(10501, true))
            {
                petShow.gameObject.SetActive(false);
                petIcon.gameObject.SetActive(false);
            }
            else
            {
                petShow.gameObject.SetActive(true);
                petIcon.gameObject.SetActive(true);
                PetMessageShow();
            }
            OnUpdateRole();
        }

        private void OnUpdateRole()
        {
            if (Net_Combat.Instance.m_IsWatchBattle)
            {
                roleLv.text = Sys_Role.Instance.Role.Level.ToString();
                roleName.text = Sys_Role.Instance.Role.Name.ToStringUtf8();
                roleMagic.value = (float)Sys_Attr.Instance.curMp / Sys_Attr.Instance.pkAttrs[17];
                roleLife.value = (float)Sys_Attr.Instance.curHp / Sys_Attr.Instance.pkAttrs[15];
            }
            else
            {
                roleLv.text = Sys_Role.Instance.Role.Level.ToString();
                roleName.text = Sys_Role.Instance.Role.Name.ToStringUtf8();
                roleMagic.value = (float)Sys_Attr.Instance.curMp / Sys_Attr.Instance.pkAttrs[17];
                roleLife.value = (float)Sys_Attr.Instance.curHp / Sys_Attr.Instance.pkAttrs[15];
            }
            Sys_Head.Instance.SetHeadAndFrameData(roleIcon);
        }

        public void OnUpdatePet(bool isAdd, int petId)
        {
            PetMessageShow();
        }

        private void PetMessageShow()
        {
            if (Net_Combat.Instance.m_IsWatchBattle)
            {
                if (Sys_Pet.Instance.fightPet.HasFightPet())
                {
                    petShow.gameObject.SetActive(true);
                    petIcon.gameObject.SetActive(true);
                    FightSimplePet petUnit = Sys_Pet.Instance.fightPet;
                    SimplePet simplePet = petUnit.GetSimplePet();
                    CSVPetNew.Data petdate = CSVPetNew.Instance.GetConfData(simplePet.PetId);
                    ImageHelper.SetIcon(petIcon, petdate.icon_id);
                    if (simplePet.Name.IsEmpty)
                        petName.text = LanguageHelper.GetTextContent(petdate.name);
                    else
                        petName.text = simplePet.Name.ToStringUtf8();
                    petLv.text = simplePet.Level.ToString();
                    petLife.value = petUnit.GetHpSliderValue();
                    petMagic.value = petUnit.GetMpSliderValue();
                }
                else
                {
                    petShow.gameObject.SetActive(false);
                    petIcon.gameObject.SetActive(false);
                }
            }
            else
            {
                if (GameCenter.mainFightPet == null || GameCenter.mainFightPet.battleUnit == null)
                {
                    PetIconShow(false);
                }
                else
                {
                    PetIconShow(true);
                   CSVPetNew.Data petdate = GameCenter.mainFightPet.cSVPetData;
                    PetUnit petuint = null;
                    if (Sys_Pet.Instance.petsList != null)
                    {
                        for (int i = 0; i < Sys_Pet.Instance.petsList.Count; ++i)
                        {
                            if (Sys_Pet.Instance.petsList[i].petUnit.Uid == GameCenter.mainFightPet.battleUnit.PetId)
                            {
                                petuint = Sys_Pet.Instance.petsList[i].petUnit;
                            }
                        }
                        if (petdate != null && petuint != null)
                        {
                            ImageHelper.SetIcon(petIcon, petdate.icon_id);
                            if (petuint.SimpleInfo.Name.IsEmpty)
                                petName.text = LanguageHelper.GetTextContent(petdate.name);
                            else
                                petName.text = petuint.SimpleInfo.Name.ToStringUtf8();
                        }
                        petLv.text = GameCenter.mainFightPet.battleUnit.Level.ToString();
                        petLife.value = (float)GameCenter.mainFightPet.battleUnit.CurHp / (float)GameCenter.mainFightPet.battleUnit.MaxHp;
                        petMagic.value = (float)GameCenter.mainFightPet.battleUnit.CurMp / (float)GameCenter.mainFightPet.battleUnit.MaxMp;
                        ImageHelper.SetImageGray(petIcon, GameCenter.mainFightPet.battleUnit.CurHp == 0);
                    }
                }
            }
        }

        private void PetIconShow(bool hasFightPet)
        {
            nopetGo.gameObject.SetActive(!hasFightPet);
            petIcon.gameObject.SetActive(hasFightPet);
            petLvGo.gameObject.SetActive(hasFightPet);
            petName.gameObject.SetActive(hasFightPet);
            petLv.gameObject.SetActive(hasFightPet);
            petLife.gameObject.SetActive(hasFightPet);
            petLifeGo.gameObject.SetActive(hasFightPet);
            petMagicGo.gameObject.SetActive(hasFightPet);
            petMagic.gameObject.SetActive(hasFightPet);
        }
    }
    #endregion Icon

    #region Right_Menu
    public class UI_MainBattle_Right_Menu : UIComponent
    {
        public Button Button_Role_Skill;
        public Button Button_Pet_Skill;
        private Button Button_Attack;
        private Button Button_Defense;
        public Button Button_Last_Skill;
        public Button Button_Pet;
        public Button Button_Recall;
        public Button Button_Transposition;
        public Button Button_Goods;
        public Button Button_Runaway;
        public Button Button_Retreat;
        public Button Button_Seal;
        public Button Button_SparSkill;
        public Image typeIcon;

        public GameObject Skill_Role_Dark;
        public GameObject Escape_Dark;
        public GameObject Retreat_Dark;
        public GameObject Recall_Dark;
        public GameObject Pet_Dark;
        public GameObject Item_Dark;
        public GameObject Seal_Dark;
        public GameObject Last_Skill_Dark;
        public GameObject SparSkill_Dark;
        public GameObject TransPosition_Dark;

        public GameObject Go_Attack;
        public GameObject Go_Seal;
        public GameObject Go_Last_Skill;
        public GameObject Go_Escape;
        public GameObject Go_Retreat;
        public GameObject Go_Recall;
        public GameObject Go_Pet;
        public GameObject Go_Skill_Role;
        public GameObject Go_Skill_Pet;
        public GameObject Go_Item;
        public GameObject Go_Defense;
        public GameObject Go_Transposition;

        public GameObject Go_Last_Skill_Light;
        public GameObject Go_Attack_Light;
        public GameObject Go_SparSkill;

        public Text Text_SealOrBoom;
        public Image Image_SealOrBoom;


        public override void Hide()
        {
            base.Hide();
        }

        protected override void Loaded()
        {
            Button_Role_Skill = transform.Find("Grid01/Skill/Image_BG/Button_Skill").GetComponent<Button>();
            Button_Role_Skill.onClick.AddListener(OnSkill_Role_ButtonClicked);
            Button_Pet_Skill = transform.Find("Grid01/PetSkill/Image_BG/Button_Skill").GetComponent<Button>();
            Button_Pet_Skill.onClick.AddListener(OnSkill_Role_ButtonClicked);
            Button_Attack = transform.Find("Grid01/Attack/Button_Attack").GetComponent<Button>();
            Button_Attack.onClick.AddListener(OnAttack_ButtonClicked);
            Button_Defense = transform.Find("Grid01/Defense/Button_Defense").GetComponent<Button>();
            Button_Defense.onClick.AddListener(OnDefense_ButtonClicked);
            Button_Last_Skill = transform.Find("Grid01/Last_Skill/Button_Last_Skill").GetComponent<Button>();
            Button_Last_Skill.onClick.AddListener(OnLast_Skill_ButtonClicked);
            Button_Pet = transform.Find("Grid02/Pet/Button_Pet").GetComponent<Button>();
            Button_Pet.onClick.AddListener(OnPet_ButtonClicked);
            Button_Recall = transform.Find("Grid02/Recall/Button_Recall").GetComponent<Button>();
            Button_Recall.onClick.AddListener(OnRecall_ButtonClicked);
            Button_Transposition = transform.Find("Grid02/Transposition/Button_Transposition").GetComponent<Button>();
            Button_Transposition.onClick.AddListener(OnTransposition_ButtonClicked);
            Button_Goods = transform.Find("Grid02/Goods/Button_Goods").GetComponent<Button>();
            Button_Goods.onClick.AddListener(OnGoods_ButtonClicked);
            Button_Runaway = transform.Find("Grid02/Runaway/Button_Runaway").GetComponent<Button>();
            Button_Runaway.onClick.AddListener(OnRunaway_ButtonClicked);
            Button_Retreat = transform.Find("Grid02/Retreat/Button_Runaway").GetComponent<Button>();
            Button_Retreat.onClick.AddListener(OnRetreat_ButtonClicked);
            Button_Seal = transform.Find("Grid02/Seal/Button_Pet").GetComponent<Button>();
            Button_Seal.onClick.AddListener(OnSeal_ButtonClicked);
            Button_SparSkill = transform.Find("Grid01/SparSkill/Image_BG/Button_Skill").GetComponent<Button>();
            Button_SparSkill.onClick.AddListener(OnSparSkill_ButtonClicked);
            typeIcon = transform.Find("Grid01/Last_Skill/Image_Type").GetComponent<Image>();

            Skill_Role_Dark = transform.Find("Grid01/Skill/Image_BG/Image_Dark").gameObject;
            Skill_Role_Dark.GetComponent<Button>().onClick.AddListener(OnSkill_DarkClicked);
            Escape_Dark = transform.Find("Grid02/Runaway/Image_Dark").gameObject;
            Escape_Dark.GetComponent<Button>().onClick.AddListener(OnEscape_DarkClicked);
            Retreat_Dark = transform.Find("Grid02/Retreat/Image_Dark").gameObject;
            Retreat_Dark.GetComponent<Button>().onClick.AddListener(OnRetreat_DarkClicked);
            Item_Dark = transform.Find("Grid02/Goods/Image_Dark").gameObject;
            Item_Dark.GetComponent<Button>().onClick.AddListener(OnItem_DarkClicked);
            Seal_Dark = transform.Find("Grid02/Seal/Image_Dark").gameObject;
            Seal_Dark.GetComponent<Button>().onClick.AddListener(OnSeal_DarkClicked);
            Recall_Dark = transform.Find("Grid02/Recall/Image_Dark").gameObject;
            Recall_Dark.GetComponent<Button>().onClick.AddListener(OnRecall_DarkClicked);
            Pet_Dark = transform.Find("Grid02/Pet/Image_Dark").gameObject;
            Pet_Dark.GetComponent<Button>().onClick.AddListener(OnPet_DarkClicked);
            Last_Skill_Dark = transform.Find("Grid01/Last_Skill/Image_Dark").gameObject;
            Last_Skill_Dark.GetComponent<Button>().onClick.AddListener(OnLast_Skill_DarkClicked);
            SparSkill_Dark = transform.Find("Grid01/SparSkill/Image_BG/Image_Dark").gameObject;
            SparSkill_Dark.GetComponent<Button>().onClick.AddListener(OnSparSkill_DarkClicked);
            TransPosition_Dark = transform.Find("Grid02/Transposition/Image_Dark").gameObject;
            TransPosition_Dark.GetComponent<Button>().onClick.AddListener(OnTransPosition_DarkClicked);

            Go_Attack = transform.Find("Grid01/Attack").gameObject;
            Go_Seal = transform.Find("Grid02/Seal").gameObject;
            Go_Last_Skill = transform.Find("Grid01/Last_Skill").gameObject;
            Go_Escape = transform.Find("Grid02/Runaway").gameObject;
            Go_Retreat = transform.Find("Grid02/Retreat").gameObject;
            Go_Recall = transform.Find("Grid02/Recall").gameObject;
            Go_Pet = transform.Find("Grid02/Pet").gameObject;
            Go_Skill_Role = transform.Find("Grid01/Skill").gameObject;
            Go_Skill_Pet = transform.Find("Grid01/PetSkill").gameObject;
            Go_Item = transform.Find("Grid02/Goods").gameObject;
            Go_Defense = transform.Find("Grid01/Defense").gameObject;
            Go_Transposition = transform.Find("Grid02/Transposition").gameObject;

            Go_Last_Skill_Light = transform.Find("Grid01/Last_Skill/Image_Select").gameObject;
            Go_Attack_Light = transform.Find("Grid01/Attack/Image_Select").gameObject;
            Go_SparSkill = transform.Find("Grid01/SparSkill").gameObject;

            Text_SealOrBoom = transform.Find("Grid02/Seal/Text").GetComponent<Text>();
            Image_SealOrBoom = transform.Find("Grid02/Seal/Button_Pet").GetComponent<Image>();
        }

        private void OnRetreat_DarkClicked()
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(359999995));
        }

        private void OnSparSkill_DarkClicked()
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3900013));
        }

        private void OnLast_Skill_DarkClicked()
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3900012));
        }

        private void OnPet_DarkClicked()
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3900015));
        }

        private void OnRecall_DarkClicked()
        {
            if (GameCenter.mainFightPet == null)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3900008));
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3900014));
            }
        }

        private void OnItem_DarkClicked()
        {
            if (Sys_Bag.Instance.GetBattleUseItem(1).Count == 0 && Sys_Bag.Instance.GetBattleUseItem(2).Count == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3900009));
            }
            else
            {
                if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForSecondOperation && GameCenter.mainFightPet != null)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(359999993));
                }
            }
        }

        private void OnSparSkill_ButtonClicked()
        {
            if ((GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForFirstOperation || GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForSecondOperation) && GameCenter.fightControl.CanUseSkill)
            {
                UIManager.OpenUI(EUIID.UI_MainBattle_SparSkills);
            }
        }

        private void OnSeal_DarkClicked()
        {
            if (GameCenter.fightControl.HaveSealedPet() == 0)
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009712));
            else
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3900011));
        }

        private void OnSeal_ButtonClicked()
        {
            if ((GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForFirstOperation || GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForSecondOperation) && GameCenter.fightControl.CanUseSkill)
            {
                UIManager.OpenUI(EUIID.UI_MainBattle_SealItem);
            }
        }

        private void OnEscape_DarkClicked()
        {
             Sys_Hint.Instance.PushContent_Normal( LanguageHelper.GetTextContent(359999995));
        }

        private void OnSkill_DarkClicked()
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3900005));
        }

        private void OnTransPosition_DarkClicked()
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3900005));
        }

        public void OnAttack_ButtonClicked()
        {
            GameCenter.fightControl.AttackById(Constants.NEARNORMALATTACKID, 0);
        }

        public void OnDefense_ButtonClicked()
        {
            GameCenter.fightControl.DoDefense();
        }

        public void OnGoods_ButtonClicked()
        {
            if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForSecondOperation && GameCenter.mainFightPet!=null)
            {
                UIManager.OpenUI(EUIID.UI_MainBattle_Good,false,false);
            }
            else
            {
                UIManager.OpenUI(EUIID.UI_MainBattle_Good, false, true);
            }
        }

        public void OnLast_Skill_ButtonClicked()
        {
            if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForSecondOperation && Sys_Fight.Instance.HasPet())
            {
                if (Net_Combat.Instance._lastPetSkillData != 0)
                    GameCenter.fightControl.AttackById(Net_Combat.Instance._lastPetSkillData, 0);
            }
            else
            {
                if (Net_Combat.Instance._lastRoleSkillData != 0)
                    GameCenter.fightControl.AttackById(Net_Combat.Instance._lastRoleSkillData, 0);
            }
        }

        public void OnPet_ButtonClicked()
        {
            Sys_Pet.Instance.OnGetPetInfoReq();
            UIManager.OpenUI(EUIID.UI_MainBattle_Pet);
        }

        public void OnRecall_ButtonClicked()
        {
            GameCenter.fightControl.currentOperationTpye = FightControl.EOperationType.Pet;
            GameCenter.fightControl.currentOperationID = 0;
            if (GameCenter.fightControl.CanUseSkill)
            {
                GameCenter.fightControl.DoSelectPet();
            }
        }

        public void OnRunaway_ButtonClicked()
        {
            BattleUnit mainFightPlayerBattleUnit = MobManager.Instance.GetPlayerBattleUnit();

            if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForFirstOperation)
            {
                GameCenter.fightControl.DoEscape();
            }
            else if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForSecondOperation || !Sys_Fight.Instance.HasPet())
            {
                GameCenter.fightControl.DoEscape();
            }
        }

        private void OnRetreat_ButtonClicked()
        {
            Net_Combat.Instance.SendCmdBattleCancelReq();
        }

        public void OnSkill_Role_ButtonClicked()
        {
            if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForFirstOperation)
            {
                UIManager.OpenUI(EUIID.UI_MainBattle_Skills);
            }
            else if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForSecondOperation)
            {
                if (Sys_Fight.Instance.HasPet())
                {
                    UIManager.OpenUI(EUIID.UI_MainBattle_Skills);
                }
                else
                {
                    if (GameCenter.fightControl.CanUseSkill)
                        UIManager.OpenUI(EUIID.UI_MainBattle_Skills);
                }
            }
        }

        public void OnTransposition_ButtonClicked()
        {

            GameCenter.fightControl.DoTransposition();
        }
    }

    #endregion

    public class UI_MainBattle : UIBase, UI_MainBattle_Layout.IListener
    {
        private UI_MainBattle_Layout layout = new UI_MainBattle_Layout();
        private UI_MainBattle_Right_Menu Right_Menu;
        private UI_MainBattle_Icon Icon;
        private UI_MainBattle_AutoBattle Auto_Battle;
        private UI_Fast_FrightingClick Fast_FrightingClick;
        private UI_BulletChat Bullet_View;

        private Timer timerauto;
        private Timer timefx;
        private Timer timeOnShowRefresh;
        private Timer timerAutoRound;
        private Timer timerRoundInFamilyBattle;
        private Timer timerStageInFamilyBattle;
        private Timer timerTrialGateFx;

        private Animator animator;
        private Animator animatorRound;
        private uint round = 0;
        private bool isRefreshAutoSkillShow = false;

        private TouchInput touchInput;
        float longpresstime;
        private uint bossmaxhp = 0;
        private float bosscurhp = 1;
        private int bosstargethp = 0;
        private bool bosshpischange = false;
        private bool isGuide = false;
        private List<uint> bufflist;
        private bool hasRecievedOnRoundStartNtf = false;
        private PetUnit petuint;
        private List<uint> petskills = new List<uint>();

        private RawImage _uiBlackRawImg;
        private int _originSortingOrder;

        private AsyncOperationHandle<GameObject> mHandle;
        private CSVBattleType.Data csvBattleTypeData;
        private uint curStage;
        private UI_TrialGate_Icon TrialGateItem;
        private bool isBoom;

        protected override void OnLoaded()
        {
            touchInput = transform.gameObject.AddComponent<TouchInput>();
            float.TryParse(CSVParam.Instance.GetConfData(501).str_value, out longpresstime);
            touchInput.interval = longpresstime / 1000;
            layout.Init(transform);
            layout.RegisterEvents(this);
            Right_Menu = AddComponent<UI_MainBattle_Right_Menu>(layout.View_Right_Menu.transform);
            Icon = AddComponent<UI_MainBattle_Icon>(layout.View_Icon.transform);
            Auto_Battle = AddComponent<UI_MainBattle_AutoBattle>(layout.View_Auto_Battle.transform);
            Fast_FrightingClick = AddComponent<UI_Fast_FrightingClick>(layout.m_FastCommand);
            Bullet_View = AddComponent<UI_BulletChat>(layout.Go_BulletView.transform);
            animator = layout.View_Right_Menu.GetComponent<Animator>();
            animatorRound = layout.Text_Round.GetComponent<Animator>();

            bufflist = new List<uint>();

            _originSortingOrder = nSortingOrder;
            canvas.sortingOrder = _originSortingOrder+1;
        }

        protected override void OnShow()
        {
            if (GameCenter.fightControl == null || CombatManager.Instance.m_BattleTypeTb == null)
            {
                return;
            }

            GameCenter.fightControl.CheckShowSelect(false);
            layout.Button_Menu_Close.gameObject.SetActive(!UIManager.IsOpen(EUIID.UI_FunctionMenu));     
            layout.Go_Revoke.SetActive(false);
            SetTimeShow(false);
            Right_Menu.Go_Retreat.SetActive(false);
            animator.enabled = true;
            OnUpdateRound();
            SetExp();
            Icon.SetValue();
            RefreshRealFightViewShow();
            SetOnShowRefresh();
        }

        private void RefreshWorldBoss(CSVBattleType.Data csvBattleType) {
            if (Net_Combat.Instance.m_IsWatchBattle) {
                return;
            }

            bool showCamp = false;
            if (csvBattleType != null)
            {
                showCamp = csvBattleType.show_boss_id != 0;
            }

            this.layout.btnCamp.gameObject.SetActive(false);
            if (showCamp) {
                this.layout.btnCamp.gameObject.SetActive(true);
                var csvBoss = CSVBOSSInformation.Instance.GetConfData(csvBattleType.show_boss_id);
                if (csvBoss != null && Sys_WorldBoss.Instance.unlockedBossManuales.TryGetValue(csvBoss.bossManual_id, out var manual)) {
                    this.layout.btnCampRed.SetActive(manual.HasRewardUnGot);
                }
            }
        }

        private void RefreshHangupBtns() {
            if (Net_Combat.Instance.m_IsWatchBattle) {
                layout.Button_StopHangup.gameObject.SetActive(false);
            }
            else {
                layout.Button_StopHangup.gameObject.SetActive(Sys_Pet.Instance.clientStateId == Sys_Role.EClientState.Hangup);
            }
        }

        private void OnPatrolStateChange(int oldV, int newV) {
            RefreshHangupBtns();
        }

        protected override void OnUpdate()
        {
            OnUpdateSpecialBossHp();
            CheckShowRoundLeftTime();
        }

        protected override void OnHide()
        {
            DebugUtil.LogFormat(ELogType.eCombat, "UI_MainBattle is Hide！！！");
            hasRecievedOnRoundStartNtf = false;
            SetTipsShow(0);
            layout.Text_Round.text = string.Empty;
            layout.Text_Round.gameObject.SetActive(false);
            SetTimePosition(false);
            layout.View_CloseShow.SetActive(false);
            layout.Go_RoundShow.SetActive(false);
            layout.Go_BossHp.SetActive(false);
            timerauto?.Cancel();
            timeOnShowRefresh?.Cancel();
            timefx?.Cancel();
            timerAutoRound?.Cancel();
            timerRoundInFamilyBattle?.Cancel();
            timerStageInFamilyBattle?.Cancel();
            timerTrialGateFx?.Cancel();
            isGuide = false;
            Fast_FrightingClick.Hide();
            Bullet_View.Hide();
            if (GameCenter.fightControl != null)
            {
                if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForFirstChoose)
                    GameCenter.fightControl.operationState = FightControl.EOperationState.WaitForFirstOperation;
                if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForSecondChoose)
                    GameCenter.fightControl.operationState = FightControl.EOperationState.WaitForSecondOperation;
            }
            if (TrialGateItem != null)
            {
                TrialGateItem.OnDestroy();
                TrialGateItem = null;
            }
            AddressablesUtil.ReleaseInstance(ref mHandle, MHandle_Completed);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Input.Instance.TouchInputRegister(touchInput, toRegister);
            Net_Combat.Instance.eventEmitter.Handle(Net_Combat.EEvents.OnRoundNtf, OnRoundNtf, toRegister);
            Net_Combat.Instance.eventEmitter.Handle(Net_Combat.EEvents.OnFirstOperationOver, OnFirstOperationOver, toRegister);
            Net_Combat.Instance.eventEmitter.Handle(Net_Combat.EEvents.OnSecondOperationOver, OnSecondOperationOver, toRegister);
            Net_Combat.Instance.eventEmitter.Handle<bool>(Net_Combat.EEvents.OnCloseShowOff, OnCloseShowOff, toRegister);
            Net_Combat.Instance.eventEmitter.Handle<ChooseEvt>(Net_Combat.EEvents.OnCloseShowOn, OnCloseShowOn, toRegister);
            Net_Combat.Instance.eventEmitter.Handle<bool>(Net_Combat.EEvents.OnDisableSkillBtn, OnDisableSkillBtn, toRegister);
            Net_Combat.Instance.eventEmitter.Handle<HpValueUpdateEvt>(Net_Combat.EEvents.OnUpdateHp, OnUpdateHp, toRegister);
            Net_Combat.Instance.eventEmitter.Handle<MpValueUpdateEvt>(Net_Combat.EEvents.OnUpdateMp, OnUpdateMp, toRegister);
            Net_Combat.Instance.eventEmitter.Handle<bool>(Net_Combat.EEvents.OnAutoFight, OnAutoFight, toRegister);
            Net_Combat.Instance.eventEmitter.Handle<AutoBattleSkillEvt>(Net_Combat.EEvents.OnSetAutoSkill, OnSetAutoSkill, toRegister);
            Net_Combat.Instance.eventEmitter.Handle(Net_Combat.EEvents.OnReconnect, OnReconnect, toRegister);
            Net_Combat.Instance.eventEmitter.Handle<uint>(Net_Combat.EEvents.OnWaitOthersCommands, OnWaitOthersCommands, toRegister);
            Net_Combat.Instance.eventEmitter.Handle(Net_Combat.EEvents.OnDoRound, OnDoRound, toRegister);
            Net_Combat.Instance.eventEmitter.Handle(Net_Combat.EEvents.OnRemoveBattleUI, OnRemoveBattleUI, toRegister);
            Net_Combat.Instance.eventEmitter.Handle(Net_Combat.EEvents.OnBackeBattleUI, OnBackeBattleUI, toRegister);
            Net_Combat.Instance.eventEmitter.Handle<bool, int>(Net_Combat.EEvents.OnUpdatePet, OnUpdatePet, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<BuffHuDUpdateEvt>(Sys_HUD.EEvents.OnUpdateBuffHUD, OnUpdateBossBuff, toRegister);
            Sys_Guide.Instance.eventEmitter.Handle(Sys_Guide.EEvents.OnFightForceGuide, OnFightForceGuide, toRegister);
            Net_Combat.Instance.eventEmitter.Handle(Net_Combat.EEvents.OnCombatOperateOver, OnCombatOperateOver, toRegister);
            Net_Combat.Instance.eventEmitter.Handle(Net_Combat.EEvents.OnCommandOperateStart, OnCommandOperateStart, toRegister);
            Net_Combat.Instance.eventEmitter.Handle<bool>(Net_Combat.EEvents.OnCommandChoose, OnCommandState, toRegister);
            Net_Combat.Instance.eventEmitter.Handle<uint, uint>(Net_Combat.EEvents.OnCommandIsOk, OnCommandIsOk, toRegister);
            Net_Combat.Instance.eventEmitter.Handle(Net_Combat.EEvents.OnFunctionMenuClose, OnFunctionMenuClose, toRegister);
            Net_Combat.Instance.eventEmitter.Handle<bool, int>(Net_Combat.EEvents.OnUIBlackSrceen, OnUIBlackSrceen, toRegister);
            Net_Combat.Instance.eventEmitter.Handle<bool>(Net_Combat.EEvents.OnOpenAutoSkill, OnOpenAutoSkill, toRegister);
            Sys_WorldBoss.Instance.eventEmitter.Handle(Sys_WorldBoss.EEvents.OnRewardGot, OnRewardGot, toRegister);

            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, OnUpdateLevel, toRegister);
            Sys_Head.Instance.eventEmitter.Handle(Sys_Head.EEvents.OnUsingUpdate, OnUsingUpdate, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnAddExp, SetExp, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnExtraExp, SetExp, toRegister);

            Net_Combat.Instance.eventEmitter.Handle(Net_Combat.EEvents.OnLoadMobsOver, LoadMobsOver, toRegister);
            Sys_Role.Instance.eventEmitter.Handle(Sys_Role.EEvents.OnReName, OnReNameUpdate, toRegister);
            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.BeginEnter, BeginEnter, toRegister);
            Sys_Video.Instance.eventEmitter.Handle(Sys_Video.EEvents.OnChangeBulletState, OnChangeBulletState, toRegister);
            Sys_Video.Instance.eventEmitter.Handle(Sys_Video.EEvents.OnSendBulletSuccess, OnSendBulletSuccess, toRegister);

            Auto_Battle.ProcessEventsRegiste(toRegister);
            Bullet_View.ProcessEventsRegiste(toRegister);
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Net_Combat.Instance.eventEmitter.Handle<BattleUnit, int, uint>(Net_Combat.EEvents.OnUpdateBossBlood, OnUpdateBossBlood, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle<int, int>(Sys_Pet.EEvents.OnPatrolStateChange, OnPatrolStateChange, toRegister);
            Net_Combat.Instance.eventEmitter.Handle<uint>(Net_Combat.EEvents.OnChangeBattleStage, OnChangeBattleStage, toRegister);
        }

        #region  CallBack

        void OnRoundNtf()
        {
            DebugUtil.Log(ELogType.eCombat, "OnRoundNtf");
            RefreshRealFightViewShow(true);
            Refresh();
            SetTime();
            SetSparSkillShow(true);
        }

        void OnAutoFight(bool auto)
        {
            if (!Net_Combat.Instance.m_IsExecuteState)
            {
                GameCenter.fightControl.operationState = FightControl.EOperationState.WaitForFirstOperation;
            }
            RefreshShow(true);
        }

        void OnSetAutoSkill(AutoBattleSkillEvt skillevt)
        {
            UIManager.CloseUI(EUIID.UI_MainBattle_Skills);
            Auto_Battle.RefreshShow(skillevt, true);
            if (!UIManager.IsOpen(EUIID.UI_FunctionMenu))
            {
                layout.Button_Menu_Close.gameObject.SetActive(true);
            }
        }

        void OnFirstOperationOver()
        {
            if (Sys_Fight.Instance.HasPet())
            {
                SetTipsShow(3000003);
                Right_Menu.Go_Escape.SetActive(false);
                Right_Menu.Go_Recall.SetActive(false);
                Right_Menu.Go_Seal.SetActive(false);
                Right_Menu.Go_Pet.SetActive(false);
                Right_Menu.Go_SparSkill.SetActive(false);
                Right_Menu.Go_Transposition.SetActive(false);
                Right_Menu.Go_Retreat.SetActive(false);              
                SetPetSkillIcon();
                SetArrowShow(false, true);
                if  ((Net_Combat.Instance.m_TeachingID==0&& Sys_Bag.Instance.GetBattleUseItem(1).Count == 0 && Sys_Bag.Instance.GetBattleUseItem(2).Count == 0)
                    ||Net_Combat.Instance.m_TeachingID!=0&&GameCenter.mainFightHero.battleUnit.Iteminfo.Count!=0)
                {
                    Right_Menu.Button_Goods.enabled = false;
                    Right_Menu.Item_Dark.SetActive(true);
                }
                CheckPetGoodUse();
            }
            else
            {
                SetTipsShow(3000002);
                Right_Menu.Go_Skill_Pet.SetActive(false);
                if (CombatManager.Instance.m_BattleTypeTb.is_escape == 0)
                {
                    Right_Menu.Escape_Dark.SetActive(true);
                    Right_Menu.Retreat_Dark.SetActive(true);
                }
            }
            if (GameCenter.fightControl.IsManyPeopleFight() && CombatManager.Instance.m_BattleTypeTb.is_order_cancel && !GameCenter.fightControl.isDoRound)
            {
                layout.Go_Revoke.SetActive(true);
            }
            else
            {
                layout.Go_Revoke.SetActive(false);
            }           
            layout.View_Right_Menu.SetActive(true);
            layout.Animator_Tips.Play("UI_MainBattle_View_Time_Text_Tips", -1, 0);
        }

        void OnSecondOperationOver()
        {
            layout.View_Right_Menu.SetActive(false);
            SetArrowShow(false, false);
            if (GameCenter.fightControl.IsManyPeopleFight())
            {
                SetTimePosition(true);
                layout.Go_Revoke.SetActive(CombatManager.Instance.m_BattleTypeTb.is_order_cancel);
            }
            else
            {
                SetTimeShow(false);
            }
            layout.View_CloseShow.SetActive(false);
            Auto_Battle.autofightToggle.gameObject.SetActive(true);
            UIManager.CloseUI(EUIID.UI_Buff);
            Net_Combat.Instance.m_IsExecuteState = true;
        }

        private void OnDoRound()
        {
            hasRecievedOnRoundStartNtf = false;
            SetTimeShow(false);
            layout.View_Right_Menu.SetActive(false);
            layout.View_CloseShow.SetActive(false);
            UIManager.CloseUI(EUIID.UI_Buff);
            OnCloseShowOff(false);
            SetSparSkillShow(false);
            layout.Go_Revoke.SetActive(false);
            if (!UIManager.IsOpen(EUIID.UI_FunctionMenu))
            {
                layout.Button_Menu_Close.gameObject.SetActive(true);
            }
            UIManager.CloseUI(EUIID.UI_MainBattle_SealItem);
        }

        private void OnReconnect()
        {
            Refresh();
            OnUpdateRound();
            if (!Net_Combat.Instance.m_IsWatchBattle)
            {
                if (Sys_Fight.Instance.AutoFightData.AutoState)
                {
                    SetAutoTime();
                    SetTipsShow(3000005);
                }
                else
                {
                    SetTime();
                    SetTipsShow(3000001);
                }
                layout.Fix_Tips.SetActive(false);
                layout.Fix_Tips.SetActive(true);
                layout.Animator_Tips.Play("UI_MainBattle_View_Time_Text_Tips", -1, 0);
            }
            layout.Go_Revoke.SetActive(false);
            curStage = Net_Combat.Instance.m_CurServerBattleStage;
            SetTrialGateStageShow(curStage);
        }

        private void OnWaitOthersCommands(uint uintid)
        {
            if ((GameCenter.mainFightHero.battleUnit.UnitId == uintid || (Sys_Fight.Instance.HasPet() && GameCenter.mainFightPet.battleUnit.UnitId == uintid)) && GameCenter.fightControl.operationState == FightControl.EOperationState.OperationOver)
            {
                if (CombatManager.Instance.m_BattleTypeTb.battle_type == 2)
                {
                    SetWaitOthersTips();
                }
                else
                {
                    if (Sys_Fight.Instance.HasPet() && GameCenter.mainFightPet.battleUnit.UnitId == uintid)
                    {
                        if (GameCenter.fightControl.isWaitOtherOp(GameCenter.mainFightHero.battleUnit.UnitId))
                        {
                            SetWaitOthersTips();
                        }
                    }
                    else
                    {
                        if (GameCenter.fightControl.isWaitOtherOp(uintid))
                        {
                            SetWaitOthersTips();
                        }
                    }
                }
            }
        }

        private void OnUpdateHp(HpValueUpdateEvt obj)
        {
            if (obj.id == 0)
                Icon.roleLife.value = obj.ratio;
            else
                Icon.petLife.value = obj.ratio;
            if (GameCenter.mainFightPet != null && GameCenter.mainFightPet.battleUnit != null)
            {
                ImageHelper.SetImageGray(Icon.petIcon, GameCenter.mainFightPet.battleUnit.CurHp == 0);
            }
        }

        private void OnUpdateMp(MpValueUpdateEvt obj)
        {
            if (obj.id == 0)
                Icon.roleMagic.value = obj.ratio;
            else
                Icon.petMagic.value = obj.ratio;
        }

        private void OnDisableSkillBtn(bool canuse)
        {
            if (GameCenter.fightControl == null || GameCenter.mainFightHero == null)
                return;
            GameCenter.fightControl.CanUseSkill = canuse;
            Right_Menu.Button_Role_Skill.enabled = canuse;
            Right_Menu.Skill_Role_Dark.SetActive(!canuse);

            Right_Menu.Button_Seal.enabled = canuse;
            Right_Menu.Seal_Dark.SetActive(!canuse);

            Right_Menu.Button_Goods.enabled = canuse;
            Right_Menu.Item_Dark.SetActive(!canuse);

            Right_Menu.Button_Pet.enabled = canuse;
            Right_Menu.Pet_Dark.SetActive(!canuse);

            Right_Menu.Last_Skill_Dark.SetActive(!canuse);
            Right_Menu.Button_Last_Skill.enabled = canuse;

            Right_Menu.SparSkill_Dark.SetActive(!canuse);
            Right_Menu.Button_SparSkill.enabled = canuse;

            Right_Menu.TransPosition_Dark.SetActive(!canuse);
            Right_Menu.Button_Transposition.enabled = canuse;

            Right_Menu.Recall_Dark.SetActive(!canuse);
            Right_Menu.Button_Recall.enabled = canuse;
            Right_Menu.Last_Skill_Dark.SetActive(!canuse);
            Right_Menu.Button_Last_Skill.enabled = canuse;
            Right_Menu.Go_Item.SetActive(Sys_FunctionOpen.Instance.IsOpen(60107, false));
            if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForSecondOperation && Sys_Fight.Instance.HasPet())
            {
                Right_Menu.Button_Goods.enabled = true;
                Right_Menu.Item_Dark.SetActive(false);
                if (GameCenter.mainFightPet.battleUnit.CurHp > 0)
                {
                    Right_Menu.Recall_Dark.SetActive(true);
                    Right_Menu.Button_Recall.enabled = false;
                    Right_Menu.Last_Skill_Dark.SetActive(false);
                    Right_Menu.Button_Last_Skill.enabled = true;
                }
            }
            else if (!Sys_Fight.Instance.HasPet())
            {
                MobEntity mob = MobManager.Instance.GetMob(GameCenter.mainFightHero.battleUnit.UnitId);
                if (mob == null)
                {
                    return;
                }
                MobBuffComponent mobBuffComponent = mob.GetComponent<MobBuffComponent>();
                if (mobBuffComponent != null && mobBuffComponent.HaveBuff(7200011))
                {
                    Right_Menu.Recall_Dark.SetActive(!canuse);
                    Right_Menu.Button_Recall.enabled = canuse;
                    Right_Menu.Button_Pet.enabled = false;
                    Right_Menu.Pet_Dark.SetActive(true);
                }
                else
                {
                    Right_Menu.Recall_Dark.SetActive(true);
                    Right_Menu.Button_Recall.enabled = false;
                }
                Right_Menu.Last_Skill_Dark.SetActive(!canuse);
                Right_Menu.Button_Last_Skill.enabled = canuse;
            }     
        }

        private void OnRemoveBattleUI()
        {
            layout.Go_Mainbattle.transform.localPosition = new Vector3(0, -50000, 0);
        }

        private void OnBackeBattleUI()
        {
            layout.Go_Mainbattle.transform.localPosition = new Vector3(0, 0, 0);
        }

        private void OnCloseShowOn(ChooseEvt evt)
        {
            GameCenter.fightControl.CheckShowSelect(true, evt.type, evt.req, evt.isHero);
            layout.View_CloseShow.SetActive(true);
            layout.View_Right_Menu.SetActive(false);
            layout.Go_Revoke.SetActive(false);
            Auto_Battle.autofightToggle.gameObject.SetActive(false);
            layout.m_FastCommand.gameObject.SetActive(false);
            SetTipsShow(3000004);
            layout.Animator_Tips.Play("UI_MainBattle_View_Time_Text_Tips", -1, 0);
            layout.Text_Skill_Name.text = LanguageHelper.GetTextContent(CSVActiveSkillInfo.Instance.GetConfData(evt.id).name);
            if (evt.id == 1001)
            {
                if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForSecondChoose && Sys_Fight.Instance.HasPet())
                    ImageHelper.SetIcon(layout.Image_Icon, 992302);
                else
                    ImageHelper.SetIcon(layout.Image_Icon, 990077);
            }
            else
            {
                ImageHelper.SetIcon(layout.Image_Icon, CSVActiveSkillInfo.Instance.GetConfData(evt.id).icon);
            }
        }

        private void OnCloseShowOff(bool isSpecial)
        {
            if (!Net_Combat.Instance.IsRealCombat())
            {
                return;
            }
            if (!isSpecial && !GameCenter.fightControl.isCommanding)
            {
                GameCenter.fightControl.CheckShowSelect(false);
            }
            layout.View_CloseShow.SetActive(false);
            layout.View_Right_Menu.SetActive(!Sys_Fight.Instance.AutoFightData.AutoState);
            if (!Sys_Fight.Instance.AutoFightData.AutoState)
            {
                UIManager.CloseUI(EUIID.UI_MainBattle_Skills);
            }
            UIManager.CloseUI(EUIID.UI_MainBattle_SparSkills);
            UIManager.CloseUI(EUIID.UI_MainBattle_Pet);
            UIManager.CloseUI(EUIID.UI_MainBattle_PetDetail);
            UIManager.CloseUI(EUIID.UI_MainBattle_Good, false, false);
            if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForFirstOperation)
            {
                SetTipsShow(3000001);
            }
            else if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForSecondOperation)
            {
                if (Sys_Fight.Instance.HasPet())
                {
                    SetTipsShow(3000003);
                }
                else
                {
                    SetTipsShow(3000002);
                }
            }
            else if (GameCenter.fightControl.operationState == FightControl.EOperationState.OperationOver)
            {
                layout.View_Right_Menu.SetActive(false);
            }

            bool bCommand = Sys_Role_Info.Instance.CanMakeFightCommand();
           layout.m_FastCommand.gameObject.SetActive(!GameCenter.fightControl.isCommanding && bCommand);
            if (!GameCenter.fightControl.isCommanding)
            {
                if (bCommand)
                {
                    layout.SetCommandCancleActive(false);
                }
                Auto_Battle.autofightToggle.gameObject.SetActive(true);

            }
            else if (GameCenter.fightControl.isCommanding && GameCenter.fightControl.CommandingState == EFightCommandingState.Opreating)
            {
                Auto_Battle.autofightToggle.gameObject.SetActive(false);
            }
            if (layout.Animator_Tips.isActiveAndEnabled)
            {
                layout.Animator_Tips.Play("UI_MainBattle_View_Time_Text_Tips", -1, 0);
            }
        }

        private void OnUpdateBossBlood(BattleUnit battleUnit, int curHp, uint maxHp)
        {
            bool selfCamp = CombatHelp.IsSameCamp(battleUnit, GameCenter.mainFightHero.battleUnit);
            if (selfCamp)
                return;
            if (GameCenter.fightControl.BossUnitDic.ContainsKey(battleUnit.UnitId))
            {
                GameCenter.fightControl.BossUnitDic[battleUnit.UnitId].curHp = curHp;
                GameCenter.fightControl.BossUnitDic[battleUnit.UnitId].maxHp = maxHp;
            }
            else
            {
                if ((UnitType)battleUnit.UnitType == UnitType.Monster)
                {
                    BossHpUpdateEvt evt = new BossHpUpdateEvt();
                    evt.curHp = curHp;
                    evt.maxHp = maxHp;
                    GameCenter.fightControl.BossUnitDic.Add(battleUnit.UnitId, evt);
                }
            }
            ShowBossHp();
        }

        private void ShowBossHp()
        {
            bosstargethp = 0;
            bossmaxhp = 0;
            foreach (var data in GameCenter.fightControl.BossUnitDic)
            {
                if (data.Value.curHp > 0)
                {
                    bosstargethp += data.Value.curHp;
                }
                bossmaxhp += data.Value.maxHp;
            }
            bosshpischange = true;
            if (bosstargethp >= 0 && bossmaxhp > 0)
                layout.Slider_BossHp.value = (float)bosstargethp / bossmaxhp;
        }

        private void OnUpdateBossBuff(BuffHuDUpdateEvt evt)
        {
            if (CombatManager.Instance.m_BattleTypeTb == null || !CombatManager.Instance.m_BattleTypeTb.show_UI_hp)
                return;
            if (CSVBuff.Instance.GetConfData(evt.buffid).is_show == 0 || evt.buffid==101)
                return;
            if (bufflist.Contains(evt.buffid))
            {
                if (!evt.add)
                {
                    bufflist.Remove(evt.buffid);
                }
            }
            else
            {
                if (evt.add)
                {
                    bufflist.Add(evt.buffid);
                }
            }
            RefreshBuffIcon();
        }

        private void OnFightForceGuide()
        {
            isGuide = true;
        }

        private void RefreshBuffIcon()
        {
            DefaultBuffItem();
            int count = bufflist.Count;
            Transform trans = layout.Go_BossBuff.transform;
            FrameworkTool.CreateChildList(trans, count);
            for (int i = 0; i < count; i++)
            {
                uint buffId = bufflist[i];
                trans.GetChild(i).gameObject.SetActive(true);
                ImageHelper.SetIcon(trans.GetChild(i).gameObject.GetComponent<Image>(), CSVBuff.Instance.GetConfData(buffId).hud_icon);
            }

        }

        private void DefaultBuffItem()
        {
            FrameworkTool.DestroyChildren(layout.Go_BossBuff, layout.Go_BossBuff.transform.GetChild(0).name);
        }

        private void OnUpdatePet(bool isAdd, int petId)
        {
            if (Net_Combat.Instance.m_IsWatchBattle)
            {
                return;
            }
            Icon.OnUpdatePet(isAdd, petId);
            AutoBattleSkillEvt evt = new AutoBattleSkillEvt();
            if (isAdd && GameCenter.mainFightPet != null)
            {
                GameCenter.fightControl.autoSkillEvt.petid =211;
                evt.petid = 211;
            }
            else
            {
                if (GameCenter.fightControl.autoSkillEvt.heroid2 == 0)
                {
                    if (GameCenter.mainFightHero.battleUnit.AutoSkillId.Contains(GameCenter.mainFightHero.battleUnit.AutoSkillId[1]))
                    {
                        evt.heroid2 = (uint)GameCenter.mainFightHero.battleUnit.AutoSkillId[1];
                    }
                }
                else
                {
                    evt.heroid2 = GameCenter.fightControl.autoSkillEvt.heroid2;
                }
            }
            Auto_Battle.RefreshShow(evt, true);
        }


        private void OnCombatOperateOver()
        {
            if (GameCenter.fightControl == null)
            {
                return;
            }
            GameCenter.fightControl.isCommanding = false;
            GameCenter.fightControl.CommandingState = EFightCommandingState.Over;
            Auto_Battle.autofightToggle.gameObject.SetActive(true);
            layout.m_FastCommand.gameObject.SetActive(Sys_Role_Info.Instance.CanMakeFightCommand());
            if (Sys_Fight.Instance.AutoFightData.AutoState)
            {
                layout.View_Right_Menu.SetActive(false);
                if (GameCenter.fightControl.operationState == FightControl.EOperationState.OperationOver)
                {
                    SetTipsShow(0);
                }
                else
                {
                    SetTipsShow(3000005);
                }
            }
            else
            {
                layout.View_Right_Menu.SetActive(true);
                if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForFirstOperation)
                {
                    SetTipsShow(3000001);
                }
                else if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForSecondOperation)
                {
                    if (Sys_Fight.Instance.HasPet())
                    {
                        SetTipsShow(3000003);
                    }
                    else
                    {
                        SetTipsShow(3000002);
                    }
                }
                else if (GameCenter.fightControl.operationState == FightControl.EOperationState.OperationOver)
                {
                    layout.View_Right_Menu.SetActive(false);
                    SetTipsShow(0);
                }
            }
        }

        private void OnCommandOperateStart()
        {
            layout.View_Right_Menu.SetActive(false);
            Auto_Battle.autofightToggle.gameObject.SetActive(false);
            layout.m_FastCommand.gameObject.SetActive(false);
            if (GameCenter.fightControl == null)
            {
                return;
            }
            SetTipsShow(3000014);
            GameCenter.fightControl.isCommanding = true;
            GameCenter.fightControl.CommandingState = EFightCommandingState.Opreating;
        }

        private void OnCommandState(bool state)
        {
            // layout.SetCommandActive(!state);
            layout.SetCommandCancleActive(state);

            if (GameCenter.fightControl != null)
                GameCenter.fightControl.isCommanding = state;

            if (!state)
            {
                Auto_Battle.autofightToggle.gameObject.SetActive(true);

                if (!Sys_Fight.Instance.AutoFightData.AutoState && !Net_Combat.Instance.m_IsExecuteState)
                {
                    layout.View_Right_Menu.SetActive(true);
                }
            }
        }

        private void OnCommandIsOk(uint uniteId, uint isOk)
        {
            DebugUtil.LogFormat(ELogType.eBattleCommand, "CommandIsOk Reply:  unitId:{0},isOk:{1}", uniteId, isOk);
            if (uniteId == GameCenter.mainFightHero.battleUnit.UnitId)
            {
                if (isOk == 0)
                {
                    layout.View_Right_Menu.SetActive(true);
                    layout.Go_Revoke.SetActive(false);
                    GameCenter.fightControl.operationState = FightControl.EOperationState.WaitForFirstOperation;
                    RefreshShow();
                }
            }
        }

        private void OnOpenAutoSkill(bool isOpen)
        {
            if (Sys_Fight.Instance.AutoFightData.AutoState)
            {
                layout.Button_Menu_Close.gameObject.SetActive(!isOpen);
            }
            else
            {
                if (isOpen)
                {
                    layout.Button_Menu_Close.gameObject.SetActive(false);
                }
                else
                {
                    if (GameCenter.fightControl.operationState == FightControl.EOperationState.OperationOver)
                    {
                        layout.Button_Menu_Close.gameObject.SetActive(true);
                    }
                    else
                    {
                        layout.Button_Menu_Close.gameObject.SetActive(false);
                    }
                }
            }
            UIManager.CloseUI(EUIID.UI_FunctionMenu);
        }

        private void OnRewardGot()
        {
            CSVBattleType.Data csvBattleType = CSVBattleType.Instance.GetConfData(Sys_Fight.Instance.BattleTypeId);
            if (csvBattleType == null) {
                return;
            }

            RefreshWorldBoss(csvBattleType);
        }

        private void OnFunctionMenuClose()
        {
            layout.Button_Menu_Close.gameObject.SetActive(true);
        }

        private void OnUIBlackSrceen(bool isShow, int alpha)
        {
            if (!isShow)
            {
                SetSortingOrder(_originSortingOrder);

                if (_uiBlackRawImg != null)
                    _uiBlackRawImg.gameObject.SetActive(false);

                return;
            }

            if (_uiBlackRawImg == null)
            {
                GameObject rawImgGo = new GameObject();
                _uiBlackRawImg = rawImgGo.AddComponent<RawImage>();
                rawImgGo.transform.SetParent(transform);
                RectTransform rawImgTrans = rawImgGo.transform as RectTransform;
                rawImgTrans.localPosition = Vector3.zero;
                rawImgTrans.localScale = Vector3.one;
                rawImgTrans.Setlayer(ELayerMask.UI, true);
                rawImgTrans.sizeDelta = new Vector2(100000f, 100000f);

                _uiBlackRawImg.color = Color.black;

                if (_originSortingOrder != nSortingOrder)
                {
                    DebugUtil.LogError($"OnUIBlackSrceen    _originSortingOrder:{_originSortingOrder.ToString()}    nSortingOrder:{nSortingOrder.ToString()}");
                    _originSortingOrder = nSortingOrder;
                }
            }
            else
                _uiBlackRawImg.gameObject.SetActive(true);

            Color color = _uiBlackRawImg.color;
            _uiBlackRawImg.color = new Color(color.r, color.g, color.b, alpha * 0.01f);

            SetSortingOrder(_originSortingOrder * 1000);
        }

        private void OnUpdateLevel()
        {
            Icon.roleLv.text = Sys_Role.Instance.Role.Level.ToString();
        }
        private void OnReNameUpdate()
        {
            Icon.roleName.text= Sys_Role.Instance.Role.Name.ToStringUtf8();
        }

        private void OnUsingUpdate()
        {
            Sys_Head.Instance.SetHeadAndFrameData(Icon.roleIcon);
        }

        private void BeginEnter(uint stack, int id)
        {
            EUIID eId = (EUIID)id;
            if (eId == EUIID.UI_Chat&&UIManager.IsOpen(EUIID.UI_Sequence))
            {
                UIManager.CloseUI(EUIID.UI_Sequence);
            }
        }

        private void OnChangeBulletState()
        {
            layout.Go_Bullet_Close.gameObject.SetActive(!Sys_Video.Instance.isOpenBullet);
            layout.Go_Bullet_Open.gameObject.SetActive(Sys_Video.Instance.isOpenBullet);
            Bullet_View.SetState(Sys_Video.Instance.isOpenBullet);
            layout.Go_BulletView.SetActive(Sys_Video.Instance.isOpenBullet);
        }
        
        private void OnChangeBattleStage(uint stage)
        {
            curStage = stage;
            SetTrialGateStageShow(stage);
        }

        private void OnSendBulletSuccess()
        {
            Sys_Video.Instance.eventEmitter.Trigger<string>(Sys_Video.EEvents.OnPlaySelfBullet, layout.InputField.text);
            layout.InputField.text = string.Empty;
            layout.InputField.placeholder.enabled = true;
            layout.Go_BulletChat.SetActive(false);
            layout.Go_Video.SetActive(true);
            UIManager.OpenUI(EUIID.UI_ChatSimplify);
        }

        #endregion

        #region Function

        private void CheckShowRoundLeftTime()
        {
            uint leftTime = Net_Combat.Instance.RoundCountDown;
            if (leftTime > 0)
            {
                uint elapseTime = Net_Combat.Instance.CountDownValue - leftTime;

                if (GameCenter.fightControl != null && layout.Text_Time != null)
                {
                    layout.Text_Time.text = Math.Ceiling((double)leftTime).ToString();
                    Auto_Battle.passtime = leftTime;
                }

                if (Sys_Role.Instance.Role.Level < int.Parse(CSVParam.Instance.GetConfData(647).str_value) && !isGuide)
                {
                    if (elapseTime >= int.Parse(CSVParam.Instance.GetConfData(646).str_value) / 1000)
                    {
                        if (Right_Menu.Go_Last_Skill.activeInHierarchy && Right_Menu.Button_Last_Skill.enabled)
                        {
                            Right_Menu.Go_Last_Skill_Light.SetActive(true);
                            Right_Menu.Go_Attack_Light.SetActive(false);
                        }
                        else
                        {
                            Right_Menu.Go_Attack_Light.SetActive(true);
                            Right_Menu.Go_Last_Skill_Light.SetActive(false);
                        }
                    }
                }
            }
            else
            {
                if (Net_Combat.Instance.m_IsExecuteState)
                {
                    return;
                }
                if (GameCenter.fightControl != null)
                {
                    SetTimeShow(false);
                    layout.View_Right_Menu.SetActive(false);
                    OnCloseShowOff(false);
                    UIManager.CloseUI(EUIID.UI_Buff);
                    Auto_Battle.passtime = Net_Combat.Instance.CountDownValue;
                    Net_Combat.Instance.m_IsExecuteState = true;
                }
            }
        }

        private void SetArrowShow(bool isHeroShow, bool isPetShow)
        {
            foreach (var v in MobManager.Instance.m_MobDic)
            {
                if (v.Value.m_MobCombatComponent.m_BattleUnit.RoleId == Sys_Role.Instance.Role.RoleId)
                {
                    if (Sys_Fight.Instance.HasPet() && v.Value.m_MobCombatComponent.m_BattleUnit.UnitType == (uint)UnitType.Pet)
                    {
                        UpdateArrowEvt evt = new UpdateArrowEvt();
                        evt.id = v.Key;
                        evt.active = isPetShow;
                        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnUpdateArrow, evt);
                    }
                    else if (v.Value.m_MobCombatComponent.m_BattleUnit.UnitType == (uint)UnitType.Hero)
                    {
                        UpdateArrowEvt evt = new UpdateArrowEvt();
                        evt.id = v.Key;
                        evt.active = isHeroShow;
                        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnUpdateArrow, evt);
                    }
                }
                else
                {
                    UpdateArrowEvt evt = new UpdateArrowEvt();
                    evt.id = v.Key;
                    evt.active = false;
                    Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnUpdateArrow, evt);
                }
            }
        }

        private void SetSparSkillShow(bool isShow)
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(60110, false))
            {
                return;
            }
            foreach (var v in MobManager.Instance.m_MobDic)
            {
                if (v.Value.m_MobCombatComponent.m_BattleUnit.RoleId == Sys_Role.Instance.Role.RoleId && v.Value.m_MobCombatComponent.m_BattleUnit.UnitType == (uint)UnitType.Hero)
                {
                    UpdateSparSkillEvt evt = new UpdateSparSkillEvt();
                    evt.id = v.Key;
                    evt.active = isShow;
                    Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnUpdateSparSkill, evt);
                }
                else
                {
                    UpdateSparSkillEvt evt = new UpdateSparSkillEvt();
                    evt.id = v.Key;
                    evt.active = false;
                    Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnUpdateSparSkill, evt);
                }
            }
        }

        private void SetTimeSand()
        {
            if (Net_Combat.Instance.m_IsVideo)
            {
                return;
            }
            foreach (var v in MobManager.Instance.m_MobDic)
            {
                Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnUpdateTimeSand, v.Key, v.Value.m_MobCombatComponent.m_BattleUnit.UnitType != (uint)UnitType.Monster);
            }
        }

        private void SetSpecialShow()
        {
            if (CombatManager.Instance.m_BattleTypeTb == null)
                return;
            if (CombatManager.Instance.m_BattleTypeTb.show_UI_hp)
            {
                layout.Go_BossHp.SetActive(true);
                uint id = Sys_Fight.Instance.GetBossUnitInfoId();
                layout.Text_BossName.text = LanguageHelper.GetTextContent(CSVMonster.Instance.GetConfData(id).monster_name);
                foreach (var mob in MobManager.Instance.m_MobDic)
                {
                    BattleUnit battleUnit = mob.Value.m_MobCombatComponent.m_BattleUnit;
                    bool selfCamp = CombatHelp.IsSameCamp(battleUnit, GameCenter.mainFightHero.battleUnit);         
                    if (!selfCamp && !GameCenter.fightControl.BossUnitDic.ContainsKey(battleUnit.UnitId) && (UnitType)battleUnit.UnitType == UnitType.Monster)
                    {
                        BossHpUpdateEvt evt = new BossHpUpdateEvt();
                        evt.curHp = battleUnit.CurHp;
                        evt.maxHp = battleUnit.MaxHp;
                        GameCenter.fightControl.BossUnitDic.Add(battleUnit.UnitId, evt);
                    }
                }
                ShowBossHp();
            }
            else
            {
                layout.Go_BossHp.SetActive(false);
            }
            bufflist.Clear();
            layout.Image_BossBuff.gameObject.SetActive(false);
        }

        private void SetRoleSkillIcon()
        {
            Right_Menu.Go_Skill_Role.SetActive(Sys_FunctionOpen.Instance.IsOpen(60102, false));
            Right_Menu.Go_Skill_Pet.SetActive(false);
            ImageHelper.SetIcon(layout.Image_NormalAttack_Icon, 990077);
        }

        private void SetPetSkillIcon()
        {
            if (isBoom)
            {
                Right_Menu.Go_Skill_Pet.SetActive(false);
                return;
            }
            petskills.Clear();
            bool isOpen = Sys_FunctionOpen.Instance.IsOpen(60104, false);
            Right_Menu.Go_Skill_Pet.SetActive(isOpen);
            Right_Menu.Go_Skill_Role.SetActive(false);
            ImageHelper.SetIcon(layout.Image_NormalAttack_Icon, 992302);
            if (Sys_FunctionOpen.Instance.IsOpen(60102, false))
            {
                Right_Menu.Go_Last_Skill.SetActive(true);
                petskills = GameCenter.fightControl.GetFightPetSkills((uint)GameCenter.mainFightPet.battleUnit.PetId);
                MobEntity mobEntity = MobManager.Instance.GetMob(GameCenter.mainFightPet.battleUnit.UnitId);
                if (mobEntity == null)
                {
                    return;
                }
                MobBuffComponent mobBuffComponent = mobEntity.GetComponent<MobBuffComponent>();
                List<MobBuffComponent.BuffData> buffs = new List<MobBuffComponent.BuffData>();
                if (mobBuffComponent != null)
                {
                    buffs = mobBuffComponent.m_Buffs;
                }
                for (int i = petskills.Count - 1; i >= 0; i--)
                {

                    if (GameCenter.fightControl.CheckSkillLimitList(petskills[i], true,true, buffs).Count != 0)
                    {
                        petskills.Remove(petskills[i]);
                    }
                }
                if (petskills.Count == 0)
                {
                    Right_Menu.Go_Last_Skill.SetActive(false);
                    return;
                }
                petskills.Sort();
                if (Net_Combat.Instance._lastPetSkillData == 0 || (Net_Combat.Instance._lastPetSkillData != 0 && !petskills.Contains(Net_Combat.Instance._lastPetSkillData)))
                {
                    Net_Combat.Instance._lastPetSkillData = petskills[0];
                    CSVActiveSkillInfo.Data data = CSVActiveSkillInfo.Instance.GetConfData(petskills[0]);
                    ImageHelper.SetIcon(Right_Menu.Button_Last_Skill.image, data.icon);
                    Right_Menu.typeIcon.gameObject.SetActive(data.typeicon != 0);
                    if (data.typeicon != 0)
                    {
                        ImageHelper.SetIcon(Right_Menu.typeIcon, data.typeicon);
                    }
                }
                else
                {
                    CSVActiveSkillInfo.Data data = CSVActiveSkillInfo.Instance.GetConfData(Net_Combat.Instance._lastPetSkillData);
                    ImageHelper.SetIcon(Right_Menu.Button_Last_Skill.image, data.icon);
                    Right_Menu.typeIcon.gameObject.SetActive(data.typeicon != 0);
                    if (data.typeicon != 0)
                    {
                        ImageHelper.SetIcon(Right_Menu.typeIcon, data.typeicon);
                    }
                }
            }
            else
            {
                Right_Menu.Go_Last_Skill.SetActive(false);
            }
        }

        private void SetLastSkillIcon()
        {
            if (Sys_FunctionOpen.Instance.IsOpen(60102, false))
            {
                if (GameCenter.mainFightHero == null || GameCenter.mainFightHero.battleUnit == null)
                {
                    if (CombatManager.Instance.m_CombatStyleState != 999)
                        DebugUtil.LogError($"获取的GameCenter.mainFightHero或者GameCenter.mainFightHero.battleUnit为null");
                    return;
                }

                Right_Menu.Go_Last_Skill.SetActive(true);
                List<SkillComponent.SkillData> skilldic = new List<SkillComponent.SkillData>();
                skilldic = GameCenter.mainFightHero.heroSkillComponent.GetHoldingActiveSkillsExt((uint)GameCenter.mainFightHero.battleUnit.WeaponId);
                MobEntity mobEntity = MobManager.Instance.GetMob(GameCenter.mainFightHero.battleUnit.UnitId);
                if (mobEntity == null)
                {
                    return;
                }
                MobBuffComponent mobBuffComponent = mobEntity.GetComponent<MobBuffComponent>();
                List<MobBuffComponent.BuffData> buffs = new List<MobBuffComponent.BuffData>();
                if (mobBuffComponent != null)
                {
                    buffs = mobBuffComponent.m_Buffs;
                }
                for (int i = skilldic.Count - 1; i >= 0; i--)
                {
                    if (GameCenter.fightControl.CheckSkillLimitList(skilldic[i].CSVActiveSkillInfoData.id, skilldic[i].Available,false, buffs).Count != 0)
                    {
                        skilldic.Remove(skilldic[i]);
                    }
                }
                if (GameCenter.mainFightHero != null && skilldic.Count > 0)
                {
                    ImageHelper.SetIcon(Right_Menu.Button_Last_Skill.image, skilldic[0].CSVActiveSkillInfoData.icon);
                    Right_Menu.typeIcon.gameObject.SetActive(skilldic[0].CSVActiveSkillInfoData.typeicon != 0);
                    if (skilldic[0].CSVActiveSkillInfoData.typeicon != 0)
                    {
                        ImageHelper.SetIcon(Right_Menu.typeIcon, skilldic[0].CSVActiveSkillInfoData.typeicon);
                    }
                    SkillComponent.SkillData firstAvailableSkill = SetFirstAvailableSkill(skilldic);
                    if (Net_Combat.Instance._lastRoleSkillData == 0)
                    {
                        if (firstAvailableSkill == null)
                        {
                            Right_Menu.Go_Last_Skill.SetActive(false);
                        }
                        else
                        {
                            Net_Combat.Instance._lastRoleSkillData = firstAvailableSkill.CSVActiveSkillInfoData.id;
                            CSVActiveSkillInfo.Data data = CSVActiveSkillInfo.Instance.GetConfData(Net_Combat.Instance._lastRoleSkillData);
                            ImageHelper.SetIcon(Right_Menu.Button_Last_Skill.image, data.icon);
                            Right_Menu.typeIcon.gameObject.SetActive(data.typeicon != 0);
                            if (data.typeicon != 0)
                            {
                                ImageHelper.SetIcon(Right_Menu.typeIcon, data.typeicon);
                            }
                        }
                    }
                    else
                    {
                        uint lastMainId = CSVActiveSkill.Instance.GetConfData(Net_Combat.Instance._lastRoleSkillData).main_skill_id;
                        for (int i = 0; i < skilldic.Count; ++i)
                        {
                            uint mainId = CSVActiveSkill.Instance.GetConfData(skilldic[i].CSVActiveSkillInfoData.id).main_skill_id;
                            if (mainId == lastMainId && skilldic[i].CSVActiveSkillInfoData.quick_show)
                            {
                                Net_Combat.Instance._lastRoleSkillData = skilldic[i].CSVActiveSkillInfoData.id;
                                CSVActiveSkillInfo.Data data = CSVActiveSkillInfo.Instance.GetConfData(Net_Combat.Instance._lastRoleSkillData);
                                ImageHelper.SetIcon(Right_Menu.Button_Last_Skill.image, data.icon);
                                Right_Menu.typeIcon.gameObject.SetActive(data.typeicon != 0);
                                if (data.typeicon != 0)
                                {
                                    ImageHelper.SetIcon(Right_Menu.typeIcon, data.typeicon);
                                }
                                return;
                            }
                        }
                        if (firstAvailableSkill == null)
                        {
                            Net_Combat.Instance._lastRoleSkillData = 0;
                            Right_Menu.Go_Last_Skill.SetActive(false);
                        }
                        else
                        {
                            Net_Combat.Instance._lastRoleSkillData = firstAvailableSkill.CSVActiveSkillInfoData.id;
                            CSVActiveSkillInfo.Data data = CSVActiveSkillInfo.Instance.GetConfData(Net_Combat.Instance._lastRoleSkillData);
                            ImageHelper.SetIcon(Right_Menu.Button_Last_Skill.image, data.icon);
                            ImageHelper.SetIcon(Right_Menu.typeIcon, data.typeicon);
                        }
                    }
                }
                else
                {
                    Right_Menu.Go_Last_Skill.SetActive(false);
                    Net_Combat.Instance._lastRoleSkillData = 0;
                }
            }
        }

        private void SetExp()
        {
            CSVCharacterAttribute.Data data = CSVCharacterAttribute.Instance.GetConfData(Sys_Role.Instance.Role.Level + 1);
            if (Sys_Role.Instance.Role != null && data != null)
            {
                if (Sys_Role.Instance.Role.ExtraExp == 0)
                {
                    layout.exp.fillAmount = (float)Sys_Role.Instance.Role.Exp / data.upgrade_exp;
                }
                else
                {
                    layout.exp.fillAmount = 0;
                }
                uint.TryParse(CSVParam.Instance.GetConfData(968).str_value, out uint compensationExpPercent);
            }
        }

        //模型加载完
        private  void LoadMobsOver()
        {
            SetSealOrBoomShow();
        }

        private SkillComponent.SkillData SetFirstAvailableSkill(List<SkillComponent.SkillData> newdicob)
        {
            for (int i = 0; i < newdicob.Count; ++i)
            {
                if (newdicob[i].Available && newdicob[i].CSVActiveSkillInfoData.quick_show)
                {
                    return newdicob[i];
                }
            }
            return null;
        }

        private void Refresh()
        {
            if (Time.unscaledTime - CombatManager.Instance.m_CurRoundStartTime > 30f)
            {
                round = Net_Combat.Instance.m_CurRound;
                SetTimeShow(false);
                layout.View_Right_Menu.SetActive(false);
                if (!UIManager.IsOpen(EUIID.UI_FunctionMenu))
                {
                    layout.Button_Menu_Close.gameObject.SetActive(true);
                }
            }
            else
            {
                RefreshShow();
            }
            hasRecievedOnRoundStartNtf = true;
        }

        private void RefreshShow(bool isChangeAutoState = false)
        {
            timerAutoRound?.Cancel();
            OnUpdateRound();
            layout.Go_Revoke.SetActive(false);
            layout.Fix_Round.SetActive(true);
            timefx?.Cancel();
            timefx = Timer.Register(1f, () =>
            {
                layout.Fix_Round.SetActive(false);
            }, null, false, true);
            if (!Net_Combat.Instance.IsRealCombat())
            {
                SetTimeShow(false);
                layout.View_Right_Menu.SetActive(false);
                if (Net_Combat.Instance.m_IsWatchBattle)
                {
                    SetTimeSand();
                }
                layout.Button_Menu_Close.gameObject.SetActive(true);
            }
            else
            {
                if (Sys_Fight.Instance.AutoFightData.AutoState)
                {
                    layout.View_Right_Menu.SetActive(false);
                    SetAutoButtonShow();
                    if (Net_Combat.Instance.m_IsExecuteState)
                    {
                        if (isChangeAutoState)
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3900006));
                        }
                    }
                    else
                    {
                        SetTimeShow(true);
                        layout.Go_RoundShow.SetActive(false);
                        layout.Go_RoundShowNum.SetActive(!isChangeAutoState);
                        //精简模式非第一回合 隐藏自动战斗倒计时
                        if (CSVBattleType.Instance.GetConfData(Sys_Fight.Instance.BattleTypeId).auto_fight_mode == 2)
                        {
                            if (Net_Combat.Instance.m_CurRound != 1 && round != 0)
                            {
                                SetTimeShow(false);
                                layout.Go_RoundShow.SetActive(true);
                                timerAutoRound = Timer.Register(1f, () =>
                                {
                                    layout.Go_RoundShow.SetActive(false);
                                }, null, false, false);
                            }
                            else
                            {
                                SetTipsShow(3000005);
                            }
                        }
                        else
                        {
                            uint leftTime = Net_Combat.Instance.RoundCountDown;
                            if (leftTime <= 27 && round != 0)
                            {
                                SetTimeShow(false);
                                layout.Go_RoundShow.SetActive(true);
                                timerAutoRound = Timer.Register(1f, () =>
                                {
                                    layout.Go_RoundShow.SetActive(false);
                                }, null, false, false);
                            }
                            else
                            {
                                SetTipsShow(3000005);
                            }
                        }
                        SetAutoTimePosition(true);
                        SetTimePosition(false);
                        layout.Animator_Tips.Play("UI_MainBattle_View_Time_Text_Tips", -1, 0);
                        SetArrowShow(false, false);
                        SetTimeSand();
                        if (Auto_Battle.passtime > 3)
                        {
                            SetAutoTime();
                        }
                        else
                        {
                            SetTimePosition(true);
                            SetAutoTimePosition(false);
                        }
                        layout.Go_Revoke.SetActive(false);
                        layout.Text_Auto_Round.text = LanguageHelper.GetTextContent(3000008, Net_Combat.Instance.m_CurRound.ToString());

                    }
                    if (!UIManager.IsOpen(EUIID.UI_FunctionMenu))
                    {
                        layout.Button_Menu_Close.gameObject.SetActive(true);
                    }
                    Auto_Battle.autofightToggle.gameObject.SetActive(!GameCenter.fightControl.isCommanding);
                }
                else
                {
                    Auto_Battle.autofightOn.SetActive(false);
                    Auto_Battle.autofightOff.SetActive(true);
                    layout.Go_RoundShow.SetActive(false);
                    if (Net_Combat.Instance.m_IsExecuteState && GameCenter.fightControl.operationState != FightControl.EOperationState.WaitForFirstOperation)  //二动结束后撤销
                    {
                        layout.View_Right_Menu.SetActive(false);
                        UIManager.CloseUI(EUIID.UI_Buff);
                        layout.Button_Menu_Close.gameObject.SetActive(!UIManager.IsOpen(EUIID.UI_FunctionMenu));
                        if (GameCenter.fightControl.operationState == FightControl.EOperationState.OperationOver)
                        {
                            if (GameCenter.fightControl.IsManyPeopleFight()&& CombatManager.Instance.m_BattleTypeTb.is_order_cancel&&!GameCenter.fightControl.isDoRound)
                            {
                                layout.Go_Revoke.SetActive(true);
                            }
                            else
                            {
                                layout.Go_Revoke.SetActive(false);
                            }
                        }
                    }
                    else
                    {
                        UIManager.CloseUI(EUIID.UI_FunctionMenu);
                        layout.Button_Menu_Close.gameObject.SetActive(false);
                        Right_Menu.Escape_Dark.SetActive(false);
                        Right_Menu.Retreat_Dark.SetActive(false);
                        BattleUnit mainFightPlayerBattleUnit = MobManager.Instance.GetPlayerBattleUnit();                 
                        if (GameCenter.fightControl.isCommanding && GameCenter.fightControl.CommandingState == EFightCommandingState.Opreating)
                        {
                            layout.View_Right_Menu.SetActive(false);
                            Auto_Battle.autofightToggle.gameObject.SetActive(false);
                        }
                        else
                        {
                            layout.View_Right_Menu.SetActive(true);
                            Right_Menu.Go_Last_Skill_Light.SetActive(false);
                            Right_Menu.Go_Attack_Light.SetActive(false);
                            Auto_Battle.autofightToggle.gameObject.SetActive(true);
                        }
                        if (GameCenter.fightControl != null)
                        {
                            if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForFirstOperation)
                            {
                                GameCenter.fightControl.CanUseSkill = true;
                                SetTipsShow(3000001);
                                Right_Menu.Go_Recall.SetActive(Sys_FunctionOpen.Instance.IsOpen(60107, false));
                                Right_Menu.Go_Pet.SetActive(Sys_FunctionOpen.Instance.IsOpen(60106, false));
                                Right_Menu.Go_SparSkill.SetActive(Sys_FunctionOpen.Instance.IsOpen(60110, false));
                                Right_Menu.Go_Transposition.SetActive(Sys_FunctionOpen.Instance.IsOpen(60113, false));
                                Right_Menu.Go_Seal.SetActive(Sys_FunctionOpen.Instance.IsOpen(60114, false));
                                Right_Menu.Go_Skill_Pet.SetActive(false);
                                SetRoleSkillIcon();
                                SetLastSkillIcon();
                                SetArrowShow(true, false);
                                OnDisableSkillBtn(GameCenter.fightControl.CanUseSkill);
                                if (mainFightPlayerBattleUnit == null || mainFightPlayerBattleUnit.CurHp <= 0)  //主角死亡
                                {
                                    Right_Menu.Go_Retreat.SetActive(Sys_FunctionOpen.Instance.IsOpen(60108, false));
                                    Right_Menu.Go_Escape.SetActive(false);
                                }
                                else
                                {
                                    Right_Menu.Go_Retreat.SetActive(false);
                                    Right_Menu.Go_Escape.SetActive(Sys_FunctionOpen.Instance.IsOpen(60108, false));
                                }
                                if (CombatManager.Instance.m_BattleTypeTb.is_escape==0)
                                {
                                    Right_Menu.Escape_Dark.SetActive(true);
                                    Right_Menu.Retreat_Dark.SetActive(true);
                                }
                                if ((Net_Combat.Instance.m_TeachingID == 0 && Sys_Bag.Instance.GetBattleUseItem(1).Count == 0 && Sys_Bag.Instance.GetBattleUseItem(2).Count == 0)
    || (Net_Combat.Instance.m_TeachingID != 0 && GameCenter.mainFightHero.battleUnit.Iteminfo.Count == 0))
                                {
                                    Right_Menu.Button_Goods.enabled = false;
                                    Right_Menu.Item_Dark.SetActive(true);
                                }
                            }
                            else if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForSecondOperation)
                            {
                                OnFirstOperationOver();
                            }
                        }
                        SetSealOrBoomShow();
                        SetTimeSand();
                        timerauto?.Cancel();
                        SetTimeShow(true);
                        SetTimePosition(true);
                        SetAutoTimePosition(false);
                        layout.Animator_Tips.Play("UI_MainBattle_View_Time_Text_Tips", -1, 0);
                    }
                }
            }
        }

        private void SetSealOrBoomShow()
        {
            if(isBoom)  //年兽挑战
            {
                Right_Menu.Button_Seal.enabled = true;
                Right_Menu.Seal_Dark.SetActive(false);
                TextHelper.SetText(Right_Menu.Text_SealOrBoom, 2016117);
                // ImageHelper.SetIcon(Right_Menu.Image_SealOrBoom,);

                Right_Menu.Go_Item.SetActive(false);
                Right_Menu.Go_Last_Skill.SetActive(false);
                Right_Menu.Go_Attack.SetActive(false);
                Right_Menu.Go_Recall.SetActive(false);
                Right_Menu.Go_Skill_Pet.SetActive(false);
                Right_Menu.Go_Skill_Role.SetActive(false);
                Right_Menu.Go_SparSkill.SetActive(false);
                Right_Menu.Go_Transposition.SetActive(false);
            }
            else if (GameCenter.fightControl.HaveSealedPet() == 0)
            {
                Right_Menu.Button_Seal.enabled = false;
                Right_Menu.Seal_Dark.SetActive(true);
                TextHelper.SetText(Right_Menu.Text_SealOrBoom, 2012082);
                // ImageHelper.SetIcon(Right_Menu.Image_SealOrBoom,);
                Right_Menu.Go_Attack.SetActive(true);
            }
        }

        private void SetTrialGateStageShow(uint stage, bool isShow=true)
        {
            if (stage == 0)
            {
                DebugUtil.LogErrorFormat(" BattleStage:{0}", stage.ToString());
                return;
            }
            if (csvBattleTypeData.stageSwitch_id.Count >0 && csvBattleTypeData.stageSwitch_id.Count >= stage)
            {
                uint id = csvBattleTypeData.stageSwitch_id[(int)stage - 1];
                CSVSwitchCondition.Data csvSwitchConditionData = CSVSwitchCondition.Instance.GetConfData(id);
                if (csvSwitchConditionData == null|| !csvSwitchConditionData.isShowStage)
                {
                    layout.Button_TrialGate.gameObject.SetActive(false);
                    return;
                }
                layout.Button_TrialGate.gameObject.SetActive(true);
                TextHelper.SetText(layout.Text_Satge, csvSwitchConditionData.stageName);
                if (Net_Combat.Instance.m_CurRound == 0 || Net_Combat.Instance.m_CurRound == 1|| TrialGateItem==null)
                {
                    AddressablesUtil.InstantiateAsync(ref mHandle, csvSwitchConditionData.stagePrefabe, MHandle_Completed, true, layout.TrialGate_Root);
                }
                else
                {
                    TrialGateItem.PlayTransformFx();
                    timerTrialGateFx?.Cancel();
                    timerTrialGateFx = Timer.Register(0.3f, () =>
                    {
                        AddressablesUtil.InstantiateAsync(ref mHandle, csvSwitchConditionData.stagePrefabe, MHandle_Completed, true, layout.TrialGate_Root);
                        timerTrialGateFx.Cancel();
                    }, null, false, true);
                }
                if (UIManager.IsShowState(EUIID.UI_MainBattle)&& isShow)
                {
                    UIManager.OpenUI(EUIID.UI_TrialGate_StageTip, false, stage);
                }
            }
            else
            {
                layout.Button_TrialGate.gameObject.SetActive(false);
            }
        }

        private void MHandle_Completed(AsyncOperationHandle<GameObject> handle)
        {
            Transform trans = handle.Result.transform;
            if (Net_Combat.Instance.m_CurRound != 1 && TrialGateItem != null)
            {
                TrialGateItem.OnDestroy();
            }
            TrialGateItem = new UI_TrialGate_Icon();
            TrialGateItem.Init(trans);
            trans.localScale = Vector3.one;
        }

        private void SetOnShowRefresh()
        {
            timeOnShowRefresh?.Cancel();
            float onShowTime = 0;
            float.TryParse(CSVParam.Instance.GetConfData(135).str_value, out onShowTime);
            timeOnShowRefresh = Timer.Register(onShowTime / 1000, () =>
              {
                  if (!hasRecievedOnRoundStartNtf)
                  {
                      DebugUtil.Log(ELogType.eCombat, "OnShowRefresh");
                      Refresh();
                      timeOnShowRefresh.Cancel();
                  }
              }, null, false, true);
        }

        private void RefreshRealFightViewShow(bool isRefresh=false)
        {
            SetSpecialShow();
            CSVNienParameters.Data csvNienParametersData = CSVNienParameters.Instance.GetConfData(9);
            uint.TryParse(csvNienParametersData.str_value, out uint battleTypeId);
            isBoom = Sys_Fight.Instance.BattleTypeId == battleTypeId;
            csvBattleTypeData = CSVBattleType.Instance.GetConfData(Sys_Fight.Instance.BattleTypeId);
            uint groupId = Sys_Fight.curMonsterGroupId;
            if (CSVLevelName.Instance.TryGetValue(groupId, out CSVLevelName.Data data))
            {
                if (data.level_name != 0)
                {
                    layout.Text_Prompt_Name.text = LanguageHelper.GetTextContent(data.level_name);
                }
                layout.Button_Prompt.gameObject.SetActive(true);
            }
            else
            {
                layout.Button_Prompt.gameObject.SetActive(csvBattleTypeData.show_battle_victory == 1);
                layout.Text_Prompt_Name.text = LanguageHelper.GetTextContent(csvBattleTypeData.battle_type_name);
            }
            RefreshHangupBtns();
            // 百人道场buff图标
            if (Sys_HundredPeopleArea.Instance.IsInstance)
            {
                layout.Button_Hundred.gameObject.SetActive(true);
                bool isBigger = Sys_HundredPeopleArea.Instance.IsBigerThanAwakeLevel(out uint awakeId, out uint awakeBufferId);
                // 光效控制
                layout.hundredBufferRed.SetActive(isBigger);
                layout.hundredBufferBlue.SetActive(!isBigger);
            }
            else
            {
                layout.Button_Hundred.gameObject.SetActive(false);
            }
            // 家族资源战
            if (Sys_FamilyResBattle.Instance.InFamilyBattle)
            {
                layout.container.ShowHideBySetActive(false);
            }

            //试炼之门
            if (csvBattleTypeData.stageSwitch_id != null && csvBattleTypeData.stageSwitch_id.Count > 0)
            {
                curStage = Net_Combat.Instance.m_CurServerBattleStage;
                SetTrialGateStageShow(curStage, !isRefresh);
            }
            else
            {
                layout.Button_TrialGate.gameObject.SetActive(false);
            }
            layout.Button_TrialGateSkill.gameObject.SetActive(csvBattleTypeData.isTrialGate == 1);
            layout.Go_Watch_Tip.SetActive(Net_Combat.Instance.m_IsWatchBattle);
            layout.Go_Video.SetActive(!Net_Combat.Instance.IsRealCombat());
            if (!Net_Combat.Instance.IsRealCombat())
            {
                layout.m_FastCommand.gameObject.SetActive(false);
                layout.m_BtnCommandCancle.gameObject.SetActive(false);
                layout.Button_SpeedUp.gameObject.SetActive(false);
                layout.View_Right_Menu.SetActive(false);
                layout.View_Auto_Battle.SetActive(false);
                layout.Go_BulletChat.SetActive(false);
                layout.Go_Video_Func.SetActive(Net_Combat.Instance.m_IsVideo);
                layout.Go_Video_Speed.SetActive(Net_Combat.Instance.m_IsVideo);
                Bullet_View.gameObject.SetActive(Net_Combat.Instance.m_IsVideo);
                layout.Go_BulletView.SetActive(false);
                if (!isRefresh)
                {
                    Bullet_View.Show();
                }
                layout.Button_Video_Close.gameObject.SetActive(true);
                this.layout.btnCamp.gameObject.SetActive(false);
                if (Net_Combat.Instance.m_IsVideo)
                {
                    ClientVideo clientVideo = Net_Combat.Instance.m_CurClientVideoIntroduceData;
                    bool isCollect = Sys_Video.Instance.isCollected(clientVideo.video, clientVideo.authorBrif.Author);
                    layout.Toggle_Mark.SetIsOnWithoutNotify(isCollect);
                    bool isLike = Sys_Video.Instance.isLiked(clientVideo.video, clientVideo.authorBrif.Author);
                    layout.Toggle_Like.SetIsOnWithoutNotify(isLike);
                    layout.Toggle_Barrage.SetIsOnWithoutNotify(Sys_Video.Instance.isOpenBullet);
                    layout.Go_Bullet_Close.gameObject.SetActive(!Sys_Video.Instance.isOpenBullet);
                    layout.Go_Bullet_Open.gameObject.SetActive(Sys_Video.Instance.isOpenBullet);
                    layout.Go_BulletView.SetActive(Sys_Video.Instance.isOpenBullet);
                    if (Net_Combat.Instance.m_IsPauseVideo)
                    {
                        ImageHelper.SetIcon(layout.Image_PlayOrStop, 5000401, true);
                    }
                    else
                    {
                        ImageHelper.SetIcon(layout.Image_PlayOrStop, 5000402, true);
                    }
                }
            }
            else
            {
                layout.Go_BulletView.SetActive(false);
                layout.m_FastCommand.gameObject.SetActive(true);
                layout.Button_SpeedUp.gameObject.SetActive(true);
                layout.View_Right_Menu.SetActive(true);
                layout.View_Auto_Battle.SetActive(true);
                SetTime();
                Fast_FrightingClick.Show();
                isRefreshAutoSkillShow = true;
                SetRoleSkillIcon();
                SetLastSkillIcon();
                Fast_FrightingClick.gameObject.SetActive(Sys_Role_Info.Instance.CanMakeFightCommand());
                layout.SetCommandCancleActive(false);
                GameCenter.fightControl.isCommanding = false;

                if (Sys_FunctionOpen.Instance.IsOpen(60112) && csvBattleTypeData.is_speed_up)
                {
                    layout.Button_SpeedUp.gameObject.SetActive(Sys_Team.Instance.canManualOperate);
                    SetTimeScaleText(layout.Text_SpeedUp, CombatManager.Instance.m_TimeScale);
                }
                else
                {
                    layout.Button_SpeedUp.gameObject.SetActive(false);
                }

                RefreshWorldBoss(csvBattleTypeData);
            }
        }

        private void SetAutoButtonShow()
        {
            Auto_Battle.autofightOn.SetActive(true);
            Auto_Battle.autofightOff.SetActive(false);

            AutoBattleSkillEvt evt = new AutoBattleSkillEvt();
            if (GameCenter.fightControl.autoSkillEvt.heroid == 0)
            {
                if (GameCenter.mainFightHero.battleUnit.AutoSkillId.Contains(GameCenter.mainFightHero.battleUnit.AutoSkillId[0]))
                {
                    evt.heroid = (uint)GameCenter.mainFightHero.battleUnit.AutoSkillId[0];
                }
            }
            else
            {
                evt.heroid = GameCenter.fightControl.autoSkillEvt.heroid;
            }
            if (GameCenter.fightControl.autoSkillEvt.heroid2 == 0)
            {
                if (GameCenter.mainFightHero.battleUnit.AutoSkillId.Contains(GameCenter.mainFightHero.battleUnit.AutoSkillId[1]))
                {
                    evt.heroid2 = (uint)GameCenter.mainFightHero.battleUnit.AutoSkillId[1];
                }
            }
            else
            {
                evt.heroid2 = GameCenter.fightControl.autoSkillEvt.heroid2;
            }

            if (Sys_Fight.Instance.HasPet())
            {
                List<uint> skills = new List<uint>();
                skills= GameCenter.fightControl.GetFightPetSkills ((uint)GameCenter.mainFightPet.battleUnit.PetId);
                uint autoPetSkillId = GameCenter.fightControl.autoSkillEvt.petid;
                if (autoPetSkillId == 0)
                {
                    int skillId = GameCenter.mainFightPet.battleUnit.AutoSkillId[0];
                    if ((skills.Contains((uint)skillId) && GameCenter.mainFightPet.battleUnit.AutoSkillId.Contains(skillId))||skillId==211)
                    {
                        evt.petid = (uint)skillId;
                    }
                    else if (isPetSkillUpdate((uint)skillId, skills, out uint updateSkillInfoId))
                    {
                        evt.petid = updateSkillInfoId;
                        evt.isPetSkillUpdate = true;
                    }
                    else
                    {
                        if (skillId == 101 || skillId == 1001)
                        {
                            evt.petid = (uint)skillId;
                        }
                        else
                        {
                            evt.petid = 1001;
                        }
                    }
                }
                else
                {
                    if (skills.Contains(autoPetSkillId))
                    {
                        evt.petid = autoPetSkillId;
                    }
                    else
                    {
                        if (autoPetSkillId == 101 || autoPetSkillId == 1001|| autoPetSkillId == 211)
                        {
                            evt.petid = autoPetSkillId;
                        }
                        else if (isPetSkillUpdate(autoPetSkillId, skills, out uint updateSkillInfoId))
                        {
                            evt.petid = updateSkillInfoId;
                            evt.isPetSkillUpdate = true;
                        }
                        else
                        {
                            evt.petid = 1001;
                        }
                    }
                }
            }
            Auto_Battle?.RefreshShow(evt, false);
        }

        private bool isPetSkillUpdate(uint skillinfoid, List<uint> skillList, out uint updateskillinfoid)
        {
            if (CSVActiveSkillInfo.Instance.TryGetValue(skillinfoid, out CSVActiveSkillInfo.Data dataSkill))
            {
                for (int i = 0; i < skillList.Count; ++i)
                {
                    if (CSVActiveSkillInfo.Instance.TryGetValue(skillList[i], out CSVActiveSkillInfo.Data data) && data.skill_id == dataSkill.skill_id)
                    {
                        updateskillinfoid = skillList[i];
                        return true;
                    }
                }
            }
            updateskillinfoid = 0;
            return false;
        }

        private void SetAutoTime()
        {
            if (Net_Combat.Instance.m_IsExecuteState)
            {
                SetTimeShow(false);
                return;
            }
            timerauto?.Cancel();
            timerauto = Timer.Register(Constants.AUTOTIME + 1, () =>
              {
                  if (GameCenter.fightControl != null)
                  {
                      timerauto.Cancel();
                      layout.View_Right_Menu.SetActive(false);
                      Net_Combat.Instance.m_IsExecuteState = true;
                      SetArrowShow(false, false);
                      if (GameCenter.fightControl.IsManyPeopleFight()&& !Net_Combat.Instance.m_IsExecuteState)
                      {
                          SetTimePosition(true);
                          SetAutoTimePosition(false);
                          SetTipsShow(3000006);              
                      }
                      else
                      {
                          SetTimeShow(false);
                          Auto_Battle.passtime = Net_Combat.Instance.CountDownValue;
                          UIManager.CloseUI(EUIID.UI_Buff);
                      }
                  }
              }, (time) =>
              {
                  if (GameCenter.fightControl != null && layout.Text_Time != null)
                  {
                      layout.Text_Time_Auto.text = ((int)Math.Ceiling(Constants.AUTOTIME - time)).ToString();
                      if (Math.Ceiling(Constants.AUTOTIME - time) < 1&&! GameCenter.fightControl.IsManyPeopleFight())
                      {
                          SetTimeShow(false);
                      }
                  }
              }, false, false);
        }

        private void SetTime()
        {
            if (Net_Combat.Instance.m_IsExecuteState)
            {
                SetTimeShow(false);
                return;
            }
            uint lefTime = Net_Combat.Instance.RoundCountDown;
            if (lefTime < 1)
            {
                if (GameCenter.fightControl != null)
                {
                    SetTimeShow(false);
                    layout.View_Right_Menu.SetActive(false);
                    OnCloseShowOff(false);
                    UIManager.CloseUI(EUIID.UI_Buff);
                    Auto_Battle.passtime = Net_Combat.Instance.CountDownValue;
                    Net_Combat.Instance.m_IsExecuteState = true;
                }
            }
        }

        private void SetWaitOthersTips()
        {
            SetTipsShow(3000006);
            layout.Animator_Tips.Play("UI_MainBattle_View_Time_Text_Tips", -1, 0);
        }
         
        private void OnUpdateRound(bool isChangeAutoState = false)
        {
            round = Net_Combat.Instance.m_CurRound;
            layout.Text_Round.gameObject.SetActive(true);
            //家族资源战
            if (!isChangeAutoState && Net_Combat.Instance.m_MultiBattleStage != 0)
            {
                layout.Text_Round.text = LanguageHelper.GetTextContent(3000012, round.ToString());
                timerRoundInFamilyBattle?.Cancel();
                timerRoundInFamilyBattle = Timer.Register(1.2f, () =>
                {
                    animatorRound.Play("Close", -1, 0);
                    timerRoundInFamilyBattle.Cancel();
                    timerStageInFamilyBattle?.Cancel();
                    timerStageInFamilyBattle = Timer.Register(0.3f, () =>
                    {
                        timerStageInFamilyBattle.Cancel();
                        layout.Text_Round.gameObject.SetActive(false);
                        layout.Text_Round.gameObject.SetActive(true);
                        layout.Text_Round.text = LanguageHelper.GetTextContent(3000013, Net_Combat.Instance.m_MultiBattleStage.ToString(), Net_Combat.Instance.m_CurStage_Round.ToString());

                    }, null, false, false);
                }, null, false, false);
            }
            else
            {
                layout.Text_Round.text = LanguageHelper.GetTextContent(3000008, round.ToString());
            }
        }

        private void OnUpdateSpecialBossHp()
        {
            if (bosshpischange)
            {
                if (bosscurhp > layout.Slider_BossHp.value)
                {
                    if (bosscurhp == 1)
                    {
                        bosscurhp = layout.Slider_BossHp.value;
                    }
                    else
                    {
                        bosscurhp = Mathf.Lerp(bosscurhp, layout.Slider_BossHp.value, deltaTime * 2);
                    }
                }
                else
                {
                    bosscurhp = layout.Slider_BossHp.value;
                }
                if (Mathf.Abs(bosscurhp - layout.Slider_BossHp.value) <= 0.01f)
                {
                    bosscurhp = layout.Slider_BossHp.value;
                    bosshpischange = false;
                }
                if (bosscurhp >= 0)
                    layout.Slider_BossHpMiddle.value = bosscurhp;
            }
        }

        private void SetTimePosition(bool isShow)
        {
            if (isShow)
            {
                layout.Text_Time.transform.localPosition = new Vector3(0,37,0);
            }
            else
            {
                layout.Text_Time.transform.localPosition = new Vector3(-1000, 37, 0);
            }
        }

        private void SetAutoTimePosition(bool isShow)
        {
            if (isShow)
            {
                layout.Text_Time_Auto.gameObject.SetActive(isShow);
                layout.Text_Time_Auto.transform.localPosition = new Vector3(0, 37, 0);
            }
            else
            {
                layout.Text_Time_Auto.transform.localPosition = new Vector3(-1000, 37, 0);
            }
        }

        private void SetTimeShow(bool isShow)
        {
            if (isShow)
            {
                layout.View_Time.transform.localPosition = new Vector3(16, 55, 0);
            }
            else
            {
                layout.View_Time.transform.localPosition = new Vector3(-1000, 55, 0);
                if (GameCenter.fightControl.CommandingState != EFightCommandingState.Opreating)
                {
                    SetTipsShow(0);
                }
            }
        }

        private void CheckPetGoodUse()
        {
            Right_Menu.Button_Goods.enabled = Sys_Role.Instance.Role.CareerRank != 0;
            Right_Menu.Item_Dark.SetActive(Sys_Role.Instance.Role.CareerRank == 0);
        }

        private void SetTipsShow(uint lanId)
        {
            if (lanId == 0)
            {
                layout.Text_Tips.text = string.Empty;
                layout.Text_Tips_01.text = string.Empty;
                return;
            }
           
            if (GameCenter.fightControl.CommandingState == EFightCommandingState.Opreating)
            {
                return;
            }
            TextHelper.SetText(layout.Text_Tips, lanId);
            TextHelper.SetText(layout.Text_Tips_01, lanId);
            layout.Fix_Tips.SetActive(false);
            layout.Fix_Tips.SetActive(true);
        }

        #endregion

        #region ButtonClick
        public void OnButton_CloseTargetClicked()
        {
            GameCenter.fightControl.DoCloseChooseState();
        }

        public void OnButton_ExitClicked()
        {
          //  Sys_Fight.Instance.EndReq();
        }

        public void OnButton_Team()
        {
            UIManager.OpenUI(EUIID.UI_Team_Member, false, UI_Team_Member.EType.Team);
        }

        public void OnButton_Command()
        {
            UIManager.OpenUI(EUIID.UI_FrightingClick, false);
        }

        public void OnButton_CommandCancle()
        {
            ExitCommand();
        }

        private void ExitCommand()
        {
            if (GameCenter.fightControl.isCommanding)
            {
                UIManager.CloseUI(EUIID.UI_FrightingClick, false);
                Fast_FrightingClick.CancelCommand();
                layout.m_FastCommand.gameObject.SetActive(true);
            }
        }

        public void OncloseBtnClicked()
        {
            if (Sys_Role.Instance.isCrossSrv)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11898));
                return;
            }
            UIManager.OpenUI(EUIID.UI_FunctionMenu);
            layout.Button_Menu_Close.gameObject.SetActive(false);
        }

        public void OnButton_Revoke()
        {         
            layout.View_Right_Menu.SetActive(true);
            layout.Go_Revoke.SetActive(false);
            OnDisableSkillBtn(true);
            GameCenter.fightControl.CanUseSkill = true;
            SetTipsShow(3000001);
            Right_Menu.Go_Escape.SetActive(Sys_FunctionOpen.Instance.IsOpen(60108, false));
            if (!isBoom)  //年兽挑战
            {
                Right_Menu.Go_Recall.SetActive(Sys_FunctionOpen.Instance.IsOpen(60107, false));
                Right_Menu.Go_Seal.SetActive(Sys_FunctionOpen.Instance.IsOpen(60114, false));
                Right_Menu.Go_Pet.SetActive(Sys_FunctionOpen.Instance.IsOpen(60106, false));
                Right_Menu.Go_SparSkill.SetActive(Sys_FunctionOpen.Instance.IsOpen(60110, false));
                Right_Menu.Go_Transposition.SetActive(Sys_FunctionOpen.Instance.IsOpen(60113, false));
                SetRoleSkillIcon();
                SetLastSkillIcon();
            }
            SetArrowShow(true, false);
            Net_Combat.Instance.CmdStateReq(CombatManager.Instance.m_BattleId, GameCenter.mainFightHero.battleUnit.UnitId, 0);
        }

        public void OnBtnCampClicked()
        {
            var csv = CSVBattleType.Instance.GetConfData(Sys_Fight.Instance.BattleTypeId);
            if (csv != null)
            {
                uint bossId = csv.show_boss_id;
                CSVBOSSInformation.Data csvBoss = CSVBOSSInformation.Instance.GetConfData(bossId);
                if (csvBoss != null) {
                    if (Sys_WorldBoss.Instance.unlockedBossManuales.TryGetValue(csvBoss.bossManual_id, out BossManualData bd)) {
                        uint campId = bd.campId;

                        UIManager.OpenUI(EUIID.UI_WorldBossCampInfo, false, new Tuple<uint, uint>(campId, csvBoss.bossManual_id));
                    }
                }
            }
        }

        private readonly static string X1 = "x1";
        private readonly static string X2 = "x2";
        private readonly static string X3 = "x3";
        public void OnButton_SpeedUp()
        {
            if (CombatManager.Instance.m_TimeScale == CombatManager.Instance.OneTimeScale)
            {
                CombatManager.Instance.SetTimeScale(CombatManager.Instance.TwoTimeScale);
                layout.Text_SpeedUp.text = X2;
            }
            else if (CombatManager.Instance.m_TimeScale == CombatManager.Instance.TwoTimeScale)
            {
                CombatManager.Instance.SetTimeScale(CombatManager.Instance.ThridTimeScale);
                layout.Text_SpeedUp.text = X3;
            }
            else if (CombatManager.Instance.m_TimeScale == CombatManager.Instance.ThridTimeScale)
            {
                CombatManager.Instance.SetTimeScale(CombatManager.Instance.OneTimeScale);
                layout.Text_SpeedUp.text = X1;
            }
            else
            {
                CombatManager.Instance.SetTimeScale(CombatManager.Instance.OneTimeScale);
                layout.Text_SpeedUp.text = X1;
            }
        }

        public void OnButton_StopHangup() {
            Sys_Hangup.Instance.TryStopHangup();
        }

        public void OnButton_Hundred() {
            UIManager.OpenUI(EUIID.UI_HundredPeopelAwakenTip);
        }

        private void SetTimeScaleText(Text uiText, float timeScale)
        {
            if (timeScale == CombatManager.Instance.OneTimeScale)
            {
                uiText.text = X1;
            }
            else if (timeScale == CombatManager.Instance.TwoTimeScale)
            {
                uiText.text = X2;
            }
            else if (timeScale == CombatManager.Instance.ThridTimeScale)
            {
                uiText.text = X3;
            }
        }

        public void OnButton_Video_Close_Clicked()
        {
            if (Net_Combat.Instance.m_IsWatchBattle)
            {
                Sys_Fight.Instance.CmdBattleWatchQuitReq();
            }
            Net_Combat.Instance.ExitVideo();
        }

        public void OnButton_Stop_Clicked()
        {
            Net_Combat.Instance.PauseVideo();
            if (Net_Combat.Instance.m_IsPauseVideo)
            {
                ImageHelper.SetIcon(layout.Image_PlayOrStop, 5000401, true);
            }
            else
            {
                ImageHelper.SetIcon(layout.Image_PlayOrStop, 5000402, true);
            }
        }

        public void OnButton_Speed_Clicked()
        {
            Net_Combat.Instance.PlayVideoNextRound();
        }

        public void OnButton_Slow_Clicked()
        {
            Net_Combat.Instance.PlayVideoPreRound();
        }

        public void OnButton_Barrage_Clicked()
        {
            layout.Go_BulletChat.SetActive(true);
            layout.Go_Video.SetActive(false);      
            UIManager.CloseUI(EUIID.UI_ChatSimplify);
        }

        public void OnToggle_Barrage_ValueChanged(bool isOn)
        {
            if (isOn)
            {
                Sys_Video.Instance.OpenBulletReq(Sys_Role.Instance.Role.RoleId);
            }
            else
            {
                Sys_Video.Instance.CloseBulletReq(Sys_Role.Instance.Role.RoleId);
            }
        }

        public void OnButton_Prompt_Clicked()
        {
            CSVBattleType.Data csvBattleType = CSVBattleType.Instance.GetConfData(Sys_Fight.Instance.BattleTypeId);
            if (csvBattleType.show_battle_victory == 1)
            {
                Sys_Fight.Instance.CmdBattleScoreInfoReq();
                UIManager.OpenUI(EUIID.UI_Battle_Decide);
            }
            else
            {
                UIManager.OpenUI(EUIID.UI_Battle_Explain);
            }          
        }

        public void OnButton_Sequence_Clicked()
        {
            UIManager.OpenUI(EUIID.UI_Sequence);
        }

        public void OnToggle_Like_ValueChanged(bool isOn)
        {
            if (Net_Combat.Instance.m_CurClientVideoIntroduceData == null)
            {
                return;
            }
            else
            {
                ClientVideo clientVideo = Net_Combat.Instance.m_CurClientVideoIntroduceData;
                if (isOn && !Sys_Video.Instance.isLiked(clientVideo.video, clientVideo.authorBrif.Author))
                {
                    Sys_Video.Instance.LikeVideoReq(Sys_Role.Instance.RoleId, clientVideo.video, clientVideo.authorBrif.Author, clientVideo.where);
                }
                else if (!isOn && Sys_Video.Instance.isLiked(clientVideo.video, clientVideo.authorBrif.Author))
                {
                    Sys_Video.Instance.CancelLikeReq(Sys_Role.Instance.RoleId, clientVideo.video, clientVideo.authorBrif.Author);
                }
            }
        }

        public void OnToggle_Mark_ValueChanged(bool isOn)
        {
            if (Net_Combat.Instance.m_CurClientVideoIntroduceData == null)
            {
                return;
            }
            else
            {
                ClientVideo clientVideo = Net_Combat.Instance.m_CurClientVideoIntroduceData;
                if (isOn &&! Sys_Video.Instance.isCollected(clientVideo.video, clientVideo.authorBrif.Author))
                {
                    Sys_Video.Instance.CollectVideoReq(Sys_Role.Instance.RoleId, clientVideo.video, clientVideo.authorBrif.Author, clientVideo.where);
                }
                else if (!isOn && Sys_Video.Instance.isCollected(clientVideo.video, clientVideo.authorBrif.Author))
                {
                    Sys_Video.Instance.CancelCollectReq(Sys_Role.Instance.RoleId, clientVideo.video, clientVideo.authorBrif.Author);
                }
            }
        }

        public void OnButton_TrialGate_Clicked()
        {
            if (UIManager.IsOpen(EUIID.UI_TrialGate_StageTip))
            {
                UIManager.CloseUI(EUIID.UI_TrialGate_StageTip);
            }
            else
            {           
                UIManager.OpenUI(EUIID.UI_TrialGate_StageTip,false, curStage);
            }
        }

        public void OnButton_TrialGateSkill_Clicked()
        {
            UIManager.OpenUI(EUIID.UI_TrialSkillDeploy);
        }

        public void OnButton_Element_Clicked()
        {
            UIManager.OpenUI(EUIID.UI_Element);
        }

        public void OnButton_Barrage_Back_Clicked()
        {
            layout.Go_BulletChat.SetActive(false);
            layout.Go_Video.SetActive(true);
            UIManager.OpenUI(EUIID.UI_ChatSimplify);
        }

        public void OnButton_Barrage_Send_Clicked()
        {
            if (layout.InputField.text == string.Empty)
            {
                return;
            }
            ClientVideo clientVide = Net_Combat.Instance.m_CurClientVideoIntroduceData;
            ClientBullet clientBullet = new ClientBullet();
            clientBullet.RoundId = Net_Combat.Instance.m_CurRound;
            clientBullet.Context = ByteString.CopyFromUtf8(layout.InputField.text);
            Sys_Video.Instance.SendBulletReq(clientVide.authorBrif.Author, clientVide.video, clientBullet);
        }
        #endregion
    }
}


