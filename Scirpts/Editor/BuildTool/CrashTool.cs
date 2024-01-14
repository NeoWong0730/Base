using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AnalysisCrashSetting : ScriptableObject
{
    //path
    public string addr2linePath64 = string.Empty; //ndk��������·��64λ
    public string addr2linePath32 = string.Empty; //ndk��������·��32λ

    public string unitydebugsoPath64 = string.Empty;  //unity���ϱ�·��
    public string unitydebugsoPath32 = string.Empty;  //unity���ϱ�·��


    public string il2cppdebugsoPath64 = string.Empty; //android ���ű�·��
    public string il2cppdebugsoPath32 = string.Empty; //android ���ű�·��


    public string MyCashPath = string.Empty;
    //public static bool arm64v8a = true; //Ĭ����64λ��������32λ


    [MenuItem("Assets/AnalysisCrashSetting")]
    public static void CreateBuildSetting()
    {
        DefaultAsset[] selects = Selection.GetFiltered<DefaultAsset>(SelectionMode.Assets);
        DefaultAsset floder = selects[0];
        string path = floder != null ? AssetDatabase.GetAssetPath(floder) : "Assets";
        CreateAnalysisCrashSetting(path, "AnalysisCrashSetting");
    }

    public static AnalysisCrashSetting CreateAnalysisCrashSetting(string path, string name)
    {
        //ʹ�ô˹��ߣ����޸ı�������ƥ���·����
        AnalysisCrashSetting crashSetting = ScriptableObject.CreateInstance<AnalysisCrashSetting>();
        crashSetting.addr2linePath64 = @"D:\Program Files\Unity\2019.4.39f1\Editor\Data\PlaybackEngines\AndroidPlayer\NDK\toolchains\aarch64-linux-android-4.9\prebuilt\windows-x86_64\bin\aarch64-linux-android-addr2line.exe";
        crashSetting.unitydebugsoPath64 = @"D:\Android\bundletool\CrossGate_2022.1201.1221_0_IL2CPP_ILRuntime_1.3-1.1.3-v13.symbols\arm64-v8a\libunity.sym.so";
        crashSetting.il2cppdebugsoPath64 = @"D:\Android\bundletool\CrossGate_2022.1201.1221_0_IL2CPP_ILRuntime_1.3-1.1.3-v13.symbols\arm64-v8a\libil2cpp.sym.so";

        crashSetting.addr2linePath32 = @"D:\Program Files\Unity\2019.4.39f1\Editor\Data\PlaybackEngines\AndroidPlayer\NDK\toolchains\arm-linux-androideabi-4.9\prebuilt\windows-x86_64\bin\arm-linux-androideabi-addr2line.exe";
        crashSetting.unitydebugsoPath32 = @"D:\Android\bundletool\CrossGate_2022.1201.1221_0_IL2CPP_ILRuntime_1.3-1.1.3-v13.symbols\armeabi-v7a\libunity.sym.so";
        crashSetting.il2cppdebugsoPath32 = @"D:\Android\bundletool\CrossGate_2022.1201.1221_0_IL2CPP_ILRuntime_1.3-1.1.3-v13.symbols\armeabi-v7a\libil2cpp.sym.so";

        crashSetting.MyCashPath = @"D:\Android\bundletool";
        AssetDatabase.CreateAsset(crashSetting, string.Format("{0}/{1}.asset", path, "AnalysisCrashSetting"));
        return crashSetting;
    }
}




public enum PathType
{
    unitySoPath = 1,
    il2cppSoPath,
    addr2Line
}

public class AnalysisCrashWindow : EditorWindow
{
    bool groupEnabled;
    //path
    string addr2linePath = string.Empty; //ndk��������·��
    string unitydebugsoPath = string.Empty;  //unity  ���ϱ�·��
    string il2cppdebugsoPath = string.Empty; //android ���ű�·��
    string MyCashPath = string.Empty;
    //flag
    string crashlibunityEndFlag = "libunity.so";
    string crashlibil2cppEndFlag = "libil2cpp.so";
    string unityflag = "libunity";
    string il2cppflag = "libil2cpp";

    string unityPath = @"\Data\PlaybackEngines\AndroidPlayer\Variations\mono\Release\Symbols\armeabi-v7a\libunity.sym.so";

    bool isAnalysis = false;//�ļ��Ƿ����
    bool isArm64v8a = true;//Ĭ����64λ

    AnalysisCrashSetting analysisCrashSetting;
    private void OnEnable()
    {
        string settingPath = string.Format("{0}/{1}", Application.dataPath, "AnalysisCrashSetting.asset");
        if (File.Exists(settingPath))
        {
            analysisCrashSetting = AssetDatabase.LoadAssetAtPath<AnalysisCrashSetting>("Assets/AnalysisCrashSetting.asset");
        }
        if (analysisCrashSetting == null)
        {
            analysisCrashSetting = AnalysisCrashSetting.CreateAnalysisCrashSetting("Assets", "AnalysisCrashSetting");
        }

        //GetPathByMemory();
    }

    [MenuItem("__Tools__/AnalysCrashWindow")]
    static void Init()
    {
        AnalysisCrashWindow window = (AnalysisCrashWindow)EditorWindow.GetWindow(typeof(AnalysisCrashWindow));
        window.Show();
    }

    void OnGUI()
    {
        groupEnabled = EditorGUILayout.BeginToggleGroup("��������(������ȷ����·��)", groupEnabled);

        isArm64v8a = EditorGUILayout.ToggleLeft("Ĭ��64λ(ȡ��32λ)", isArm64v8a, GUILayout.Width(150));
        if (isArm64v8a)
        {
            addr2linePath = EditorGUILayout.TextField("NDK����·����addr2linePath��", analysisCrashSetting.addr2linePath64, GUILayout.Width(400));
            unitydebugsoPath = EditorGUILayout.TextField("unity���ű�unitydebugsoPath��", analysisCrashSetting.unitydebugsoPath64, GUILayout.MaxWidth(400));
            il2cppdebugsoPath = EditorGUILayout.TextField("ill2cpp���ϱ�il2cppdebugsoPath��", analysisCrashSetting.il2cppdebugsoPath64, GUILayout.MaxWidth(400));
        }
        else
        {
            addr2linePath = EditorGUILayout.TextField("NDK����·����addr2linePath��", analysisCrashSetting.addr2linePath32, GUILayout.Width(400));
            unitydebugsoPath = EditorGUILayout.TextField("unity���ű�unitydebugsoPath��", analysisCrashSetting.unitydebugsoPath32, GUILayout.MaxWidth(400));
            il2cppdebugsoPath = EditorGUILayout.TextField("ill2cpp���ϱ�il2cppdebugsoPath��", analysisCrashSetting.il2cppdebugsoPath32, GUILayout.MaxWidth(400));
        }
        MyCashPath = EditorGUILayout.TextField("������־�ļ�·��", analysisCrashSetting.MyCashPath, GUILayout.MaxWidth(400));

        EditorGUILayout.Space();

        GUILayout.Label("��������", EditorStyles.boldLabel);
        GetCrashByPath(MyCashPath);
    }

    /// <summary>
    /// ���ڴ��л�ȡ�洢��·��
    /// </summary>
    void GetPathByMemory()
    {
        addr2linePath = EditorPrefs.GetString("addr2linePath");
        il2cppdebugsoPath = EditorPrefs.GetString("il2cppdebugsoPath");
        unitydebugsoPath = EditorPrefs.GetString("unitydebugsoPath");
        if (string.IsNullOrEmpty(unitydebugsoPath))
        {
            unitydebugsoPath = string.Concat(System.AppDomain.CurrentDomain.BaseDirectory, unityPath);
            JudgePath(PathType.unitySoPath, unitydebugsoPath);
        }
        MyCashPath = EditorPrefs.GetString("MyCashPath", MyCashPath);
    }

    /// <summary>
    /// ·���ж�
    /// </summary>
    /// <param name="type">·������</param>
    /// <param name="path"></param>
    bool JudgePath(PathType type, string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }
        bool temp = true;
        if ((int)type == 1)
        {
            if (!path.EndsWith("libunity.sym.so"))
            {
                path = string.Empty;
                Debug.LogError("�Զ����unity���ϱ�·���������ֶ����");
                temp = false;
            }
            else
            {
                if (!File.Exists(path))
                {
                    temp = false;
                    Debug.LogErrorFormat("��ǰ·��{0}unity���ű�����", path);
                }
            }
        }
        else if ((int)type == 2)
        {
            if (!path.EndsWith("libil2cpp.sym.so"))
            {
                temp = false;
            }
            else
            {
                if (!File.Exists(path))
                {
                    temp = false;
                }
            }
        }
        else
        {
            if (isArm64v8a)//v8a
            {
                if (!path.EndsWith("aarch64-linux-android-addr2line.exe"))
                {
                    temp = false;
                }
                else
                {
                    if (!File.Exists(path))
                    {
                        temp = false;
                    }
                }
            }
            else //v7a
            {
                if (!path.EndsWith("arm-linux-androideabi-addr2line.exe"))
                {
                    temp = false;
                }
                else
                {
                    if (!File.Exists(path))
                    {
                        temp = false;
                    }
                }
            }
        }
        return temp;
    }

    /// <summary>
    /// ����Button
    /// </summary>
    /// <param name="name"></param>
    /// <param name="path"></param>
    void CreatorButton(string name, string path)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextField("����", name, GUILayout.MaxWidth(400));
        GUILayout.Space(10);
        if (GUILayout.Button("����", GUILayout.Width(50)))
        {
            if (!JudgePath(PathType.addr2Line, addr2linePath))
            {
                Debug.LogError("Ndk����·������");
                return;
            }
            if (!JudgePath(PathType.unitySoPath, unitydebugsoPath) && !JudgePath(PathType.il2cppSoPath, il2cppdebugsoPath))
            {
                Debug.LogError("unity��il2cppSoPanth���ϱ�·������");
                return;
            }
            if (!JudgePath(PathType.il2cppSoPath, il2cppdebugsoPath))
            {
                Debug.LogError("il2cppSoPanth���ϱ�·������");
            }
            OutCrash(name, path);
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// ���ݻ�ȡCrash�ļ����ļ�����Button����ʾ��
    /// </summary>
    /// <param name="path"></param>
    void GetCrashByPath(string path)
    {
        if (Directory.Exists(path))
        {
            var dirctory = new DirectoryInfo(path);
            var files = dirctory.GetFiles("*", SearchOption.AllDirectories);
            foreach (var fi in files)
            {
                CreatorButton(fi.Name, path);
            }
        }
    }

    List<string> rlts = new List<string>();

    /// <summary>
    /// ��Crash
    /// </summary>
    void OutCrash(string filename, string path)
    {
        rlts.Clear();

        isAnalysis = false;
        string filePath = string.Join("/", path, filename);
        using (StreamReader sr = new StreamReader(filePath))
        {
            while (!sr.EndOfStream)
            {
                OutCmd(sr.ReadLine());
            }
        }
        if (!isAnalysis)
        {
            Debug.LogError("�޷�������ǰcash�ļ��������ļ��Ƿ�Ϊ�豸������־");
        }

        Debug.Log(string.Join("\n", rlts));

        rlts.Clear();
    }

    /// <summary>
    /// ����Crash
    /// </summary>
    void OutCmd(string log)
    {
        if (log == null)
        {
            return;
        }

        //��̨�������ı�����־������пո�
        log = log.TrimEnd();

        if (log.EndsWith(crashlibunityEndFlag) || log.EndsWith(crashlibil2cppEndFlag))//����libunity.so��β�ı�����־��libil2cpp.so��β��
        {
            if (log.Contains("pc"))
            {
                int startIndex = log.IndexOf("pc") + 3;
                if (log.Contains("/data/"))
                {
                    int endIndex = log.IndexOf("/data/");
                    string addStr = log.Substring(startIndex, endIndex - startIndex - 1);
                    if (log.EndsWith(crashlibunityEndFlag))
                    {
                        string tempUnitySoPath = string.Format("\"{0}\"", unitydebugsoPath);
                        ExecuteCmd(tempUnitySoPath, addStr);
                    }
                    else if (log.EndsWith(crashlibil2cppEndFlag))
                    {
                        string tempill2cppSoPath = string.Format("\"{0}\"", il2cppdebugsoPath);
                        ExecuteCmd(tempill2cppSoPath, addStr);
                    }
                    else
                    {
                        rlts.Add(log);
                    }
                }
                else if (log.StartsWith("#"))
                {
                    if (log.EndsWith(crashlibunityEndFlag))
                    {
                        int endIndex = log.IndexOf("libunity.so");
                        string addStr = log.Substring(startIndex, endIndex - startIndex - 1);
                        string tempUnitySoPath = string.Format("\"{0}\"", unitydebugsoPath);
                        ExecuteCmd(tempUnitySoPath, addStr);
                    }
                    else if (log.EndsWith(crashlibil2cppEndFlag))
                    {
                        int endIndex = log.IndexOf("libil2cpp.so");
                        string addStr = log.Substring(startIndex, endIndex - startIndex - 1);
                        string tempill2cppSoPath = string.Format("\"{0}\"", il2cppdebugsoPath);
                        ExecuteCmd(tempill2cppSoPath, addStr);
                    }
                    else
                    {
                        rlts.Add(log);
                    }
                }
                else
                {
                    rlts.Add(log);
                }
            }
            else
            {
                rlts.Add(log);
            }
        }
        else//�� il2cpp��libunity ������־
        {
            if (log.Contains(il2cppflag) && JudgePath(PathType.il2cppSoPath, il2cppdebugsoPath))
            {
                string tempill2cppSoPath = string.Format("\"{0}\"", il2cppdebugsoPath);
                FindMiddleCrash(log, il2cppflag, tempill2cppSoPath);
            }
            else if (log.Contains(unityflag))
            {
                string tempUnitySoPath = string.Format("\"{0}\"", unitydebugsoPath);
                FindMiddleCrash(log, unityflag, tempUnitySoPath);
            }
            else
            {
                rlts.Add(log);
            }
        }
    }

    /// <summary>
    /// �� il2cpp��libunity ������־
    /// </summary>
    /// <param name="log"></param>
    /// <param name="debugFlag">��־Ԫ��</param>
    /// <param name="SoPath">���ű�·��</param>
    void FindMiddleCrash(string log, string debugFlag, string SoPath)
    {
        if (!string.IsNullOrEmpty(SoPath))
        {
            int startIndex = log.IndexOf(debugFlag);
            startIndex = startIndex + debugFlag.Length + 1;
            if (log.Contains("("))
            {
                int endIndex = log.IndexOf("(");
                if (endIndex > 0)
                {
                    string addStr = log.Substring(startIndex, endIndex - startIndex);
                    ExecuteCmd(SoPath, addStr);
                }
            }
        }
        else
        {
            Debug.LogErrorFormat("{0}�ķ��ű�·��Ϊ��", debugFlag);
        }

    }


    /// <summary>
    /// ִ��CMD����
    /// </summary>
    /// <param name="SoPath">���ű�·��</param>
    /// <param name="addStr">���������ַ</param>
    void ExecuteCmd(string soPath, string addStr)
    {
        string tempaddr2linePath = string.Format("\"{0}\"", addr2linePath);

        string cmdStr = string.Join(" ", tempaddr2linePath, "-f", "-C", "-e", soPath, addStr);

        CmdHandler.RunCmd(cmdStr, (str) =>
        {
            string log = string.Format("������{0}", ResultStr(str, addStr));
            //Debug.Log(log);
            rlts.Add(log);
            isAnalysis = true;
        });

    }
    /// <summary>
    /// �Խ���������з���
    /// </summary>
    /// <param name="str"></param>
    /// <param name="addStr"></param>
    /// <returns></returns>
    string ResultStr(string str, string addStr)
    {
        string tempStr = string.Empty;
        if (!string.IsNullOrEmpty(str))
        {
            if (str.Contains("exit"))
            {
                int startIndex = str.IndexOf("exit");
                if (startIndex < str.Length)
                {
                    tempStr = str.Substring(startIndex);
                    if (tempStr.Contains(")"))
                    {
                        startIndex = tempStr.IndexOf("t") + 1;
                        int endIndex = tempStr.LastIndexOf(")");
                        tempStr = tempStr.Substring(startIndex, endIndex - startIndex + 1);
                        tempStr = string.Format("<color=red>[{0}]</color> :<color=yellow>{1}</color>", addStr, tempStr);
                    }
                    else
                    {
                        startIndex = tempStr.IndexOf("t") + 1;
                        tempStr = tempStr.Substring(startIndex);
                        tempStr = string.Format("<color=red>[{0}]</color> :<color=yellow>{1}</color>", addStr, tempStr);
                    }

                }
            }
            else
            {
                Debug.LogErrorFormat("��ǰ���δִ��cmd����", str);
            }
        }
        else
        {
            Debug.LogErrorFormat("ִ��cmd:{0}�������ֵΪ��", str);
        }
        return tempStr;
    }

    //private void OnDestroy()
    //{
    //    EditorPrefs.SetString("addr2linePath", addr2linePath);
    //    EditorPrefs.SetString("il2cppdebugsoPath", il2cppdebugsoPath);
    //    EditorPrefs.SetString("unitydebugsoPath", unitydebugsoPath);
    //    EditorPrefs.SetString("MyCashPath", MyCashPath);
    //}


}