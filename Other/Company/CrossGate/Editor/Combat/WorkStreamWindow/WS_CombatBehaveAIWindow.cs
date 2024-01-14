using System.Collections.Generic;
using System.IO;
using System.Linq;
using Table;
using UnityEditor;
using UnityEngine;

public class WS_CombatBehaveAIWindow : WorkStreamWindow
{
    private static List<uint> s_behaveIdsTxtList = new List<uint>();
    private static readonly string _combatBehaveIdsTxtFile = "Assets/Designer_Editor/CombatData/CombatBehaveAIIds.txt";

    public static WS_CombatBehaveAIWindow _window;

    private bool _isNotFirstPlaying;
    private static bool _isLoad;

    private bool _timeScaleToggle;

    [MenuItem("Tools/工作流WorkStream/Open/CombatBehaveAI")]
	public static void OpenWS_CombatBehaveAIWindow()
	{
        ParseCombatBehaveFiles();

        _window = GetWindow<WS_CombatBehaveAIWindow>(false, "WS_CombatBehaveAIWindow", true);
		ShowWorkStream();
		PlayerPrefs.SetInt("OpenWS_CombatBehaveAIWindow", 1);
	}

	public static void ShowWorkStream()
	{
        if (!Application.isPlaying && CSVActiveSkill.Instance == null)
        {
            CSVActiveSkill.Load();
            CSVActiveSkillBehavior.Load();
            CSVParam.Load();

            _isLoad = true;
        }

        _window.Clear();
		_window.Show();
		_window.WorkStreamTypeKey = "WS_CombatBehaveAIWindowType";
		_window.SavePath = "Assets/../../Designer_Editor/WorkStreamData/CombatBehaveAI{0}/";
		_window.SaveAllPath = "Assets/Config/WorkStreamData/CombatBehaveAI{0}/CombatBehaveAI.txt";
		_window.SetHelpToolData("Assets/Scripts/Logic/WorkStream/CombatBehaveAI/CombatBehaveAIBlock/", 
            "Assets/Scripts/Logic/WorkStream/CombatBehaveAI/CombatBehaveAINode/", "WS_CombatBehaveAI_{0}_SComponent", (int)StateCategoryEnum.CombatBehaveAI, typeof(CombatBehaveAIEnum), 1001);

		_window.SetWorkStreamEnum<CombatBehaveAIEnum>(1001);

        _window.CopyIdsTxtFile = "Assets/Designer_Editor/CombatData/CopyCombatBehaveAIIds.txt";

        _window.m_CollectNodeType = 26;

        _window.RefreshWorkData();

        CombatManager.Instance.m_CombatAI_7_Increase = uint.Parse(CSVParam.Instance.GetConfData(601).str_value);
        CombatManager.Instance.m_CombatAI_8_Increase = uint.Parse(CSVParam.Instance.GetConfData(602).str_value);
        CombatManager.Instance.m_CombatAI_9_Increase = uint.Parse(CSVParam.Instance.GetConfData(603).str_value);
        CombatManager.Instance.m_CombatAI_10_Increase = uint.Parse(CSVParam.Instance.GetConfData(604).str_value);
    }

	[UnityEditor.Callbacks.DidReloadScripts]
	public static void Refresh()
	{
		bool isOpen = false;
		if (PlayerPrefs.HasKey("OpenWS_CombatBehaveAIWindow"))
			isOpen = PlayerPrefs.GetInt("OpenWS_CombatBehaveAIWindow") == 1 ? true : false;

		if (!isOpen)
			return;

		if (_window == null)
		{
			Debug.Log("已打开Window，再打开");
			OpenWS_CombatBehaveAIWindow();
		}
		else
		{
			Debug.Log("已打开Window，重新刷新");
			ShowWorkStream();
		}
	}

    public override void OnUpdate()
    {
        if (!_isNotFirstPlaying && Application.isPlaying && _isLoad)
        {
            CSVActiveSkill.Unload();
            CSVActiveSkillBehavior.Unload();
            CSVParam.Unload();

            _isNotFirstPlaying = true;
            _isLoad = false;
        }

        if (!Application.isPlaying)
            _isNotFirstPlaying = false;

        if (!Application.isPlaying || (Application.isPlaying && CombatManager.Instance.m_IsAwake && !CombatManager.Instance.m_IsFight))
        {
            ObjectEvents.Instance.Update();
        }
    }

    public override void DoCustomGUI()
    {
        base.DoCustomGUI();
    }

    private void OnDestroy()
    {
        Clear();

        if (!Application.isPlaying && CSVActiveSkill.Instance != null)
        {
            CSVActiveSkill.Unload();
            CSVActiveSkillBehavior.Unload();
        }

        PlayerPrefs.SetInt("OpenWS_CombatBehaveAIWindow", 0);
    }

    public override void RefreshWorkData()
    {
        base.RefreshWorkData();
        
        if (PlayerPrefs.HasKey("Combat_m_EnableTimeScaleKeyCode"))
            _timeScaleToggle = PlayerPrefs.GetInt("Combat_m_EnableTimeScaleKeyCode") == 1;
        else
            _timeScaleToggle = false;

        m_WorkMenuList.Clear();

        foreach (var kv in CSVActiveSkill.Instance.GetAll())
        {
            if (kv.new_behavior != 0u || kv.behavior_tool == 0u)
                continue;

            if (IsContainWorkMenu(kv.behavior_tool))
                continue;

            m_WorkMenuList.Add(new WorkStreamMenuInfoEditor
            {
                WorkId = kv.id,
            });
        }

        foreach (var kv in CSVActiveSkillBehavior.Instance.GetAll())
        {
            if (kv.behavior_tool == null)
                continue;

            foreach (var bt in kv.behavior_tool)
            {
                if (bt == 0u)
                    continue;

                if (IsContainWorkMenu(bt))
                    continue;

                m_WorkMenuList.Add(new WorkStreamMenuInfoEditor
                {
                    WorkId = bt,
                });
            }
        }

        foreach (var behaveId in s_behaveIdsTxtList)
        {
            if (IsContainWorkMenu(behaveId))
                continue;

            m_WorkMenuList.Add(new WorkStreamMenuInfoEditor
            {
                WorkId = behaveId,
            });
        }
    }

    private static void ParseCombatBehaveFiles()
    {
        s_behaveIdsTxtList.Clear();
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

                        uint behaveId = uint.Parse(behaveIdStr);
                        if (behaveId == 0u)
                            continue;

                        if (s_behaveIdsTxtList.Contains(behaveId))
                        {
                            Debug.LogError($"文件{_combatBehaveIdsTxtFile}中有重复Id:{behaveId.ToString()}");
                            continue;
                        }

                        s_behaveIdsTxtList.Add(behaveId);
                    }
                }
            }
        });
    }

    public override void RefreshWorkStreamEnum()
    {
        base.RefreshWorkStreamEnum();

        SetWorkStreamEnum<CombatBehaveAIEnum>(1001);
    }

    public override void DoWorkStream()
    {
        base.DoWorkStream();

        //StateCategoryManager.Instance.OnAwake(false);

        //CombatConfigManager.Instance.ResetConfigData(5);

        //WS_CombatBehaveAIManagerEntity me = WS_CombatBehaveAIManagerEntity.CreateManager();
        //WS_CombatBehaveAIControllerEntity t = me.CreateController<WS_CombatBehaveAIControllerEntity>(m_SelectWorkId, 0, SwitchWorkStreamEnum.Stop_AllWorkStream, (StateControllerEntity stateControllerEntity) =>
        //{
        //    m_UseWorkStreamTranstionComponent = stateControllerEntity.m_FirstMachine.m_StateTranstionComponent as WorkStreamTranstionComponent;
        //});
        //if (t != null)
        //{
        //    WorkStreamTranstionComponent workStreamTranstionComponent = t.m_FirstMachine.m_StateTranstionComponent as WorkStreamTranstionComponent;
        //    if (!workStreamTranstionComponent.StartWorkStream())
        //        me.Dispose();
        //}
        //else
        //    me.Dispose();
    }

    //第二栏自定义按钮（用来添加自定义需求按钮,需要基类参数可以把private改成protected）
    protected override void DrawRightSecondColumn()
    {
        int oldWorkStreamType = _workStreamType;
        int workStreamType = EditorGUILayout.IntField(_workStreamType, _textSt, GUILayout.Width(40f));
        if (workStreamType != _workStreamType)
        {
            _workStreamType = workStreamType;
            if (Directory.Exists(SavePath))
            {
                PlayerPrefs.SetInt(WorkStreamTypeKey, workStreamType);
                Clear();
                RefreshWorkData();
            }
            else
            {
                _workStreamType = oldWorkStreamType;
                EditorUtility.DisplayDialog("提示", $"不存在文件路径:{SavePath}", "OK");
            }
            return;
        }

        if (GUILayout.Button("批量转换数据", _buttonSt, GUILayout.Width(150)))
        {
            string saveAsFilePath = EditorUtility.SaveFolderPanel("批量生成文件的文件夹位置)", "A_HBS/Design_Editor/", string.Empty);
            if (!string.IsNullOrEmpty(saveAsFilePath))
            {
                CopyCombatWorkStreamDatas($"{Path.GetDirectoryName(saveAsFilePath).Replace("\\", "/")}/");
                RefreshWorkData();
                return;
            }
        }

        GUILayout.Space(5.5f);

        ShowDynamicGUI();
        
        var tst = GUILayout.Toggle(_timeScaleToggle, "使用战斗缩放快捷键");
        if (tst != _timeScaleToggle)
        {
            PlayerPrefs.SetInt("Combat_m_EnableTimeScaleKeyCode", tst ? 1 : 0);

            _timeScaleToggle = tst;

            if (Application.isPlaying && CombatManager.Instance != null)
            {
                CombatManager.Instance.m_EnableTimeScaleKeyCode = _timeScaleToggle;
            }
        }

        GUILayout.Space(5.5f);

        if (GUILayout.Button("转换数据", _buttonSt, GUILayout.Width(120)))
        {
            //SwitchData();
        }

        GUILayout.Space(5.5f);
        _isShowCollectContent = GUILayout.Toggle(_isShowCollectContent, "显示收集内容", GUILayout.Width(100f));
        if (_isShowCollectContent)
        {
            if (_collectIdList == null)
            {
                _collectIdList = new List<uint>();
                for (int i = 0; i < m_WorkBlockDataList.Count; i++)
                {
                    WorkBlockData workBlockData = m_WorkBlockDataList[i];
                    GetCollectNode(workBlockData, _collectIdList);
                }
            }
        }
        else
        {
            _collectIdList = null;
            _collectWorkBlockDic = null;
        }
    }

    protected override void SaveAll()
    {
        Dictionary<uint, Dictionary<uint, string>> dataTeamDic = new Dictionary<uint, Dictionary<uint, string>>();

        if (_dataPathDic.Count > 0)
        {
            string dir = Path.GetDirectoryName(SaveAllPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string fileName = Path.GetFileNameWithoutExtension(SaveAllPath);

            foreach (var kv in _dataPathDic)
            {
                uint teamKey = CombatHelp.CustomCombatWorkId(kv.Key, CombatManager.Instance.m_CombatAI_7_Increase, CombatManager.Instance.m_CombatAI_8_Increase,
                     CombatManager.Instance.m_CombatAI_9_Increase, CombatManager.Instance.m_CombatAI_10_Increase);

                if (!dataTeamDic.TryGetValue(teamKey, out Dictionary<uint, string> dic) || dic == null)
                {
                    dic = new Dictionary<uint, string>();
                    dataTeamDic[teamKey] = dic;
                }

                dic[kv.Key] = kv.Value;
            }

            foreach (var teamKv in dataTeamDic)
            {
                SaveConfigDataToFile(teamKv.Value, $"{dir}/{fileName}_{teamKv.Key}.txt");
            }
        }

        AssetDatabase.Refresh();
    }
}