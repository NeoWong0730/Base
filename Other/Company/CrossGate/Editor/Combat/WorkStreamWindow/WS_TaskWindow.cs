using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Table;

public class WS_TaskGoalWindow : WorkStreamWindow
{
    private static List<uint> s_TaskGoalIdsTxtList = new List<uint>();
    private static readonly string _TaskGoalIdsTxtFile = "Assets/../../Designer_Editor/WorkStreamData/TaskGoalIds.txt";

    public static WS_TaskGoalWindow _window;

    [MenuItem("Tools/工作流WorkStream/Open/TaskGoal")]
    public static void OpenWS_TaskGoalWindow()
    {
        ParseTaskGoalFiles();

        _window = GetWindow<WS_TaskGoalWindow>(false, "WS_TaskGoalWindow", true);
        ShowWorkStream();
        PlayerPrefs.SetInt("OpenWS_TaskGoalWindow", 1);
    }

    public static void ShowWorkStream()
    {
        if (!Application.isPlaying)
        {
            if (CSVTaskGoal.Instance == null)
                CSVTaskGoal.Load();
        }

        _window.Clear();
        _window.Show();
        _window.WorkStreamTypeKey = "WS_TaskGoalWindowType";
        _window.SavePath = "Assets/../../Designer_Editor/WorkStreamData/TaskGoal{0}/";
        _window.SaveAllPath = "Assets/Config/WorkStreamData/TaskGoal{0}/TaskGoal.txt";
        _window.SetHelpToolData("Assets/Scripts/Logic/WorkStream/TaskGoal/TaskGoalBlock/", 
            "Assets/Scripts/Logic/WorkStream/TaskGoal/TaskGoalNode/", "WS_TaskGoal_{0}_SComponent", (int)StateCategoryEnum.TaskGoal, typeof(TaskGoalEnum), 5001);

        _window.SetWorkStreamEnum<TaskGoalEnum>(1101);

        _window.CopyIdsTxtFile = "Assets/../../Designer_Editor/WorkStreamData/CopyTaskGoalIds.txt";

        _window.RefreshWorkData();
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    public static void Refresh()
    {
        bool isOpen = false;
        if (PlayerPrefs.HasKey("OpenWS_TaskGoalWindow"))
            isOpen = PlayerPrefs.GetInt("OpenWS_TaskGoalWindow") == 1 ? true : false;

        if (!isOpen)
            return;

        if (_window == null)
        {
            Debug.Log("已打开Window，再打开");
            OpenWS_TaskGoalWindow();
        }
        else
        {
            Debug.Log("已打开Window，重新刷新");
            ShowWorkStream();
        }
    }

    public void OnDestroy()
    {
        PlayerPrefs.SetInt("OpenWS_TaskGoalWindow", 0);
    }

    public override void RefreshWorkData()
    {
        base.RefreshWorkData();

        m_WorkMenuList.Clear();

        foreach (var kv in CSVTaskGoal.Instance.GetAll())
        {
            m_WorkMenuList.Add(new WorkStreamMenuInfoEditor
            {
                WorkId = kv.id,
            });
        }

        foreach (var taskGoalId in s_TaskGoalIdsTxtList)
        {
            if (IsContainWorkMenu(taskGoalId))
                continue;

            m_WorkMenuList.Add(new WorkStreamMenuInfoEditor
            {
                WorkId = taskGoalId,
            });
        }
    }

    private static void ParseTaskGoalFiles()
    {
        s_TaskGoalIdsTxtList.Clear();
        EditorToolHelp.ParseTxtInLine(_TaskGoalIdsTxtFile, (string line) =>
        {
            if (!string.IsNullOrEmpty(line))
            {
                string[] strs = line.Split('|');
                if (strs != null && strs.Length > 0)
                {
                    foreach (var behaveIdStr in strs)
                    {
                        if (string.IsNullOrEmpty(behaveIdStr))
                            continue;

                        uint behaveId = uint.Parse(behaveIdStr);
                        if (behaveId == 0u)
                            continue;

                        if (s_TaskGoalIdsTxtList.Contains(behaveId))
                        {
                            Debug.LogError($"文件{_TaskGoalIdsTxtFile}中有重复Id:{behaveId.ToString()}");
                            continue;
                        }

                        s_TaskGoalIdsTxtList.Add(behaveId);
                    }
                }
            }
        });
    }

    public override void RefreshWorkStreamEnum()
    {
        base.RefreshWorkStreamEnum();

        SetWorkStreamEnum<TaskGoalEnum>(1101);
    }

    protected override void DrawRightSecondColumn()
    {
        if (GUILayout.Button("批量转换数据", _buttonSt, GUILayout.Width(150)))
        {
            string saveAsFilePath = EditorUtility.SaveFolderPanel("批量生成文件的文件夹位置)", "A_HBS/Design_Editor/WorkStreamData/TaskGoal", string.Empty);
            if (!string.IsNullOrEmpty(saveAsFilePath))
            {
                string path = Path.GetDirectoryName(saveAsFilePath) + "\\TaskGoal";
                path = path.Replace("\\", "/");
                path += "/";
                CopyCombatWorkStreamDatas(path);
                RefreshWorkData();
                return;
            }
        }
    }
}
