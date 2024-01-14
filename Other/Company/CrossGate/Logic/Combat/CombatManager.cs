using Logic;
using Table;
using UnityEngine;
using Lib.Core;
using System;

public class CombatManager : Logic.Singleton<CombatManager>
{
    public enum EEvents
    {
        DestroyMaterial,           //Gameobject被销毁时，删除实例化材质
        WaitAttack_CombineAttack,   //等待合击
        HinderNode, //当前行为的阻碍节点
        /// <summary>
        /// 等待反击, 被反击者UnitId，反击者UnitId，反击字符串标记
        /// </summary>
        WaitFightBackOver,
    }

    public readonly EventEmitter<EEvents> m_EventEmitter = new EventEmitter<EEvents>();

    public uint m_CurBattleSceneId;
    public Vector3 CombatSceneCenterPos;
    public bool PosFollowSceneCamera;
    public float m_TimeScale = 1f;

    public uint m_BattleId;
    
    public float m_CurRoundStartTime;

    public float m_LeftUpPosX;
    public float m_LeftUpPosZ;

    public float m_RightUpPosX;
    public float m_RightUpPosZ;

    public float m_RightDownPosX;
    public float m_RightDownPosZ;

    public float m_LeftDownPosX;
    public float m_LeftDownPosZ;

    public float m_Scene_U2D_AxisX;
    public float m_Scene_U2D_AxisZ;

    public float m_Scene_R2L_AxisX;
    public float m_Scene_R2L_AxisZ;

    public float m_Scene_D2U_AxisX;
    public float m_Scene_D2U_AxisZ;

    public float m_Scene_L2R_AxisX;
    public float m_Scene_L2R_AxisZ;

    /// <summary>
    /// =0进副本战斗，=1进技能演示战斗, =999模拟战斗, =10001为UIModel展示
    /// </summary>
    public int m_CombatStyleState;

    public bool m_IsRunEnterScene;

    public bool m_IsFight;

    public float[] m_SceneBorders;

    public Matrix4x4 m_BaseCameraW2CMatrix = new Matrix4x4(new Vector4(-0.6691308f, 0.4472357f, -0.5935019f, 0f),
                                                            new Vector4(0f, 0.7986355f, 0.601815f, 0f),
                                                            new Vector4(-0.7431448f, -0.4026929f, 0.5343916f, 0f),
                                                            new Vector4(2.331578f, -10.05229f, -33.99099f, 1f));

    /// <summary>
    /// 0,1,2为调整后的X,Y,Z轴
    /// </summary>
    public Vector3[] m_AdjustSceneViewAxiss = new Vector3[3];

#if UNITY_EDITOR
    public uint m_KeepCombat;
    public bool m_EnableTimeScaleKeyCode;
#endif

    private CSVBattleType.Data _battleTypeTb;
    public CSVBattleType.Data m_BattleTypeTb
    {
        get
        {
            if (_battleTypeTb == null)
            {
                DebugUtil.LogError($"特别注意！！！ 使用CombatManager.m_BattleTypeTb数据时是在该属性赋值前，要注意");

                //保底处理
                _battleTypeTb = CSVBattleType.Instance.GetConfData(1u);
            }

            return _battleTypeTb;
        }
        set
        {
            _battleTypeTb = value;
        }
    }
    public uint m_BattlePosType;
    public bool m_IsNotMirrorPos;

    private Transform _workStreamTrans;
    public Transform m_WorkStreamTrans
    {
        get
        {
            if (_workStreamTrans == null)
            {
                _workStreamTrans = new GameObject("WorkStreamWorld").transform;
                GameObject.DontDestroyOnLoad(_workStreamTrans);

                m_UseBulletTrans = new GameObject("UseBullet").transform;
                m_UseBulletTrans.SetParent(m_WorkStreamTrans);

                m_FreeBulletTrans = new GameObject("FreeBullet").transform;
                m_FreeBulletTrans.SetParent(m_WorkStreamTrans);
            }

            return _workStreamTrans;
        }
    }
    public Transform m_UseBulletTrans;
    public Transform m_FreeBulletTrans;

    public uint m_CombatAI_7_Increase;
    public uint m_CombatAI_8_Increase;
    public uint m_CombatAI_9_Increase;
    public uint m_CombatAI_10_Increase;

    public bool m_IsLoadMobsOver;
    private uint _delayFrameCount;
    private Action _loadMobsOverAction;

    public bool m_IsAwake;

    private string _combatTimeScale;
    private bool _isNeedCombatTimeScale;

    public float m_SkillShoutStartTime;

    public CombatEntity m_CombatEntity;

    public uint m_CommonBattleSceneId;
    public string m_CommonBattleScenePrefabPath;

    #region 战斗生命周期
    public void OnAwake()
    {
        if (m_IsAwake)
            return;

        m_IsAwake = true;

        m_CombatEntity = EntityFactory.Create<CombatEntity>();

        CombatObjectPool.Instance.OnAwake();
        StateCategoryManager.Instance.OnAwake();
        CombatModelManager.Instance.OnAwake();
        MobManager.Instance.OnAwake();
        FxManager.Instance.OnAwake();
        Net_Combat.Instance.OnAwake();
        ShaderController.Instance.OnAwake();

#if UNITY_EDITOR_NO_USE
        CombatSimulateManager.Instance.OnAwake();
#endif

#if UNITY_EDITOR
        string enableTimeScaleKeyCodeStr = "Combat_m_EnableTimeScaleKeyCode";
        if (PlayerPrefs.HasKey(enableTimeScaleKeyCodeStr))
            m_EnableTimeScaleKeyCode = PlayerPrefs.GetInt(enableTimeScaleKeyCodeStr) == 1;
        else
            m_EnableTimeScaleKeyCode = false;
#endif

        var borderStrs = CSVParam.Instance.GetConfData(420).str_value.Split('|');
        m_SceneBorders = new float[borderStrs.Length];
        for (int i = 0; i < borderStrs.Length; i++)
        {
            m_SceneBorders[i] = float.Parse(borderStrs[i]) * 0.0001f;
        }

        m_CombatAI_7_Increase = uint.Parse(CSVParam.Instance.GetConfData(601).str_value);
        m_CombatAI_8_Increase = uint.Parse(CSVParam.Instance.GetConfData(602).str_value);
        m_CombatAI_9_Increase = uint.Parse(CSVParam.Instance.GetConfData(603).str_value);
        m_CombatAI_10_Increase = uint.Parse(CSVParam.Instance.GetConfData(604).str_value);

        m_CommonBattleSceneId = uint.Parse(CSVParam.Instance.GetConfData(423u).str_value);
        m_CommonBattleScenePrefabPath = CSVParam.Instance.GetConfData(424u).str_value;

        m_TimeScale = 1f;
    }

    public void OnEnable()
    {
        OnAwake();

        CombatObjectPool.Instance.OnEnable();
        MobManager.Instance.OnEnable();
        CombatModelManager.Instance.OnEnable();
        BulletManager.Instance.OnEnable();
        Net_Combat.Instance.OnEnable();

        m_IsFight = true;

        OnTransData();

        PreLoadFx();

        if (m_CombatStyleState == 0)
        {
            CombatSceneEntity.Instance.GetNeedComponent<CombatCheckCameraComponent>();

            Net_Combat.Instance.eventEmitter.Trigger<bool>(Net_Combat.EEvents.OnBattleOver, false);
        }

        if (m_CombatStyleState == 0)
            SDKManager.SDK_SetGameFightStatus(true);

        OnInitTimeScale();
    }

    public void OnUpdate()
    {
        if (!m_IsAwake)
        {
            //DebugUtil.LogError($"没有走OnLogin就执行Update，生命周期流程出错");
            OnAwake();
        }

        ObjectEvents.Instance.Update();
        FxManager.Instance.OnUpdate();

        if (m_IsFight)
        {
            if (_delayFrameCount > 0u)
            {
                --_delayFrameCount;

                if (_delayFrameCount == 0u)
                {
                    if (_loadMobsOverAction != null)
                    {
                        _loadMobsOverAction();
                        _loadMobsOverAction = null;
                    }
                }
            }

            Net_Combat.Instance.OnUpdate();

#if UNITY_EDITOR
            if (m_EnableTimeScaleKeyCode)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    SetTimeScale(0.3f, true, false);
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    SetTimeScale(1f, true, false);
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    SetTimeScale(2f, true, false);
                }
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    SetTimeScale(3f, true, false);
                }
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                //Net_Combat.Instance.DoBattleResult();

                MobEntity mobEntity = MobManager.Instance.GetPlayerMob();
                if (mobEntity != null)
                    Net_Combat.Instance.BattleShowRoundEndReq(m_BattleId, mobEntity.m_MobCombatComponent.m_BattleUnit.UnitId, Net_Combat.Instance.m_CurRound);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
#if !ILRUNTIME_MODE
                if (CreateCombatDataTest.s_ClientType)
                {
                    Sys_Fight.Instance.EndReq();
                    Net_Combat.Instance.m_BattleOverType = 2u;
                    Net_Combat.Instance.DoBattleResult();
                }
                else
                    Sys_Fight.Instance.EndReq();
#else
                Sys_Fight.Instance.EndReq();
#endif
            }
#endif
        }

        CombatModelManager.Instance.OnUpdate();
    }

    public void OnDisable()
    {
        if (m_IsFight)
            SDKManager.SDK_SetGameFightStatus(false);

        m_IsFight = false;

        if (_isNeedCombatTimeScale)
            PlayerPrefs.SetFloat(_combatTimeScale, m_TimeScale);

        CombatSceneEntity.Instance.Dispose();

        ShaderController.Instance.OnDisable();

        MobManager.Instance.Dispose();
        BulletManager.Instance.Dispose();

        FxManager.Instance.Dispose();
        CombatModelManager.Instance.OnDisable();

        CombatObjectPool.Instance.OnDisable();

        Net_Combat.Instance.OnDisable();

        Sys_HUD.Instance.ClearBattleHUDs();
        
        m_CombatStyleState = -1;
        m_IsRunEnterScene = false;

        PosFollowSceneCamera = false;

        m_BattleTypeTb = null;
        m_BattlePosType = 1;
        m_IsNotMirrorPos = true;

        m_IsLoadMobsOver = false;
        _delayFrameCount = 0u;
        _loadMobsOverAction = null;

        ResetNormalTimeScale();

        m_SkillShoutStartTime = 0f;

        m_CurBattleSceneId = 0u;

#if UNITY_EDITOR
        if (m_KeepCombat > 0u)
        {
            Timer.Register(4f, () =>
            {
                Sys_Fight.Instance.StartFightReq(m_KeepCombat);
            });
        }
#endif
    }

    //代码清除是有顺序的
    public void OnDestroy()
    {
        if (!m_IsAwake)
            return;

        m_IsAwake = false;

        if (m_IsFight)
            SDKManager.SDK_SetGameFightStatus(false);

        m_IsFight = false;

        if (!string.IsNullOrWhiteSpace(_combatTimeScale))
        {
            if (_isNeedCombatTimeScale)
                PlayerPrefs.SetFloat(_combatTimeScale, m_TimeScale);
            _combatTimeScale = null;
        }

        if (m_CombatEntity != null)
        {
            m_CombatEntity.Dispose();
            m_CombatEntity = null;
        }

        CombatSceneEntity.Instance.Dispose();

        ShaderController.Instance.OnDispose();

        MobManager.Instance.Dispose();
        BulletManager.Instance.Dispose();

        FxManager.Instance.Dispose();
        CombatModelManager.Instance.Dispose();

        ObjectEvents.Instance.Dispose();

        Net_Combat.Instance.OnDestroy();

        Sys_HUD.Instance.ClearBattleHUDs();
        
        m_CombatStyleState = -1;
        m_IsRunEnterScene = false;

        PosFollowSceneCamera = false;

        m_BattleTypeTb = null;
        m_BattlePosType = 1;
        m_IsNotMirrorPos = true;

        m_IsLoadMobsOver = false;
        _delayFrameCount = 0u;
        _loadMobsOverAction = null;

        m_SkillShoutStartTime = 0f;

#if UNITY_EDITOR_NO_USE
        CombatSimulateManager.Instance.OnDestroy();
#endif

        CombatHelp.ClearStrParse();

        ResetNormalTimeScale();

        CombatObjectPool.Instance.OnDestroy();
    }
#endregion

    public void OnInitCombatData()
    {
        SetCombatSceneCameraData();

        OnTransData();
    }
    
    public void SetCombatScene(CSVBattleScene.Data cSVBattleSceneData, Vector3 fightSceneCenterPos, bool isSetCommonBattle = false)
    {
        if (cSVBattleSceneData == null)
            return;
        
        CombatSceneCenterPos = fightSceneCenterPos;
        m_CurBattleSceneId = cSVBattleSceneData.id;

        if (cSVBattleSceneData.id == m_CommonBattleSceneId || isSetCommonBattle)
        {
            CombatSceneEntity.Instance.SetCommonBattle();
        }

        DebugUtil.Log(ELogType.eCombat, $"CombatSceneCenterPos:{CombatSceneCenterPos.ToString()}    战斗场景Id:{cSVBattleSceneData.id.ToString()}");
    }

    public void SetCombatSceneCameraData(Camera camera = null)
    {
        SetCombatSceneCameraData(m_SceneBorders[0], m_SceneBorders[1], m_SceneBorders[2], m_SceneBorders[3],
                            m_SceneBorders[4], m_SceneBorders[5], m_SceneBorders[6], m_SceneBorders[7], camera);
    }
    public void SetCombatSceneCameraData(float lu_l2rShrinkLen, float lu_l2dShrinkLen, float ru_r2lShrinkLen, float ru_r2dShrinkLen,
        float rd_r2lShrinkLen, float rd_r2uShrinkLen, float ld_l2rShrinkLen, float ld_l2uShrinkLen, Camera camera = null)
    {
        C2W(0f, 1f, ref m_LeftUpPosX, ref m_LeftUpPosZ, camera);
        C2W(1f, 1f, ref m_RightUpPosX, ref m_RightUpPosZ, camera);
        C2W(1f, 0f, ref m_RightDownPosX, ref m_RightDownPosZ, camera);
        C2W(0f, 0f, ref m_LeftDownPosX, ref m_LeftDownPosZ, camera);

        ShrinkCorner(m_RightUpPosX, m_RightUpPosZ, lu_l2rShrinkLen, ref m_LeftUpPosX, ref m_LeftUpPosZ);
        ShrinkCorner(m_LeftDownPosX, m_LeftDownPosZ, lu_l2dShrinkLen, ref m_LeftUpPosX, ref m_LeftUpPosZ);

        ShrinkCorner(m_LeftUpPosX, m_LeftUpPosZ, ru_r2lShrinkLen, ref m_RightUpPosX, ref m_RightUpPosZ);
        ShrinkCorner(m_RightDownPosX, m_RightDownPosZ, ru_r2dShrinkLen, ref m_RightUpPosX, ref m_RightUpPosZ);

        ShrinkCorner(m_LeftDownPosX, m_LeftDownPosZ, rd_r2lShrinkLen, ref m_RightDownPosX, ref m_RightDownPosZ);
        ShrinkCorner(m_RightUpPosX, m_RightUpPosZ, rd_r2uShrinkLen, ref m_RightDownPosX, ref m_RightDownPosZ);

        ShrinkCorner(m_RightDownPosX, m_RightDownPosZ, ld_l2rShrinkLen, ref m_LeftDownPosX, ref m_LeftDownPosZ);
        ShrinkCorner(m_LeftUpPosX, m_LeftUpPosZ, ld_l2uShrinkLen, ref m_LeftDownPosX, ref m_LeftDownPosZ);

        GetAxis(m_LeftUpPosX, m_LeftUpPosZ, m_RightUpPosX, m_RightUpPosZ, ref m_Scene_U2D_AxisX, ref m_Scene_U2D_AxisZ);
        GetAxis(m_RightUpPosX, m_RightUpPosZ, m_RightDownPosX, m_RightDownPosZ, ref m_Scene_R2L_AxisX, ref m_Scene_R2L_AxisZ);
        GetAxis(m_RightDownPosX, m_RightDownPosZ, m_LeftDownPosX, m_LeftDownPosZ, ref m_Scene_D2U_AxisX, ref m_Scene_D2U_AxisZ);
        GetAxis(m_LeftDownPosX, m_LeftDownPosZ, m_LeftUpPosX, m_LeftUpPosZ, ref m_Scene_L2R_AxisX, ref m_Scene_L2R_AxisZ);
    }

    private void C2W(float viewX, float veiwY, ref float crossX, ref float crossZ, Camera camera)
    {
        if (camera == null)
            camera = CameraManager.mCamera;

        Vector3 v = (camera.ViewportToWorldPoint(new Vector3(viewX, veiwY, 1f)) - camera.ViewportToWorldPoint(new Vector3(viewX, veiwY, 0f))).normalized;

        float d = (camera.transform.position.y - CombatSceneCenterPos.y) / v.y;
        if (d < 0f)
            d = -d;

        crossX = camera.transform.position.x + d * v.x;
        crossZ = camera.transform.position.z + d * v.z;
    }

    private void GetAxis(float ax, float az, float bx, float bz, ref float axisX, ref float axisZ)
    {
        float abx = ax - bx;
        float abz = az - bz;

        float normalx = -abz;
        float normalz = abx;

        float d = Mathf.Sqrt(normalx * normalx + normalz * normalz);

        axisX = normalx / d;
        axisZ = normalz / d;
    }

    private void ShrinkCorner(float bx, float bz, float shrinkLen, ref float ax, ref float az)
    {
        float abx = bx - ax;
        float abz = bz - az;

        float d = Mathf.Sqrt(abx * abx + abz * abz);
        abx = abx / d;
        abz = abz / d;

        ax = ax + shrinkLen * abx;
        az = az + shrinkLen * abz;
    }

    public void SetLayerByStyle(GameObject go)
    {
        SetLayerByStyle(go, m_CombatStyleState);
    }

    public void SetLayerByStyle(GameObject go, int styleState)
    {
        if (styleState == 1 || styleState == 10001)
        {
            if (!LayerMaskUtil.ContainLayerInt(ELayerMask.ModelShow, go.layer))
                go.transform.Setlayer(ELayerMask.ModelShow);
        }
        else if (styleState == 0)
        {
            if (!LayerMaskUtil.ContainLayerInt(ELayerMask.Default, go.layer))
                go.transform.Setlayer(ELayerMask.Default);
        }
#if UNITY_EDITOR
        else if (styleState == 999)
        {
            if (!LayerMaskUtil.ContainLayerInt(ELayerMask.Default, go.layer))
                go.transform.Setlayer(ELayerMask.Default);
        }
#endif
    }

    public Vector3 TransC2W_MultiplyVector(Vector3 v3)
    {
        return CameraManager.mCamera.cameraToWorldMatrix.MultiplyVector(m_BaseCameraW2CMatrix.MultiplyVector(v3));
    }

    public void OnTransData()
    {
        if (PosFollowSceneCamera)
        {
            m_AdjustSceneViewAxiss[0] = CameraManager.mCamera.cameraToWorldMatrix.MultiplyVector(new Vector3(-0.6691308f, 0.4472357f, -0.5935019f));
            m_AdjustSceneViewAxiss[1] = CameraManager.mCamera.cameraToWorldMatrix.MultiplyVector(new Vector3(0f, 0.7986355f, 0.601815f));
            m_AdjustSceneViewAxiss[2] = CameraManager.mCamera.cameraToWorldMatrix.MultiplyVector(new Vector3(-0.7431448f, -0.4026929f, 0.5343916f));
            CombatSceneCenterPos = CameraManager.mCamera.cameraToWorldMatrix.MultiplyPoint3x4(new Vector3(0.1338244f, -0.4887637f, -46.68222f));
        }
    }

    public void SetBattleTypeData(uint battleTypeId, uint levelID, Google.Protobuf.Collections.RepeatedField<Packet.BattleUnit> units)
    {
        CSVBattleType.Data cSVBattleTypeTb = CSVBattleType.Instance.GetConfData(battleTypeId);
        if (cSVBattleTypeTb == null)
        {
            DebugUtil.LogError($"特别注意！！！ 战斗类型表CSVBattleType中没有数据Id：{battleTypeId.ToString()}，会导致战斗后面表现全都不对");

            //保底处理
            cSVBattleTypeTb = CSVBattleType.Instance.GetConfData(1u);
        }

        if (cSVBattleTypeTb != null)
        {
            m_BattleTypeTb = cSVBattleTypeTb;

            m_BattlePosType = cSVBattleTypeTb.position_type;
            CSVBattleScene.Data cSVBattleSceneData = null;
            if (m_BattlePosType == 0u && levelID > 0)
            {
                cSVBattleSceneData = CSVBattleScene.Instance.GetConfData(levelID);
                if (cSVBattleSceneData != null)
                {
                    m_BattlePosType = cSVBattleSceneData.position_type;
                }
            }
            DLogManager.Log(ELogType.eCombat, $"设置战斗类型：CSVBattleTypeData表battleTypeId：{battleTypeId.ToString()}[{cSVBattleTypeTb.position_type.ToString()}]  CSVBattleSceneData表levelID:{levelID.ToString()}[{(cSVBattleSceneData == null ? "不用" : cSVBattleSceneData.position_type.ToString())}]  m_BattlePosType:{m_BattlePosType.ToString()}");

            m_IsRunEnterScene = cSVBattleTypeTb.running_enter_self;

            if (Net_Combat.Instance.m_IsWatchBattle)
                m_IsNotMirrorPos = Net_Combat.Instance.m_WatchSide == 0u;
            else
            {
                m_IsNotMirrorPos = true;
                if (units != null)
                {
                    foreach (var data in units)
                    {
                        if (MobManager.Instance.IsPlayer(data))
                        {
                            if (cSVBattleTypeTb.mirror_position && data.Pos > 19)
                            {
                                m_IsNotMirrorPos = false;
                            }

                            break;
                        }
                    }
                }
            }
        }
    }

    public void EnterCombatSkillPreView()
    {
        SetBattleTypeData(999u, 0u, null);

        CSVBattleScene.Data cSVBattleSceneData = CSVBattleScene.Instance.GetConfData(999);
        if (cSVBattleSceneData != null)
        {
            CombatSceneCenterPos = new Vector3(cSVBattleSceneData.battle_scene_pointx / 10000f, cSVBattleSceneData.battle_scene_pointy / 10000f, cSVBattleSceneData.battle_scene_pointz / 10000f);
        }
        else
        {
            DebugUtil.LogError($"cSVBattleSceneData is null, id: 999");
        }

        m_CombatStyleState = 1;
        OnEnable();
    }

    public void LoadMobsOver(uint delayFrameCount, Action loadMobsOverAction)
    {
        Net_Combat.Instance.m_CurClientBattleStage = Net_Combat.Instance.m_CurServerBattleStage;

        if (!m_IsLoadMobsOver)
        {
            _delayFrameCount = delayFrameCount;
            _loadMobsOverAction = loadMobsOverAction;
        }

        m_IsLoadMobsOver = true;
    }

    public void PreLoadFx()
    {
        if (CSVEffect.Instance != null)
        {
            foreach (var kv in CSVEffect.Instance.GetAll())
            {
                var item = kv;
                if (item == null)
                    continue;

                if (item.pre_load == 1u)
                {
                    FxManager.Instance.ShowFX(item.id, null, null, null, null, -1, false, 0, true);
                }
            }
        }
    }

    public void ResetNormalTimeScale()
    {
#if UNITY_EDITOR
#if !ILRUNTIME_MODE
        if (CreateCombatDataTest.s_ClientType)
            return;
#endif
#endif

        Time.timeScale = 1f;
    }

    public readonly float OneTimeScale = 1f;
    public readonly float TwoTimeScale = 1.3f;
    public readonly float ThridTimeScale = 1.5f;
    public void OnInitTimeScale()
    {
        if (m_BattleTypeTb.is_speed_up)
        {
            if (IsNeedHandleTimeSpeedUp())
            {
                if (string.IsNullOrWhiteSpace(_combatTimeScale))
                    _combatTimeScale = $"{Sys_Role.Instance.Role.RoleId.ToString()}_Combat_TimeScale";

                if (PlayerPrefs.HasKey(_combatTimeScale))
                    SetTimeScale(PlayerPrefs.GetFloat(_combatTimeScale));
                else
                    SetTimeScale(1f);

                _isNeedCombatTimeScale = true;
            }
            else
                _isNeedCombatTimeScale = false;
        }
        else
        {
            _isNeedCombatTimeScale = false;
            if (m_TimeScale < 1f || m_BattleTypeTb.init_show_speed == 0u)
                m_TimeScale = 1f;
        }
    }

    public void SetTimeScale(float timeScale, bool isForce = false, bool isClientSet = true)
    {
        m_TimeScale = timeScale;

        if (isForce || !Net_Combat.Instance.m_RoundOver)
        {
            Time.timeScale = m_TimeScale;
        }

        if (isClientSet)
        {
            if (IsNeedHandleTimeSpeedUp())
                Net_Combat.Instance.SendCmdBattleCmdSpeedUpReq(m_TimeScale);
        }
    }

    public void SwitchOldTimeScale()
    {
        Time.timeScale = m_TimeScale;
    }

    public uint SwitchTimeScale2Uint(float timeScale)
    {
        if (timeScale == OneTimeScale)
        {
            return 1u;
        }
        else if (timeScale == TwoTimeScale)
        {
            return 2u;
        }
        else if (timeScale == ThridTimeScale)
        {
            return 3u;
        }
        else
            return 1u;
    }

    public float SwitchUint2TimeScale(uint ts)
    {
        if (ts == 1u)
            return OneTimeScale;
        else if (ts == 2u)
            return TwoTimeScale;
        else if (ts == 3u)
            return ThridTimeScale;
        else
            return OneTimeScale;
    }

    private bool IsNeedHandleTimeSpeedUp()
    {
        return !Net_Combat.Instance.m_IsWatchBattle &&
                (Sys_Team.Instance.TeamMemsCount == 0 || Sys_Team.Instance.isCaptain(Sys_Role.Instance.Role.RoleId));
    }

    public void ShowPassiveName(uint unitId, uint extendId)
    {
        CSVPassiveSkill.Data cSVPassiveSkillData = CSVPassiveSkill.Instance.GetConfData(extendId);
        if (cSVPassiveSkillData == null || cSVPassiveSkillData.behavior_name == 0u)
        {
            DebugUtil.Log(ELogType.eCombatBehave, $"ShowPassiveName----unitId:{unitId.ToString()}   extendId:{extendId.ToString()}  cSVPassiveSkillData:{cSVPassiveSkillData.ToString()}   cSVPassiveSkillData.behavior_name:{cSVPassiveSkillData?.behavior_name.ToString()}");
            return;
        }

        MobEntity triggerMob = MobManager.Instance.GetMob(unitId);
        if (triggerMob == null)
            return;

        TriggerAnimEvt triggerAnimEvt = CombatObjectPool.Instance.Get<TriggerAnimEvt>();
        triggerAnimEvt.id = unitId;
        triggerAnimEvt.AnimType = AnimType.e_PassiveName;
        triggerAnimEvt.passiveId = extendId;
        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnTriggerAnim, triggerAnimEvt);
    }

    public bool GetPosByClientNum(int clientNum, out CombatPosData posData, ref Vector3 pos)
    {
        posData = CombatConfigManager.Instance.GetCombatPosData(m_BattlePosType * 1000000u + 20000u + (uint)clientNum * 100u + 1u);
        if (posData == null)
        {
            DebugUtil.LogError($"编辑数据不存在PosId：{(m_BattlePosType * 1000000 + 20000u + (uint)clientNum * 100u + 1u).ToString()}");
            return false;
        }
        
        if (PosFollowSceneCamera)
        {
            pos = CombatSceneCenterPos + m_AdjustSceneViewAxiss[0] * posData.PosX + m_AdjustSceneViewAxiss[2] * posData.PosZ;
            pos.y = CombatSceneCenterPos.y;
        }
        else
            pos = CombatSceneCenterPos + new Vector3(posData.PosX, 0f, posData.PosZ);

        return true;
    }

    /// <summary>
    /// Packet.UnitType=serverUnitType = 1英雄,= 2宠物,= 3怪物,= 4伙伴,= 5机器人
    /// </summary>
    public uint GetAudioId(uint serverUnitType, uint unitInfoId, uint weaponId, string audioState)
    {
        CSVSkillSE.Data skillSEDataTb = GetSkillAudioTb(serverUnitType, unitInfoId, weaponId);
        if (skillSEDataTb == null)
            return 0u;

        foreach (var kv in CSVSEState.Instance.GetAll())
        {
            if (kv.SE_state == audioState)
            {
                uint audioId = skillSEDataTb.audio_tool[(int)(kv.id % 100u) - 1];

#if DEBUG_MODE
                if (audioId == 0u)
                {
                    DebugUtil.LogError($"CSVSkillSE表中Id：{skillSEDataTb.id.ToString()}的{audioState}状态数据为0");
                }
#endif

                return audioId;
            }
        }

        DebugUtil.LogError($"CSVSkillSE表中Id：{skillSEDataTb.id.ToString()}的数据没有{audioState}状态");
        return 0u;
    }

    public CSVSkillSE.Data GetSkillAudioTb(uint serverUnitType, uint unitInfoId, uint weaponId)
    {
        CSVEquipment.Data edTb = CSVEquipment.Instance.GetConfData(weaponId);
        if (edTb == null)
        {
            DebugUtil.LogError($"CSVEquipment表中没有Id ：{weaponId.ToString()}");
            return null;
        }

        uint skillSETbId = GetActId(serverUnitType, unitInfoId) * 100000u + edTb.equipment_type * 100u;
        CSVSkillSE.Data skillSEDataTb = CSVSkillSE.Instance.GetConfData(skillSETbId);
        if (skillSEDataTb == null)
        {
            DebugUtil.LogError($"CSVSkillSE表中没有Id：{skillSETbId.ToString()}");
            return null;
        }

        return skillSEDataTb;
    }

    private uint GetActId(uint serverUnitType, uint unitInfoId)
    {
        if (serverUnitType == (uint)Packet.UnitType.Monster)
        {
            CSVMonster.Data cSVMonsterData = CSVMonster.Instance.GetConfData(unitInfoId);
            if (cSVMonsterData == null)
            {
                DebugUtil.LogError($"CSVMonsterData表中没有Id ：{unitInfoId.ToString()}");
                return 0u;
            }
            return cSVMonsterData.monster_id;
        }
        else if (serverUnitType == (uint)Packet.UnitType.Pet)
        {
            CSVPetNew.Data cSVPetData = CSVPetNew.Instance.GetConfData(unitInfoId);
            if (cSVPetData == null)
            {
                DebugUtil.LogError($"CSVPetData表中没有Id ：{unitInfoId.ToString()}");
                return 0u;
            }
            return cSVPetData.action_id;
        }
        else
        {
            return unitInfoId;
        }
    }

#if DEBUG_MODE
    public uint GetBattleTypeId()
    {
        if (_battleTypeTb == null)
            return 0u;

        return _battleTypeTb.id;
    }
#endif
}