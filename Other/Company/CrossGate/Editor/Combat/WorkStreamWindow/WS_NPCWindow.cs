using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Table;

public class WS_NPCWindow : WorkStreamWindow
{
    private static List<uint> s_NPCIdsTxtList = new List<uint>();
    private static readonly string _NPCIdsTxtFile = "Assets/../../Designer_Editor/WorkStreamData/NPCIds.txt";

    public static WS_NPCWindow _window;

	[MenuItem("Tools/工作流WorkStream/Open/NPC")]
	public static void OpenWS_NPCWindow()
	{
        ParseNPCFiles();

        _window = GetWindow<WS_NPCWindow>(false, "WS_NPCWindow", true);
		ShowWorkStream();
		PlayerPrefs.SetInt("OpenWS_NPCWindow", 1);
	}

	public static void ShowWorkStream()
	{
        if (!Application.isPlaying)
        {
            if (CSVNpc.Instance == null)
                CSVNpc.Load();

            if (CSVInteractivePerform.Instance == null)
                CSVInteractivePerform.Load();
        }

		_window.Clear();
		_window.Show();
		_window.WorkStreamTypeKey = "WS_NPCWindowType";
		_window.SavePath = "Assets/../../Designer_Editor/WorkStreamData/NPC{0}/";
		_window.SaveAllPath = "Assets/Config/WorkStreamData/NPC{0}/NPC.txt";
		_window.SetHelpToolData("Assets/Scripts/Logic/WorkStream/NPC/NPCBlock/", 
            "Assets/Scripts/Logic/WorkStream/NPC/NPCNode/", "WS_NPC_{0}_SComponent", (int)StateCategoryEnum.NPC, typeof(NPCEnum), 5001);

		_window.SetWorkStreamEnum<NPCEnum>(1101);

        _window.CopyIdsTxtFile = "Assets/../../Designer_Editor/WorkStreamData/CopyNpcIds.txt";

        _window.RefreshWorkData();
	}

	[UnityEditor.Callbacks.DidReloadScripts]
	public static void Refresh()
	{
		bool isOpen = false;
		if (PlayerPrefs.HasKey("OpenWS_NPCWindow"))
			isOpen = PlayerPrefs.GetInt("OpenWS_NPCWindow") == 1 ? true : false;

		if (!isOpen)
			return;

		if (_window == null)
		{
			Debug.Log("已打开Window，再打开");
			OpenWS_NPCWindow();
		}
		else
		{
			Debug.Log("已打开Window，重新刷新");
			ShowWorkStream();
		}
	}

	public void OnDestroy()
	{
		PlayerPrefs.SetInt("OpenWS_NPCWindow", 0);
	}

	public override void RefreshWorkData()
	{
		base.RefreshWorkData();

        m_WorkMenuList.Clear();

        foreach (var kv in CSVNpc.Instance.GetAll())
        {
            m_WorkMenuList.Add(new WorkStreamMenuInfoEditor
            {
                WorkId = kv.id,
            });
        }

        foreach (var kv in CSVInteractivePerform.Instance.GetAll())
        {
            m_WorkMenuList.Add(new WorkStreamMenuInfoEditor
            {
                WorkId = kv.id,
            });
        }

        foreach (var npcId in s_NPCIdsTxtList)
        {
            if (IsContainWorkMenu(npcId))
                continue;

            m_WorkMenuList.Add(new WorkStreamMenuInfoEditor
            {
                WorkId = npcId,
            });
        }
    }

    private static void ParseNPCFiles()
    {
        s_NPCIdsTxtList.Clear();
        EditorToolHelp.ParseTxtInLine(_NPCIdsTxtFile, (string line) =>
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

                        if (s_NPCIdsTxtList.Contains(behaveId))
                        {
                            Debug.LogError($"文件{_NPCIdsTxtFile}中有重复Id:{behaveId.ToString()}");
                            continue;
                        }

                        s_NPCIdsTxtList.Add(behaveId);
                    }
                }
            }
        });
    }

    public override void RefreshWorkStreamEnum()
	{
		base.RefreshWorkStreamEnum();

		SetWorkStreamEnum<NPCEnum>(1101);
	}

    protected override void DrawRightSecondColumn()
    {
        if (GUILayout.Button("批量转换数据", _buttonSt, GUILayout.Width(150)))
        {
            string saveAsFilePath = EditorUtility.SaveFolderPanel("批量生成文件的文件夹位置)", "A_HBS/Design_Editor/WorkStreamData/NPC", string.Empty);
            if (!string.IsNullOrEmpty(saveAsFilePath))
            {
                string path = Path.GetDirectoryName(saveAsFilePath) + "\\NPC";
                path = path.Replace("\\", "/");
                path += "/";
                CopyCombatWorkStreamDatas(path);
                RefreshWorkData();
                return;
            }
        }
    }
}