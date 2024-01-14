using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WS_CommunalAIWindow : WorkStreamWindow
{
	public static WS_CommunalAIWindow _window;

    private static Dictionary<uint, string> s_communalAIIdsTxtDic = new Dictionary<uint, string>();
    private static readonly string _communalAIIdsTxtFile = "Assets/../../Designer_Editor/WorkStreamData/CommunalAIIds.txt";

    [MenuItem("Tools/工作流WorkStream/Open/CommunalAI")]
	public static void OpenWS_CommunalAIWindow()
	{
        ParseFiles();
        
        _window = GetWindow<WS_CommunalAIWindow>(false, "WS_CommunalAIWindow", true);
		ShowWorkStream();
		PlayerPrefs.SetInt("OpenWS_CommunalAIWindow", 1);
	}

	public static void ShowWorkStream()
	{
		_window.Clear();
		_window.Show();
		_window.WorkStreamTypeKey = "WS_CommunalAIWindowType";
		_window.SavePath = "Assets/../../Designer_Editor/WorkStreamData/CommunalAI{0}/";
		_window.SaveAllPath = "Assets/Config/WorkStreamData/CommunalAI{0}/CommunalAI.txt";
		_window.SetHelpToolData("Assets/Scripts/Logic/WorkStream/CommunalAI/CommunalAIBlock/", "Assets/Scripts/Logic/WorkStream/CommunalAI/CommunalAINode/", "WS_CommunalAI_{0}_SComponent", (int)StateCategoryEnum.CommunalAI, typeof(CommunalAIEnum), 5001);

		_window.SetWorkStreamEnum<CommunalAIEnum>(5001);

		_window.RefreshWorkData();
	}

	[UnityEditor.Callbacks.DidReloadScripts]
	public static void Refresh()
	{
		bool isOpen = false;
		if (PlayerPrefs.HasKey("OpenWS_CommunalAIWindow"))
			isOpen = PlayerPrefs.GetInt("OpenWS_CommunalAIWindow") == 1 ? true : false;

		if (!isOpen)
			return;

		if (_window == null)
		{
			Debug.Log("已打开Window，再打开");
			OpenWS_CommunalAIWindow();
		}
		else
		{
			Debug.Log("已打开Window，重新刷新");
			ShowWorkStream();
		}
	}

	public void OnDestroy()
	{
		PlayerPrefs.SetInt("OpenWS_CommunalAIWindow", 0);
	}

	public override void RefreshWorkData()
	{
		base.RefreshWorkData();

        m_WorkMenuList.Clear();

        foreach (var kv in s_communalAIIdsTxtDic)
        {
            if (IsContainWorkMenu(kv.Key))
                continue;

            m_WorkMenuList.Add(new WorkStreamMenuInfoEditor
            {
                WorkId = kv.Key,
                WorkStreamDes = kv.Value
            });
        }
    }

	public override void RefreshWorkStreamEnum()
	{
		base.RefreshWorkStreamEnum();

		SetWorkStreamEnum<CommunalAIEnum>(5001);
	}

    private static void ParseFiles()
    {
        s_communalAIIdsTxtDic.Clear();
        EditorToolHelp.ParseTxtInLine(_communalAIIdsTxtFile, (string line) =>
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

                        string[] strs02 = behaveIdStr.Split('&');

                        uint behaveId = uint.Parse(strs02[0]);
                        if (behaveId == 0u)
                            continue;

                        if (s_communalAIIdsTxtDic.ContainsKey(behaveId))
                        {
                            Debug.LogError($"文件{_communalAIIdsTxtFile}中有重复Id:{behaveId.ToString()}");
                            continue;
                        }

                        s_communalAIIdsTxtDic[behaveId] = strs02.Length > 1 ? strs02[1] : null;
                    }
                }
            }
        });
    }
}