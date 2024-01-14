//
#define USE_HOTFIX_LOGIC

using Lib.AssetLoader;
using Lib.Core;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Framework.Table;

namespace Table
{
#if USE_HOTFIX_LOGIC

	sealed public partial class CSVTask : Framework.Table.TableBase<CSVTask.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint taskName;
			public readonly uint taskDescribe;
			public readonly List<uint> taskContent;
			public readonly int taskCategory;
			public readonly List<int> taskGoals;
			public readonly List<List<int>> taskGoalsData;
			public bool taskGoalExecDependency { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly List<int> TargetOrder;
			public readonly uint AcceotType;
			public readonly uint receiveNpc;
			public readonly uint submitNpc;
			public bool conditionType { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
			public readonly uint TalkChooseId;
			public readonly uint InvestigateCondition;
			public readonly uint taskLvLowerLimit;
			public readonly uint taskLvUpperLimit;
			public readonly uint taskAdventureLv;
			public readonly uint taskDetectiveLv;
			public readonly uint LifeSkillLv;
			public readonly uint occupationLimit;
			public readonly List<uint> preposeTask;
			public readonly uint ExecuteLvLowerLimit;
			public readonly uint ExecuteLvUpperLimit;
			public readonly uint LvConfineTips;
			public readonly uint LvUpperTips;
			public readonly uint WhetherTips;
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
			public readonly uint rewardType;
			public readonly uint specialRewardShow;
			public readonly uint taskMap;
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
			public readonly uint taskChapter;
			public readonly uint taskStep;
			public readonly uint TaskExecTask;
			public readonly uint TaskSkipPlot;
			public readonly uint TaskSkipPlotID;
			public readonly uint LvConfineTipsUI;
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

		public static string ConfigPath()
		{
			return "Config/Table/CSVTask.bytes";
		}

		private static CSVTask instance = null;			
		public static CSVTask Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTask 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTask forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTask();
                instance.ReadByFilePath(ConfigPath(), OnCreat, OnReadShareData);
            }
            else if (forceReload)
            {
                instance.Clear();
                instance.ReadByFilePath(ConfigPath(), OnCreat, OnReadShareData);
            }
        }

        public static void Unload()
        {
            DebugUtil.Log(ELogType.eTable, "卸载CSVTask");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }

        private static Data OnCreat(uint id, BinaryReader binaryReader, TableShareData shareData)
        {
            Data data = new Data(id, binaryReader, shareData);
            return data;
        }

        private static TableShareData OnReadShareData(BinaryReader binaryReader)
		{
			TableShareData shareData = new TableShareData(3);
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArrays<int>(binaryReader, 1, ReadHelper.ReadArray_ReadInt);
			shareData.ReadArray2s<int>(binaryReader, 2, 1);

			return shareData;
		}
	}

#else

    sealed public partial class CSVTask : FCSVTask
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTask.bytes";
		}

		private static CSVTask instance = null;			
		public static CSVTask Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTask 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTask forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTask();
                instance.ReadByFilePath(ConfigPath());
            }
            else if (forceReload)
            {
                instance.Clear();
                instance.ReadByFilePath(ConfigPath());
            }
        }

        public static void Unload()
        {
            DebugUtil.Log(ELogType.eTable, "卸载CSVTask");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}