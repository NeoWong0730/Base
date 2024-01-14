#if UNITY_EDITOR
using Framework;
using Logic;
using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Table;
using UnityEngine;
using static Net_Combat;

[System.Serializable]
public class BattleUnitTest
{
    //public UnitType UnitType;
    //public uint UnitId;
    //public uint UnitInfoId;
    public int ServerNum;
    public int ClientNum;
    public uint WeaponId;
}

[System.Serializable]
public class BuffTest
{
    public uint ClientNum;
    public uint BuffId;
    public uint Count;
}

[System.Serializable]
public class AttackDataTest
{
    [System.Serializable]
    public class CallingDataTest
    {
        public UnitType UnitType;
        public uint UnitInfoId;
        public int ClientNum;
    }

    [System.Serializable]
    public class HpMpChangeDataTest
    {
        public int ChangeType;
        public int HpChange = 1;
        public int CurHp = 1;
        public int MpChange;
        public int CurMp;
    }

    public AnimType AnimType = AnimType.e_Normal;
    public bool IsDead;
    public bool HitToFly;
    public HpMpChangeDataTest HMpChangeDataTest;
    public List<BuffTest> Buffs = new List<BuffTest>();
    public List<uint> AttackNums = new List<uint>() { 2u, };
    public List<uint> TargetNums = new List<uint>() { 10u, };
    public List<CallingDataTest> CallingDatas = new List<CallingDataTest>();
    public uint SkillId = 1001;
}

public class CreateCombatDataTest : MonoBehaviour
{
    public static bool s_ClientType;
    public static uint s_UnitStartId = 999u;
    private bool _clientType = true;

    public bool m_RefreshAllConfigFlag;

    [Header("---------调试战斗---------")]
    public bool RefreshWorkStreamData;
    public Dictionary<BattleUnitTest, BattleUnit> battleUnitDic = new Dictionary<BattleUnitTest, BattleUnit>();
    public List<BattleUnitTest> battleUnitList = new List<BattleUnitTest>();
    public bool m_RefreshFlag;
    public AttackDataTest m_AttackDataTest;
    public bool m_AttackFlag;

    [Space(15f)]
    [Header("---------调试屏幕定点内缩大小---------")]
    public float lu_l2rShrinkLen;
    public float lu_l2dShrinkLen;
    public float ru_r2lShrinkLen;
    public float ru_r2dShrinkLen;
    public float rd_r2lShrinkLen;
    public float rd_r2uShrinkLen;
    public float ld_l2rShrinkLen;
    public float ld_l2uShrinkLen;
    public bool RefreshSceneCorner;

    [Space(15f)]
    [Header("---------获取World空间到View空间的矩阵---------")]
    public bool IsGetW2C;
    [HideInInspector]
    public Camera m_Camera;

    [Space(15f)]
    [Header("---------新的编辑器调试---------")]
    public bool StartWorkStream;
    public uint WorkId;
    public int AttachType;
    public int BlockType;

    private uint _startId;

    public void OnDrawGizmos()
    {
        ObjectEvents.Instance.OnDrawGizmos();

        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hit;
        //if (Physics.Raycast(ray, out hit))
        //{
        //    var point = hit.point;

        //    Debug.DrawRay(ray.origin, point - ray.origin);
        //    Gizmos.DrawSphere(point, 0.1f);
        //}

        //C2W(0f, 0f, 1f);  //leftDown
        //C2W(0f, 1f, 1f);  //leftUp
        //C2W(1f, 0f, 1f);  //rightDown
        //C2W(1f, 1f, 1f);  //rightUp

        if(RefreshSceneCorner)
            CombatManager.Instance.SetCombatSceneCameraData(lu_l2rShrinkLen, lu_l2dShrinkLen, ru_r2lShrinkLen, ru_r2dShrinkLen, rd_r2lShrinkLen, rd_r2uShrinkLen, ld_l2rShrinkLen, ld_l2uShrinkLen);

        float y = CombatManager.Instance.CombatSceneCenterPos.y;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(CombatManager.Instance.m_LeftUpPosX, y, CombatManager.Instance.m_LeftUpPosZ), 0.5f);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(new Vector3(CombatManager.Instance.m_RightUpPosX, y, CombatManager.Instance.m_RightUpPosZ), 0.5f);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(new Vector3(CombatManager.Instance.m_RightDownPosX, y, CombatManager.Instance.m_RightDownPosZ), 0.5f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(new Vector3(CombatManager.Instance.m_LeftDownPosX, y, CombatManager.Instance.m_LeftDownPosZ), 0.5f);

        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawPolyLine(new Vector3(CombatManager.Instance.m_LeftUpPosX, y, CombatManager.Instance.m_LeftUpPosZ),
            new Vector3(CombatManager.Instance.m_RightUpPosX, y, CombatManager.Instance.m_RightUpPosZ),
            new Vector3(CombatManager.Instance.m_RightDownPosX, y, CombatManager.Instance.m_RightDownPosZ),
            new Vector3(CombatManager.Instance.m_LeftDownPosX, y, CombatManager.Instance.m_LeftDownPosZ),
            new Vector3(CombatManager.Instance.m_LeftUpPosX, y, CombatManager.Instance.m_LeftUpPosZ));

        UnityEditor.Handles.color = Color.cyan;
        UnityEditor.Handles.DrawPolyLine(new Vector3(CombatManager.Instance.m_RightUpPosX, y, CombatManager.Instance.m_RightUpPosZ),
            new Vector3(CombatManager.Instance.m_RightUpPosX + CombatManager.Instance.m_Scene_U2D_AxisX, y, CombatManager.Instance.m_RightUpPosZ + CombatManager.Instance.m_Scene_U2D_AxisZ));
        UnityEditor.Handles.DrawPolyLine(new Vector3(CombatManager.Instance.m_RightDownPosX, y, CombatManager.Instance.m_RightDownPosZ),
            new Vector3(CombatManager.Instance.m_RightDownPosX + CombatManager.Instance.m_Scene_R2L_AxisX, y, CombatManager.Instance.m_RightDownPosZ + CombatManager.Instance.m_Scene_R2L_AxisZ));
        UnityEditor.Handles.DrawPolyLine(new Vector3(CombatManager.Instance.m_LeftDownPosX, y, CombatManager.Instance.m_LeftDownPosZ),
            new Vector3(CombatManager.Instance.m_LeftDownPosX + CombatManager.Instance.m_Scene_D2U_AxisX, y, CombatManager.Instance.m_LeftDownPosZ + CombatManager.Instance.m_Scene_D2U_AxisZ));
        UnityEditor.Handles.DrawPolyLine(new Vector3(CombatManager.Instance.m_LeftUpPosX, y, CombatManager.Instance.m_LeftUpPosZ),
            new Vector3(CombatManager.Instance.m_LeftUpPosX + CombatManager.Instance.m_Scene_L2R_AxisX, y, CombatManager.Instance.m_LeftUpPosZ + CombatManager.Instance.m_Scene_L2R_AxisZ));

        UnityEditor.Handles.color = Color.white;
    }

    private void C2W(float x, float y, float dis)
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(CameraManager.mCamera.ViewportToWorldPoint(new Vector3(x, y, 0f)), CameraManager.mCamera.ViewportToWorldPoint(new Vector3(x, y, dis)));

        Vector3 v = CameraManager.mCamera.ViewportToWorldPoint(new Vector3(x, y, dis)) - CameraManager.mCamera.ViewportToWorldPoint(new Vector3(x, y, 0f));
        v = v.normalized;

        float d = CameraManager.mCamera.transform.position.y / v.y;
        if (d < 0f)
            d = -d;

        Gizmos.DrawSphere(CameraManager.mCamera.transform.position + d * v, 0.5f);
    }

    public void Start()
    {
        _clientType = true;

        foreach (var kv in MobManager.Instance.m_MobDic)
        {
            BattleUnitTest battleUnitTest = new BattleUnitTest();
            //battleUnitTest.UnitId = kv.Value.m_MobCombatComponent.m_BattleUnit.UnitId;
            //battleUnitTest.UnitInfoId = kv.Value.m_MobCombatComponent.m_BattleUnit.UnitInfoId;
            battleUnitTest.ServerNum = kv.Value.m_MobCombatComponent.m_BattleUnit.Pos;
            battleUnitTest.ClientNum = kv.Value.m_MobCombatComponent.m_ClientNum;
            battleUnitTest.WeaponId = kv.Value.m_MobCombatComponent.m_WeaponId;

            battleUnitDic[battleUnitTest] = kv.Value.m_MobCombatComponent.m_BattleUnit;
            battleUnitList.Add(battleUnitTest);
        }
    }

    public void Update()
    {
        s_ClientType = _clientType;

        if (m_RefreshAllConfigFlag)
        {
            m_RefreshAllConfigFlag = false;

            CombatConfigManager.Instance.ResetConfigData();
        }

        if (m_RefreshFlag)
        {
            m_RefreshFlag = false;
//#if UNITY_EDITOR
            foreach (var bu in battleUnitList)
            {
                bu.ServerNum = CombatHelp.ClientToServerNum(bu.ClientNum);
                battleUnitDic[bu].Pos = bu.ServerNum;
                MobEntity mobEntity = MobManager.Instance.GetMob(battleUnitDic[bu].UnitId);
                if(mobEntity != null)
                    mobEntity.m_MobCombatComponent.SetBattleUnit(battleUnitDic[bu], bu.WeaponId);
            }
//#endif
        }

        if (m_AttackFlag)
        {
            m_AttackFlag = false;

            AttackTest();
        }

        if (RefreshWorkStreamData)
        {
            RefreshWorkStreamData = false;
            CombatConfigManager.Instance.ResetConfigData(5);
        }

        if (StartWorkStream)
        {
            StartWorkStream = false;
            MobEntity me = null;
            foreach (var kv in MobManager.Instance.m_MobDic)
            {
                me = kv.Value;
                break;
            }
            WS_CombatBehaveAIManagerEntity.StartCombatBehaveAI02<WS_CombatSceneAIControllerEntity>(WorkId, AttachType);
        }

        if (IsGetW2C)
        {
            IsGetW2C = false;
            Matrix4x4 matrix = default;
            if (m_Camera == null)
            {
                matrix = CameraManager.mCamera.worldToCameraMatrix;
            }
            else
            {
                matrix = m_Camera.worldToCameraMatrix;
            }
            

            //Debug.Log($"m00 : {matrix.m00.ToString()}   m01 : {matrix.m01.ToString()}   m02 : {matrix.m02.ToString()}   m03 : {matrix.m03.ToString()} \n" +
            //        $"m10 : {matrix.m10.ToString()}   m11 : {matrix.m11.ToString()}   m12 : {matrix.m12.ToString()}   m13 : {matrix.m13.ToString()} \n" +
            //        $"m20 : {matrix.m20.ToString()}   m21 : {matrix.m21.ToString()}   m22 : {matrix.m22.ToString()}   m23 : {matrix.m23.ToString()} \n" +
            //        $"m30 : {matrix.m30.ToString()}   m31 : {matrix.m31.ToString()}   m32 : {matrix.m32.ToString()}   m33 : {matrix.m33.ToString()}");

            Debug.Log($"{matrix.m00.ToString()}|{matrix.m01.ToString()}|{matrix.m02.ToString()}|{matrix.m03.ToString()}|" +
                    $"{matrix.m10.ToString()}|{matrix.m11.ToString()}|{matrix.m12.ToString()}|{matrix.m13.ToString()}|" +
                    $"{matrix.m20.ToString()}|{matrix.m21.ToString()}|{matrix.m22.ToString()}|{matrix.m23.ToString()}" +
                    $"{matrix.m30.ToString()}|{matrix.m31.ToString()}|{matrix.m32.ToString()}|{matrix.m33.ToString()}");
        }
    }

    private void OnDestroy()
    {
        s_ClientType = false;
    }

    public void AttackTest()
    {
        if (m_AttackDataTest.AttackNums.Count == 0)
            return;

        foreach (var buff in m_AttackDataTest.Buffs)
        {
            var def = GetMobEntity(buff.ClientNum);
            var buffComp = def.GetComponent<MobBuffComponent>();
            if (buffComp != null)
                buffComp.Dispose();
        }
        foreach (var buff in m_AttackDataTest.Buffs)
        {
            var def = GetMobEntity(buff.ClientNum);
            def.UpdateBuffComponent(buff.BuffId, buff.Count);
        }

        foreach (var targerNum in m_AttackDataTest.TargetNums)
        {
            var target = GetMobEntity(targerNum);
            target.m_MobCombatComponent.m_HpChangeDataQueue.Clear();
        }

        CSVActiveSkill.Data skillDataTb = CSVActiveSkill.Instance.GetConfData(m_AttackDataTest.SkillId);
        if (skillDataTb == null)
        {
            Debug.LogError($"CSVActiveSkill表中没有技能Id：{m_AttackDataTest.SkillId.ToString()}");
            return;
        }

        List<MobEntity> targetList = new List<MobEntity>(); ;
        foreach (var targerNum in m_AttackDataTest.TargetNums)
        {
            var target = GetMobEntity(targerNum);
            int hpChange = m_AttackDataTest.HMpChangeDataTest.HpChange;
            target.m_MobCombatComponent.m_BeHitToFlyState = m_AttackDataTest.HitToFly;
            if (skillDataTb.skill_effect_id != null)
            {
                for (int skillLogicIdIndex = 0, skillLogicCount = skillDataTb.skill_effect_id.Count; skillLogicIdIndex < skillLogicCount; skillLogicIdIndex++)
                {
                    if (skillDataTb.skill_effect_id[skillLogicIdIndex] == 0u)
                        continue;

                    CombatHpChangeData curHpChangeData = CombatObjectPool.Instance.Get<CombatHpChangeData>();
                    curHpChangeData.m_ChangeType = m_AttackDataTest.HMpChangeDataTest.ChangeType;
                    curHpChangeData.m_AnimType = m_AttackDataTest.AnimType;
                    curHpChangeData.m_HpChange = hpChange;
                    curHpChangeData.m_CurHp = m_AttackDataTest.HMpChangeDataTest.CurHp;
                    curHpChangeData.m_MpChange = m_AttackDataTest.HMpChangeDataTest.MpChange;
                    curHpChangeData.m_CurMp = m_AttackDataTest.HMpChangeDataTest.CurMp;
                    curHpChangeData.m_Revive = (target.m_MobCombatComponent.m_Death && hpChange > 0);
                    curHpChangeData.m_Death = m_AttackDataTest.IsDead;
                    if (m_AttackDataTest.AttackNums.Count > 1)
                        curHpChangeData.m_AnimType |= AnimType.e_ConverAttack;
                    target.m_MobCombatComponent.m_HpChangeDataQueue.Enqueue(curHpChangeData);
                }
            }
            target.m_MobCombatComponent.m_Death = m_AttackDataTest.IsDead;
            targetList.Add(target);
        }

        List<BattleUnit> targetBattleUnits = new List<BattleUnit>();
        foreach (var item in m_AttackDataTest.CallingDatas)
        {
            BattleUnit battleUnit = new BattleUnit();
            battleUnit.UnitId = ++s_UnitStartId;
            battleUnit.UnitType = (uint)item.UnitType;
            battleUnit.UnitInfoId = item.UnitInfoId;
            battleUnit.RoleId = Sys_Role.Instance.Role.RoleId;
            battleUnit.Pos = CombatHelp.ClientToServerNum(item.ClientNum);
            battleUnit.MaxHp = 9999u;
            battleUnit.CurHp = 9999;
            if (item.UnitType == UnitType.Partner)
            {
                battleUnit.Level = 1;
            }
            battleUnit.Side = CombatHelp.GetServerCampSide(battleUnit.Pos);

            targetBattleUnits.Add(battleUnit);
        }

        if (!Net_Combat.Instance.m_NetExcuteTurnInfoDic.TryGetValue(0, out NetExcuteTurnInfo netExcuteTurnInfo) || netExcuteTurnInfo == null)
        {
            netExcuteTurnInfo = new NetExcuteTurnInfo();
            Net_Combat.Instance.m_NetExcuteTurnInfoDic[0] = netExcuteTurnInfo;
        }
        else
            netExcuteTurnInfo.Clear();

        if (netExcuteTurnInfo.CombineAttack_SrcUnits == null)
            netExcuteTurnInfo.CombineAttack_SrcUnits = new List<uint>();
        netExcuteTurnInfo.CombineAttack_SrcUnits.Clear();
        netExcuteTurnInfo.CombineAttack_ScrIndex = 0;
        netExcuteTurnInfo.CombineAttack_WaitCount = 0;
        
        foreach (var attackNum in m_AttackDataTest.AttackNums)
        {
            MobEntity attack = GetMobEntity(attackNum);

            netExcuteTurnInfo.CombineAttack_SrcUnits.Add(attack.m_MobCombatComponent.m_BattleUnit.UnitId);

            for (int targetIndex = 0, count = targetList.Count; targetIndex < count; targetIndex++)
            {
                if (skillDataTb.skill_effect_id != null)
                {
                    for (int skillLogicIdIndex = 0, skillLogicCount = skillDataTb.skill_effect_id.Count; skillLogicIdIndex < skillLogicCount; skillLogicIdIndex++)
                    {
                        if (skillDataTb.skill_effect_id[skillLogicIdIndex] == 0u)
                            continue;

                        Net_Combat.Instance.AddTurnBehaveInfo(netExcuteTurnInfo.TurnBehaveInfoList, 1, 0, attack.m_MobCombatComponent.m_BattleUnit.UnitId,
                                targetList[targetIndex].m_MobCombatComponent.m_BattleUnit.UnitId, ++CombatHelp.m_UnitStartId, 1,
                                m_AttackDataTest.SkillId, 0u, 0u, m_AttackDataTest.AttackNums.Count > 1, null, 0u, skillDataTb.skill_effect_id[skillLogicIdIndex]);
                    }
                }
                else
                {
                    Net_Combat.Instance.AddTurnBehaveInfo(netExcuteTurnInfo.TurnBehaveInfoList, 1, 0, attack.m_MobCombatComponent.m_BattleUnit.UnitId,
                               targetList[targetIndex].m_MobCombatComponent.m_BattleUnit.UnitId, ++CombatHelp.m_UnitStartId, 1,
                               m_AttackDataTest.SkillId, 0u, 0u, m_AttackDataTest.AttackNums.Count > 1, null, 0u, m_AttackDataTest.SkillId * 10);
                }
            }

            for (int targetBattleIndex = 0, targetBattleCount = targetBattleUnits.Count; targetBattleIndex < targetBattleCount; targetBattleIndex++)
            {
                if (skillDataTb.skill_effect_id != null)
                {
                    for (int skillLogicIdIndex = 0, skillLogicCount = skillDataTb.skill_effect_id.Count; skillLogicIdIndex < skillLogicCount; skillLogicIdIndex++)
                    {
                        if (skillDataTb.skill_effect_id[skillLogicIdIndex] == 0u)
                            continue;

                        Net_Combat.Instance.AddTurnBehaveInfo(netExcuteTurnInfo.TurnBehaveInfoList, 1, 0, attack.m_MobCombatComponent.m_BattleUnit.UnitId,
                            0u, ++CombatHelp.m_UnitStartId, 1,
                            m_AttackDataTest.SkillId, 0u, 0u, m_AttackDataTest.AttackNums.Count > 1, targetBattleUnits[targetBattleIndex], 0u, skillDataTb.skill_effect_id[skillLogicIdIndex]);
                    }
                }
            }
        }

        bool isHaveBehave = false;
        Net_Combat.Instance.PerformBehave(netExcuteTurnInfo.TurnBehaveInfoList, 0, true, true, -1, ref isHaveBehave);
    }

    public MobEntity GetMobEntity(uint clientNum)
    {
        int serverNum = CombatHelp.ClientToServerNum((int)clientNum);
        foreach (var kv in battleUnitDic)
        {
            if (kv.Value.Pos == serverNum)
            {
                MobEntity mobEntity = MobManager.Instance.GetMob(kv.Value.UnitId);
                return mobEntity;
            }
        }

        return null;
    }
}
#endif