using Lib.AssetLoader;
using Lib.Core;
using System.Collections.Generic;
using System.IO;

using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections;
using UnityEngine;

namespace Framework.Table
{
    public class FCSVTask
    {
	    sealed public class Data
	    {
			public uint id { get; private set; }
			public uint taskName { get; private set; }
			public uint taskDescribe { get; private set; }
			public readonly List<uint> taskContent;
			public int taskCategory { get; private set; }
			public readonly List<int> taskGoals;
			public readonly List<List<int>> taskGoalsData;
			public bool taskGoalExecDependency { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly List<int> TargetOrder;
			public uint AcceotType { get; private set; }
			public uint receiveNpc { get; private set; }
			public uint submitNpc { get; private set; }
			public bool conditionType { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
			public uint TalkChooseId { get; private set; }
			public uint InvestigateCondition { get; private set; }
			public uint taskLvLowerLimit { get; private set; }
			public uint taskLvUpperLimit { get; private set; }
			public uint taskAdventureLv { get; private set; }
			public uint taskDetectiveLv { get; private set; }
			public uint LifeSkillLv { get; private set; }
			public uint occupationLimit { get; private set; }
			public readonly List<uint> preposeTask;
			public uint ExecuteLvLowerLimit { get; private set; }
			public uint ExecuteLvUpperLimit { get; private set; }
			public uint LvConfineTips { get; private set; }
			public uint LvUpperTips { get; private set; }
			public uint WhetherTips { get; private set; }
			public readonly List<int> notAcceptedState;
			public readonly List<int> acceptedState;
			public readonly List<int> completeState;
			public bool automaticPathfinding { get { return ReadHelper.GetBoolByIndex(boolArray0, 2); } }
			public bool taskShate { get { return ReadHelper.GetBoolByIndex(boolArray0, 3); } }
			public bool outdoorFight { get { return ReadHelper.GetBoolByIndex(boolArray0, 4); } }
			public bool whetherAbandon { get { return ReadHelper.GetBoolByIndex(boolArray0, 5); } }
			public bool whetherReset { get { return ReadHelper.GetBoolByIndex(boolArray0, 6); } }
			public bool CanManualTrace { get { return ReadHelper.GetBoolByIndex(boolArray0, 7); } }
			public bool cancelTrace { get { return ReadHelper.GetBoolByIndex(boolArray1, 0); } }
			public bool TraceGuide { get { return ReadHelper.GetBoolByIndex(boolArray1, 1); } }
			public uint rewardType { get; private set; }
			public uint specialRewardShow { get; private set; }
			public uint taskMap { get; private set; }
			public readonly List<uint> taskTriggerMap;
			public readonly List<uint> taskTraceMap;
			public bool WhetherShowProgressBar { get { return ReadHelper.GetBoolByIndex(boolArray1, 2); } }
			public readonly List<int> ShowUIId;
			public readonly List<uint> ChapterExpressionId;
			public readonly List<uint> SubtitleSpeakAside;
			public readonly List<uint> DropId;
			public readonly List<uint> RewardExp;
			public readonly List<uint> RewardGold;
			public bool TaskCompleteTips { get { return ReadHelper.GetBoolByIndex(boolArray1, 3); } }
			public bool TaskTraceTips { get { return ReadHelper.GetBoolByIndex(boolArray1, 4); } }
			public readonly List<uint> TaskInstanceID;
			public bool TaskWhetherRepeat { get { return ReadHelper.GetBoolByIndex(boolArray1, 5); } }
			public uint taskChapter { get; private set; }
			public uint taskStep { get; private set; }
			public uint TaskExecTask { get; private set; }
			public uint TaskSkipPlot { get; private set; }
			public uint TaskSkipPlotID { get; private set; }
			public uint LvConfineTipsUI { get; private set; }
		private readonly byte boolArray0;
		private readonly byte boolArray1;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				taskName = ReadHelper.ReadUInt(binaryReader);
				taskDescribe = ReadHelper.ReadUInt(binaryReader);
				taskContent = shareData.GetShareData<List<uint>>(binaryReader, 0);
				taskCategory = ReadHelper.ReadInt(binaryReader);
				taskGoals = shareData.GetShareData<List<int>>(binaryReader, 1);
				taskGoalsData = shareData.GetShareData<List<List<int>>>(binaryReader, 2);
				TargetOrder = shareData.GetShareData<List<int>>(binaryReader, 1);
				AcceotType = ReadHelper.ReadUInt(binaryReader);
				receiveNpc = ReadHelper.ReadUInt(binaryReader);
				submitNpc = ReadHelper.ReadUInt(binaryReader);
				TalkChooseId = ReadHelper.ReadUInt(binaryReader);
				InvestigateCondition = ReadHelper.ReadUInt(binaryReader);
				taskLvLowerLimit = ReadHelper.ReadUInt(binaryReader);
				taskLvUpperLimit = ReadHelper.ReadUInt(binaryReader);
				taskAdventureLv = ReadHelper.ReadUInt(binaryReader);
				taskDetectiveLv = ReadHelper.ReadUInt(binaryReader);
				LifeSkillLv = ReadHelper.ReadUInt(binaryReader);
				occupationLimit = ReadHelper.ReadUInt(binaryReader);
				preposeTask = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ExecuteLvLowerLimit = ReadHelper.ReadUInt(binaryReader);
				ExecuteLvUpperLimit = ReadHelper.ReadUInt(binaryReader);
				LvConfineTips = ReadHelper.ReadUInt(binaryReader);
				LvUpperTips = ReadHelper.ReadUInt(binaryReader);
				WhetherTips = ReadHelper.ReadUInt(binaryReader);
				notAcceptedState = shareData.GetShareData<List<int>>(binaryReader, 1);
				acceptedState = shareData.GetShareData<List<int>>(binaryReader, 1);
				completeState = shareData.GetShareData<List<int>>(binaryReader, 1);
				rewardType = ReadHelper.ReadUInt(binaryReader);
				specialRewardShow = ReadHelper.ReadUInt(binaryReader);
				taskMap = ReadHelper.ReadUInt(binaryReader);
				taskTriggerMap = shareData.GetShareData<List<uint>>(binaryReader, 0);
				taskTraceMap = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ShowUIId = shareData.GetShareData<List<int>>(binaryReader, 1);
				ChapterExpressionId = shareData.GetShareData<List<uint>>(binaryReader, 0);
				SubtitleSpeakAside = shareData.GetShareData<List<uint>>(binaryReader, 0);
				DropId = shareData.GetShareData<List<uint>>(binaryReader, 0);
				RewardExp = shareData.GetShareData<List<uint>>(binaryReader, 0);
				RewardGold = shareData.GetShareData<List<uint>>(binaryReader, 0);
				TaskInstanceID = shareData.GetShareData<List<uint>>(binaryReader, 0);
				taskChapter = ReadHelper.ReadUInt(binaryReader);
				taskStep = ReadHelper.ReadUInt(binaryReader);
				TaskExecTask = ReadHelper.ReadUInt(binaryReader);
				TaskSkipPlot = ReadHelper.ReadUInt(binaryReader);
				TaskSkipPlotID = ReadHelper.ReadUInt(binaryReader);
				LvConfineTipsUI = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
			boolArray1 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

        protected Dictionary<uint, int> Datas;
        protected List<Data> DataList;

        public int Count { get { return DataList.Count; } }

        public void ReadByFilePath(string path)
        {            
            Stream stream = AssetMananger.Instance.LoadStream(path);
            BinaryReader binaryReader = new BinaryReader(stream);
            Read(path, binaryReader);
            binaryReader.Close();
            stream.Close();
        }

        public void Read(string path, BinaryReader binaryReader)
        {
            if (binaryReader == null)
            {
                DebugUtil.LogErrorFormat("{0} binaryReader为空", path);
                return;
            }

            if (Datas != null)
            {
                DebugUtil.LogErrorFormat("{0} 多次读取配置", path);
                return;
            }

            TableShareData shareData = OnReadShareData(binaryReader);

            //buff的实际长度
            int bufferSize = binaryReader.ReadInt32();

            //读取内容
            int count = binaryReader.ReadInt32();

            Datas = new Dictionary<uint, int>(count);
            DataList = new List<Data>(count);

            if (count <= 0)
            {
                DebugUtil.LogErrorFormat("{0} 是空表", path);
                return;
            }

            int entrySize = bufferSize / count;

            if (entrySize * count != bufferSize)
            {
                DebugUtil.LogErrorFormat("{0} 数据长度异常", path);
                return;
            }

            for (int i = 0; i < count; ++i)
            {
#if DEBUG_MODE
                try
                {                                        
                    uint id = ReadHelper.ReadUInt(binaryReader);
                    Data data = new Data(id, binaryReader, shareData);
                    if (Datas.ContainsKey(id))
                    {
                        DebugUtil.LogErrorFormat("表格 {1} 有重复id{0}", id.ToString(), path);
                    }
                    Datas[id] = i;
                    DataList.Add(data);
                }
                catch (System.Exception e)
                {
                    DebugUtil.LogErrorFormat("表格 {2} 第 {0} 行错误： {1}", (i + 1).ToString(), e.StackTrace, path);
                    break;
                }
#else
                uint id = ReadHelper.ReadUInt(binaryReader);
                Data data = new Data(id, binaryReader, shareData);
                Datas[id] = i;
                DataList.Add(data);
#endif
            }
        }

        public Data GetConfData(uint id)
        {
            if (Datas.TryGetValue(id, out int index))
                return GetByIndex(index);
            return null;
        }

        public Data GetByIndex(int index)
        {
            Data tableData = DataList[index];
            return tableData;
        }

        public bool TryGetValue(uint id, out Data data)
        {
            if (Datas.TryGetValue(id, out int index))
            {
                data = GetByIndex(index);
                return true;
            }

            data = null;
            return false;
        }

        public IReadOnlyList<Data> GetAll()
        {
            return DataList;
        }

        public Dictionary<uint, int>.KeyCollection GetKeys()
        {
            return Datas.Keys;
        }

        public bool ContainsKey(uint id)
        {
            return Datas.ContainsKey(id);
        }

        public void Clear()
        {
            Datas?.Clear();
            DataList?.Clear();            
        }

        public static TableShareData OnReadShareData(BinaryReader binaryReader)
        {
			TableShareData shareData = new TableShareData(3);
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArrays<int>(binaryReader, 1, ReadHelper.ReadArray_ReadInt);
			shareData.ReadArray2s<int>(binaryReader, 2, 1);

            return shareData;
        }
    }

    public class FCSVTaskAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FCSVTask);
            }
        }
    
        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }
    
        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }
    
        public class Adapter : FCSVTask, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;
    
            public Adapter()
            {
            
            }
    
            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }
    
            public ILTypeInstance ILInstance { get { return instance; } }
    
            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return instance.ToString();
                }
                else
                    return instance.Type.FullName;
            }
        }
    }
}