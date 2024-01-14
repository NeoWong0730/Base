using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Packet;
using System.Text;
using System.IO;

public sealed class TQueue<T>
{
    private Queue<T> _queue = new Queue<T>();

    public float DestroyTime = 90f;
    public string Name;
    public float LastUseTime;
    public int Count
    {
        get
        {
            return _queue.Count;
        }
    }

    public TQueue() { }

    public TQueue(float destroyTime)
    {
        DestroyTime = destroyTime;
    }

    public TQueue(string name)
    {
        Name = name;
    }

    public void Clear()
    {
        _queue.Clear();
    }

    public bool CheckExpire()
    {
        return Time.realtimeSinceStartup - LastUseTime > DestroyTime;
    }

    public T Dequeue()
    {
        LastUseTime = Time.realtimeSinceStartup;
        return _queue.Dequeue();
    }

    public void Enqueue(T go)
    {
        LastUseTime = Time.realtimeSinceStartup;
        _queue.Enqueue(go);
    }

    public bool IsContain(T go)
    {
        return _queue.Contains(go);
    }

#if DEBUG_MODE
    public T[] GetArray()
    {
        return _queue.ToArray();
    }
#endif
}

public enum StrParseEnum
{
    Comma,
    Colon,
    VerticalLine,
}

[System.Flags]
public enum CombatUnitType
{
    Zero = 0,
    Hero = 1 << 1,
    Pet = 1 << 2,
    Monster = 1 << 3,
    Partner = 1 << 4,
    Robot = 1 << 5,
}

[System.Serializable]
public class Bezier3CurvesData
{
    [FileDataOperation(0, 0)]
    public float EulerAngleX;
    [FileDataOperation(0, 1)]
    public float EulerAngleY;
    [FileDataOperation(0, 2)]
    public float EulerAngleZ;

    [FileDataOperation(0, 3)]
    public Bezier3PosData[] m_BezierPosDatas;

    [FileDataOperation(0, 4)]
    public int[] m_BezierParams;
}

[System.Serializable]
public class Bezier3PosData
{
    [FileDataOperation(0, 0)]
    public float BezierCurverLen;
    [FileDataOperation(0, 1)]
    public uint Segment;
    [FileDataOperation(0, 2)]
    public Vector3 LeftHelpPos;     //相对Pos的位置
    [FileDataOperation(0, 3)]
    public Vector3 Pos;             //相对编辑时根节点的位置
    [FileDataOperation(0, 4)]
    public Vector3 RightHelpPos;    //相对Pos的位置
    [FileDataOperation(0, 5)]
    public BezierCurverEvent[] EventInfoArray;
}

[System.Serializable]
public class Bezier1CurvesData
{
    [FileDataOperation(0, 0)]
    public float Y_MaxVal;
    [FileDataOperation(0, 1)]
    public float Y_MinVal;

    [FileDataOperation(0, 2)]
    public float LeftTopY;
    [FileDataOperation(0, 3)]
    public float Hight;

    [FileDataOperation(0, 4)]
    public Bezier1GroupPosData[] Bezier1GroupPosDataArray;
}

/// <summary>
/// 左辅助点，重点，又辅助点为一个组点
/// </summary>
[System.Serializable]
public class Bezier1GroupPosData
{
    [FileDataOperation(0, 0)]
    public float LeftPos;
    [FileDataOperation(0, 1)]
    public float Pos;
    [FileDataOperation(0, 2)]
    public float RightPos;

    [FileDataOperation(0, 3)]
    public float LeftRatioInTotal;
    [FileDataOperation(0, 4)]
    public float PosRatioInTotal;
    [FileDataOperation(0, 5)]
    public float RightRatioInTotal;
}

[System.Serializable]
public class BezierCurverEvent
{
    [FileDataOperation(0, 0)]
    public int EventId;
    [FileDataOperation(0, 1)]
    public float EventRatio;
    [FileDataOperation(0, 2)]
    public int EventCount;
    [FileDataOperation(0, 3)]
    public string EventFlagName;
}

public sealed class CombatHelp
{
    public static readonly string s_Int64 = "Int64";
    public static readonly string s_UInt64 = "UInt64";
    public static readonly string s_Int32 = "Int32";
    public static readonly string s_UInt32 = "UInt32";
    public static readonly string s_Int16 = "Int16";
    public static readonly string s_UInt16 = "UInt16";
    public static readonly string s_Single = "Single";
    public static readonly string s_String = "String";
    public static readonly string s_Boolean = "Boolean";
    public static readonly string s_Byte = "Byte";
    public static readonly string s_SByte = "SByte";

    public static readonly string s_BindHeadName = "head";
    public static readonly string s_BindBodyName = "body";
    public static readonly string s_BindSkillName = "skill";

    public static ulong m_StartId;
    public static uint m_UnitStartId;

    public static readonly Vector3 FarV3 = new Vector3(5000f, 5000f, 5000f);

    private static Dictionary<string, string[]> _strParse1Dic = new Dictionary<string, string[]>();
    private static Dictionary<string, string[][]> _strParse2Dic = new Dictionary<string, string[][]>();

    private static Dictionary<string, float[]> _strParseFloat1Dic = new Dictionary<string, float[]>();
    private static Dictionary<string, uint[]> _strParseUint1Dic = new Dictionary<string, uint[]>();
    private static Dictionary<string, int[]> _strParseInt1Dic = new Dictionary<string, int[]>();

    public static string[] GetStrParse1Array(string str, StrParseEnum strParseEnum = StrParseEnum.VerticalLine)
    {
        if (string.IsNullOrEmpty(str))
            return null;

        string[] strParseArray = null;
        if (!_strParse1Dic.TryGetValue(str, out strParseArray) || strParseArray == null)
        {
            string[] one = null;
            if (strParseEnum == StrParseEnum.VerticalLine)
            {
                one = str.Split('|');
            }
            else if (strParseEnum == StrParseEnum.Colon)
            {
                one = str.Split(':');
            }
            if (one == null || one.Length == 0)
                return null;

            strParseArray = one;

            _strParse1Dic[str] = strParseArray;
        }

        return strParseArray;
    }

    public static string[][] GetStrParse2Array(string str)
    {
        if (string.IsNullOrEmpty(str))
            return null;

        string[][] strParseArray = null;
        if (_strParse2Dic.ContainsKey(str))
        {
            strParseArray = _strParse2Dic[str];
        }

        if (strParseArray == null)
        {
            string[] one = str.Split(':');
            if (one.Length == 0)
                return null;

            strParseArray = new string[one.Length][];
            for (int i = 0; i < one.Length; i++)
            {
                if (string.IsNullOrEmpty(one[i]))
                    continue;

                bool haveTwoLayer = false;

                string[] two = one[i].Split('|');
                strParseArray[i] = new string[two.Length];
                if (two.Length > 1)
                {
                    for (int j = 0; j < two.Length; j++)
                    {
                        if (string.IsNullOrEmpty(two[j]))
                            continue;

                        haveTwoLayer = true;

                        strParseArray[i][j] = two[j];
                    }
                }

                if (!haveTwoLayer)
                {
                    strParseArray[i][0] = one[i];
                }
            }

            _strParse2Dic[str] = strParseArray;
        }

        return strParseArray;
    }

    public static float[] GetStrParseFloat1Array(string str, StrParseEnum strParseEnum = StrParseEnum.VerticalLine)
    {
        if (string.IsNullOrEmpty(str))
            return null;

        float[] strParseFloatArray = null;
        if (!_strParseFloat1Dic.TryGetValue(str, out strParseFloatArray) || strParseFloatArray == null)
        {
            string[] one = null;
            if (strParseEnum == StrParseEnum.VerticalLine)
            {
                one = str.Split('|');
            }
            else if (strParseEnum == StrParseEnum.Colon)
            {
                one = str.Split(':');
            }
            if (one == null || one.Length == 0)
                return null;

            strParseFloatArray = new float[one.Length];
            for (int i = 0; i < one.Length; i++)
            {
                strParseFloatArray[i] = float.Parse(one[i]);
            }

            _strParseFloat1Dic[str] = strParseFloatArray;
        }

        return strParseFloatArray;
    }

    public static uint[] GetStrParseUint1Array(string str, StrParseEnum strParseEnum = StrParseEnum.VerticalLine)
    {
        if (string.IsNullOrEmpty(str))
            return null;

        uint[] strParseUintArray = null;
        if (!_strParseUint1Dic.TryGetValue(str, out strParseUintArray) || strParseUintArray == null)
        {
            string[] one = null;
            if (strParseEnum == StrParseEnum.Comma)
            {
                one = str.Split(',');
            }
            else if (strParseEnum == StrParseEnum.Colon)
            {
                one = str.Split(':');
            }
            else if (strParseEnum == StrParseEnum.VerticalLine)
            {
                one = str.Split('|');
            }
            if (one == null || one.Length == 0)
                return null;

            strParseUintArray = new uint[one.Length];
            for (int i = 0; i < one.Length; i++)
            {
                strParseUintArray[i] = uint.Parse(one[i]);
            }

            _strParseUint1Dic[str] = strParseUintArray;
        }

        return strParseUintArray;
    }

    public static int[] GetStrParseInt1Array(string str, StrParseEnum strParseEnum = StrParseEnum.VerticalLine)
    {
        if (string.IsNullOrEmpty(str))
            return null;

        int[] strParseIntArray = null;
        if (!_strParseInt1Dic.TryGetValue(str, out strParseIntArray) || strParseIntArray == null)
        {
            string[] one = null;
            if (strParseEnum == StrParseEnum.VerticalLine)
            {
                one = str.Split('|');
            }
            else if (strParseEnum == StrParseEnum.Comma)
            {
                one = str.Split(',');
            }
            else if (strParseEnum == StrParseEnum.Colon)
            {
                one = str.Split(':');
            }
            if (one == null || one.Length == 0)
                return null;

            strParseIntArray = new int[one.Length];
            for (int i = 0; i < one.Length; i++)
            {
                strParseIntArray[i] = int.Parse(one[i]);
            }

            _strParseInt1Dic[str] = strParseIntArray;
        }

        return strParseIntArray;
    }

    public static void ClearStrParse()
    {
        _strParse2Dic.Clear();
        _strParse1Dic.Clear();

        _strParseFloat1Dic.Clear();
        _strParseUint1Dic.Clear();
        _strParseInt1Dic.Clear();
    }

    private static float SquareDis(float center, float min, float max)
    {
        if (center < min)
        {
            return Mathf.Pow(min - center, 2f);
        }
        else if (center > max)
        {
            return Mathf.Pow(center - max, 2f);
        }
        else
        {
            return 0;
        }
    }

    public static T GetNeedComponent<T>(GameObject go) where T : Component
    {
        T t = go.GetComponent<T>();
        if (t == null)
            t = go.AddComponent<T>();

        return t;
    }

    public static void Swap<T>(ref T t1, ref T t2)
    {
        T t3 = t1;
        t1 = t2;
        t2 = t3;
    }

    public static float Get2dDisSquare(Vector2 startPos, Vector2 endPos)
    {
        Vector2 v = endPos - startPos;
        return (v.x * v.x + v.y * v.y);
    }

    public static float Get2dDisSquare(Vector3 startPos, Vector3 endPos)
    {
        Vector3 v = endPos - startPos;
        return (v.x * v.x + v.y * v.y + v.z * v.z);
    }

    public static float SimulateDot(Vector2 a, Vector2 b)
    {
        return (a.x * b.x + a.y * b.y);
    }

    public static float SimulateDot(Vector3 a, Vector3 b)
    {
        return (a.x * b.x + a.y * b.y + a.z * b.z);
    }

    public static float CrossVector2(Vector2 a, Vector2 b)
    {
        return a.x * b.y - b.x * a.y;
    }

    public static Vector3 CalLineFormula(float speedX, float gx, float time, Vector3 right)
    {
        return ((speedX + 0.5f * gx * time) * time * right);
    }

    public static Vector3 CalLineFormula02(float speedX, float gx, float time, float deltaTime, Vector3 right)
    {
        return (speedX + gx * time) * deltaTime * right;
    }

    public static Vector3 CalParabolaFormula(float speedX, float gx, float speedY, float gy, float time, int dir)
    {
        return ((speedX + 0.5f * gx * time) * time * dir * Vector3.right + (speedY - 0.5f * gy * time) * time * Vector3.up);
    }

    public static void ParseBehaveDatas(string str, List<BehaveInfo> behaveInfoList, BehaveData behaveData)
    {
        if (string.IsNullOrEmpty(str))
            return;

        BehaveInfo behaveInfo = null;
        BehaveInfoNode node = null;

        string[] b_strs = str.Split('#');
        for (int i = 0; i < b_strs.Length; i++)
        {
            if (string.IsNullOrEmpty(b_strs[i]))
                continue;

            string[] c_strs = b_strs[i].Split('*');
            if (c_strs.Length > 0 && !string.IsNullOrEmpty(c_strs[0]))
            {
                behaveInfo = new BehaveInfo();
                behaveInfo.CurBehaveType = int.Parse(c_strs[0]);
                behaveInfo.AttachSide = int.Parse(c_strs[1]);
                behaveInfo.NextBehaveType = int.Parse(c_strs[2]);

                if (behaveInfoList != null)
                {
                    behaveInfoList.Add(behaveInfo);
                }
                if (behaveData != null)
                {
                    if (behaveInfo.AttachSide == 1)
                    {
                        if (behaveData.m_TargetBehaveInfos == null)
                            behaveData.m_TargetBehaveInfos = new List<BehaveInfo>();
                        behaveData.m_TargetBehaveInfos.Add(behaveInfo);
                    }
                    else
                    {
                        if (behaveData.m_AttackBehaveInfos == null)
                            behaveData.m_AttackBehaveInfos = new List<BehaveInfo>();
                        behaveData.m_AttackBehaveInfos.Add(behaveInfo);
                    }
                }

                if (!string.IsNullOrEmpty(c_strs[3]))
                {
                    string[] d_strs = c_strs[3].Split('{');
                    for (int k = 0; k < d_strs.Length; k++)
                    {
                        if (string.IsNullOrEmpty(d_strs[k]))
                            continue;

                        string[] e_strs = d_strs[k].Split('}');
                        if (e_strs.Length < 2)
                            continue;

                        node = new BehaveInfoNode();
                        node.BebaveNodeType = int.Parse(e_strs[0]);
                        node.NodeContentStr = e_strs[1];

                        if (behaveInfo.NodeList == null)
                            behaveInfo.NodeList = new List<BehaveInfoNode>();
                        behaveInfo.NodeList.Add(node);
                    }
                }
            }
        }
    }

    public static BezierData ParseBezierDatas(string str)
    {
        if (string.IsNullOrEmpty(str))
            return null;

        BezierData bezierData = new BezierData();

        string[] a_strs = str.Split('&');
        bezierData.Speed = float.Parse(a_strs[0]);

        string[] b_strs = a_strs[1].Split('#');

        BezierPosData[] bds = new BezierPosData[b_strs.Length];
        for (int i = 0; i < b_strs.Length; i++)
        {
            string[] c_strs = b_strs[i].Split('*');

            string[] d_strs = c_strs[1].Split('|');
            BezierPosData bd = new BezierPosData();
            if (!string.IsNullOrEmpty(d_strs[0]))
            {
                string[] f_strs = d_strs[0].Split(':');
                bd.LeftHelpPos.x = float.Parse(f_strs[0]);
                bd.LeftHelpPos.y = float.Parse(f_strs[1]);
                bd.LeftHelpPos.z = float.Parse(f_strs[2]);
            }
            if (!string.IsNullOrEmpty(d_strs[1]))
            {
                string[] f_strs = d_strs[1].Split(':');
                bd.Pos.x = float.Parse(f_strs[0]);
                bd.Pos.y = float.Parse(f_strs[1]);
                bd.Pos.z = float.Parse(f_strs[2]);
            }
            if (!string.IsNullOrEmpty(d_strs[2]))
            {
                string[] f_strs = d_strs[2].Split(':');
                bd.RightHelpPos.x = float.Parse(f_strs[0]);
                bd.RightHelpPos.y = float.Parse(f_strs[1]);
                bd.RightHelpPos.z = float.Parse(f_strs[2]);
            }

            bds[i] = bd;
        }
        bezierData.BezierPosDatas = bds;

        return bezierData;
    }

    public static int ClientToServerNum(int clientNum)
    {
        return clientNum / 5 * 10 + clientNum % 5;
    }

    public static int ServerToClientNum(int serverNum, bool isNoMirror = true)
    {
        if (isNoMirror)
        {
            int tNum = serverNum / 10;
            return serverNum - tNum * 5;
        }
        else
        {
            int tNum = serverNum / 10;
            return serverNum - (tNum - 1) * 15;
        }
    }

    public static int GetServerCampSide(int serverPos)
    {
        return serverPos < 20 ? 0 : 1;
    }

    public static int GetClientCampSide(int ClientPos)
    {
        return ClientPos < 10 ? 0 : 1;
    }

    public static bool IsSameCamp(BattleUnit battleUnit01, BattleUnit battleUnit02)
    {
        if (battleUnit01 == null || battleUnit02 == null)
            return false;

        return battleUnit01.Side == battleUnit02.Side;
    }

    public static bool IsSameCamp(BattleUnit battleUnit01, int _side)
    {
        if (battleUnit01 == null)
            return false;

        return battleUnit01.Side == _side;
    }

    public static bool ContainAnimType(Logic.AnimType a, Logic.AnimType b)
    {
        if ((a & b) != 0)
        {
            return true;
        }
        return false;
    }

    public static CombatUnitType SwitchCombatUnitType(uint netUnitType)
    {
        switch (netUnitType)
        {
            case (uint)UnitType.Zero:
                return CombatUnitType.Zero;

            case (uint)UnitType.Hero:
                return CombatUnitType.Hero;

            case (uint)UnitType.Pet:
                return CombatUnitType.Pet;

            case (uint)UnitType.Monster:
                return CombatUnitType.Monster;

            case (uint)UnitType.Partner:
                return CombatUnitType.Partner;

            case (uint)UnitType.Robot:
                return CombatUnitType.Robot;

            default:
                return CombatUnitType.Zero;
        }
    }

    public static string ChangeFieldTypeStr(string str)
    {
        if (str == "Int64")
        {
            return "long";
        }
        else if (str == "UInt64")
        {
            return "ulong";
        }
        else if (str == "Int32")
        {
            return "int";
        }
        else if (str == "UInt32")
        {
            return "uint";
        }
        else if (str == "Single")
        {
            return "float";
        }
        else if (str == "Double")
        {
            return "double";
        }
        else if (str == "String")
        {
            return "string";
        }
        else if (str == "Int16")
        {
            return "short";
        }
        else if (str == "UInt16")
        {
            return "ushort";
        }
        else if (str == "Boolean")
        {
            return "bool";
        }
        else if (str == "Byte")
        {
            return "byte";
        }
        else if (str == "SByte")
        {
            return "sbyte";
        }

        return str;
    }

    public static void WriteValueByType(BinaryWriter binaryWriter, string typeName, object val)
    {
        if (typeName == s_Int64)
        {
            binaryWriter.Write((long)val);
        }
        else if (typeName == s_UInt64)
        {
            binaryWriter.Write((ulong)val);
        }
        else if (typeName == s_Int32)
        {
            binaryWriter.Write((int)val);
        }
        else if (typeName == s_UInt32)
        {
            binaryWriter.Write((uint)val);
        }
        else if (typeName == s_Int16)
        {
            binaryWriter.Write((short)val);
        }
        else if (typeName == s_UInt16)
        {
            binaryWriter.Write((ushort)val);
        }
        else if (typeName == s_Single)
        {
            binaryWriter.Write((float)val);
        }
        else if (typeName == s_String)
        {
            WriteString(binaryWriter, (string)val);
        }
        else if (typeName == s_Boolean)
        {
            binaryWriter.Write((bool)val);
        }
        else if (typeName == s_Byte)
        {
            binaryWriter.Write((byte)val);
        }
        else if (typeName == s_SByte)
        {
            binaryWriter.Write((sbyte)val);
        }
        else
        {
            Debug.LogError($"暂不支持该类型：{typeName}");
        }
    }

    public static object ReadValueByType(BinaryReader binaryReader, string typeName)
    {
        if (typeName == s_Int64)
        {
            return binaryReader.ReadInt64();
        }
        else if (typeName == s_UInt64)
        {
            return binaryReader.ReadUInt64();
        }
        else if (typeName == s_Int32)
        {
            return binaryReader.ReadInt32();
        }
        else if (typeName == s_UInt32)
        {
            return binaryReader.ReadUInt32();
        }
        else if (typeName == s_Int16)
        {
            return binaryReader.ReadInt16();
        }
        else if (typeName == s_UInt16)
        {
            return binaryReader.ReadUInt16();
        }
        else if (typeName == s_Single)
        {
            return binaryReader.ReadSingle();
        }
        else if (typeName == s_String)
        {
            ushort len = binaryReader.ReadUInt16();
            if (len == 0)
                return null;
            else
                return Encoding.UTF8.GetString(binaryReader.ReadBytes(len));
        }
        else if (typeName == s_Boolean)
        {
            return binaryReader.ReadBoolean();
        }
        else if (typeName == s_Byte)
        {
            return binaryReader.ReadByte();
        }
        else if (typeName == s_SByte)
        {
            return binaryReader.ReadSByte();
        }
        else
        {
            Debug.LogError($"暂不支持该类型：{typeName}");
        }

        return null;
    }

    public static string GenerateBaseType(System.Type objType)
    {
        string typeName = objType.Name;

        if (typeName == s_Int64)
        {
            return "br.ReadInt64()";
        }
        else if (typeName == s_UInt64)
        {
            return "br.ReadUInt64()";
        }
        else if (typeName == s_Int32)
        {
            return "br.ReadInt32()";
        }
        else if (typeName == s_UInt32)
        {
            return "br.ReadUInt32()";
        }
        else if (typeName == s_Int16)
        {
            return "br.ReadInt16()";
        }
        else if (typeName == s_UInt16)
        {
            return "br.ReadUInt16()";
        }
        else if (typeName == s_Single)
        {
            return "br.ReadSingle()";
        }
        else if (typeName == s_String)
        {
            return "CombatHelp.ReadString(br)";
        }
        else if (typeName == s_Boolean)
        {
            return "br.ReadBoolean()";
        }
        else if (typeName == s_Byte)
        {
            return "br.ReadByte()";
        }
        else if (typeName == s_SByte)
        {
            return "br.ReadSByte()";
        }
        else
        {
            Debug.LogError($"暂不支持该类型：{typeName}");
        }

        return null;
    }

    public static void WriteString(BinaryWriter binaryWriter, string val)
    {
        int count = 0;
        byte[] bs = null;
        if (!string.IsNullOrEmpty(val))
        {
            bs = Encoding.UTF8.GetBytes(val);
            count = bs.Length;
        }

        if (count > ushort.MaxValue)
        {
            Debug.LogError($"string长度暂支持长度：{ushort.MaxValue.ToString()}, 现已超出长度, 作废");
            count = 0;
        }

        ushort len = (ushort)count;
        binaryWriter.Write(len);
        if (len == 0)
            return;

        binaryWriter.Write(bs);
    }

    public static string ReadString(BinaryReader binaryReader)
    {
        ushort len = binaryReader.ReadUInt16();
        if (len == 0)
            return null;
        else
            return Encoding.UTF8.GetString(binaryReader.ReadBytes(len));
    }

    public static void GetGameObjectBinds(Transform trans, Dictionary<uint, Transform> bindGameObjectDic)
    {
        if (bindGameObjectDic == null)
            return;

        if (bindGameObjectDic.Count > 0)
            bindGameObjectDic.Clear();

        FindGameObjectBind(trans, bindGameObjectDic);
    }

    private static void FindGameObjectBind(Transform trans, Dictionary<uint, Transform> bindGameObjectDic)
    {
        if (trans == null || bindGameObjectDic.Count >= 3)
            return;

        string name = trans.name;

        if (name == s_BindHeadName && !bindGameObjectDic.ContainsKey(1u))
        {
            bindGameObjectDic[1u] = trans;
        }
        else if (name == s_BindBodyName && !bindGameObjectDic.ContainsKey(2u))
        {
            bindGameObjectDic[2u] = trans;
        }
        else if (name == s_BindSkillName && !bindGameObjectDic.ContainsKey(3u))
        {
            bindGameObjectDic[3u] = trans;
        }

        for (int i = 0, count = trans.childCount; i < count; i++)
        {
            var child = trans.GetChild(i);
            if (child == null)
                continue;

            FindGameObjectBind(child, bindGameObjectDic);
        }
    }

    public static uint CustomCombatWorkId(uint dataWorkId, uint Increase_7, uint Increase_8, uint Increase_9, uint Increase_10)
    {
        uint k = dataWorkId / 1000000u;
        uint teamKey = 1u;
        //if (k > 0u && k < 10u)
        //{
        //    teamKey = (k / Increase_7) + 1;
        //}
        //else if (k > 9u && k < 100u)
        //{
        //    teamKey = (k / (Increase_8 * 10u)) + 1;
        //    teamKey *= 10u;
        //}
        if (k > 99u && k < 1000u)
        {
            teamKey = k / (Increase_9 * 10u);
            teamKey += 100u;
        }
        else if (k > 999u && k < 10000u)
        {
            teamKey = 1000u;
        }

        return teamKey;
    }

    public static float GetFloat3Decimal(float f)
    {
        int i = (int)f;
        float frac = f - i;
        int fi = (int)(frac * 1000);

        return i + fi / 1000f;

        //return (int)(f * 1000) / 1000f;
    }

    public static Vector3 GetVector3(BinaryReader br)
    {
        float x = br.ReadSingle();
        float y = br.ReadSingle();
        float z = br.ReadSingle();

        return new Vector3(x, y, z);
    }

    #region 一维贝塞尔曲线
    //三阶贝塞尔曲线
    //(1-t)^3P0 + 3(1-t)^2tP1 + 3(1-t)t^2P2 + t^3*P3
    public static float Calculate3BezierPoint_1D(float t, float p0, float p1, float p2, float p3)
    {
        float u = 1f - t;
        float u2 = u * u;
        float t2 = t * t;

        return u2 * u * p0 + 3 * t * u2 * p1 + 3 * t2 * u * p2 + t2 * t * p3;
    }
    #endregion

    #region 二维贝塞尔曲线
    //二阶贝塞尔曲线
    //(1-t)^2P0 + 2(1-t)tP1 + t^2*P2
    public static Vector2 Calculate2BezierPoint_2D(float t, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        float u = 1f - t;

        return u * u * p0 + 2 * t * u * p1 + t * t * p2;
    }

    //三阶贝塞尔曲线
    //(1-t)^3P0 + 3(1-t)^2tP1 + 3(1-t)t^2P2 + t^3*P3
    public static Vector2 Calculate3BezierPoint_2D(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        float u = 1f - t;
        float u2 = u * u;
        float t2 = t * t;

        return u2 * u * p0 + 3 * t * u2 * p1 + 3 * t2 * u * p2 + t2 * t * p3;
    }

    //四阶贝塞尔曲线
    //(1-t)^4P0 + 4(1-t)^3tP1 + 6(1-t)^2t^2P2 + 4(1-t)t^3P3 + t^4*P4
    public static Vector2 Calculate4BezierPoint_2D(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        float u = 1f - t;
        float u2 = u * u;
        float t2 = t * t;

        return u2 * u2 * p0 + 4 * u2 * u * t * p1 + 6 * u2 * t2 * p2 + 4 * u * t2 * t * p3 + t2 * t2 * p4;
    }
    #endregion

    #region 三维贝塞尔曲线
    //二阶贝塞尔曲线
    public static Vector3 Calculate2BezierPoint_3D(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1f - t;

        return u * u * p0 + 2 * t * u * p1 + t * t * p2;
    }

    //三阶贝塞尔曲线
    public static Vector3 Calculate3BezierPoint_3D(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1f - t;
        float u2 = u * u;
        float t2 = t * t;

        return u2 * u * p0 + 3 * t * u2 * p1 + 3 * t2 * u * p2 + t2 * t * p3;
    }

    //四阶贝塞尔曲线
    public static Vector3 Calculate4BezierPoint_3D(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        float u = 1f - t;
        float u2 = u * u;
        float t2 = t * t;

        return u2 * u2 * p0 + 4 * u2 * u * t * p1 + 6 * u2 * t2 * p2 + 4 * u * t2 * t * p3 + t2 * t2 * p4;
    }
    #endregion

    /// <summary>
    /// clientTrigger=clientNum * 10000u + serverTimingtrigger
    /// </summary>
    public static uint EffectTriggerTypeServerToClient(uint serverTimingtrigger)
    {
        if (serverTimingtrigger == 8u)
            return 100008u;
        if (serverTimingtrigger == 37u)
            return 110037u;
        else if (serverTimingtrigger == 14u)
            return 200014u;
        else if (serverTimingtrigger == 21u)
            return 300021u;
        else if (serverTimingtrigger == 22u)
            return 400022u;
        else if (serverTimingtrigger == 26u)
            return 500026u;
        else if (serverTimingtrigger == 27u)
            return 600027u;
        else if (serverTimingtrigger == 28u)
            return 700028u;
        else if (serverTimingtrigger == 31u)
            return 800031u;
        else if (serverTimingtrigger == 23u)
            return 900023u;
        else if (serverTimingtrigger == 1u)
            return 1000001u;
        else if (serverTimingtrigger == 20u)
            return 1100020u;
        else if (serverTimingtrigger == 34u)
            return 1110034u;
        else if (serverTimingtrigger == 32u)
            return 1200032u;
        else if (serverTimingtrigger == 11u)
            return 1300011u;
        else if (serverTimingtrigger == 15u)
            return 1400015u;
        else if (serverTimingtrigger == 3u)        //bo    Start
            return 1500003u;
        else if (serverTimingtrigger == 5u)
            return 1600005u;
        else if (serverTimingtrigger == 7u)
            return 1700007u;
        else if (serverTimingtrigger == 19u)
            return 1800019u;
        else if (serverTimingtrigger == 16u)
            return 1900016u;
        else if (serverTimingtrigger == 33u)
            return 1910033u;
        else if (serverTimingtrigger == 39u)
            return 1911039u;
        else if (serverTimingtrigger == 9u)
            return 2000009u;
        else if (serverTimingtrigger == 24u)
            return 2100024u;
        else if (serverTimingtrigger == 36u)
            return 2110036u;
        else if (serverTimingtrigger == 40u)
            return 2111040u;
        else if (serverTimingtrigger == 25u)
            return 2300025u;
        else if (serverTimingtrigger == 10u)
            return 2400010u;
        else if (serverTimingtrigger == 38u)
            return 2410038u;
        else if (serverTimingtrigger == 13u)
            return 2500013u;
        else if (serverTimingtrigger == 47u)
            return 2710047u;
        else if (serverTimingtrigger == 42u)
            return 2720042u;
        else if (serverTimingtrigger == 45u)
            return 2730045u;
        else if (serverTimingtrigger == 46u)
            return 2740046u;
        else if (serverTimingtrigger == 48u)
            return 2750048u;
        else if (serverTimingtrigger == 49u)
            return 2760049u;
        else if (serverTimingtrigger == 50u)
            return 2770050u;
        else if (serverTimingtrigger == 54u)
            return 2780054u;
        else if (serverTimingtrigger == 6u)
            return 2800006u;
        else if (serverTimingtrigger == 12u)
            return 3000012u;
        else if (serverTimingtrigger == 4u)
            return 3000104u;
        else if (serverTimingtrigger == 29u)
            return 3010029u;
        else if (serverTimingtrigger == 18u)        //bo    End
            return 3020018u;
        else if (serverTimingtrigger == 59u)
            return 3090059u;
        else if (serverTimingtrigger == 17u)
            return 3100017u;
        else if (serverTimingtrigger == 35u)
            return 3110035u;
        else if (serverTimingtrigger == 2u)
            return 3200002u;

        return 0u;
    }

    public static uint BuffStateToLevelPriority(uint buffState)
    {
        if (buffState == 1)
            return 100001u;
        else if (buffState == 2)
            return 200002u;
        else if (buffState == 3)
            return 300003u;
        else if (buffState == 4)
            return 400004u;
        else if (buffState == 5)
            return 500005u;
        else if (buffState == 6)
            return 600006u;
        else if (buffState == 7)
            return 700007u;
        else if (buffState == 8)
            return 800008u;
        else if (buffState == 9)
            return 900009u;
        else if (buffState == 10)
            return 1000010u;

        return 0u;
    }

    public static bool IsAllowDistanceFor2Point(float a, float b)
    {
        return Mathf.Abs(a - b) < 0.01f;
    }
}
