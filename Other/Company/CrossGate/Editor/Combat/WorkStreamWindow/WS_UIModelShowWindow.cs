using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WS_UIModelShowWindow : WorkStreamWindow
{
    private static Dictionary<uint, string> s_behaveIdsTxtDic = new Dictionary<uint, string>();
    private static readonly string _combatBehaveIdsTxtFile = "Assets/../../Designer_Editor/WorkStreamData/UIModelShowIds.txt";

    public static WS_UIModelShowWindow _window;

	[MenuItem("Tools/工作流WorkStream/Open/UIModelShow")]
	public static void OpenWS_UIModelShowWindow()
	{
        ParseFiles();

        _window = GetWindow<WS_UIModelShowWindow>(false, "WS_UIModelShowWindow", true);
		ShowWorkStream();
		PlayerPrefs.SetInt("OpenWS_UIModelShowWindow", 1);
	}

	public static void ShowWorkStream()
	{
		_window.Clear();
		_window.Show();
		_window.WorkStreamTypeKey = "WS_UIModelShowWindowType";
		_window.SavePath = "Assets/../../Designer_Editor/WorkStreamData/UIModelShow{0}/";
		_window.SaveAllPath = "Assets/Config/WorkStreamData/UIModelShow{0}/UIModelShow.txt";
		_window.SetHelpToolData("Assets/Scripts/Logic/WorkStream/UIModelShow/UIModelShowBlock/", 
            "Assets/Scripts/Logic/WorkStream/UIModelShow/UIModelShowNode/", "WS_UIModelShow_{0}_SComponent", (int)StateCategoryEnum.UIModelShow, typeof(UIModelShowEnum), 5001);

		_window.SetWorkStreamEnum<UIModelShowEnum>(5001);

		_window.RefreshWorkData();
	}

	[UnityEditor.Callbacks.DidReloadScripts]
	public static void Refresh()
	{
		bool isOpen = false;
		if (PlayerPrefs.HasKey("OpenWS_UIModelShowWindow"))
			isOpen = PlayerPrefs.GetInt("OpenWS_UIModelShowWindow") == 1 ? true : false;

		if (!isOpen)
			return;

		if (_window == null)
		{
			Debug.Log("已打开Window，再打开");
			OpenWS_UIModelShowWindow();
		}
		else
		{
			Debug.Log("已打开Window，重新刷新");
			ShowWorkStream();
		}
	}

	public void OnDestroy()
	{
		PlayerPrefs.SetInt("OpenWS_UIModelShowWindow", 0);
	}

	public override void RefreshWorkData()
	{
		base.RefreshWorkData();

        m_WorkMenuList.Clear();
        
        foreach (var kv in s_behaveIdsTxtDic)
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

		SetWorkStreamEnum<UIModelShowEnum>(5001);
	}

    private static void ParseFiles()
    {
        s_behaveIdsTxtDic.Clear();
        EditorToolHelp.ParseTxtInLine(_combatBehaveIdsTxtFile, (string line) =>
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

                        if (s_behaveIdsTxtDic.ContainsKey(behaveId))
                        {
                            Debug.LogError($"文件{_combatBehaveIdsTxtFile}中有重复Id:{behaveId.ToString()}");
                            continue;
                        }

                        s_behaveIdsTxtDic[behaveId] = strs02.Length > 1 ? strs02[1] : null;
                    }
                }
            }
        });
    }

    //第二栏自定义按钮（用来添加自定义需求按钮,需要基类参数可以把private改成protected）
    protected override void DrawRightSecondColumn()
    {
        ShowDynamicGUI();
    }
}