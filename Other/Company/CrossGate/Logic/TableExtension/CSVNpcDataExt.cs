using Logic;
using System.Collections.Generic;
using Lib.Core;

namespace Table
{
    /// <summary>
    /// NPC表扩展///
    /// </summary>
    public static class CSVNpcDataExt
    {
        const string TaskFunctionFullName = "Logic.TaskFunction";

        /// <summary>
        /// 生成NPC上挂载的功能///
        /// </summary>
        /// <returns></returns>
        public static List<FunctionBase> CreateFunctionBases(this CSVNpc.Data data)
        {
            List<FunctionBase> _functions = new List<FunctionBase>();

            //NPC常驻功能
            if (data.function != null && data.function.Count > 0)
            {
                for (int i = 0, len = data.function.Count; i < len; i++)
                {
                    FunctionBase re;
                    if (data.functionCondition != null)
                    {
                        re = InitializeFunction(data.function[i], data.OpenShop, data.functionCondition[i]);
                    }
                    else
                    {
                        re = InitializeFunction(data.function[i], data.OpenShop);
                    }
                    if (re != null)
                    {
                        _functions.Add(re);
                    }
                }
            }

            //任务功能本身
            data.CreateTaskFunctions(ref _functions);
            data.CreateTaskGoalFunctions(ref _functions);
            return _functions;
        }

        static void CreateTaskFunctions(this CSVNpc.Data data, ref List<FunctionBase> functionBases)
        {
            if (!CSVNpcFunctionData.Instance.TryGetValue(data.id, out CSVNpcFunctionData.Data functionData))
                return;

            var taskDatas = functionData.taskFunctions;
            if (taskDatas != null && taskDatas.Count > 0)
            {
                for (int i = 0, len = taskDatas.Count; i < len; i++)
                {
                    FunctionBase re = InitializeTaskFunction(taskDatas[i]);
                    if (re != null)
                    {
                        functionBases.Add(re);
                    }
                }
            }
        }

        static void CreateTaskGoalFunctions(this CSVNpc.Data data, ref List<FunctionBase> functionBases)
        {
            if (CSVNpcFunctionData.Instance.GetConfData(data.id) == null)
                return;
            var taskDatas = CSVNpcFunctionData.Instance.GetConfData(data.id).taskGoalFunctions;
            if (taskDatas != null && taskDatas.Count > 0)
            {
                for (int i = 0, len = taskDatas.Count; i < len; i++)
                {
                    FunctionBase re = InitializeTaskGoalFunction(taskDatas[i]);
                    if (re != null)
                    {
                        functionBases.Add(re);
                    }
                }
            }
        }

        static FunctionBase InitializeTaskFunction(List<uint> strs)
        {
            FunctionBase functionBase = FunctionManager.CreateFunction((EFunctionType)strs[0]);

            if (functionBase != null)
            {
                functionBase.DeserializeObject(strs);
                functionBase.Init();
                functionBase.CreateConditions();
            }
            else
            {
                DebugUtil.LogError($"ERROR!!! new() Function failed, FunctionType:{((EFunctionType)(strs[0])).ToString()}");
            }

            return functionBase;
        }

        static FunctionBase InitializeTaskGoalFunction(List<uint> strs)
        {
            FunctionBase functionBase = FunctionManager.CreateFunction((EFunctionType)strs[0]);

            if (functionBase != null)
            {
                functionBase.DeserializeObject(strs, true);
                functionBase.Init();
                functionBase.CreateConditions();
            }
            else
            {
                DebugUtil.LogError($"ERROR!!! new() Function failed, FunctionType:{((EFunctionType)(strs[0])).ToString()}");
            }

            return functionBase;
        }

        /// <summary>
        /// 发序列化生成功能///
        /// </summary>
        /// <param name="strs"></param>
        /// <param name="conditionID"></param>
        /// <returns></returns>
        static FunctionBase InitializeFunction(List<uint> strs, List<uint> openShop, uint conditionID = 0)
        {
            FunctionBase functionBase = FunctionManager.CreateFunction((EFunctionType)strs[0]);

            if (functionBase != null)
            {
                functionBase.DeserializeObject(strs);
                if ((EFunctionType)(strs[0]) == EFunctionType.Shop)
                {
                    functionBase.DeserializeObjectExt(openShop);
                }
                functionBase.ConditionGroupID = conditionID;
                functionBase.Init();
            }
            else
            {
                DebugUtil.LogError($"ERROR!!! new() Function failed, FunctionType:{((EFunctionType)(strs[0])).ToString()}");
            }

            return functionBase;
        }

        public static List<uint> AcquiesceDialogue(this CSVNpc.Data data)
        {
            //if (acquiesceDialogue == null)
            //{
            //    List<uint> dias = new List<uint>();
            //    dias.Add(100000);
            //    return dias;
            //}
            return data.acquiesceDialogue;
        }
    }
}
