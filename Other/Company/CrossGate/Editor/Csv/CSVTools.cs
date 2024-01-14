using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using Table;
using UnityEditor;
using UnityEngine;

public class CSVTools
{
    static string sExcelDir = "../../../data";
    //static string sCsvDir = "../../../data/client";

    //public readonly static string sCsvByteOutDir = "Config/Table";
    //public readonly static string sCodeOutDir = "Scripts/Logic/Table";
    //static string sCodeTemplatePath = "CsvCodeTemplate.txt";

    //static string fm = "\t\tentry.{0} = ReadHelper.{1}(binaryReader);";
    //static string fm_Array = "\t\tentry.{0} = ReadHelper.ReadArray<{1}>(binaryReader, ReadHelper.{2});";
    //static string fm_List = "\t\tentry.{0} = ReadHelper.ReadList<{1}>(binaryReader, ReadHelper.{2});";
    //static string fm_Dic = "\t\tentry.{0} = ReadHelper.ReadDic<{1},{2}>(binaryReader, ReadHelper.{3}, ReadHelper.{4});";

    private static readonly string csvTemplateDir = "/Scripts/Logic/Table";
    private static readonly string csvManagerPath = "/Scripts/Logic/Config/CSVRegister.cs";

    [MenuItem("_CSV_/打开CSV工具")]
    static void OpenCsvTool()
    {
        try
        {
            string excelFullDir = Path.Combine(Application.dataPath, sExcelDir);
            DirectoryInfo directoryInfo = Directory.CreateDirectory(excelFullDir);
            excelFullDir = directoryInfo.FullName;

            System.Diagnostics.Process pro = new System.Diagnostics.Process();
            pro.StartInfo.WorkingDirectory = excelFullDir;
            pro.StartInfo.FileName = "ExcelExport.bat";
            //pro.EnableRaisingEvents = true;
            pro.Start();
            pro.WaitForExit();
            Debug.Log("svn更新完毕");
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
            Debug.LogError("相对路径不对");
        }
    }

    [MenuItem("_CSV_/生成CSVRegister")]
    static void AutoGenCSVManager()
    {
        EditorUtility.DisplayDialog("通知", "外部工具生成表格代码的时候已经自动生成CSVRegister.cs", "确定");
        /*
        if (!EditorUtility.DisplayDialog("提示 生成CSVRegister.cs", string.Format("src : {0}\ndes : {1}", csvTemplateDir, csvManagerPath), "生成", "取消"))
        {
            return;
        }                
        StringBuilder sbLoad = new StringBuilder();
        StringBuilder sbUnload = new StringBuilder();

        int count = 0;

        DirectoryInfo directoryInfo = Directory.CreateDirectory(Application.dataPath + csvTemplateDir);
        FileInfo[] fileInfos = directoryInfo.GetFiles();
        foreach (FileInfo fileInfo in fileInfos)
        {
            if (string.Equals(fileInfo.Extension, ".cs"))
            {
                string className = Path.GetFileNameWithoutExtension(fileInfo.Name);
                //sbLoad.AppendFormat("\t\t\t{0}.Load();\n", className);
                sbLoad.AppendFormat("\t\t\t_loaders.Add({0}.Load);\n", className);
                sbUnload.AppendFormat("\t\t\t{0}.Unload();\n", className);
                ++count;
            }
        }

        StringBuilder sbAll = new StringBuilder();
        //sbAll.AppendLine("//Auto Generate Code");
        //sbAll.AppendLine("using Lib.Core;");
        //sbAll.AppendLine("using Table;");
        //sbAll.AppendLine("namespace Logic");
        //sbAll.AppendLine("{");
        //sbAll.AppendLine("\tpublic static class CSVRegister");
        //sbAll.AppendLine("\t{");
        //
        //sbAll.AppendLine("\t\tdelegate int CSVLoadFunction();");
        //sbAll.AppendLine("\t\tprivate static List<CSVLoadFunction> _loaders;");

        sbAll.Append(@"//Auto Generate Code
using System.Collections.Generic;
using Table;
namespace Logic
{
	public static class CSVRegister
	{
		delegate int CSVLoadFunction();
        public static bool isFinished { get { return _csvCount == _csvLoadedCount; } }
		private static List<CSVLoadFunction> _loaders;
		private static int _csvCount = 0;
		private static int _csvLoadedCount = 0;
		private static float _loadTime = 0;
");

        sbAll.AppendLine("\t\tpublic static void Load()");
        sbAll.AppendLine("\t\t{");

        sbAll.AppendFormat("\t\t\t_loaders = new List<CSVLoadFunction>({0});\n", count);

        //sbAll.AppendLine("\t\t\tfloat start = UnityEngine.Time.realtimeSinceStartup;");
        sbAll.AppendLine();
        sbAll.Append(sbLoad);
        sbAll.AppendLine();
        //sbAll.AppendLine("\t\t\tDebugUtil.LogFormat(ELogType.eTable, \"加载CSV 总耗时 {0}\", (UnityEngine.Time.realtimeSinceStartup - start).ToString());");

        sbAll.AppendLine("\t\t\t_csvCount = _loaders.Count;");
        sbAll.AppendLine("\t\t\t_csvLoadedCount = 0;");
        sbAll.AppendLine("\t\t\t_loadTime = 0;");        

        sbAll.AppendLine("\t\t}");

        sbAll.AppendLine("\t\tpublic static void Unload()");
        sbAll.AppendLine("\t\t{");
        sbAll.Append(sbUnload);
        sbAll.AppendLine();
        sbAll.AppendLine("\t\t\t_csvLoadedCount = 0;");
        sbAll.AppendLine("\t\t\t_loadTime = 0;");
        sbAll.AppendLine("\t\t}");

        sbAll.Append(
@"		public static float UpdateLoad()
		{
			while (_csvLoadedCount < _csvCount)
			{
				float start = UnityEngine.Time.realtimeSinceStartup;
				_loaders[_csvLoadedCount].Invoke();
				_loadTime += (UnityEngine.Time.realtimeSinceStartup - start);

				++_csvLoadedCount;

				if (_loadTime >= 0.33f)
				{
					break;
				}
			}

            if (_csvLoadedCount >= _csvCount)
            {
                _loaders = null;
				return 1f;
            }

			return (float)_csvLoadedCount / _csvCount;
		}
");

        sbAll.AppendLine("\t}");
        sbAll.AppendLine("}");

        string s = sbAll.ToString();
        File.WriteAllText(Application.dataPath + csvManagerPath, s);

        sbAll.Clear();
        sbLoad.Clear();
        sbUnload.Clear();

        EditorUtility.DisplayDialog("提示", "生成完成", "确定");
           */
    }

    static Dictionary<int, string> taskFunctionStrs;
    static Dictionary<int, string> taskGoalFunctionStrs;
    static Dictionary<uint, int> npcID_Index;

    /// <summary>
    /// 处理NPC TASK TASKGOAL,填充NPC功能数据///
    /// taskFunction:Type&ID&Desc&Dialogue&Anim&HandlerID&HandlerIndex&FunctionSourceType&State&IconID
    /// taskGoalFunction:Type&ID&Desc&Dialogue&Anim&HandlerID&HandlerIndex&FunctionSourceType
    /// </summary>
    [MenuItem("_CSV_/填充NPC功能数据")]
    static void FillNpcFunctionDatas()
    {
        EditorUtility.DisplayProgressBar("填充NPC功能数据", "开始填充NPC功能数据", 0);

        npcID_Index = new Dictionary<uint, int>();
        taskFunctionStrs = new Dictionary<int, string>();
        taskGoalFunctionStrs = new Dictionary<int, string>();

        string sExcelPath = Application.dataPath + "/../../../data/excel";
        string taskExcelPath = Path.Combine(sExcelPath, "r任务表.xlsm");
        string npcFunctionDataExcelPath = Path.Combine(sExcelPath, "NPCFunctionData.xlsx");

        if (CSVTask.Instance == null)
            CSVTask.Load();

        if (CSVTaskGoal.Instance == null)
            CSVTaskGoal.Load();

        if (CSVNpc.Instance == null)
            CSVNpc.Load();

        var npcDatas = CSVNpc.Instance.GetAll();
        foreach (var npcData in npcDatas)
        {
            npcID_Index.Add(npcData.id, (int)npcData.id_index);
        }

        EditorUtility.DisplayProgressBar("填充NPC功能数据", "开始生成任务功能", 0.2f);
        var taskDatas = CSVTask.Instance.GetAll();
        foreach (var taskData in taskDatas)
        {
            string taskFunctionStr = string.Empty;
            if (taskData.AcceotType == 2 && taskData.receiveNpc != 0 && taskData.notAcceptedState != null && taskData.notAcceptedState.Count > 0)
            {
                if (taskData.notAcceptedState.Count > 3)
                    taskFunctionStr = $"3&{taskData.id}&{taskData.notAcceptedState[1]}&{taskData.notAcceptedState[2]}&{taskData.notAcceptedState[3]}&{taskData.id}&0&1&0&{taskData.notAcceptedState[0]}";
                else if (taskData.notAcceptedState.Count > 2)
                    taskFunctionStr = $"3&{taskData.id}&{taskData.notAcceptedState[1]}&{taskData.notAcceptedState[2]}&0&{taskData.id}&0&1&0&{taskData.notAcceptedState[0]}";
                else if (taskData.notAcceptedState.Count > 1)
                    taskFunctionStr = $"3&{taskData.id}&{taskData.notAcceptedState[1]}&0&0&{taskData.id}&0&1&0&{taskData.notAcceptedState[0]}";

                if (npcID_Index.ContainsKey(taskData.receiveNpc))
                {
                    if (taskFunctionStrs.ContainsKey(npcID_Index[taskData.receiveNpc]))
                    {
                        taskFunctionStrs[npcID_Index[taskData.receiveNpc]] += $"|{taskFunctionStr}";
                    }
                    else
                    {
                        taskFunctionStrs.Add(npcID_Index[taskData.receiveNpc], taskFunctionStr);
                    }
                }
                else
                {
                    Debug.LogError($"taskData.receiveNpc is null! npcid: {taskData.receiveNpc}  taskID: {taskData.id}");
                }

                string taskFunctionConditionIdStr = string.Empty;

            }

            //Type&ID&Desc&Dialogue&Anim&HandlerID&HandlerIndex&FunctionSourceType&State&IconID
            if (taskData.submitNpc != 0 && taskData.completeState != null && taskData.completeState.Count > 0)
            {
                if (taskData.completeState.Count > 3)
                    taskFunctionStr = $"1&{taskData.id}&{taskData.completeState[1]}&{taskData.completeState[2]}&{taskData.completeState[3]}&{taskData.id}&0&1&3&{taskData.completeState[0]}";
                else if (taskData.completeState.Count > 2)
                    taskFunctionStr = $"1&{taskData.id}&{taskData.completeState[1]}&{taskData.completeState[2]}&0&{taskData.id}&0&1&3&{taskData.completeState[0]}";
                else if (taskData.completeState.Count > 1)
                    taskFunctionStr = $"1&{taskData.id}&{taskData.completeState[1]}&0&0&{taskData.id}&0&1&3&{taskData.completeState[0]}";

                if (npcID_Index.ContainsKey(taskData.submitNpc))
                {
                    if (taskFunctionStrs.ContainsKey(npcID_Index[taskData.submitNpc]))
                    {
                        taskFunctionStrs[npcID_Index[taskData.submitNpc]] += $"|{taskFunctionStr}";
                    }
                    else
                    {
                        taskFunctionStrs.Add(npcID_Index[taskData.submitNpc], taskFunctionStr);
                    }
                }
                else
                {
                    Debug.LogError($"taskData.submitNpc is null! npcid: {taskData.submitNpc}  taskID: {taskData.id}");
                }
            }
        }

        var taskGoalDatas = CSVTaskGoal.Instance.GetAll();
        foreach (var taskGoalData in taskGoalDatas)
        {
            string taskGoalFunctionStr = string.Empty;
            if (taskGoalData.PathfindingTargetID != 0 && taskGoalData.GenerateFunctionType != 0 && taskGoalData.PathfindingType == 1)
            {
                taskGoalFunctionStr = $"{taskGoalData.GenerateFunctionType}&{taskGoalData.FunctionParameter}&{taskGoalData.TitleText}&{taskGoalData.FunctionFrontDialogue}&{taskGoalData.FunctionFrontAnimation}&{taskGoalData.TaskID}&{taskGoalData.id - taskGoalData.TaskID * 10}&1&{taskGoalData.FunctionOpenList}&0";

                if (npcID_Index.ContainsKey(taskGoalData.PathfindingTargetID))
                {
                    if (taskGoalFunctionStrs.ContainsKey(npcID_Index[taskGoalData.PathfindingTargetID]))
                    {
                        taskGoalFunctionStrs[npcID_Index[taskGoalData.PathfindingTargetID]] += $"|{taskGoalFunctionStr}";
                    }
                    else
                    {
                        taskGoalFunctionStrs.Add(npcID_Index[taskGoalData.PathfindingTargetID], taskGoalFunctionStr);
                    }
                }
                else
                {
                    Debug.LogError($"taskGoalData.PathfindingTargetID is null! npcid: {taskGoalData.PathfindingTargetID}  taskID: {taskGoalData.id}");
                }
            }

            if (taskGoalData.LimitOpenNpc != 0 && taskGoalData.LimitOpenChoose != 0)
            {
                taskGoalFunctionStr = $"{taskGoalData.LimitOpenType}&{taskGoalData.LimitOpenChoose}&{taskGoalData.LimitOpenText}&0&0&{taskGoalData.TaskID}&{taskGoalData.id - taskGoalData.TaskID * 10}&1&1&1";

                if (npcID_Index.ContainsKey(taskGoalData.LimitOpenNpc))
                {
                    if (taskGoalFunctionStrs.ContainsKey(npcID_Index[taskGoalData.LimitOpenNpc]))
                    {
                        taskGoalFunctionStrs[npcID_Index[taskGoalData.LimitOpenNpc]] += $"|{taskGoalFunctionStr}";
                    }
                    else
                    {
                        taskGoalFunctionStrs.Add(npcID_Index[taskGoalData.LimitOpenNpc], taskGoalFunctionStr);
                    }
                }
                else
                {
                    Debug.LogError($"taskGoalData.LimitOpenNpc is null! npcid: {taskGoalData.LimitOpenNpc}  taskID: {taskGoalData.id}");
                }
            }
        }

        using (ExcelPackage npcFunctionDataExcelPackage = new ExcelPackage(new FileInfo(npcFunctionDataExcelPath)))
        {
            ExcelWorksheet npcFunctionDataSheet = npcFunctionDataExcelPackage.Workbook.Worksheets[1];

            for (int row = 7, rows = npcFunctionDataSheet.Dimension.Rows; row <= rows; row++)
            {
                for (int col = 1, cols = npcFunctionDataSheet.Dimension.Columns; col <= cols; col++)
                {
                    npcFunctionDataSheet.Cells[row, col].Value = string.Empty;
                }
            }

            foreach (var pair in npcID_Index)
            {
                npcFunctionDataSheet.Cells[pair.Value, 1].Value = pair.Key;
            }

            EditorUtility.DisplayProgressBar("填充NPC功能数据", "写入NPCFunctionData表", 0.5f);
            foreach (var pair in taskFunctionStrs)
            {
                //Debug.Log($"taskFunctionStrs index: {pair.Key}  str: {pair.Value}");
                npcFunctionDataSheet.Cells[pair.Key, 2].Value = pair.Value;
            }

            foreach (var pair in taskGoalFunctionStrs)
            {
                //Debug.Log($"taskGoalFunctionStrs index: {pair.Key}  str: {pair.Value}");
                npcFunctionDataSheet.Cells[pair.Key, 3].Value = pair.Value;
            }

            EditorUtility.DisplayProgressBar("填充NPC功能数据", "保存NPCFunctionData表", 0.6f);
            npcFunctionDataExcelPackage.Save();
            EditorUtility.ClearProgressBar();
        }
    }
}
