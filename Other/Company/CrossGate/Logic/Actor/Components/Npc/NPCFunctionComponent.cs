using System.Collections.Generic;
using Table;

namespace Logic
{
    /// <summary>
    /// NPC功能组件///
    /// </summary>
    public class NPCFunctionComponent : Logic.Core.Component
    {
        public Npc Npc
        {
            get;
            private set;
        }

        List<FunctionBase> _functions = new List<FunctionBase>();
        static List<FunctionBase> allFunctions;

        List<FunctionBase> _filteredFunctions = new List<FunctionBase>(16);

        protected override void OnConstruct()
        {
            base.OnConstruct();

            Npc = actor as Npc;

            _filteredFunctions?.Clear();

            _functions?.Clear();
            _functions = Npc.cSVNpcData.CreateFunctionBases();
            for (int index = 0, len = _functions.Count; index < len; index++)
            {
                _functions[index].npc = Npc;
                _functions[index].InitCompleted();
            }

            Sys_Title.Instance.eventEmitter.Handle<uint>(Sys_Title.EEvents.OnTitleGet, OnNpcGetTitleRes, true);
        }

        protected override void OnDispose()
        {
            _filteredFunctions?.Clear();

            if (_functions != null)
            {
                for (int index = 0, len = _functions.Count; index < len; index++)
                {
                    _functions[index].Dispose();
                }
                _functions.Clear();
            }

            Npc = null;
            Sys_Title.Instance.eventEmitter.Handle<uint>(Sys_Title.EEvents.OnTitleGet, OnNpcGetTitleRes, false);

            base.OnDispose();
        }

        /// <summary>
        /// 回调:声望称号发生改变///
        /// </summary>
        void OnNpcGetTitleRes(uint titleId)
        {
            for (int index = 0, len = _functions.Count; index < len; index++)
            {
                if (_functions[index].Type == EFunctionType.Prestige)
                {
                    PrestigeFunction prestigeFunction = _functions[index] as PrestigeFunction;
                    prestigeFunction?.ResetDesc();
                }
            }
        }

        /// <summary>
        /// 过滤功能，获得当前可用的功能///
        /// </summary>
        /// <returns></returns>
        public List<FunctionBase> FilterFunctions()
        {
            _filteredFunctions.Clear();
            if (_functions == null)
                return _filteredFunctions;

            for (int index = 0, len = _functions.Count; index < len; index++)
            {
                if (_functions[index] != null && _functions[index].IsValid())
                    _filteredFunctions.Add(_functions[index]);
            }

            return _filteredFunctions;
        }

        public List<FunctionBase> GetAllFunctions()
        {
            return _functions;
        }

        /// <summary>
        /// 过滤功能，获得当前可用的功能(静态工具方法)///
        /// </summary>
        /// <param name="cSVNpcData"></param>
        /// <param name="filteredFunctions"></param>
        public static void FilterFunctions(CSVNpc.Data cSVNpcData, out List<FunctionBase> filteredFunctions)
        {
            allFunctions = cSVNpcData.CreateFunctionBases();
            filteredFunctions = new List<FunctionBase>();
            for (int index = 0, len = allFunctions.Count; index < len; index++)
            {
                if (allFunctions[index].IsValid())
                    filteredFunctions.Add(allFunctions[index]);
            }
        }

        /// <summary>
        /// 是否含有指定类型的功能///
        /// </summary>
        /// <param name="functionType"></param>
        /// <returns></returns>
        public bool HasFunction(EFunctionType functionType)
        {
            for (int index = 0, len = _functions.Count; index < len; index++)
            {
                if (functionType == _functions[index].Type)
                {
                    return true;
                }
            }
            return false;
        }

        public static List<TaskFunction> GetTaskFunctions(uint npcInfoID)
        {
            List<TaskFunction> taskFunctions = new List<TaskFunction>();

            Npc npc;
            if (GameCenter.uniqueNpcs.TryGetValue(npcInfoID, out npc))
            {
                List<FunctionBase> functionBases = npc.NPCFunctionComponent.GetAllFunctions();
                for (int index = 0, len = functionBases.Count; index < len; index++)
                {
                    if (functionBases[index].Type == EFunctionType.Task)
                    {
                        taskFunctions.Add(functionBases[index] as TaskFunction);
                    }
                }
            }

            return taskFunctions;
        }

        /// <summary>
        /// 是否含有指定类型的可用功能///
        /// </summary>
        /// <param name="functionType"></param>
        /// <returns></returns>
        public bool HasActiveFunction(EFunctionType functionType)
        {
            for (int index = 0, len = _functions.Count; index < len; index++)
            {
                if (functionType == _functions[index].Type && _functions[index].IsValid())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
