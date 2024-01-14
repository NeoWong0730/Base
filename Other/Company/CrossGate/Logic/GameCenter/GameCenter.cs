using System;
using System.Collections.Generic;
using Framework;
using Lib.Core;
using Logic.Core;
using Packet;
using Table;
using UnityEngine;
using UnityEngine.AI;

namespace Logic
{
    public class GameCenter
    {
        #region Shortcut
        public static LvPlay mLvPlay = null;
        public static PlayerControlSystem mPlayerControlSystem
        {
            get { return mLvPlay.mPlayerControlSystem; }
        }
        public static PathFindControlSystem mPathFindControlSystem
        {
            get { return mLvPlay.mPathFindControlSystem; }
        }
        public static UploadTransformSystem mUploadTransformSystem
        {
            get { return mLvPlay.mUploadTransformSystem; }
        }
        public static CheckHeroVisualSystem mCheckHeroVisualSystem
        {
            get { return mLvPlay.mCheckHeroVisualSystem; }
        }
        public static WayPointSystem mWayPointSystem
        {
            get { return mLvPlay.mWayPointSystem; }
        }
        public static NPCHUDSystem mNPCHUDSystem
        {
            get { return mLvPlay.mNPCHUDSystem; }
        }
        public static NPCAreaCheckSystem mNPCAreaCheckSystem
        {
            get { return mLvPlay.mNPCAreaCheckSystem; }
        }
        public static NearbyNpcSystem mNearbyNpcSystem
        {
            get { return mLvPlay.mNearbyNpcSystem; }
        }
        #endregion

        #region MainWorld

        //public static World mainWorld;
        public static Transform mainWorldRoot;
        public static GameObject heroRoot;
        public static GameObject partnerRoot;
        public static GameObject npcRoot;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        public static GameObject transmitRoot;
#endif
        public static GameObject wayPointRoot;

        public static GameObject sceneShowRoot;

        public static Transform fightActorRoot;
        public static Transform modelShowRoot;
        public static Transform skillPreViewRoot;

        public static Hero mainHero;    //玩家角色

        //没有删除接口
        public static Dictionary<ulong, MPartner> partners = new Dictionary<ulong, MPartner>();
        //public static PlayerController mPlayerController;
        public static GameObject keyboardObj;
        public static SuperHero superHero;

        //public static PathFindController pathFindContrl;

        /// <summary>
        /// 主场景其它玩家集合///
        /// </summary>
        public static Dictionary<ulong, Hero> otherActorsDic = new Dictionary<ulong, Hero>();
        public static List<Hero> otherActorList = new List<Hero>();

        /// <summary>
        /// 所有的NPC 字典
        /// </summary>
        public static Dictionary<ulong, Npc> npcsDic = new Dictionary<ulong, Npc>();
        /// <summary>
        /// 所有的NPC 列表
        /// </summary>
        public static List<Npc> npcsList = new List<Npc>();
        /// <summary>
        /// 世界boss 列表
        /// </summary>
        public static List<WorldBossNpc> worldBossList = new List<WorldBossNpc>();
        /// <summary>
        /// 主场景NPC集合///
        /// key1: npcInfoID///
        /// key2: npcUID///
        /// </summary>
        public static Dictionary<uint, Dictionary<ulong, Npc>> npcs = new Dictionary<uint, Dictionary<ulong, Npc>>();        
        /// <summary>
        /// 主场景唯一性NPC集合///
        /// </summary>
        public static Dictionary<uint, Npc> uniqueNpcs = new Dictionary<uint, Npc>();

        public static List<TeleporterActor> teleports = new List<TeleporterActor>();
        public static List<WallActor> walls = new List<WallActor>();
        public static List<SafeAreaActor> safeAreas = new List<SafeAreaActor>();

        //public static List<WayPoint> wayPoints = new List<WayPoint>();

        //自定义UId
        private static ulong m_UID = 0;

        #endregion

        #region FightWorld

        //public static World fightWorld;
        public static FightHero mainFightHero;  //战斗中玩家角色
        public static FightPet mainFightPet;    //战斗中玩家宠物
        public static FightControl fightControl;    //战斗操作控制器

        #endregion

        #region ChooseCareer

        //public static World modelShowWorld;
        //public static World SkillPreViewWorld;

        #endregion

        public static int _loadStage = 0;
        public static int nLoadStage
        {
            get { return _loadStage; }
            set
            {
                if(_loadStage != value)
                {
                    _loadStage = value;
                    if(_loadStage == 3)
                    {
                        List<LevelSystemBase> levelSystems = mLvPlay.mLevelSystems;
                        for (int i = 0, len = levelSystems.Count; i < len; ++i)
                        {
                            LevelSystemBase levelSystem = levelSystems[i];
                            levelSystem.isActive = true;
                        }
                    }
                    else
                    {
                        List<LevelSystemBase> levelSystems = mLvPlay.mLevelSystems;
                        for (int i = 0, len = levelSystems.Count; i < len; ++i)
                        {
                            LevelSystemBase levelSystem = levelSystems[i];
                            levelSystem.isActive = false;
                        }
                    }
                }
            }
        }

        public static CameraController mCameraController;

        public static float fSwitchMapFadeOutTime = 0.25f;
        public static float fSwitchMapFadeInTime = 0.25f;

        //static Vector3 mainHeroRotation = Vector3.zero;

        static Timer enterMapPathFindDelayTimer;
#if UNITY_EDITOR && !ILRUNTIME_MODE
        public static bool ShowSceneRootInspectorFlag;
#endif
        private static int totalCount;
        public static float InitialProgress { get { return 1f - listInitActions.Count / (totalCount * 1f); } }
        public static bool IsInitialFinish { get { return listInitActions.Count == 0; } }

        public static Queue<System.Action> listInitActions = new Queue<System.Action>();

        public static Action onSelfHeroCreated;

        private static bool IsFirstEnterMap = true;
        public static void InitWorld()
        {
            IsFirstEnterMap = true;

            OptionManager.Instance.InitDisplayRoleCounts();

            //地图加载淡出时间
            CSVParam.Data paramData = CSVParam.Instance.GetConfData(957);
            if (paramData != null)
            {
                float.TryParse(paramData.str_value, out fSwitchMapFadeOutTime);
                fSwitchMapFadeOutTime = Mathf.Max(0.1f, fSwitchMapFadeOutTime * 0.001f);
                DebugUtil.LogFormat(ELogType.eNone, "fSwitchMapFadeOutTime {0}", fSwitchMapFadeOutTime.ToString());
            }
            //地图加载淡入时间
            paramData = CSVParam.Instance.GetConfData(958);
            if (paramData != null)
            {
                float.TryParse(paramData.str_value, out fSwitchMapFadeInTime);
                fSwitchMapFadeInTime = Mathf.Max(0.1f, fSwitchMapFadeInTime * 0.001f);
                DebugUtil.LogFormat(ELogType.eNone, "fSwitchMapFadeInTime {0}", fSwitchMapFadeInTime.ToString());
            }

            listInitActions.Clear();

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            //创建输入控制
            listInitActions.Enqueue(new System.Action(() =>
            {
                keyboardObj = new GameObject("KeyboardInput");
                GameObject.DontDestroyOnLoad(keyboardObj);
                KeyboardInput input = keyboardObj.AddComponent<KeyboardInput>();
                Sys_Input.Instance.KeyboardInputRegister(input);
            }));
#endif
#if UNITY_STANDALONE_WIN
#if !UNITY_EDITOR
            Sys_PCExpandChatUI.Instance.WinInitData();
#endif
#endif

            //创建主世界
            listInitActions.Enqueue(new System.Action(() =>
            {
                //mainWorld = new World("MainWorld");
                GameObject go = new GameObject("MainWorld");
                GameObject.DontDestroyOnLoad(go);
                mainWorldRoot = go.transform;
            }));

            //创建3D模型场景展示节点
            listInitActions.Enqueue(new System.Action(() =>
            {
                sceneShowRoot = new GameObject("SceneShowRoot");
                GameObject.DontDestroyOnLoad(sceneShowRoot);

#if UNITY_EDITOR && !ILRUNTIME_MODE

                ShowSceneRootInspector showSceneRootInspector = sceneShowRoot.GetNeedComponent<ShowSceneRootInspector>();
                showSceneRootInspector.CreateAction = (leftType, leftID, leftStatus, leftAnim, rightType, rightID, rightStatus, rightAnim) =>
                {
                    ShowSceneRootInspectorFlag = true;

                    List<Sys_Dialogue.DialogueDataWrap> dialogueDataWraps = new List<Sys_Dialogue.DialogueDataWrap>();
                    Sys_Dialogue.DialogueDataWrap dialogueDataWrap = new Sys_Dialogue.DialogueDataWrap();
                    dialogueDataWrap.ActorType = (uint)EDialogueActorType.Player;
                    dialogueDataWrap.CharID = 0;
                    dialogueDataWrap.ContentID = 0;

                    dialogueDataWrap.LeftShowActorType = (uint)leftType;
                    dialogueDataWrap.LeftShowCharID = (uint)leftID;
                    dialogueDataWrap.LeftShowStatus = (uint)leftStatus;
                    dialogueDataWrap.LeftShowAnimID = (uint)leftAnim;

                    dialogueDataWrap.RightShowActorType = (uint)rightType;
                    dialogueDataWrap.RightShowCharID = (uint)rightID;
                    dialogueDataWrap.RightShowStatus = (uint)rightStatus;
                    dialogueDataWrap.RightShowAnimID = (uint)rightAnim;

                    dialogueDataWraps.Add(dialogueDataWrap);

                    CSVDialogue.Data cSVDialogueData = CSVDialogue.Instance.GetConfData(210000000);
                    ResetDialogueDataEventData resetDialogueDataEventData = PoolManager.Fetch(typeof(ResetDialogueDataEventData)) as ResetDialogueDataEventData;
                    resetDialogueDataEventData.Init(dialogueDataWraps, null, cSVDialogueData);

                    Sys_Dialogue.Instance.OpenDialogue(resetDialogueDataEventData);
                };

                MenuDialogueShowSceneRootInspector menuDialogueShowSceneRootInspector = sceneShowRoot.GetNeedComponent<MenuDialogueShowSceneRootInspector>();
                menuDialogueShowSceneRootInspector.CreateAction = (type, id, animID) =>
                {
                    List<Sys_MenuDialogue.MenuDialogueDataWrap> dialogueDataWraps = new List<Sys_MenuDialogue.MenuDialogueDataWrap>();
                    Sys_MenuDialogue.MenuDialogueDataWrap dialogueDataWrap = new Sys_MenuDialogue.MenuDialogueDataWrap();
                    dialogueDataWrap.ActorType = (uint)type;
                    dialogueDataWrap.ShowCharID = (uint)id;
                    dialogueDataWrap.ShowActorAnimID = (uint)animID;
                    dialogueDataWrap.NeedShowActorAndTitle = true;
                    dialogueDataWraps.Add(dialogueDataWrap);

                    Sys_MenuDialogue.ResetMenuDialogueDataEventData resetMenuDialogueDataEventData = PoolManager.Fetch(typeof(Sys_MenuDialogue.ResetMenuDialogueDataEventData)) as Sys_MenuDialogue.ResetMenuDialogueDataEventData;
                    //Sys_MenuDialogue.ResetMenuDialogueDataEventData resetMenuDialogueDataEventData = Logic.Core.ObjectPool<Sys_MenuDialogue.ResetMenuDialogueDataEventData>.Fetch(typeof(Sys_MenuDialogue.ResetMenuDialogueDataEventData));
                    resetMenuDialogueDataEventData.Init(dialogueDataWraps);

                    Sys_MenuDialogue.Instance.OpenMenuDialogue(resetMenuDialogueDataEventData);
                };
#endif
            }));

            //创建角色父节点
            listInitActions.Enqueue(new System.Action(() =>
            {
                heroRoot = FrameworkTool.CreateGameObject(mainWorldRoot, "HeroRoot");
            }));

            //创建伙伴父节点
            listInitActions.Enqueue(new System.Action(() =>
            {
                partnerRoot = FrameworkTool.CreateGameObject(mainWorldRoot, "ParnterRoot");
            }));

            //创建NPC父节点
            listInitActions.Enqueue(new System.Action(() =>
            {
                npcRoot = FrameworkTool.CreateGameObject(mainWorldRoot, "NPCRoot");
            }));

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            //创建传送父节点
            listInitActions.Enqueue(new System.Action(() =>
            {
                transmitRoot = FrameworkTool.CreateGameObject(mainWorldRoot, "TransmitRoot");
            }));
#endif
            //创建传送父节点
            listInitActions.Enqueue(new System.Action(() =>
            {
                wayPointRoot = FrameworkTool.CreateGameObject(mainWorldRoot, "WayPointRoot");
            }));

            //创建玩家英雄
            listInitActions.Enqueue(new System.Action(() =>
            {
                CreateMainHero();

                //设置玩家英雄的初始方向
                uint paraId = Sys_Role.Instance.FirstEnterGame ? 142 : (uint)141;
                float rotationY = 0;
                float.TryParse(CSVParam.Instance.GetConfData(paraId).str_value, out rotationY);
                mainHero.transform.localRotation = Quaternion.Euler(0, rotationY, 0);
            }));

            //创建相机控制
            listInitActions.Enqueue(new System.Action(() =>
            {
                //mCameraController = mainWorld.CreateActor<CameraController>(0);
                mCameraController = World.AllocActor<CameraController>(0);
                mCameraController.SetFollowActor(mainHero);
                mCameraController.SetVirtualCamera(CameraManager.mCamera);
                mCameraController.EnterWorld();
                //mCameraController.virtualCamera.enabled = false;
            }));

            //创建战斗世界
            listInitActions.Enqueue(new System.Action(() =>
            {
                //fightWorld = new World("FightWorld");

                GameObject root = new GameObject("FightWorld");
                GameObject.DontDestroyOnLoad(root);
                fightActorRoot = root.transform;
            }));

            //打开基础UI
            listInitActions.Enqueue(new System.Action(() =>
            {
                Sys_HUD.Instance.OpenHud();
                UIManager.OpenUI(EUIID.UI_PerForm);
            }));

            //3D模型展示节点(不包括场景的)
            listInitActions.Enqueue(new System.Action(() =>
            {
                //modelShowWorld = new World("ModelShowWorld");

                GameObject root = new GameObject("ModelShowWorld");
                GameObject.DontDestroyOnLoad(root);
                modelShowRoot = root.transform;
            }));

            //技能展示场景
            listInitActions.Enqueue(new System.Action(() =>
            {
                //SkillPreViewWorld = new World("SkillPreViewWorld");

                GameObject root = new GameObject("SkillPreViewWorld");
                GameObject.DontDestroyOnLoad(root);
                skillPreViewRoot = root.transform;
            }));

            ////创建寻路控制器
            //listInitActions.Enqueue(new System.Action(() =>
            //{
            //    pathFindContrl = mainWorld.CreateActor<PathFindController>(6);
            //}));

            //添加状态控制
            listInitActions.Enqueue(new System.Action(() =>
            {
                GameMain.Procedure.StartProcedure<ProcedureNormal>();
                GameObject.Find("AppMain").GetNeedComponent<ProcedureComponent>();
            }));

            totalCount = listInitActions.Count;
        }

        /// <summary>
        /// 执行初始化action
        /// </summary>
        public static void OnExcuteActions()
        {
            if (listInitActions.Count > 0)
            {
                listInitActions.Dequeue().Invoke();
            }
        }

        public static void CreateMainHero()
        {
            _CreaterHero(
                Sys_Role.Instance.Role.RoleId,
                Sys_Role.Instance.Role.HeroId,
                Sys_Title.Instance.curShowTitle,
                Sys_Role.Instance.Role.Name.ToStringUtf8(),
                (ECareerType)Sys_Role.Instance.Role.Career,
                Sys_Role.Instance.Role.Level,
                Sys_Fashion.Instance.GetDressData(),
                Sys_Equip.Instance.GetCurWeapon(),
                PosConvertUtil.Svr2Client(Sys_Map.Instance.svrPosX, Sys_Map.Instance.svrPosY),
                true,
                (uint)Sys_Team.Instance.teamID,
                Sys_Team.Instance.isCaptain(),
                (uint)Sys_Team.Instance.TeamMemsCount,
                (Sys_Pet.Instance.GetMountPet() != null) ? Sys_Pet.Instance.GetMountPet().GetFollowPetInfo() : 0,
                Sys_Pet.Instance.GetMountPetSuitFashionId(),
                (Sys_Pet.Instance.GetFollwPet() != null) ? Sys_Pet.Instance.GetFollwPet().GetFollowPetInfo() : 0,
                Sys_Pet.Instance.GetFollowPetSuitFashionId(),
                (uint)Sys_Pet.Instance.GetMountPerfectRemakeCount(),
                (uint)Sys_Pet.Instance.GetFollowPerfectRemakeCount(),
                Sys_Pet.Instance.IsMountPetShowDemonSpiritFx(),
                Sys_Pet.Instance.IsFollowPetShowDemonSpiritFx(),
                Sys_Cooking.Instance.GetScaleFoodId(),
                0,
                Sys_Attr.Instance.pkAttrs[101] / 10000f,
                Sys_Family.Instance.GetFamilyName(),
                Sys_Family.Instance.familyData.CheckMe() == null ? 0 : Sys_Family.Instance.familyData.CheckMe().Position,
                Sys_Head.Instance.GetTeamLogoId(),
                null,
                Sys_FamilyResBattle.Instance.TeamNum,
                Sys_FamilyResBattle.Instance.MaxCount,
                Sys_FamilyResBattle.Instance.InFamilyBattle,
                Sys_WarriorGroup.Instance.MyWarriorGroup.GroupName,
                Sys_Role.Instance.RoleId == Sys_WarriorGroup.Instance.MyWarriorGroup.LeaderRoleID?1u:0u,
                Sys_Role.Instance.IsBackRole
                );

            onSelfHeroCreated?.Invoke();
            onSelfHeroCreated = null;
        }
        public static void AddHero(MapRole mapRole, bool highModle = false)
        {
            _CreaterHero(
                mapRole.RoleId,
                mapRole.HeroId,
                mapRole.TitleId,
                mapRole.Name.ToStringUtf8(),
                (ECareerType) mapRole.Career,
                mapRole.Level,
                Sys_Fashion.Instance.GetDressData(mapRole.FashionInfo, mapRole.HeroId),
                mapRole.WeaponItemID,
                PosConvertUtil.Svr2Client(mapRole.PosX, mapRole.PosY),
                false,
                mapRole.TeamInfo == null ? 0 : mapRole.TeamInfo.TeamId,
                (mapRole.TeamLogo != 0),
                mapRole.TeamInfo == null ? 0 : mapRole.TeamInfo.MemNum,
                mapRole.MountInfoId,
                mapRole.MountPetSuitAppearance == 0 ? 0 : CSVPetEquipSuitAppearance.Instance.GetConfData(mapRole.MountPetSuitAppearance).show_id,
                mapRole.FollowPetInfo,
                mapRole.PetSuitAppearance == 0 ? 0 : CSVPetEquipSuitAppearance.Instance.GetConfData(mapRole.PetSuitAppearance).show_id,
                mapRole.MountPetBuild,
                mapRole.FollowPetBuild,
                mapRole.MountPetSoul,
                mapRole.FollowPetSoul,
                mapRole.ScaleFoodId,
                mapRole.RoleSign,
                mapRole.MoveSpeed / 10000f,
                mapRole.GuildInfo.Name.ToStringUtf8(),
                mapRole.GuildInfo.Pos,
                mapRole.TeamLogo,
                mapRole,
                0,
                0,
                mapRole != null && mapRole.BattleMapData != null,
                mapRole.BgroupInfo == null ? string.Empty : mapRole.BgroupInfo.Name.ToStringUtf8(),
                mapRole.BgroupInfo == null ? 0 : mapRole.BgroupInfo.Pos,
                mapRole.IsReturn,
                highModle
                );
        }
        public static void RemoveHero(ulong roleID)
        {
            if (otherActorsDic.TryGetValue(roleID, out Hero hero))
            {
                otherActorsDic.Remove(roleID);
                otherActorList.Remove(hero);
                World.CollecActor<Hero>(ref hero);
            }
            //GameCenter.mainWorld.DestroyActor(Hero.Type, roleID);
        }

        public static Hero _CreaterHero(
            ulong RoleId,
            uint HeroId,
            uint TitleId,
            string name,
            ECareerType Career,
            uint level,
            Dictionary<uint, List<dressData>> fashionInfo,
            uint WeaponItemID,
            Vector3 pos,
            bool isMainHero,
            ulong teamID,
            bool IsCaptain,
            uint teamMumCount,
            uint mountID,
            uint mountSuitID,
            uint petID,
            uint petSuitID,
            uint mountBuild,
            uint followPetBuild,
            bool mountPetMagicSoul,
            bool followPetMagicSoul,
            uint scaleFoodId,
            uint sign,
            float speed,
            string familyTitleName,
            uint FamilyPos,
            uint TeamLogoId,
            MapRole mapRole,
            uint guildTeamNum,
            uint guildTeamMaxCount,
            bool enterGuildBattle,
            string bGroupName,
            uint bGroupPos,
            bool isReturn,
            bool highModle = false)
        {
            if (isMainHero)
            {
                if (mainHero != null)
                {
                    DebugUtil.LogErrorFormat("创建main hero {0} 失败， 已经存在main hero {1}", RoleId, mainHero.uID);
                    return null;
                }
            }
            else
            {
                if (otherActorsDic.ContainsKey(RoleId))
                {
                    DebugUtil.LogErrorFormat("创建hero {0} 失败,已经存在相同id hero", RoleId);
                    return null;
                }                
            }

            CSVCharacter.Data cSVCharacterData = CSVCharacter.Instance.GetConfData(HeroId);
            if (cSVCharacterData == null)
            {
                DebugUtil.LogErrorFormat("创建英雄 (RoleId : {0}) 失败,CSVCharacter没有没有 HeroId {0}", RoleId, HeroId);
                return null;
            }

            //Hero hero = mainWorld.CreateActor<Hero>(RoleId);
            Hero hero = World.AllocActor<Hero>(RoleId);
            if (isMainHero)
            {                
                hero.eHeroType = Hero.EHeroType.Self;
                hero.SetName($"heroSelf_{hero.uID.ToString()}_{name}");
                hero.SetParent(GameCenter.heroRoot.transform);
            }
            else
            {
                hero.eHeroType = Hero.EHeroType.Other;
                hero.SetName($"heroOther_{hero.uID.ToString()}_{name}");
                hero.SetParent(GameCenter.heroRoot.transform);
            }

            Vector3 scale = Vector3.one;
            CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(scaleFoodId);
            if (cSVItemData != null)
            {
                scale = new Vector3(cSVItemData.fun_value[0], cSVItemData.fun_value[1], cSVItemData.fun_value[2]);
                scale *= 0.01f;
            }

            //基础属性设置
            hero.heroBaseComponent.HeroID = HeroId;
            hero.heroBaseComponent.Name = name;
            hero.heroBaseComponent.fashData = fashionInfo;
            hero.heroBaseComponent.TitleId = TitleId;
            hero.heroBaseComponent.FamilyName = familyTitleName;
            hero.heroBaseComponent.Pos = FamilyPos;
            hero.heroBaseComponent.Level = level;
            hero.heroBaseComponent.TeamID = teamID;
            hero.heroBaseComponent.IsCaptain = IsCaptain;
            hero.heroBaseComponent.TeamMemNum = teamMumCount;            
            hero.heroBaseComponent.Scale = scale;
            hero.heroBaseComponent.TeamLogeId = TeamLogoId;
            hero.heroBaseComponent.bInFight = sign == 1;
            hero.heroBaseComponent.bGroupName = bGroupName;
            hero.heroBaseComponent.bGPos =bGroupPos;
            hero.heroBaseComponent.bIsReturn= isReturn;
        
            //职业属性设置
            //TODO：看看能不能归并到角色属性里面
            hero.careerComponent.UpdateCareerType(Career);

            //武器属性设置
            if (isMainHero)
            {
                hero.weaponComponent.UpdateWeapon(WeaponItemID, false);
            }
            else
            {
                hero.weaponComponent.CurWeaponID = WeaponItemID;
            }

            //同步位置属性
            //TODO：看看同步组件能不能和移动组件整合
            hero.syncTransformComponent.SyncNetPos(pos);

            //移动属性设置
            //hero.movementComponent.TransformToPosImmediately(hero.syncTransformComponent.netPos);

            NavMeshHit navMeshHit;
            Vector3 hitPos = hero.syncTransformComponent.netPos;
            MovementComponent.GetNavMeshHit(hitPos, out navMeshHit);
            if (navMeshHit.hit)
                hero.transform.position = navMeshHit.position;
            else
                hero.transform.position = hitPos;

            hero.movementComponent.InitNavMeshAgent();

            hero.movementComponent.fMoveSpeed = speed;

            FamilyResBattleComponent familyCp = hero.familyResBattleComponent;
            if (isMainHero)
            {
                mainHero = hero;
                
                familyCp.AssignMain(guildTeamNum, guildTeamMaxCount);
                familyCp.AssignRes(Sys_FamilyResBattle.Instance.Resource);
                if (Sys_Map.Instance.mainHeroBuffList.TryGetValue((uint) MapRoleBuffType.GuildBattleProtection, out var buff)) {
                    familyCp.AssignBuff(buff);
                }
                else {
                    familyCp.AssignBuff(null);
                }
            }
            else
            {
                otherActorsDic[RoleId] = hero;
                otherActorList.Add(hero);
                
                familyCp.AssignOther(mapRole);
            }
            

            Sys_Team.Instance.DoFollow(RoleId);
            Sys_HUD.Instance.AddHeroHUD(hero);   // addhero顺序需要在 enterGuildBattle 前面 
            
            if (enterGuildBattle)
            {                
                Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnCreateFamilyBattle, hero.uID);

                UpdateFamilyTeamNumInBattleResource updateFamilyTeamNumInBattleResource = new UpdateFamilyTeamNumInBattleResource();
                updateFamilyTeamNumInBattleResource.actorId = hero.uID;
                updateFamilyTeamNumInBattleResource.teamNum = hero.familyResBattleComponent.memberCount;
                updateFamilyTeamNumInBattleResource.maxCount = hero.familyResBattleComponent.maxCount;
                Sys_HUD.Instance.eventEmitter.Trigger<UpdateFamilyTeamNumInBattleResource>(Sys_HUD.EEvents.OnUpdateFamilyTeamNum, updateFamilyTeamNumInBattleResource);

                DebugUtil.Log(ELogType.eFamilyBattle,string.Format("OnAddObj--- isMainHero{0}---roleId{1}---TeamNum{2}---MaxCount{3}---",isMainHero, hero.uID, 
                    updateFamilyTeamNumInBattleResource.teamNum, updateFamilyTeamNumInBattleResource.maxCount) );

                uint resourceId = 0;
                if (isMainHero)
                {
                    resourceId = Sys_FamilyResBattle.Instance.Resource;
                }
                else
                {
                    resourceId = mapRole != null && mapRole.BattleMapData != null ? mapRole.BattleMapData.Resource : 0;
                }

                Sys_HUD.Instance.eventEmitter.Trigger<ulong, uint>(Sys_HUD.EEvents.OnUpdateGuildBattleResource, hero.uID, resourceId);

                if (!hero.familyResBattleComponent.isRed)
                {
                    Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnUpdateGuildBattleName, hero.uID);
                }
            }

            hero.LoadModel((actor) =>
            {
                // 加载audioListener
                hero.audioListenerComponent.Set();
                
                actor.modelGameObject.SetActive(false);
                hero.animationComponent.UpdateHoldingAnimations(hero.heroBaseComponent.HeroID, hero.weaponComponent.CurWeaponID, CSVActionState.Instance.GetHeroPreLoadActions(), hero.stateComponent.CurrentState, hero.modelGameObject);

                ulong mountPetUid = 0;
                if (isMainHero)
                {
                    if (Sys_Pet.Instance.mountPetUid != 0)
                    {
                        mountPetUid = Sys_Pet.Instance.mountPetUid;

                        if (mountPetUid == 0)
                        {
                            hero.OffMount();
                        }
                        else
                        {
                            if (hero.Mount != null)
                            {
                                SimplePet simplePet = Sys_Pet.Instance.GetSimplePetByUid(Sys_Pet.Instance.mountPetUid);
                                if (null != simplePet)
                                {
                                    if (Sys_Pet.Instance.CheckMountGradeIsFullByUid(Sys_Pet.Instance.mountPetUid))
                                    {
                                        hero.OnMount(simplePet.PetId * 10 + 1, Sys_Role.Instance.RoleId * 1000000 + simplePet.PetId * 10 + 1, Sys_Pet.Instance.GetMountPetSuitFashionId(), mountBuild, mountPetMagicSoul);
                                    }
                                    else
                                    {
                                        hero.OnMount(simplePet.PetId * 10, Sys_Role.Instance.RoleId * 1000000 + simplePet.PetId * 10 + 1, Sys_Pet.Instance.GetMountPetSuitFashionId(), mountBuild, mountPetMagicSoul);
                                    }
                                }
                            }
                            else
                            {
                                hero.OnMount(mountID, mountPetUid, Sys_Pet.Instance.GetMountPetSuitFashionId(), mountBuild, mountPetMagicSoul, false);
                            }
                        }
                    }
                }
                else
                {
                    if (mountID != 0)
                    {
                        mountPetUid = hero.uID * 1000000 + mountID * 10 + 1;
                    }

                    if (mountPetUid == 0)
                    {
                        hero.OffMount();
                    }
                    else
                    {
                        hero.OnMount(mountID, mountPetUid, mountSuitID, mountBuild, mountPetMagicSoul, false);
                    }
                }
                
                hero.ChangeModelScale(hero.heroBaseComponent.Scale.x, hero.heroBaseComponent.Scale.y, hero.heroBaseComponent.Scale.z);
            });

            if (petID != 0)
            {
                hero.AddPet(petID, hero.uID * 1000000 + petID * 10 + 2, petSuitID, followPetBuild, followPetMagicSoul);
            }

            Sys_HUD.Instance.eventEmitter.Trigger<ulong, uint>(Sys_HUD.EEvents.OnUpdateHeroFunState, hero.uID, sign);
            return hero;
        }
/*
        //备份
        public static Hero _CreaterHero2(
            ulong RoleId,
            uint HeroId,
            uint TitleId,
            string name,
            ECareerType Career,
            uint level,
            Dictionary<uint, List<dressData>> fashionInfo,
            uint WeaponItemID,
            Vector3 pos,
            bool isMainHero,
            ulong teamID,
            bool IsCaptain,
            uint teamMumCount,
            uint mountID,
            uint petID,
            uint scaleFoodId,
            uint sign,
            float speed,
            string familyTitleName,
            uint FamilyPos,
            uint TeamLogoId,
            MapRole mapRole,
            uint guildTeamNum,
            uint guildTeamMaxCount,
            bool enterGuildBattle,
            bool highModle = false)
        {
            CSVCharacter.Data cSVCharacterData = CSVCharacter.Instance.GetConfData(HeroId);
            if (cSVCharacterData == null)
            {
                DebugUtil.LogErrorFormat("创建英雄 (RoleId : {0}) 失败,CSVCharacter没有没有 HeroId {0}", RoleId, HeroId);
                return null;
            }

            //if (mainWorld.GetActor(typeof(Hero), RoleId) != null)

            if (isMainHero)
            {
                if (mainHero != null)
                {
                    DebugUtil.LogErrorFormat("创建main hero {0} 失败， 已经存在main hero {1}", RoleId, mainHero.uID);
                    return null;
                }
            }
            else
            {
                if (otherActorsDic.ContainsKey(RoleId))
                {
                    DebugUtil.LogErrorFormat("创建hero {0} 失败,已经存在相同id hero", RoleId);
                    return null;
                }
            }

            Hero hero = mainWorld.CreateActor<Hero>(RoleId);
            if (isMainHero)
            {
                //TODO:不建议在通用里面加 相对数据 最好只记录绝对数据 相对关系通过外部工具判断
                hero.eHeroType = Hero.EHeroType.Self;
                hero.SetName($"heroSelf_{hero.uID.ToString()}_{name}");
            }
            else
            {
                hero.eHeroType = Hero.EHeroType.Other;
                hero.SetName($"heroOther_{hero.uID.ToString()}_{name}");
            }

            //TODO：角色的数值属性 可以整理到一起 减少Component的数量

            //添加纯数据Components
            //hero.heroBaseComponent = World.AddComponent<HeroBaseComponent>(hero);
            hero.heroBaseComponent.HeroID = HeroId;
            hero.heroBaseComponent.Name = name;
            hero.heroBaseComponent.fashData = fashionInfo;
            hero.heroBaseComponent.TitleId = TitleId;
            hero.heroBaseComponent.FamilyName = familyTitleName;
            hero.heroBaseComponent.Pos = FamilyPos;
            hero.heroBaseComponent.Level = level;
            hero.heroBaseComponent.TeamID = teamID;
            hero.heroBaseComponent.IsCaptain = IsCaptain;
            hero.heroBaseComponent.TeamMemNum = teamMumCount;
            Vector3 scale = Vector3.one;
            CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(scaleFoodId);
            if (cSVItemData != null)
            {
                float rat_X = cSVItemData.fun_value[0] / 100f;
                float rat_Y = cSVItemData.fun_value[1] / 100f;
                float rat_Z = cSVItemData.fun_value[2] / 100f;
                scale = new Vector3(rat_X, rat_Y, rat_Z);
            }
            hero.heroBaseComponent.Scale = scale;
            hero.heroBaseComponent.TeamLogeId = TeamLogoId;
            hero.heroBaseComponent.bInFight = sign == 1;
            //添加职业属性
            //TODO：看看能不能归并到角色属性里面
            //hero.careerComponent = World.AddComponent<CareerComponent>(hero);
            hero.careerComponent.UpdateCareerType(Career);

            //添加武器组件
            //TODO：可能已经没有用
            //hero.weaponComponent = World.AddComponent<WeaponComponent>(hero);
            if (isMainHero)
            {
                hero.weaponComponent.UpdateWeapon(WeaponItemID, false);
            }
            else
            {
                hero.weaponComponent.CurWeaponID = WeaponItemID;
            }

            //添加特效组件
            //TODO：目前看来只要角色升级特效 可以统一角色表现组件 在角色表现里面去添加 HeroDisplay中已经添加了异步加载的管理（外部同步调度）
            //hero.heroFxComponent = World.AddComponent<HeroFxComponent>(hero);

            //添加同步组件
            //hero.syncTransformComponent = World.AddComponent<SyncTransformComponent>(hero);
            hero.syncTransformComponent.SyncNetPos(pos);

            //添加角色状态组件
            //hero.stateComponent = World.AddComponent<StateComponent>(hero);

            //hero.animationComponent = World.AddComponent<AnimationComponent>(hero);            

            //添加移到组件
            //TODO：看看同步组件能不能和移到组件整合
            hero.movementComponent = World.AddComponent<MovementComponent>(hero);
            hero.movementComponent.TransformToPosImmediately(hero.syncTransformComponent.netPos);
            hero.movementComponent.fMoveSpeed = speed;
            
            if (isMainHero)
            {
                HeroSkillComponent heroSkillComponent = World.AddComponent<HeroSkillComponent>(hero);
                hero.skillComponent = heroSkillComponent;
                heroSkillComponent.IsPlayer = true;

                //hero.uploadTransformComponent = World.AddComponent<UploadTransformComponent>(hero);
                //World.AddComponent<HangUpTipsComponent>(hero);
                //World.AddComponent<HangUpComponent>(hero);
                
                World.AddComponent<AudioListenerComponent>(hero);
                //World.AddComponent<NpcActiveListenerComponent>(hero);
                //World.AddComponent<WorldBossListenerComponent>(hero);
                //World.AddComponent<NearbyNpcComponent>(hero);

                //World.AddComponent<CheckHeroVisualComponent>(hero);
                //hero.pathComponent = World.AddComponent<PathComponent>(hero);

                //创建控制器
                //控制器和角色 可以使用同一个ID 方便索引
                //TODO：移到到外部 可以切换主角
                mPlayerController = mainWorld.CreateActor<PlayerController>(2);
                mPlayerController.SetTarget(hero);

                //TODO 关联一个Component
                hero.gameObject.AddComponent<SendPositionToShader>();

                //hero.gameObject.transform.localRotation = Quaternion.Euler(mainHeroRotation.x, mainHeroRotation.y, mainHeroRotation.z);

                mainHero = hero;

                FamilyResBattleComponent component = hero.familyResBattleComponent;
                component.memberCount = guildTeamNum;
                component.maxCount = guildTeamMaxCount;
                component.Camp = Sys_FamilyResBattle.Instance.redCampId;
            }
            else
            {
                otherActorsDic[RoleId] = hero;
                otherActorList.Add(hero);

                // 在家族资源战的时候，才会添加该组件
                if (mapRole.BattleMapData != null) {
                    FamilyResBattleComponent component = hero.familyResBattleComponent;
                    component.Camp = mapRole.BattleMapData.Camp;

                    uint res = mapRole.BattleMapData.Resource;
                    component.resource = res;

                    if (mapRole.BattleMapData.Leader != null)
                    {
                        uint memberCount = mapRole.BattleMapData.Leader.MemberCount;
                        component.memberCount = memberCount;
                        component.maxCount = mapRole.BattleMapData.Leader.MaxCount;
                    }
                    else
                    {
                        component.memberCount = 0;
                        component.maxCount = 0;
                    }
                }
            }

            //hero.roleActionComponent = World.AddComponent<RoleActionComponent>(hero);
            FollowCaptain.DoMemberFollow(RoleId);
            
            Sys_HUD.Instance.AddHeroHUD(hero);
            DebugUtil.Log(ELogType.eFamilyBattle, "OnAddObj:  enterGuildBattle: " + enterGuildBattle+"  roleId:"+hero.uID);
            if (enterGuildBattle)
            {
                //Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnClearTitle, hero.uID);
                //Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnClearTeamLogo, hero.uID);
                //Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnClearTeamFx, hero.uID);

                Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnCreateFamilyBattle, hero.uID);

                UpdateFamilyTeamNumInBattleResource updateFamilyTeamNumInBattleResource = new UpdateFamilyTeamNumInBattleResource();
                updateFamilyTeamNumInBattleResource.actorId = hero.uID;
                updateFamilyTeamNumInBattleResource.teamNum = hero.familyResBattleComponent.memberCount;
                updateFamilyTeamNumInBattleResource.maxCount = hero.familyResBattleComponent.maxCount;
                Sys_HUD.Instance.eventEmitter.Trigger<UpdateFamilyTeamNumInBattleResource>(Sys_HUD.EEvents.OnUpdateFamilyTeamNum, updateFamilyTeamNumInBattleResource);

                DebugUtil.Log(ELogType.eFamilyBattle, "    OnAddObj  :"+ "isMainHero:  " + isMainHero + "   roleId:  " + hero.uID  + "TeamNum: " + updateFamilyTeamNumInBattleResource.teamNum +
               "MaxCount: " + updateFamilyTeamNumInBattleResource.maxCount);

                uint resourceId = 0;
                if (isMainHero)
                {
                    resourceId = Sys_FamilyResBattle.Instance.Resource;
                }
                else
                {
                    resourceId = mapRole != null && mapRole.BattleMapData != null ? mapRole.BattleMapData.Resource : 0;
                }

                Sys_HUD.Instance.eventEmitter.Trigger<ulong, uint>(Sys_HUD.EEvents.OnUpdateGuildBattleResource, hero.uID, resourceId);
                
                if (!hero.familyResBattleComponent.isRed)
                {
                    Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnUpdateGuildBattleName, hero.uID);
                }
            }

            //if (isMainHero) {
            //    hero.rigidBodyComponent = World.AddComponent<RigidBodyComponent>(hero);
            //}

            hero.LoadModel((actor) =>
            {
                actor.modelGameObject.SetActive(false);
                hero.animationComponent.UpdateHoldingAnimations(hero.heroBaseComponent.HeroID, hero.weaponComponent.CurWeaponID, CSVActionState.Instance.GetHeroPreLoadActions(), hero.stateComponent.CurrentState, hero.modelGameObject);

                if (isMainHero)
                {
                    if (Sys_Pet.Instance.mountPetUid == 0)
                    {
                        hero.OffMount();
                    }
                    else
                    {
                        ClientPet mount = Sys_Pet.Instance.GetMountPet();
                        if (mount != null)
                        {
                            hero.OnMount(mount.petData.id, Sys_Pet.Instance.mountPetUid, false);
                        }
                    }
                    hero.ChangeModelScale(hero.heroBaseComponent.Scale.x, hero.heroBaseComponent.Scale.y, hero.heroBaseComponent.Scale.z);
                }
                else
                {
                    if (mountID == 0)
                    {
                        hero.OffMount();
                    }
                    else
                    {
                        hero.OnMount(mountID, hero.uID * 1000000 + mountID * 10 + 1, false);
                    }
                    hero.ChangeModelScale(hero.heroBaseComponent.Scale.x, hero.heroBaseComponent.Scale.y, hero.heroBaseComponent.Scale.z);
                }
            });

            if (petID != 0)
            {
                hero.AddPet(petID, hero.uID * 1000000 + petID * 10 + 2);
            }

            Sys_HUD.Instance.eventEmitter.Trigger<ulong, uint>(Sys_HUD.EEvents.OnUpdateHeroFunState, hero.uID, sign);

            return hero;
        }
*/
        public static void AddTeleporter(Sys_Map.TelData telData)
        {
            m_UID++;
            CSVCheckseq.Instance.TryGetValue(telData.condId, out CSVCheckseq.Data checkseqData);
            TeleporterActor teleporter = World.AllocActor<TeleporterActor>(m_UID);
            teleporter.SetTelData(telData, checkseqData);
            teleports.Add(teleporter);
        }

        public static void AddWall(Sys_Map.WallData wallData)
        {
            m_UID++;
            //WallActor tel = mainWorld.CreateActor<WallActor>(m_UID);
            WallActor actor = World.AllocActor<WallActor>(m_UID);
            actor.SetParent(mainWorldRoot);
            actor.data = wallData;
            walls.Add(actor);
        }

        public static void AddSafeArea(Sys_Map.SateAreaData wallData)
        {
            m_UID++;
            //SafeAreaActor actor = mainWorld.CreateActor<SafeAreaActor>(m_UID);
            SafeAreaActor actor = World.AllocActor<SafeAreaActor>(m_UID);
            actor.data = wallData;
            safeAreas.Add(actor);
        }

        private static void DestroyTeleporters()
        {
            for (int i = teleports.Count - 1; i >= 0; --i)
            {
                //mainWorld.DestroyActor(teleports[i]);
                TeleporterActor teleporter = teleports[i];
                World.CollecActor(ref teleporter);
            }
            teleports.Clear();
        }

        private static void DestroyWalls()
        {
            for (int i = walls.Count - 1; i >= 0; --i)
            {
                //mainWorld.DestroyActor(walls[i]);
                WallActor actor = walls[i];
                World.CollecActor(ref actor);
            }
            walls.Clear();
        }

        private static void DestroySafeAreas()
        {
            for (int i = safeAreas.Count - 1; i >= 0; --i)
            {
                //mainWorld.DestroyActor(safeAreas[i]);
                SafeAreaActor actor = safeAreas[i];
                World.CollecActor(ref actor);                
            }
            safeAreas.Clear();
        }

        //public static void AddWayPoint(Vector3 pos)
        //{
        //    m_UID++;
        //    //WayPoint wayPoint = mainWorld.CreateActor<WayPoint>(m_UID);
        //    //wayPoint.LoadModel("Prefab/Fx/scene/Fx_Manage.prefab", (actor) =>
        //    //{
        //    //    Vector3 targetPos = PosConvertUtil.Svr2Client((uint)(pos.x * 100), (uint)(-pos.z * 100));
        //    //    UnityEngine.AI.NavMeshHit navMeshHit;
        //    //    MovementComponent.GetNavMeshHit(targetPos, out navMeshHit);
        //    //    actor.transform.localPosition = navMeshHit.position;
        //    //    //AddComponents(npc, mapNpc);
        //    //});
        //    //wayPoints.Add(wayPoint);
        //}

        //public static void DestroyWayPoint(WayPoint wayPoint)
        //{
        //    //mainWorld.DestroyActor(wayPoint);
        //    //wayPoints.Remove(wayPoint);
        //}

        //public static void DestroyWayPoints()
        //{
        //    //if (wayPoints.Count > 0)
        //    //{
        //    //    for (int i = wayPoints.Count - 1; i >= 0; --i)
        //    //    {
        //    //        mainWorld.DestroyActor(wayPoints[i]);
        //    //    }
        //    //}
        //    //wayPoints.Clear();
        //}

        //TODO：Test
        static bool isCacheParnterLoaded = false;
        public static void CreateCacheParnters()
        {
            if (isCacheParnterLoaded)
                return;

            foreach (var data in CSVPartner.Instance.GetAll())
            {
                CreateParnter(data.id);
            }
            isCacheParnterLoaded = true;
        }

        //TODO：Temp
        static bool isSuperHeroLoaded = false;
        public static void CreateSuperHero(uint superHeroID)
        {
            if (isSuperHeroLoaded)
                return;

            CSVCharacter.Data cSVCharacterData = CSVCharacter.Instance.GetConfData(superHeroID);
            if (cSVCharacterData == null)
                return;

            //superHero = mainWorld.CreateActor<SuperHero>(superHeroID);
            superHero = World.AllocActor<SuperHero>(superHeroID);
            superHero.SetName($"SuperHero_{superHeroID.ToString()}");
            superHero.SetParent(GameCenter.partnerRoot.transform);

            superHero.SuperHeroID = superHeroID;
            superHero.LoadModel(cSVCharacterData.model, (actor) =>
            {
                superHero.gameObject.SetActive(false);
            });
            isSuperHeroLoaded = true;
        }

        public static void CreateParnter(uint partnerID, bool highModel = false)
        {
            CSVPartner.Data cSVPartnerData = CSVPartner.Instance.GetConfData(partnerID);
            if (cSVPartnerData == null)
                return;

            //MPartner parnter = mainWorld.CreateActor<MPartner>(partnerID);
            MPartner parnter = World.AllocActor<MPartner>(partnerID);
            parnter.SetParent(GameCenter.partnerRoot.transform);
            parnter.SetName($"partner_{partnerID.ToString()}");

            //创建失败直接不走下面流程
            if (parnter == null)
                return;

            parnter.PartnerID = partnerID;
            //parnter.careerComponent = World.AddComponent<CareerComponent>(parnter);
            parnter.careerComponent.UpdateCareerType((ECareerType)cSVPartnerData.occupation);

            //parnter.weaponComponent = World.AddComponent<WeaponComponent>(parnter);
            parnter.weaponComponent.UpdateWeapon(cSVPartnerData.weaponID, false);

            parnter.LoadModel(highModel ? cSVPartnerData.model_show : cSVPartnerData.model, (actor) =>
            {
                parnter.gameObject.SetActive(false);
            });
            partners.Add(partnerID, parnter);
        }

        static void ClearMapActors()
        {
            DebugUtil.Log(ELogType.eNone, "ClearMapActors()");

            for (int i = 0; i < npcsList.Count; ++i)
            {
                Npc npc = npcsList[i];
                //Npc.Destroy(ref npc);
                World.CollecActor(ref npc);
                //mainWorld.DestroyActor(Npc.Type, npc.uID);
            }
            npcs.Clear();
            npcsList.Clear();
            npcsDic.Clear();
            uniqueNpcs.Clear();
            worldBossList.Clear();

            DestroyTeleporters();
            DestroyWalls();
            DestroySafeAreas();            
            //DestroyWayPoints();

            m_UID = 0;

            //if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
            //    return;

            //var actorErator = otherActorsDic.GetEnumerator();
            //while (actorErator.MoveNext())
            //{
            //    Hero hero = actorErator.Current.Value;
            //    mainWorld.DestroyActor(Hero.Type, hero.uID);
            //}
            //otherActorsDic.Clear();
            for (int i = 0, len = otherActorList.Count; i < len; ++i)
            {
                Hero hero = otherActorList[i];
                World.CollecActor<Hero>(ref hero);
            }
            otherActorList.Clear();
            otherActorsDic.Clear();
        }

        public static void ChangeMap()
        {
            if (mainHero != null)
                mainHero.movementComponent.Stop();
            
            ClearMapActors();
            UIManager.CloseUI(EUIID.UI_Dialogue);
            GameMain.Procedure.TriggerEvent(null, (int)EProcedureEvent.EnterNormal);
            Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnEnterMap);
            Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnClearActorFx);
        }

        public static void EnterMap()
        {
            EnableMainHeroMove(true);

            if (mainHero != null)
            {
                //重置mainhero位置
                if (!IsFirstEnterMap)
                {
                    mainHero.syncTransformComponent.SyncNetPos(PosConvertUtil.Svr2Client(Sys_Map.Instance.svrPosX, Sys_Map.Instance.svrPosY));
                    //mainHero.movementComponent.TransformToPosImmediately(mainHero.syncTransformComponent.netPos);

                    NavMeshHit navMeshHit;
                    Vector3 hitPos = mainHero.syncTransformComponent.netPos;
                    MovementComponent.GetNavMeshHit(hitPos, out navMeshHit);
                    if (navMeshHit.hit)
                        mainHero.transform.position = navMeshHit.position;
                    else
                        mainHero.transform.position = hitPos;

                    mainHero.movementComponent.InitNavMeshAgent();
                }

                mainHero.ResetPetPos();
            }

            IsFirstEnterMap = false;

            enterMapPathFindDelayTimer?.Cancel();
            enterMapPathFindDelayTimer = Timer.Register(1f, () =>
            {
                //pathFindContrl?.ContinueFind();
                Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnAutoPathFind);
            });


            if (Sys_GoddnessTrial.Instance.IsInstance() == false)
            {
                CSVMapInfo.Data infoData = CSVMapInfo.Instance.GetConfData(Sys_Map.Instance.CurMapId);

                Func<bool> predication = () => {
                    return UIScheduler.popTypes[EUIPopType.WhenMaininterfaceRealOpenning].Invoke() && GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Fight && Sys_Role.Instance.hasSyncFinished &&
                           !SceneManager.bMainSceneLoading;
                };
                UIScheduler.Push(EUIID.UI_MapTips, LanguageHelper.GetTextContent(infoData.name), null, true, predication);

                if (infoData.PromptForMapUnlocking && Sys_Map.Instance.firstEnterMaps.Contains(Sys_Map.Instance.CurMapId)) {
                    UIScheduler.Push(EUIID.UI_MapFirstEnter, Sys_Map.Instance.CurMapId, null, true, predication);
                    UIManager.SendMsg(EUIID.UI_MapFirstEnter, Sys_Map.Instance.CurMapId);
                }
            }
            
            Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnHeroTel);
            UIManager.CloseUI(EUIID.UI_Mine_Result, true);//暗雷结算进入地图前关闭
            UIManager.CloseUI(EUIID.UI_MerchantFleet_Settlement);
        }

        public static void EnableMainHeroMove(bool enable)
        {
            if (mainHero != null && mainHero.movementComponent != null)
            {
                mainHero.movementComponent.enableflag = enable;
                mainHero.movementComponent.Stop();
                //mainHero.pathComponent.OnEnable(enable);
                mWayPointSystem.OnEnable(enable);

                //UploadTransformComponent uploadCom = World.GetComponent<UploadTransformComponent>(mainHero);
                //UploadTransformComponent uploadCom = mainHero.uploadTransformComponent;
                //if (uploadCom != null)
                //    uploadCom.OnEnableMainHero(enable);
                GameCenter.mUploadTransformSystem.EnableMainHero = enable;
            }
        }

        public static void SetInputEnable(bool enable)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            if (keyboardObj != null)
                keyboardObj.SetActive(enable);
#endif
            //if (mPlayerController != null && !enable) {
            //    mPlayerController.StopJoystickMove();
            //    // 中断寻路
            //    Sys_PathFind.Instance.eventEmitter.Trigger<bool>(Sys_PathFind.EEvents.OnPathFind, false);
            //}
        }

        public static void ShakeCamera(uint shakeId)
        {
            ShakeCamera(mCameraController, shakeId);
        }

        public static void ShakeCamera(CameraController mCameraController, uint shakeId)
        {
            CSVShock.Data data = CSVShock.Instance.GetConfData(shakeId);
            if (data != null && mCameraController != null)
            {
                Vector3 strength = new Vector3(data.strength_x / 1000f, data.strength_y / 1000f, data.strength_z / 1000f);
                mCameraController.DoShake(data.duration / 1000f, strength, (int)data.vibrato, data.randomness);
            }
        }

        public static void Dispose()
        {
            listInitActions.Clear();
            ActionCtrl.Instance.Reset();
            EffectUtil.Instance.Dispose();

            ClearMapActors();
            //mainWorld?.Dispose();
            //mainWorld = null;             

            if (mCameraController != null && mCameraController.virtualCamera != null)
            {
                // 下线时候，关闭相机的自动追踪
                mCameraController.virtualCamera.enabled = false;
            }

            //mainHero = null;
            if (mainHero != null)
            {
                World.CollecActor(ref mainHero);
            }

            //superHero = null;
            if (superHero != null)
            {
                World.CollecActor(ref superHero);
            }            

            //pathFindContrl = null;

            //partners.Clear();
            ClearPartner();

            //mPlayerController = null;
            //mainWorld.DestroyActor(pathFindContrl);
            //pathFindContrl?.Dispose();
            keyboardObj = null;
            m_UID = 0;

            //var actorErator = otherActorsDic.GetEnumerator();
            //while (actorErator.MoveNext())
            //{
            //    Hero hero = actorErator.Current.Value;
            //    mainWorld.DestroyActor(Hero.Type, hero.uID);
            //}
            //otherActorsDic.Clear();

            //uniqueNpcs.Clear();
            //npcs.Clear();
            //npcsList.Clear();
            //teleports.Clear();
            //walls.Clear();
            //safeAreas.Clear();

            mainFightHero = null;
            mainFightPet = null;
            fightControl = null;

            //mCameraController = null;
            if (mCameraController != null)
            {
                World.CollecActor(ref mCameraController);
            }            

            enterMapPathFindDelayTimer?.Cancel();
            enterMapPathFindDelayTimer = null;

            //fightWorld?.Dispose();
            //fightWorld = null;
            //modelShowWorld?.Dispose();
            //modelShowWorld = null;
            //SkillPreViewWorld?.Dispose();
            //SkillPreViewWorld = null;

            if (mainWorldRoot != null)
            {
                GameObject.DestroyImmediate(mainWorldRoot.gameObject);
                mainWorldRoot = null;
            }
            if (fightActorRoot != null)
            {
                GameObject.DestroyImmediate(fightActorRoot.gameObject);
                fightActorRoot = null;
            }
            if (modelShowRoot != null)
            {
                GameObject.DestroyImmediate(modelShowRoot.gameObject);
                modelShowRoot = null;
            }
            if (skillPreViewRoot != null)
            {
                GameObject.DestroyImmediate(skillPreViewRoot.gameObject);
                skillPreViewRoot = null;
            }

            GameObject.DestroyImmediate(heroRoot);
            heroRoot = null;
            GameObject.DestroyImmediate(partnerRoot);
            partnerRoot = null;
            GameObject.DestroyImmediate(npcRoot);
            npcRoot = null;
            GameObject.DestroyImmediate(sceneShowRoot);
            sceneShowRoot = null;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            GameObject.DestroyImmediate(transmitRoot);
            transmitRoot = null;
#endif

            //NPCHelper.Dispose();
            UIManager.CloseUI(EUIID.UI_PerForm, false, false);
            UIManager.CloseUI(EUIID.UI_Dialogue);
            UIManager.CloseUI(EUIID.HUD);
            isCacheParnterLoaded = false;
            isSuperHeroLoaded = false;
        }

        public static void AllowMainHeroMove(bool allow)
        {
            //MovementComponent movementComponent = World.GetComponent<MovementComponent>(mainHero);

            MovementComponent movementComponent = null;
            if (mainHero != null)
            {
                movementComponent = mainHero.movementComponent;

                if (movementComponent != null)
                {
                    if (!allow)
                    {
                        movementComponent.Stop();
                    }
                    movementComponent.enableflag = allow;
                }
            }
        }

        public static void AllowTeamPlayerMove(bool allow)
        {
            foreach (var teamPlayer in otherActorsDic.Values)
            {
                if (Sys_Team.Instance.isTeamMem(teamPlayer.uID))
                {
                    //MovementComponent movementComponent = World.GetComponent<MovementComponent>(teamPlayer);
                    MovementComponent movementComponent = teamPlayer.movementComponent;
                    if (movementComponent != null)
                    {
                        if (!allow)
                        {
                            movementComponent.Stop();
                        }
                        movementComponent.enableflag = allow;
                    }
                }
            }
        }

        /// <summary>
        /// 是否启用NPC点击///
        /// </summary>
        /// <param name="enable"></param>
        public static void EnableNpcClick(bool enable)
        {
            for (int i = 0; i < npcsList.Count; ++i)
            {
                Npc npc = npcsList[i];

                //ClickComponent clickComponent = World.GetComponent<ClickComponent>(npc);
                ClickComponent clickComponent = npc.clickComponent;

                if (clickComponent != null)
                    clickComponent.enableFlag = enable;

                //NPC 未添加过该组件
                //LongPressComponent longpressComponent = World.GetComponent<LongPressComponent>(npc);
                //LongPressComponent longpressComponent = World.GetComponent<LongPressComponent>(npc);
                //if (longpressComponent != null)
                //    longpressComponent.enableFlag = enable;

            }
        }

        /// <summary>
        /// 找到场景中符合要求的最近的NPC///
        /// </summary>
        /// <param name="npcId"></param>
        /// <param name="npc"></param>
        /// <param name="guid"></param>
        /// <param name="checkVisual">是否检测可见性</param>
        public static void FindNearestNpc(uint npcId, out Npc npc, out ulong guid, bool checkVisual = true)
        {
            float minDistance = float.MaxValue;
            npc = null;
            guid = 0;

            if (!npcs.ContainsKey(npcId))
                return;

            foreach (var kvp in npcs[npcId])
            {
                Npc tmpNpc = kvp.Value;
                Transform t = tmpNpc.transform;
                if (t != null)
                {
                    float min = Vector3.SqrMagnitude(t.position - mainHero.transform.position);
                    if (min < minDistance)
                    {
                        //VisualComponent visualComponent = World.GetComponent<VisualComponent>(kvp.Value);
                        VisualComponent visualComponent = tmpNpc.VisualComponent;
                        if ((visualComponent != null && visualComponent.Visiable) || checkVisual == false)
                        {
                            minDistance = min;
                            npc = tmpNpc;
                            guid = kvp.Key;
                        }
                    }
                }
            }
        }

        public static void FindNearestNPCInAreaWithFunction(float range, EFunctionType functionType, out Npc npc)
        {
            float minDistance = float.MaxValue;
            npc = null;

            for (int i = 0; i < npcsList.Count; ++i)
            {
                Npc tempNpc = npcsList[i];
                Transform t = tempNpc.transform;
                if (t != null)
                {
                    float distance = Vector3.Magnitude(t.position - mainHero.transform.position);
                    if (distance < range && distance < minDistance)
                    {
                        if (tempNpc.NPCFunctionComponent.HasActiveFunction(functionType))
                        {
                            //VisualComponent visualComponent = World.GetComponent<VisualComponent>(tempNpc);
                            VisualComponent visualComponent = tempNpc.VisualComponent;

                            if (visualComponent != null && visualComponent.Visiable)
                            {
                                minDistance = distance;
                                npc = tempNpc;
                            }
                        }
                    }
                }
            }
        }

        public static Hero GetSceneHero(ulong roleId, bool ignoreMainHero = false)
        {
            if (otherActorsDic.TryGetValue(roleId, out Hero hero))
                return hero;

            if (!ignoreMainHero && mainHero != null && mainHero.UID == roleId)
                return mainHero;

            return null;
        }

        public static bool TryGetSceneNPC(ulong npcId, out Npc npc)
        {
            return npcsDic.TryGetValue(npcId, out npc);
        }

        public static void ClearPartner()
        {
            foreach (MPartner v in partners.Values)
            {
                MPartner partner = v;
                World.CollecActor(ref partner);
            }

            partners.Clear();
        }

        public static IReadOnlyList<Npc> Npcs()
        {
            return npcsList;
        }
    }
}

