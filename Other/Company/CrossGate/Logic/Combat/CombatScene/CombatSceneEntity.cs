using Lib.Core;
using Logic;
using Logic.Core;
using UnityEngine;

public class CombatSceneEntity : AEntityRepeat, IAwake, IUpdate
{
    private static CombatSceneEntity _instance;
    public static CombatSceneEntity Instance
    {
        get
        {
            if (_instance == null)
                _instance = EntityFactory.Create<CombatSceneEntity>();

            return _instance;
        }
    }

    private ulong _commonBattleSceneModelId;

    private float _time;
    private float _checkTime;

    public void Awake()
    {
        Sys_Net.Instance.eventEmitter.Handle(Sys_Net.EEvents.OnReconnectStartReq, OnReconnectStart, true);

        _checkTime = 1f;
    }

    public override void Dispose()
    {
        Sys_Net.Instance.eventEmitter.Handle(Sys_Net.EEvents.OnReconnectStartReq, OnReconnectStart, false);

        if (_commonBattleSceneModelId > 0ul)
        {
            CombatModelManager.Instance.FreeModel(_commonBattleSceneModelId, CombatManager.Instance.m_WorkStreamTrans, true, true);
            _commonBattleSceneModelId = 0ul;
        }

        _time = 0f;
        _checkTime = 0f;

        base.Dispose();

        _instance = null;
    }

    public void Update()
    {
        if (_time > 5)
        {
            ObjectEvent_IsStopUpdate = true;
            return;
        }

        _time += Time.deltaTime;
        if (_time >= _checkTime)
        {
            _checkTime += 1f;
            if (CombatManager.Instance.m_IsFight)
            {
                if (GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Fight &&
                    GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.CutScene)
                {
                    DebugUtil.LogError($"当前正在战斗中，状态却是{GameMain.Procedure.CurrentProcedure.ProcedureType.ToString()}，强制切回EProcedureType.Fight");
                    GameMain.Procedure.TriggerEvent(Sys_Fight.Instance, (int)EProcedureEvent.EnterFight);
                    Sys_Fight.Instance.UpdateFightScene(CombatManager.Instance.m_CurBattleSceneId);

                    CombatManager.Instance.OnInitTimeScale();
                }
            }
        }
    }

    public void SetCommonBattle()
    {
        _commonBattleSceneModelId = CombatModelManager.Instance.CreateModel(CombatManager.Instance.m_CommonBattleScenePrefabPath, 
            null, delegate (GameObject commonBattleScenePrefabGo, ulong modelId)
        {
            CombatManager.Instance.SetLayerByStyle(commonBattleScenePrefabGo);

            Transform trans = commonBattleScenePrefabGo.transform;
            trans.SetParent(CombatManager.Instance.m_WorkStreamTrans);
            trans.localPosition = CombatManager.Instance.CombatSceneCenterPos;
        });
    }

    private void OnReconnectStart()
    {
        ObjectEvent_IsStopUpdate = true;

        CombatManager.Instance.ResetNormalTimeScale();
    }
}
