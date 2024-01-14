using Lib.AssetLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class WorkStreamConfigManager
{
    private static WorkStreamConfigManager _instance;
    public static WorkStreamConfigManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new WorkStreamConfigManager();
                _instance.Init();
            }

            return _instance;
        }
    }

    private FileDataOperationReadEntity _fileDataOperationReadEntity;

    public void Init()
    {
        for (int i = 0, count = StateCategoryManager.Instance.m_ControllerStateCategoryInfoList.Count; i < count; i++)
        {
            var info = StateCategoryManager.Instance.m_ControllerStateCategoryInfoList[i];
            if (info == null)
                continue;

            SetWorkStreamByEnum(info);
        }
    }

    public void Refresh(int stateCategoryEnum)
    {
        for (int i = 0, count = StateCategoryManager.Instance.m_ControllerStateCategoryInfoList.Count; i < count; i++)
        {
            var info = StateCategoryManager.Instance.m_ControllerStateCategoryInfoList[i];
            if (info == null)
                continue;

            if (info.ControllerStateCategoryEnum == stateCategoryEnum)
            {
                RefreshWorkStreamByEnum(info);
                return;
            }
        }
    }

    public void RefreshAll()
    {
        for (int i = 0, count = StateCategoryManager.Instance.m_ControllerStateCategoryInfoList.Count; i < count; i++)
        {
            var info = StateCategoryManager.Instance.m_ControllerStateCategoryInfoList[i];
            if (info == null)
                continue;

            RefreshWorkStreamByEnum(info);
        }
    }

    #region Logic
    public void SetWorkStream(StateCategoryManager.ControllerStateCategoryInfo controllerStateCategoryInfo)
    {
        StateCategoryManager.SingleWorkStreamConfigData singleWorkStreamConfigData = null;
        if (controllerStateCategoryInfo.m_BaseWorkStreamConfigData == null)
        {
            singleWorkStreamConfigData = new StateCategoryManager.SingleWorkStreamConfigData();
            controllerStateCategoryInfo.m_BaseWorkStreamConfigData = singleWorkStreamConfigData;
        }
        else
            singleWorkStreamConfigData = controllerStateCategoryInfo.m_BaseWorkStreamConfigData as StateCategoryManager.SingleWorkStreamConfigData;

        SetWorkStream(controllerStateCategoryInfo.WorkStreamDataFile, singleWorkStreamConfigData);
    }

    private void SetWorkStream(string workStreamDataFile, StateCategoryManager.SingleWorkStreamConfigData singleWorkStreamConfigData)
    {
        string filePath = string.Format(workStreamDataFile, string.Empty);
#if UNITY_EDITOR
        string fp = string.Format("{0}/{1}", Application.dataPath, filePath);
        if (!File.Exists(fp))
            return;
#endif

        Stream stream = AssetMananger.Instance.LoadStream(filePath);

        singleWorkStreamConfigData.DataBytes = new byte[stream.Length];
        stream.Read(singleWorkStreamConfigData.DataBytes, 0, (int)stream.Length);
        stream.Seek(0, SeekOrigin.Begin);

        if (singleWorkStreamConfigData.DataDic != null)
            singleWorkStreamConfigData.DataDic.Clear();

        if (singleWorkStreamConfigData.DataBytePosDic == null)
            singleWorkStreamConfigData.DataBytePosDic = new Dictionary<uint, uint>();
        else
            singleWorkStreamConfigData.DataBytePosDic.Clear();

        _fileDataOperationReadEntity = new FileDataOperationReadEntity();
        int count = 0;
        _fileDataOperationReadEntity.StartRead(stream, (Stream rs, BinaryReader binaryReader) =>
        {
            count = binaryReader.ReadInt32();
            if (count > 0)
            {
                uint curPos = (uint)rs.Position;
                for (int i = 0; i < count; i++)
                {
                    uint nextPos = binaryReader.ReadUInt32();
                    uint workId = binaryReader.ReadUInt32();
                    singleWorkStreamConfigData.DataBytePosDic[workId] = curPos + 4u;

                    rs.Seek(nextPos, SeekOrigin.Begin);
                    curPos = nextPos;
                }
            }
        });
        _fileDataOperationReadEntity.EndRead();
    }

    public List<WorkBlockData> GetWorkBlockDatas<T>(uint workId, int attachType)
    {
        Type wsType = typeof(T);

        return GetWorkBlockDatas(wsType, workId, attachType);
    }

    public List<WorkBlockData> GetWorkBlockDatas(Type wsType, uint workId, int attachType)
    {
        WorkStreamData workStreamData = GetWorkStreamData(wsType, workId);
        if (workStreamData == null)
            return null;

        return attachType == 0 ? workStreamData.AttackWorkBlockDatas : workStreamData.TargetWorkBlockDatas;
    }

    public WorkStreamData GetWorkStreamData(Type wsType, uint workId)
    {
        for (int i = 0, count = StateCategoryManager.Instance.m_ControllerStateCategoryInfoList.Count; i < count; i++)
        {
            var info = StateCategoryManager.Instance.m_ControllerStateCategoryInfoList[i];
            if (info == null)
                continue;

            for (int ctIndex = 0, ctCount = info.ControllerTypeList.Count; ctIndex < ctCount; ctIndex++)
            {
                if (info.ControllerTypeList[ctIndex] == wsType)
                {
                    return GetWorkStreamDataByEnum(workId, info);
                }
            }
        }

        return null;
    }
    
    public WorkStreamData GetWorkStreamData(uint workId, StateCategoryManager.ControllerStateCategoryInfo controllerStateCategoryInfo)
    {
        if (controllerStateCategoryInfo == null || controllerStateCategoryInfo.m_BaseWorkStreamConfigData == null)
            return null;

        StateCategoryManager.SingleWorkStreamConfigData singleWorkStreamConfigData = controllerStateCategoryInfo.m_BaseWorkStreamConfigData as StateCategoryManager.SingleWorkStreamConfigData;
        return GetWorkStreamData(workId, singleWorkStreamConfigData);
    }

    private WorkStreamData GetWorkStreamData(uint workId, StateCategoryManager.SingleWorkStreamConfigData singleWorkStreamConfigData)
    {
        if (singleWorkStreamConfigData == null)
            return null;

        WorkStreamData workStreamData = null;
        if (singleWorkStreamConfigData.DataDic != null && singleWorkStreamConfigData.DataDic.TryGetValue(workId, out workStreamData))
            return workStreamData;

        if (singleWorkStreamConfigData.DataBytePosDic != null && singleWorkStreamConfigData.DataBytePosDic.TryGetValue(workId, out uint pos))
        {
            workStreamData = _fileDataOperationReadEntity.DoRead(new MemoryStream(singleWorkStreamConfigData.DataBytes), (BinaryReader br) =>
            {
                return WorkStreamData_ConfigTool.Load(br);
            },
            (Stream rs, BinaryReader br) =>
            {
                rs.Seek(pos, SeekOrigin.Begin);
                uint readWorkId = br.ReadUInt32();
                if (workId != readWorkId)
                {
                    Lib.Core.DebugUtil.LogError($"获取WorkId数据不一致,需要的：{workId.ToString()}，读取的：{readWorkId.ToString()}");
                }
            });
        }

        if (workStreamData == null)
        {
            //Lib.Core.DebugUtil.LogError($"配置中解析WorkId:{workId.ToString()}数据为null");
            return null;
        }

        if (singleWorkStreamConfigData.DataDic == null)
            singleWorkStreamConfigData.DataDic = new Dictionary<uint, WorkStreamData>();

        singleWorkStreamConfigData.DataDic[workId] = workStreamData;

        return workStreamData;
    }

    private void SetWorkStreamByEnum(StateCategoryManager.ControllerStateCategoryInfo info)
    {
        if (info.ControllerStateCategoryEnum == (int)StateCategoryEnum.CombatBehaveAI)
        {

        }
        else
        {
            SetWorkStream(info);
        }
    }

    private void RefreshWorkStreamByEnum(StateCategoryManager.ControllerStateCategoryInfo info)
    {
        if (info.ControllerStateCategoryEnum == (int)StateCategoryEnum.CombatBehaveAI)
        {
            info.m_BaseWorkStreamConfigData = null;
        }
        else
        {
            SetWorkStream(info);
        }
    }

    private WorkStreamData GetWorkStreamDataByEnum(uint workId, StateCategoryManager.ControllerStateCategoryInfo controllerStateCategoryInfo)
    {
        if (controllerStateCategoryInfo.ControllerStateCategoryEnum == (int)StateCategoryEnum.CombatBehaveAI)
        {
            return GetCombatBehaveAIData(workId, controllerStateCategoryInfo);
        }

        return GetWorkStreamData(workId, controllerStateCategoryInfo);
    }
    #endregion

    #region CombatBehaveAI数据特殊处理
    private WorkStreamData GetCombatBehaveAIData(uint workId, StateCategoryManager.ControllerStateCategoryInfo controllerStateCategoryInfo)
    {
        uint teamKey = CombatHelp.CustomCombatWorkId(workId, CombatManager.Instance.m_CombatAI_7_Increase, CombatManager.Instance.m_CombatAI_8_Increase,
                     CombatManager.Instance.m_CombatAI_9_Increase, CombatManager.Instance.m_CombatAI_10_Increase);

        StateCategoryManager.MultiWorkStreamConfigData multiWorkStreamConfigData = null;
        if (controllerStateCategoryInfo.m_BaseWorkStreamConfigData == null)
        {
            multiWorkStreamConfigData = new StateCategoryManager.MultiWorkStreamConfigData();
            controllerStateCategoryInfo.m_BaseWorkStreamConfigData = multiWorkStreamConfigData;
        }
        else
            multiWorkStreamConfigData = controllerStateCategoryInfo.m_BaseWorkStreamConfigData as StateCategoryManager.MultiWorkStreamConfigData;

        if (!multiWorkStreamConfigData.m_DataDic.TryGetValue(teamKey, out StateCategoryManager.SingleWorkStreamConfigData singleWorkStreamConfigData) || singleWorkStreamConfigData == null)
        {
            singleWorkStreamConfigData = new StateCategoryManager.SingleWorkStreamConfigData();
            multiWorkStreamConfigData.m_DataDic[teamKey] = singleWorkStreamConfigData;

            string dir = Path.GetDirectoryName(controllerStateCategoryInfo.WorkStreamDataFile);
            string fileName = Path.GetFileNameWithoutExtension(controllerStateCategoryInfo.WorkStreamDataFile);

            SetWorkStream($"{dir}/{fileName}_{teamKey.ToString()}.txt", singleWorkStreamConfigData);
        }

        return GetWorkStreamData(workId, singleWorkStreamConfigData);
    }
    #endregion
}
