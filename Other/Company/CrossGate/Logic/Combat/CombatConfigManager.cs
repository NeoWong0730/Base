using Lib.AssetLoader;
using Lib.Core;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CombatConfigManager
{
    private static CombatConfigManager instance;

    public static CombatConfigManager Instance
    {
        get
        {
            if (null == instance)
            {
                instance = new CombatConfigManager();
            }
            return instance;
        }
    }

    public static string BezierTrackDataPath = "Config/CombatData/BezierCurverData/BezierTrackData.txt";
    public static string BezierRatioDataPath = "Config/CombatData/BezierCurverData/BezierRatioData.txt";

    private FileDataOperationReadEntity _fileDataOperationReadEntity;

    public int m_BehaveType;
    
    private Dictionary<uint, string> _behaveDic;
    private Dictionary<uint, BehaveData> _behaveInfoDataDic;
    
    private Dictionary<uint, string> _bezierStrDic;
    private Dictionary<uint, BezierData> _bezierDataDic;

    private Dictionary<uint, CombatPosData> _combatPosDic;

    private WS_StyleInfo<Bezier3CurvesData> _bezierTrackDatasInfo;

    private WS_StyleInfo<Bezier1CurvesData> _bezierRatioDatasInfo;

    public CombatConfigManager()
    {
        #region 贝塞尔曲线数据
        SetBezierDataConfig();
        #endregion

        #region 战斗位置
        SetCombatPosDataConfig();
        #endregion

        SetBezierDatas();
    }

    public void ResetConfigData(int configType = 0)
    {
        switch (configType)
        {
            case 0:
                WorkStreamConfigManager.Instance.Refresh((int)StateCategoryEnum.CombatBehaveAI);

                SetBezierDataConfig(true);
                
                SetCombatPosDataConfig(true);
                break;
                
            case 2:
                SetBezierDataConfig(true);
                break;

            case 3:
                SetCombatPosDataConfig(true);
                break;

            case 5:
                WorkStreamConfigManager.Instance.Refresh((int)StateCategoryEnum.CombatBehaveAI);
                break;
        }
        
    }
    
    #region 战斗行为
    public void SetCombatBehaveConfig(bool isRefresh = false)
    {
        string behavePath = $"Config/CombatData/CombatBehave/CombatBehave{(m_BehaveType == 0 ? string.Empty : m_BehaveType.ToString())}.txt";

        Lib.Core.DebugUtil.Log(Lib.Core.ELogType.eCombat, $"<color=yellow>战斗行为配置更新为:{behavePath}</color>");

        if (_behaveDic == null)
            _behaveDic = new Dictionary<uint, string>();
        if (_behaveInfoDataDic == null)
            _behaveInfoDataDic = new Dictionary<uint, BehaveData>();

        if (isRefresh)
        {
            _behaveDic.Clear();
            _behaveInfoDataDic.Clear();
        }

        Stream stream = AssetMananger.Instance.LoadStream(behavePath);
        StreamReader sr = new StreamReader(stream);
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            if (string.IsNullOrEmpty(line))
                continue;

            string[] a_strs = line.Split('@');
            if (a_strs.Length < 2 || string.IsNullOrEmpty(a_strs[0]) || string.IsNullOrEmpty(a_strs[1]))
                continue;

            uint skillId = 0u;
            if (!uint.TryParse(a_strs[0], out skillId) || skillId == 0u)
                continue;

            _behaveDic[skillId] = a_strs[1];
        }
        sr.Close();
        sr.Dispose();
        stream.Close();
        stream.Dispose();
    }

    /// <summary>
    /// attachType=0施放者，=1目标
    /// </summary>
    public List<BehaveInfo> GetBehaveData(uint skillId, int attachType)
    {
        if (_behaveInfoDataDic.ContainsKey(skillId))
        {
            var behaveData = _behaveInfoDataDic[skillId];
            if (attachType == 1)
                return behaveData.m_TargetBehaveInfos;
            else
                return behaveData.m_AttackBehaveInfos;
        }

        if (!_behaveDic.ContainsKey(skillId))
            return null;

        BehaveData bd = new BehaveData();
        _behaveInfoDataDic.Add(skillId, bd);

        CombatHelp.ParseBehaveDatas(_behaveDic[skillId], null, bd);

        if (attachType == 1)
            return bd.m_TargetBehaveInfos;
        else
            return bd.m_AttackBehaveInfos;
    }
    #endregion

    #region 贝塞尔曲线数据
    public void SetBezierDataConfig(bool isRefresh = false)
    {
        if (_bezierStrDic == null)
            _bezierStrDic = new Dictionary<uint, string>();
        if (_bezierDataDic == null)
            _bezierDataDic = new Dictionary<uint, BezierData>();

        if (isRefresh)
        {
            _bezierStrDic.Clear();
            _bezierDataDic.Clear();
        }

        Stream stream = AssetMananger.Instance.LoadStream("Config/CombatData/BezierCurve/CombatBezierData.txt");
        StreamReader sr = new StreamReader(stream);
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            if (string.IsNullOrEmpty(line))
                continue;

            string[] a_strs = line.Split('@');
            if (a_strs.Length < 2 || string.IsNullOrEmpty(a_strs[0]) || string.IsNullOrEmpty(a_strs[1]))
                continue;

            uint bezierId = 0u;
            if (!uint.TryParse(a_strs[0], out bezierId) || bezierId == 0u)
                continue;

            _bezierStrDic[bezierId] = a_strs[1];
        }
        sr.Close();
        sr.Dispose();
        stream.Close();
        stream.Dispose();
    }

    public BezierData GetBezierData(uint bezierId)
    {
        if (_bezierDataDic.ContainsKey(bezierId))
            return _bezierDataDic[bezierId];

        if (!_bezierStrDic.ContainsKey(bezierId))
            return null;

        BezierData bezierData = CombatHelp.ParseBezierDatas(_bezierStrDic[bezierId]);
        if (bezierData == null || bezierData.BezierPosDatas == null)
            return null;

        _bezierDataDic.Add(bezierId, bezierData);
        return bezierData;
    }
    #endregion

    #region 战斗位置数据
    public void SetCombatPosDataConfig(bool isRefresh = false)
    {
        if (_combatPosDic == null)
            _combatPosDic = new Dictionary<uint, CombatPosData>();

        if (isRefresh)
        {
            _combatPosDic.Clear();
        }

        Stream stream = AssetMananger.Instance.LoadStream("Config/CombatData/Pos/CombatPosData.txt");
        StreamReader sr = new StreamReader(stream);
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            if (string.IsNullOrEmpty(line))
                continue;

            string[] a_strs = line.Split('|');
            if (a_strs.Length < 2 || string.IsNullOrEmpty(a_strs[0]) || string.IsNullOrEmpty(a_strs[1]))
                continue;

            uint posId = 0u;
            if (!uint.TryParse(a_strs[0], out posId) || posId == 0u)
                continue;

            string[] b_strs = a_strs[1].Split(':');

            CombatPosData combatPosData = new CombatPosData();
            combatPosData.PosX = float.Parse(b_strs[0]);
            combatPosData.PosY = float.Parse(b_strs[1]);
            combatPosData.PosZ = float.Parse(b_strs[2]);
            combatPosData.AngleX = float.Parse(b_strs[3]);
            combatPosData.AngleY = float.Parse(b_strs[4]);
            combatPosData.AngleZ = float.Parse(b_strs[5]);

            _combatPosDic[posId] = combatPosData;
        }
        sr.Close();
        sr.Dispose();
        stream.Close();
        stream.Dispose();
    }

    public CombatPosData GetCombatPosData(uint posId)
    {
        if (_combatPosDic.ContainsKey(posId))
            return _combatPosDic[posId];

        Lib.Core.DebugUtil.LogError($"战斗位置数据没有PosId：{posId.ToString()}");

        return null;
    }
    #endregion

    #region 处理WS_StyleInfo类型数据
    public void SetWS_StyleInfoDatas<T>(string styleInfoFilePath, WS_StyleInfo<T> info)
    {
        string filePath = string.Format(styleInfoFilePath, string.Empty);
#if UNITY_EDITOR
        string fp = string.Format("{0}/{1}", Application.dataPath, filePath);
        if (!File.Exists(fp))
            return;
#endif
        Stream stream = AssetMananger.Instance.LoadStream(filePath);

        info.DataBytes = new byte[stream.Length];
        stream.Read(info.DataBytes, 0, (int)stream.Length);
        stream.Seek(0, SeekOrigin.Begin);
        
        if (info.DataBytePosDic == null)
            info.DataBytePosDic = new Dictionary<uint, uint>();
        else
            info.DataBytePosDic.Clear();

        if (info.DataDic != null)
            info.DataDic.Clear();

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
                    info.DataBytePosDic[workId] = curPos + 4u;

                    rs.Seek(nextPos, SeekOrigin.Begin);
                    curPos = nextPos;
                }
            }
        });
        _fileDataOperationReadEntity.EndRead();
    }
    #endregion

    #region BezierDatas
    public void SetBezierDatas()
    {
        if (_bezierTrackDatasInfo == null)
            _bezierTrackDatasInfo = new WS_StyleInfo<Bezier3CurvesData>();
        if (_bezierRatioDatasInfo == null)
            _bezierRatioDatasInfo = new WS_StyleInfo<Bezier1CurvesData>();

        SetWS_StyleInfoDatas(BezierTrackDataPath, _bezierTrackDatasInfo);
        SetWS_StyleInfoDatas(BezierRatioDataPath, _bezierRatioDatasInfo);
    }

    public Bezier3CurvesData GetBezier3CurvesData(uint bezierTrackId)
    {
        if (_bezierTrackDatasInfo == null)
            return null;

        Bezier3CurvesData t = null;
        if (_bezierTrackDatasInfo.DataDic != null && _bezierTrackDatasInfo.DataDic.TryGetValue(bezierTrackId, out t))
            return t;

        if (_bezierTrackDatasInfo.DataBytePosDic != null && _bezierTrackDatasInfo.DataBytePosDic.TryGetValue(bezierTrackId, out uint pos))
        {
            t = _fileDataOperationReadEntity.DoRead(new MemoryStream(_bezierTrackDatasInfo.DataBytes), (BinaryReader br) =>
            {
                return Bezier3CurvesData_ConfigTool.Load(br);
            },
            (Stream rs, BinaryReader br) =>
            {
                rs.Seek(pos, SeekOrigin.Begin);
                uint readId = br.ReadUInt32();
                if (bezierTrackId != readId)
                {
                    DebugUtil.LogError($"获取WorkId数据不一致,需要的：{bezierTrackId.ToString()}，读取的：{readId.ToString()}");
                }
            });
        }

        if (t == null)
        {
            DebugUtil.LogError($"配置中解析Id:{bezierTrackId.ToString()}数据为null");
            return null;
        }

        if (_bezierTrackDatasInfo.DataDic == null)
            _bezierTrackDatasInfo.DataDic = new Dictionary<uint, Bezier3CurvesData>();

        _bezierTrackDatasInfo.DataDic[bezierTrackId] = t;

        return t;
    }

    public Bezier1CurvesData GetBezier1CurvesData(uint ratioId)
    {
        Bezier1CurvesData t = null;
        if (_bezierRatioDatasInfo.DataDic != null && _bezierRatioDatasInfo.DataDic.TryGetValue(ratioId, out t))
            return t;

        if (_bezierRatioDatasInfo.DataBytePosDic != null && _bezierRatioDatasInfo.DataBytePosDic.TryGetValue(ratioId, out uint pos))
        {
            t = _fileDataOperationReadEntity.DoRead(new MemoryStream(_bezierRatioDatasInfo.DataBytes), (BinaryReader br) =>
            {
                return Bezier1CurvesData_ConfigTool.Load(br);
            },
            (Stream rs, BinaryReader br) =>
            {
                rs.Seek(pos, SeekOrigin.Begin);
                uint readId = br.ReadUInt32();
                if (ratioId != readId)
                {
                    DebugUtil.LogError($"Bezier1CurvesData获取ratioId数据不一致,需要的：{ratioId.ToString()}，读取的：{readId.ToString()}");
                }
            });
        }

        if (t == null)
        {
            DebugUtil.LogError($"Bezier1CurvesData配置中解析Id:{ratioId.ToString()}数据为null");
            return null;
        }

        if (_bezierRatioDatasInfo.DataDic == null)
            _bezierRatioDatasInfo.DataDic = new Dictionary<uint, Bezier1CurvesData>();

        _bezierRatioDatasInfo.DataDic[ratioId] = t;

        return t;
    }
    #endregion
}

public class BehaveData
{
    public List<BehaveInfo> m_AttackBehaveInfos;
    public List<BehaveInfo> m_TargetBehaveInfos;
}

public class BehaveInfo
{
    public int CurBehaveType;
    public int AttachSide;      //0=施放者， 1=目标
    public int NextBehaveType;
    public List<BehaveInfoNode> NodeList = new List<BehaveInfoNode>();
}

public class BehaveInfoNode
{
    public int BebaveNodeType;
    public string NodeContentStr;
}

public class BezierData
{
    public float Speed;
    public BezierPosData[] BezierPosDatas;
}

public class BezierPosData
{
    public Vector3 LeftHelpPos;
    public Vector3 Pos;
    public Vector3 RightHelpPos;
}

public class CombatPosData
{
    public float PosX;
    public float PosY;
    public float PosZ;
    public float AngleX;
    public float AngleY;
    public float AngleZ;
}

public class WS_StyleInfo<T>
{
    public byte[] DataBytes;
    public Dictionary<uint, uint> DataBytePosDic;
    public Dictionary<uint, T> DataDic;
}
