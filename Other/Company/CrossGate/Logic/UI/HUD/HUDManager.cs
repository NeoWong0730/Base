using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using UnityEngine.UI;
using Table;
using Framework;

namespace Logic
{
    public partial class HUD : UIBase
    {
        private AssetDependencies assetDependencies;

        //public const string sPrefab_ViewCook = "UI/MainInterface/View_Cook.prefab";
        //public const string sPrefab_ViewShop = "UI/MainInterface/View_Shop.prefab";
        //public const string sPrefab_ViewBatting = "UI/MainInterface/View_Batting.prefab";
        //public const string sPrefab_ViewCollect = "UI/MainInterface/View_Collect.prefab";
        //public const string sPrefab_PrestigeUp = "UI/Common/PrestigeUp.prefab";
        //public const string sPrefab_LevelUp = "UI/Common/LevelUp.prefab";
        //public const string sPrefab_AdvanceUp = "UI/Common/AdvanceUp.prefab";
        //public const string sPrefab_TeamLogo = "UI/Common/VIew_TeamMark.prefab";
        //public const string sPrefab_FamilyBattle = "UI/Common/View_FamilyBattle.prefab";

        public static GameObject prefab_ViewCookAsset;
        public static GameObject prefab_ViewShopAsset;
        public static GameObject prefab_ViewBattingAsset;
        public static GameObject prefab_ViewCollectAsset;
        public static GameObject prefab_PrestigeUpAsset;
        public static GameObject prefab_LevelUpAsset;
        public static GameObject prefab_AdvanceUpAsset;
        public static GameObject prefab_TeamLogoAsset;
        public static GameObject prefab_FamilyBattleAsset;

        public Transform root_Anim;
        public Transform root_SkillCarouselShow;
        public Transform root_OrderShow;
        private Transform root_BD;
        private Transform root_BattleBubble;
        private Transform root_SkillShow;
        private Transform root_PassiveName;
        private Transform root_ActorHUD;
        private Transform root_BuffHud;
        private Transform root_ExpressionBubble;
        private Transform root_PlayerChatBubble;
        private Transform root_NpcBubble;
        private Transform root_CutSceneBubble;
        private Transform root_SecondAction;
        private Transform root_CombatOrderFlag;
        private Transform root_ComOrderName;
        private Transform root_CombatSelect;
        private Transform root_Emotion;

        private Text templete_EmojiText;
        private Text templete_NormalEmojiText;

        public GameObject template_SkillCarouselShow;
        public GameObject template_OrderShow;
        private GameObject templete_Blood;
        private GameObject templete_BattleBubble;
        private GameObject template_ExpressionBubble;
        private GameObject template_PlayerChatBubble;
        private GameObject template_NpcBubble;
        private GameObject templete_Anim;
        private GameObject template_Anim_name;
        private GameObject template_Anim_PassiveName;
        private GameObject template_SkillShow;
        private GameObject template_Actor;
        private GameObject template_Buff;
        private GameObject template_CutScene;
        private GameObject template_SecondAction;
        private GameObject template_CombatOrderFlag;
        private GameObject template_CombatOrder_Name;
        private GameObject template_CombatSelect;
        private GameObject template_Emotion;

        private Dictionary<uint, BDShow> bloods = new Dictionary<uint, BDShow>();                           //战斗内血条显示
        private List<BDShow> bloodsList = new List<BDShow>();                                                 //用来遍历方便减少GC
        private Dictionary<uint, BuffHUDShow> buffhudShows = new Dictionary<uint, BuffHUDShow>();           //战斗内buff显示
        private List<SecondActionShow> secondActionShows = new List<SecondActionShow>();
        private Dictionary<ulong, ActorHUDShow> actorHuds = new Dictionary<ulong, ActorHUDShow>();          //战斗外actor  title(包含玩家和npc)
        private Dictionary<ulong, GameObject> actorObjs = new Dictionary<ulong, GameObject>();              //战斗外actor gameobjects
        private List<Anim_BaseShow> anim_BaseShows = new List<Anim_BaseShow>();                             //战斗内跳字表现
        private Dictionary<ulong, ExpressionBubbleShow> expressionBubbleHuds = new Dictionary<ulong, ExpressionBubbleShow>(); //战斗外气泡表情
        private Dictionary<ulong, PlayerChatBubbleShow> playerChatBubbleHuds = new Dictionary<ulong, PlayerChatBubbleShow>();//聊天气泡
        private Dictionary<ulong, NpcBubbleShow> npcBubbleHuds = new Dictionary<ulong, NpcBubbleShow>();    //战斗外npc头顶气泡
        private Dictionary<GameObject, CutSceneBubbleShow> cutSceneBubblehuds = new Dictionary<GameObject, CutSceneBubbleShow>(); //cutscene气泡
        private Dictionary<uint, BubbleShow> battleBubbleHuds = new Dictionary<uint, BubbleShow>();         //战斗内气泡
        private Dictionary<GameObject, ComboGroup> combos = new Dictionary<GameObject, ComboGroup>();
        private List<ComboGroup> combosList = new List<ComboGroup>();                                       //用于遍历用的，减少GC
        private OrderQueue orderQueue;                                                                      //战斗内指令队列
        private Dictionary<GameObject, EmotionShow> emotionhuds = new Dictionary<GameObject, EmotionShow>();//心情符号

        public GameObjectPool BdPools;
        public GameObjectPool BattleBubblePools;
        public GameObjectPool ExpressionBubblePools;
        public GameObjectPool PlayerChatBubblePools;
        public GameObjectPool NpcBubblePools;
        public GameObjectPool AnimPools;
        public GameObjectPool AnimNamePools;
        public GameObjectPool SkillPools;
        public GameObjectPool ActorHUDPools;
        public GameObjectPool BuffHUDPools;
        public GameObjectPool CutScenePools;
        public GameObjectPool SkillCarouselHUDPools;
        public GameObjectPool OrderHUDPools;
        public GameObjectPool SecondActionHUDPools;
        public GameObjectPool ComBatOrderFlagPools;
        public GameObjectPool ComBatOrderNamePools;
        public GameObjectPool ComBatSelectPools;
        public GameObjectPool EmotionPools;
        public GameObjectPool PassiveNamePools;

        private TansformWorldToScreen _tansformWorldToScreen;

        private float m_AnimPlayInterval;
        private float m_AnimPlayTimer;

        private float m_AnimExitFightRecycleTime;
        private float m_DestroyAnimTime;
        private bool b_CheckedDestroyAnim = false;

        protected override void OnInit()
        {
            m_AnimPlayInterval = int.Parse(CSVParam.Instance.GetConfData(113).str_value) / 1000f;
            m_AnimPlayTimer = 0;
            m_AnimExitFightRecycleTime = 10f;
        }

        protected override void OnLoaded()
        {
            assetDependencies = transform.GetComponent<AssetDependencies>();

            prefab_ViewCookAsset = assetDependencies.mCustomDependencies[0] as GameObject;
            prefab_ViewShopAsset = assetDependencies.mCustomDependencies[1] as GameObject;
            prefab_ViewBattingAsset = assetDependencies.mCustomDependencies[2] as GameObject;
            prefab_ViewCollectAsset = assetDependencies.mCustomDependencies[3] as GameObject;
            prefab_PrestigeUpAsset = assetDependencies.mCustomDependencies[4] as GameObject;
            prefab_LevelUpAsset = assetDependencies.mCustomDependencies[5] as GameObject;
            prefab_AdvanceUpAsset = assetDependencies.mCustomDependencies[6] as GameObject;
            prefab_TeamLogoAsset = assetDependencies.mCustomDependencies[7] as GameObject;
            prefab_FamilyBattleAsset = assetDependencies.mCustomDependencies[8] as GameObject;

            root_BD = transform.Find("BD_root");
            root_BattleBubble = transform.Find("BattleBubble_root");
            root_Anim = transform.Find("Anim_root");
            root_SkillShow = transform.Find("Skill_root");
            root_PassiveName = transform.Find("PassiveName_root");
            root_ActorHUD = transform.Find("ActorHUD_root");
            root_BuffHud = transform.Find("BuffHUD_root");
            root_ExpressionBubble = transform.Find("ExpressionBubble_root");
            root_PlayerChatBubble = transform.Find("PlayerChatBubble_root");
            root_NpcBubble = transform.Find("NpcBubble_root");
            root_CutSceneBubble = transform.Find("CutSceneBubble_root");
            root_SkillCarouselShow = transform.Find("SkillCarouse_root");
            root_OrderShow = transform.Find("Order_root");
            root_SecondAction = transform.Find("SecondAction_root");
            root_CombatOrderFlag = transform.Find("CombatOrderFlagRoot");
            root_ComOrderName = transform.Find("CombatOrderNameRoot");
            root_CombatSelect = transform.Find("CombatSelectRoot");
            root_Emotion = transform.Find("EmotionRoot");

            templete_Blood = root_BD.Find("BD_ImgBg").gameObject;
            templete_BattleBubble = root_BattleBubble.Find("bg").gameObject;
            templete_Anim = root_Anim.Find("bg").gameObject;
            template_SkillShow = root_SkillShow.Find("bg").gameObject;
            template_Anim_name = root_SkillShow.Find("bg1").gameObject;
            template_Anim_PassiveName = root_PassiveName.Find("bg").gameObject;
            template_Actor = root_ActorHUD.Find("ActorHUD").gameObject;
            template_Buff = root_BuffHud.Find("root").gameObject;
            template_ExpressionBubble = root_ExpressionBubble.Find("bg").gameObject;
            template_PlayerChatBubble = root_PlayerChatBubble.Find("bg").gameObject;
            template_NpcBubble = root_NpcBubble.Find("bg").gameObject;
            template_CutScene = root_CutSceneBubble.Find("bg").gameObject;
            template_SkillCarouselShow = root_SkillCarouselShow.Find("bg").gameObject;
            template_OrderShow = root_OrderShow.Find("bg").gameObject;
            template_SecondAction = root_SecondAction.Find("bg").gameObject;
            template_CombatOrderFlag = root_CombatOrderFlag.Find("CombatOrderFlag").gameObject;
            template_CombatOrder_Name = root_ComOrderName.Find("CombaOrderName").gameObject;
            template_CombatSelect = root_CombatSelect.Find("bg").gameObject;
            template_Emotion = root_Emotion.Find("bg").gameObject;
            templete_EmojiText = root_PlayerChatBubble.Find("TempleteText").GetComponent<Text>();
            templete_NormalEmojiText = root_PlayerChatBubble.Find("NormalTempleteText").GetComponent<Text>();

            _tansformWorldToScreen = gameObject.AddComponent<TansformWorldToScreen>();

            BdPools = new GameObjectPool(30, templete_Blood);
            BattleBubblePools = new GameObjectPool(10, templete_BattleBubble);
            AnimPools = new GameObjectPool(30, templete_Anim);
            SkillPools = new GameObjectPool(10, template_SkillShow);
            ActorHUDPools = new GameObjectPool(30, template_Actor);
            BuffHUDPools = new GameObjectPool(10, template_Buff);
            ExpressionBubblePools = new GameObjectPool(5, template_ExpressionBubble);
            PlayerChatBubblePools = new GameObjectPool(10, template_PlayerChatBubble);
            NpcBubblePools = new GameObjectPool(20, template_NpcBubble);
            CutScenePools = new GameObjectPool(5, template_CutScene);
            SkillCarouselHUDPools = new GameObjectPool(10, template_SkillCarouselShow);
            OrderHUDPools = new GameObjectPool(10, template_OrderShow);
            SecondActionHUDPools = new GameObjectPool(5, template_SecondAction);
            ComBatOrderFlagPools = new GameObjectPool(20, template_CombatOrderFlag);
            ComBatOrderNamePools = new GameObjectPool(20, template_CombatOrder_Name);
            ComBatSelectPools = new GameObjectPool(20, template_CombatSelect);
            EmotionPools = new GameObjectPool(10, template_Emotion);
            AnimNamePools = new GameObjectPool(20, template_Anim_name);
            PassiveNamePools = new GameObjectPool(20, template_Anim_PassiveName);
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_HUD.Instance.eventEmitter.Handle<int>(Sys_HUD.EEvents.OnSetLayer, SetLayer, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle(Sys_HUD.EEvents.OnRevertLayer, RevertLayer, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<int, bool>(Sys_HUD.EEvents.OnUpdateHUDMoudles, OnUpdateHUDMoudles, toRegister);

            Sys_HUD.Instance.eventEmitter.Handle<HpValueChangedEvt>(Sys_HUD.EEvents.OnUpdateHp, UpdateHp, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<MpValueChangedEvt>(Sys_HUD.EEvents.OnUpdateMp, UpdateMp, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ShapeShiftChangedEvt>(Sys_HUD.EEvents.OnUpdateShapeShift, UpdateShapeShiftValue, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ShieldValueChangedEvt>(Sys_HUD.EEvents.OnUpdateShield, UpdateShieldValue, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<EnergyValueChangedEvt>(Sys_HUD.EEvents.OnUpdateEnergy, UpdateEnergyValue, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<uint>(Sys_HUD.EEvents.OnRemoveBattleUnit, RemoveBattleUint, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<CreateBloodEvt>(Sys_HUD.EEvents.OnCreateBlood, CreateBlood, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ShowOrHideBDEvt>(Sys_HUD.EEvents.OnShowOrHideBD, ShowOrHideBlood, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<UpdateArrowEvt>(Sys_HUD.EEvents.OnUpdateArrow, UpdateArrowState, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<UpdateSparSkillEvt>(Sys_HUD.EEvents.OnUpdateSparSkill, UpdateSparSkill, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<uint>(Sys_HUD.EEvents.OnTriggerSecondAction, TriggerSecondAction, toRegister);

            Sys_HUD.Instance.eventEmitter.Handle<BuffHuDUpdateEvt>(Sys_HUD.EEvents.OnUpdateBuffHUD, UpdateBuffHUD, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<uint, bool>(Sys_HUD.EEvents.OnShowOrHideBuffHUD, ShowOrHideBuffHUD, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<BuffHUDFlashEvt>(Sys_HUD.EEvents.OnBuffHUDFalsh, BuffFalsh, toRegister);

            Sys_HUD.Instance.eventEmitter.Handle<TriggerBattleBubbleEvt>(Sys_HUD.EEvents.OnTriggerBattleBubble, CreateBattleBubble, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<TriggerExpressionBubbleEvt>(Sys_HUD.EEvents.OnTriggerExpressionBubble, CreateExpressionBubble, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<TriggerPlayerChatBubbleEvt>(Sys_HUD.EEvents.OnTriggerPlayerChatBubble, CreatePlayerChatBubble, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<TriggerNpcBubbleEvt>(Sys_HUD.EEvents.OnTriggerNpcBubble, CreateNpcBubble, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<TriggerCutSceneBubbleEvt>(Sys_HUD.EEvents.OnTriggerCutSceneBubble, CreateCutSceneBubble, toRegister);

            Sys_HUD.Instance.eventEmitter.Handle<CreateActorHUDEvt>(Sys_HUD.EEvents.OnCreateActorHUD, CreateActorHUD, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, UpdateActorHUD_icon, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ActorHUDTitleNameUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUDTitleName, UpdateActorHUD_TitleName, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ActorHUDNameUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorName, UpdateActorName, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ulong>(Sys_HUD.EEvents.OnRemoveActorHUD, RemoveActorHUD, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle(Sys_HUD.EEvents.OnActiveAllActorHUD, ActiveAllActorHUD, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle(Sys_HUD.EEvents.OnInActiveAllActorHUD, InActiveAllActorHUD, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ShowOrHideActorHUDEvt>(Sys_HUD.EEvents.OnShowOrHideActorHUD, ShowOrHideActorHUD, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ShowOrHideActorsHUDEvt>(Sys_HUD.EEvents.OnShowOrHideActorsHUD, ShowOrHideActorsHUD, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<CreateOrUpdateActorHUDStateFlagEvt>(Sys_HUD.EEvents.OnCreateOrUpdateActorHUDStateFlag, CreateOrUpdateActorHUDStateFlag_icon, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ulong>(Sys_HUD.EEvents.OnClearActorHUDStateFlag, ClearActorHUDStateFlag, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ulong, uint>(Sys_HUD.EEvents.OnUpdateHeroFunState, UpdateHeroFunState, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<CreateTitleEvt>(Sys_HUD.EEvents.OnCreateTitle, CreateTitle, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ClearTitleEvt>(Sys_HUD.EEvents.OnClearTitle, ClearTitle, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ulong, uint>(Sys_HUD.EEvents.OnUpdateFamilyTitleName, UpdateFamilyTitle, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<UpdateBGroupTitleEvt>(Sys_HUD.EEvents.OnUpdateBGroupTitleName, UpdateBGroupTitle, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<UpdateFavirabilityEvt>(Sys_HUD.EEvents.OnUpdateFavirability, UpdateFavirability, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ClearFavirabilityEvt>(Sys_HUD.EEvents.OnClearFavirability, ClearFavirability, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<UpdateWorldBossHuDEvt>(Sys_HUD.EEvents.OnUpdateWorldBossHud, UpdateWorldBoss, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ClearWorldBossHuDEvt>(Sys_HUD.EEvents.OnClearWorldBossHud, ClearWorldBoss, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<UpdateActorFightStateEvt>(Sys_HUD.EEvents.OnUpdateSceneActorFightState, UpdateFightState, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<UpMountEvt>(Sys_HUD.EEvents.OnUpMount, OnUpMount, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ulong>(Sys_HUD.EEvents.OnDownMount, OnDownMount, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ulong, uint>(Sys_HUD.EEvents.OnScaleUp, OnScaleUp, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ulong>(Sys_HUD.EEvents.OnResetScale, OnResetScale, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ulong>(Sys_HUD.EEvents.OnShowNpcArrow, OnShowNpcArrow, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ulong>(Sys_HUD.EEvents.OnHideNpcArrow, OnHideNpcArrow, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ulong, uint>(Sys_HUD.EEvents.OnShowNpcSliderNotice, OnShowNpcSliderNotice, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ulong>(Sys_HUD.EEvents.OnHideNpcSliderNotice, OnHideNpcSliderNotice, toRegister);

            Sys_HUD.Instance.eventEmitter.Handle<PlayActorLevelUpHudEvt>(Sys_HUD.EEvents.OnPlayActorLevelUpFx, PlayLevelUpFx, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<PlayActorAdvanceUpHudEvt>(Sys_HUD.EEvents.OnPlayActorAdvanceUpFx, PlayAdvanceUpFx, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<PlayActorReputationHudEvt>(Sys_HUD.EEvents.OnPlayActorReputationUpFx, PlayReputationUpFx, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<NpcBattleCdEvt>(Sys_HUD.EEvents.OnCreateNpcBattleCd, CreateNpcBattleCd, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle(Sys_HUD.EEvents.OnClearActorFx, OnClearActorFx, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ulong, uint>(Sys_HUD.EEvents.OnCreateTeamLogo, CreateTeamLogo, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ulong>(Sys_HUD.EEvents.OnClearTeamLogo, ClearTeamLogo, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ulong>(Sys_HUD.EEvents.OnCreateTeamFx, CreateTeamFx, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ulong>(Sys_HUD.EEvents.OnClearTeamFx, ClearTeamFx, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ulong>(Sys_HUD.EEvents.OnCreateFamilyBattle, OnCreateFamilyBattle, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<UpdateFamliyNameBeforeBattleRescorce>(Sys_HUD.EEvents.OnUpdateFamilyName, OnUpdateFamilyName, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ulong>(Sys_HUD.EEvents.OnClearFamilyBattle, OnClearFamilyBattle, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ulong, uint>(Sys_HUD.EEvents.OnUpdateGuildBattleResource, OnUpdateFamilyBattleResource, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<UpdateFamilyTeamNumInBattleResource>(Sys_HUD.EEvents.OnUpdateFamilyTeamNum, OnUpdateFamilyTeamNum, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<ulong>(Sys_HUD.EEvents.OnUpdateGuildBattleName, OnUpdateGuildBattleName, toRegister);

            Sys_HUD.Instance.eventEmitter.Handle(Sys_HUD.EEvents.OnClearBattleHUDs, ClearBattleHUDs, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle(Sys_HUD.EEvents.OnClearActorHUDs, ClearActorHUDs, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle(Sys_HUD.EEvents.OnClearNpcBubbles, ClearNpcBubble, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle(Sys_HUD.EEvents.OnClearCutSceenBubbles, ClearCutSceneBubble, toRegister);

            Sys_HUD.Instance.eventEmitter.Handle<uint, bool>(Sys_HUD.EEvents.OnUpdateTimeSand, UpdateTimeSand, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<uint, bool, string>(Sys_HUD.EEvents.OnTriggerBattleInstructionFlag, OnTriggerBattleInstructionFlag, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle(Sys_HUD.EEvents.OnClearBattleFlag, OnClearBattleFlag, toRegister);

            Sys_HUD.Instance.eventEmitter.Handle<CreateOrderHUDEvt>(Sys_HUD.EEvents.OnAddBattleOrder, CreateOrderHUD, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<uint>(Sys_HUD.EEvents.OnUndoBattleOrder, UndoOrderHUD, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle(Sys_HUD.EEvents.OnClearBattleOrder, ClearOrder, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<uint, bool>(Sys_HUD.EEvents.OnShowOrHideSelect, ShowOrHideSelect, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<uint>(Sys_HUD.EEvents.OnSelected, OnSelected, toRegister);

            Sys_HUD.Instance.eventEmitter.Handle<CreateEmotionEvt>(Sys_HUD.EEvents.OnCreateEmotion, CreateEmotion, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle(Sys_HUD.EEvents.OnClearEmotion, ClearEmotion, toRegister);

            //gamecenter第一次初始化之后 才执行打开hud
            if (toRegister)
            {
                DebugUtil.Log(ELogType.eFamilyBattle, "HUD.CreateHero");

                Sys_HUD.Instance.AddHeroHUD(GameCenter.mainHero);

                if (Sys_FamilyResBattle.Instance.InFamilyBattle)
                {
                    //Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnClearTitle, GameCenter.mainHero.uID);
                    //Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnClearTeamLogo, GameCenter.mainHero.uID);
                    //Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnClearTeamFx, GameCenter.mainHero.uID);

                    Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnCreateFamilyBattle, GameCenter.mainHero.uID);

                    Sys_HUD.Instance.eventEmitter.Trigger<ulong, uint>(Sys_HUD.EEvents.OnUpdateGuildBattleResource, GameCenter.mainHero.uID, Sys_FamilyResBattle.Instance.Resource);

                    UpdateFamilyTeamNumInBattleResource updateFamilyTeamNumInBattleResource = new UpdateFamilyTeamNumInBattleResource();
                    updateFamilyTeamNumInBattleResource.actorId = GameCenter.mainHero.uID;
                    updateFamilyTeamNumInBattleResource.teamNum = Sys_FamilyResBattle.Instance.TeamNum;
                    updateFamilyTeamNumInBattleResource.maxCount = Sys_FamilyResBattle.Instance.MaxCount;
                    Sys_HUD.Instance.eventEmitter.Trigger<UpdateFamilyTeamNumInBattleResource>(Sys_HUD.EEvents.OnUpdateFamilyTeamNum, updateFamilyTeamNumInBattleResource);

                    if (GameCenter.mainHero.familyResBattleComponent != null && !GameCenter.mainHero.familyResBattleComponent.isRed)
                    {
                        Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnUpdateGuildBattleName, GameCenter.mainHero.uID);
                    }
                }
                else
                {
                    Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnClearFamilyBattle, GameCenter.mainHero.uID);
                }
            }
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_HUD.Instance.eventEmitter.Handle<TriggerAnimEvt>(Sys_HUD.EEvents.OnTriggerAnim, CreateAnim, toRegister);
            Sys_HUD.Instance.eventEmitter.Handle<TriggerSkillEvt>(Sys_HUD.EEvents.OnTriggerSkill, CreateSkillShow, toRegister);
        }

        protected override void OnDestroy()
        {
            BdPools.Dispose();
            BattleBubblePools.Dispose();
            AnimPools.Dispose();
            SkillPools.Dispose();
            ActorHUDPools.Dispose();
            BuffHUDPools.Dispose();
            ExpressionBubblePools.Dispose();
            NpcBubblePools.Dispose();
            CutScenePools.Dispose();
            SkillCarouselHUDPools.Dispose();
            OrderHUDPools.Dispose();
            SecondActionHUDPools.Dispose();
            ComBatOrderFlagPools.Dispose();
            ComBatOrderNamePools.Dispose();
            ComBatSelectPools.Dispose();
            EmotionPools.Dispose();
            AnimNamePools.Dispose();

            prefab_ViewCookAsset = null;
            prefab_ViewShopAsset = null;
            prefab_ViewBattingAsset = null;
            prefab_ViewCollectAsset = null;
            prefab_PrestigeUpAsset = null;
            prefab_LevelUpAsset = null;
            prefab_AdvanceUpAsset = null;
            prefab_TeamLogoAsset = null;
            prefab_FamilyBattleAsset = null;
        }

        protected override void OnHide()
        {
            OnClearActorFx();
        }

        private void OnUpdateHUDMoudles(int moudle, bool add)
        {
            EHudMoudle eHudMoudle = (EHudMoudle)moudle;
            if (HasHudMoudle(eHudMoudle, EHudMoudle.e_Blood))
                root_BD.gameObject.SetActive(add);

            if (HasHudMoudle(eHudMoudle, EHudMoudle.e_Bubble))
                root_BattleBubble.gameObject.SetActive(add);

            if (HasHudMoudle(eHudMoudle, EHudMoudle.e_Anim))
                root_Anim.gameObject.SetActive(add);

            if (HasHudMoudle(eHudMoudle, EHudMoudle.e_Skill))
                root_SkillShow.gameObject.SetActive(add);

            if (HasHudMoudle(eHudMoudle, EHudMoudle.e_Actor))
                root_ActorHUD.gameObject.SetActive(add);

            if (HasHudMoudle(eHudMoudle, EHudMoudle.e_Buff))
                root_BuffHud.gameObject.SetActive(add);
        }

        private bool HasHudMoudle(EHudMoudle parent, EHudMoudle child)
        {
            return (parent & child) == child;
        }

        protected override void OnUpdate()
        {
            foreach (var item in m_AnimQueue)
            {
                item.Value.OnUpdate();
            }
            for (int i = 0; i < bloodsList.Count; ++i)
            {
                BDShow bDShow = bloodsList[i];
                bDShow?.Update();
            }
            for (int i = 0; i < combosList.Count; ++i)
            {
                ComboGroup comboGroup = combosList[i];
                comboGroup?.Update();
            }
            for (int i = 0; i < anim_BaseShows.Count; ++i)
            {
                Anim_BaseShow item = anim_BaseShows[i];
                item?.Update();
            }
            if (!CombatManager.Instance.m_IsFight && !b_CheckedDestroyAnim)
            {
                if (Time.time >= m_DestroyAnimTime)
                {
                    ClearAnim_GameObject();
                    b_CheckedDestroyAnim = true;
                    m_DestroyAnimTime = Time.time + m_AnimExitFightRecycleTime;
                }
            }
        }

        protected override void OnLateUpdate(float dt, float usdt)
        {
            orderQueue?.Update();
        }

        public void ClearBattleHUDs()
        {
            foreach (var item in m_AnimQueue)
            {
                item.Value.Dispose();
            }
            m_AnimQueue.Clear();
            ClearOrder();

            foreach (var item in m_BattleBubbleTimer)
            {
                Timer timer = item.Value;
                timer?.Cancel();
            }
            m_BattleBubbleTimer.Clear();

            m_BattleBubbleDatas.Clear();

            for (int i = 0; i < bloodsList.Count; ++i)
            {
                BDShow bDShow = bloodsList[i];
                bDShow?.Dispose();
                BdPools.Recovery(bDShow.mRootGameObject);
                ComBatOrderNamePools.Recovery(bDShow.mBattleOrderName);
                ComBatOrderFlagPools.Recovery(bDShow.mBattleOrderFlag);
                ComBatSelectPools.Recovery(bDShow.mBattleSelect);
                HUDFactory.Recycle(bDShow);
            }
            bloods.Clear();
            bloodsList.Clear();

            Dictionary<uint, BuffHUDShow>.Enumerator enumerator4 = buffhudShows.GetEnumerator();
            while (enumerator4.MoveNext())
            {
                BuffHUDShow buffHUDShow = enumerator4.Current.Value;
                buffHUDShow?.Dispose();
                BuffHUDPools.Recovery(buffHUDShow.mRootGameObject);
                HUDFactory.Recycle(buffHUDShow);
            }
            buffhudShows.Clear();

            BubbleShow[] bubbleShows = new BubbleShow[battleBubbleHuds.Count];
            battleBubbleHuds.Values.CopyTo(bubbleShows, 0);
            for (int i = 0; i < bubbleShows.Length; i++)
            {
                if (bubbleShows[i] != null)
                {
                    bubbleShows[i].Dispose();
                    RecyleBattleBubble(bubbleShows[i]);
                }
            }
            battleBubbleHuds.Clear();

            for (int i = 0; i < anim_BaseShows.Count; i++)
            {
                Anim_BaseShow anim_BaseShow = anim_BaseShows[i];
                if (anim_BaseShow != null)
                {
                    if (anim_BaseShow is SkillShow)
                    {
                        SkillPools.Recovery(anim_BaseShow.mRootGameObject);
                    }
                    else if (anim_BaseShow.animType == AnimType.e_MagicShort || anim_BaseShow.animType == AnimType.e_EnergyShort)
                    {
                        AnimNamePools.Recovery(anim_BaseShow.mRootGameObject);
                    }
                    else if (anim_BaseShow.animType == AnimType.e_PassiveName)
                    {
                        PassiveNamePools.Recovery(anim_BaseShow.mRootGameObject);
                    }
                    else
                    {
                        AnimPools.Recovery(anim_BaseShow.mRootGameObject);
                    }
                    anim_BaseShows[i].Dispose();
                    HUDFactory.Recycle(anim_BaseShows[i]);
                }
            }
            anim_BaseShows.Clear();

            for (int i = 0; i < secondActionShows.Count; i++)
            {
                secondActionShows[i]?.Dispose();
            }
            secondActionShows.Clear();

            for (int index = 0; index < combosList.Count; ++index)
            {
                ComboGroup comboGroup = combosList[index];

                for (int i = 0; i < comboGroup.hudComponents.Count; i++)
                {
                    Anim_BaseShow comboCharacterShow = comboGroup.hudComponents[i];
                    RecyleAnim(comboCharacterShow);
                }
            }
            combos.Clear();
            combosList.Clear();

            m_DestroyAnimTime = Time.time + m_AnimExitFightRecycleTime;

            ClearSkillCrousel_GameObject();
            ClearBuff_GameObject();
            ClearBattleBubble_GameObject();
        }

        private void ClearAnim_GameObject()
        {
            for (int i = 0; i < root_Anim.childCount; i++)
            {
                if (i == 0)
                {
                    root_Anim.GetChild(i).gameObject.SetActive(false);
                    continue;
                }
                GameObject.Destroy(root_Anim.GetChild(i).gameObject);
            }
            AnimPools.Clear();
        }

        private void ClearSkillCrousel_GameObject()
        {
            for (int i = 0; i < root_OrderShow.childCount; i++)
            {
                if (i == 0)
                {
                    root_OrderShow.GetChild(i).gameObject.SetActive(false);
                    continue;
                }
                GameObject.Destroy(root_OrderShow.GetChild(i).gameObject);
            }
            OrderHUDPools.Clear();

            for (int i = 0; i < root_SkillCarouselShow.childCount; i++)
            {
                if (i == 0)
                {
                    root_SkillCarouselShow.GetChild(i).gameObject.SetActive(false);
                    continue;
                }
                GameObject.Destroy(root_SkillCarouselShow.GetChild(i).gameObject);
            }
            SkillCarouselHUDPools.Clear();
        }

        private void ClearBuff_GameObject()
        {
            for (int i = 0; i < root_BuffHud.childCount; i++)
            {
                if (i == 0)
                {
                    root_BuffHud.GetChild(i).gameObject.SetActive(false);
                    continue;
                }
                GameObject.Destroy(root_BuffHud.GetChild(i).gameObject);
            }
            BuffHUDPools.Clear();
        }

        private void ClearBattleBubble_GameObject()
        {
            for (int i = 0; i < root_BattleBubble.childCount; i++)
            {
                if (i == 0)
                {
                    root_BattleBubble.GetChild(i).gameObject.SetActive(false);
                    continue;
                }
                GameObject.Destroy(root_BattleBubble.GetChild(i).gameObject);
            }
            BattleBubblePools.Clear();
        }

        public void ClearActorHUDs()
        {
            Dictionary<ulong, ActorHUDShow>.Enumerator enumerator = actorHuds.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ActorHUDShow actorHUDShow = enumerator.Current.Value;
                actorHUDShow?.Dispose();
            }
            actorHuds.Clear();

            Dictionary<ulong, NpcBubbleShow>.Enumerator enumerator1 = npcBubbleHuds.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                NpcBubbleShow bubbleShow = enumerator1.Current.Value;
                bubbleShow?.Dispose();
            }
            npcBubbleHuds.Clear();

            Dictionary<ulong, ExpressionBubbleShow>.Enumerator enumerator2 = expressionBubbleHuds.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                ExpressionBubbleShow bubbleShow = enumerator2.Current.Value;
                bubbleShow?.Dispose();
            }
            expressionBubbleHuds.Clear();

            Dictionary<ulong, PlayerChatBubbleShow>.Enumerator enumerator3 = playerChatBubbleHuds.GetEnumerator();
            while (enumerator3.MoveNext())
            {
                PlayerChatBubbleShow bubbleShow = enumerator3.Current.Value;
                bubbleShow?.Dispose();
            }
            playerChatBubbleHuds.Clear();
        }

        public void ClearNpcBubble()
        {
            NpcBubbleTimer?.Cancel();
            Dictionary<ulong, NpcBubbleShow>.Enumerator enumerator1 = npcBubbleHuds.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                NpcBubbleShow bubbleShow = enumerator1.Current.Value;
                if (bubbleShow != null)
                {
                    bubbleShow.Dispose();
                    HUDFactory.Recycle(bubbleShow);
                    NpcBubblePools.Recovery(bubbleShow.mRootGameobject);
                }
            }
            npcBubbleHuds.Clear();
        }

        public void ClearCutSceneBubble()
        {
            CutSceneBubbleTimer?.Cancel();
            Dictionary<GameObject, CutSceneBubbleShow>.Enumerator enumerator4 = cutSceneBubblehuds.GetEnumerator();
            while (enumerator4.MoveNext())
            {
                CutSceneBubbleShow bubbleShow = enumerator4.Current.Value;
                if (bubbleShow != null)
                {
                    bubbleShow?.Dispose();
                    HUDFactory.Recycle(bubbleShow);
                    CutScenePools.Recovery(bubbleShow.mRootGameobject);
                }
            }
            cutSceneBubblehuds.Clear();
        }

        private void SetLayer(int _bsortingOrder)
        {
            canvas.sortingOrder = _bsortingOrder;
        }

        private void RevertLayer()
        {
            canvas.sortingOrder = nSortingOrder;
        }

        private GameObject GetAnimGo()
        {
            return AnimPools.Get(root_Anim);
        }

        private GameObject GetOrderGo()
        {
            return OrderHUDPools.Get(root_OrderShow);
        }

        private void PushOrderGo(GameObject order)
        {
            OrderHUDPools.Recovery(order);
        }

        private GameObject GetSkillCarouselGo()
        {
            return SkillCarouselHUDPools.Get(root_SkillCarouselShow);
        }

        private void PushSkillCarouselGo(GameObject SkillCarousel)
        {
            SkillCarouselHUDPools.Recovery(SkillCarousel);
        }

        private void PushAnimGo(GameObject gameObject)
        {
            AnimPools.Recovery(gameObject);
        }
    }

}

