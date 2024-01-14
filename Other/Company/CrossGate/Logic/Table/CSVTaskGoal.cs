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

	sealed public partial class CSVTaskGoal : Framework.Table.TableBase<CSVTaskGoal.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint TaskID;
			public readonly uint PathfindingType;
			public readonly uint PathfindingMap;
			public readonly uint PathfindingTargetID;
			public readonly uint GenerateFunctionType;
			public readonly uint FunctionParameter;
			public readonly uint TitleText;
			public readonly uint FunctionFrontDialogue;
			public readonly uint FunctionFrontAnimation;
			public readonly uint TargetType;
			public readonly uint TargetParameter1;
			public readonly uint TargetParameter2;
			public readonly uint TargetParameter3;
			public readonly uint TargetParameter4;
			public readonly uint TargetParameter5;
			public readonly uint TargetParameter6;
			public readonly uint TargetParameter7;
			public bool automaticImplement { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public bool WhetherWayfinding { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
			public bool PathfindingDot { get { return ReadHelper.GetBoolByIndex(boolArray0, 2); } }
			public readonly uint TaskOpenUiId;
			public readonly List<List<uint>> TaskOpenGuideId;
			public readonly uint WayfindingTips;
			public readonly List<uint> EscortNpc;
			public readonly uint MoveSpeed;
			public readonly List<List<uint>> EscortNpcLocation;
			public readonly uint EscortNpcOrientations;
			public readonly uint EscortNpcTriggerDis;
			public readonly List<List<uint>> EscortLocation;
			public readonly uint EscortTips;
			public readonly uint EscortStartTips;
			public readonly uint EscortCompleteTips;
			public readonly List<uint> TracingNpc;
			public readonly uint TracingMoveSpeed;
			public readonly List<List<uint>> TracingNpcLocation;
			public readonly uint TracingNpcOrientations;
			public readonly uint TracingNpcTriggerDis;
			public readonly List<List<uint>> TracingLocation;
			public readonly uint TracingTips;
			public readonly uint TracingAlertScope;
			public readonly uint TracingAlertTime;
			public readonly uint TracingPunishWay;
			public readonly uint PunishNpcMoveSpeed;
			public readonly string PunishNpcMoveAction;
			public readonly uint TracingFailDistance;
			public readonly uint FailDistanceTime;
			public readonly uint PunishFight;
			public readonly uint TracingStartTips;
			public readonly uint TracingCompleteTips;
			public readonly uint TracingFailTips;
			public readonly List<uint> FollowNpc;
			public readonly uint FollowMoveSpeed;
			public readonly List<List<uint>> FollowNpcLocation;
			public readonly uint FollowNpcOrientations;
			public readonly List<uint> FollowNpcBubble;
			public readonly List<uint> FollowNpcBubbleInterval;
			public readonly List<uint> FollowNpcTargetPlace;
			public readonly uint FollowNpcTargetScope;
			public readonly uint FollowTips;
			public readonly uint FollowStartTips;
			public readonly uint FollowCompleteTips;
			public readonly uint TaskFrontPerform;
			public readonly uint TaskAfterPerform;
			public readonly uint LimitTime;
			public readonly uint LimitOpenNpc;
			public readonly uint LimitOpenType;
			public readonly uint LimitOpenChoose;
			public readonly uint LimitOpenText;
			public readonly uint FunctionOpenList;
			public bool WayfindingOpenIncense { get { return ReadHelper.GetBoolByIndex(boolArray0, 3); } }
			public bool StopTrusteeship { get { return ReadHelper.GetBoolByIndex(boolArray0, 4); } }
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				TaskID = ReadHelper.ReadUInt(binaryReader);
				PathfindingType = ReadHelper.ReadUInt(binaryReader);
				PathfindingMap = ReadHelper.ReadUInt(binaryReader);
				PathfindingTargetID = ReadHelper.ReadUInt(binaryReader);
				GenerateFunctionType = ReadHelper.ReadUInt(binaryReader);
				FunctionParameter = ReadHelper.ReadUInt(binaryReader);
				TitleText = ReadHelper.ReadUInt(binaryReader);
				FunctionFrontDialogue = ReadHelper.ReadUInt(binaryReader);
				FunctionFrontAnimation = ReadHelper.ReadUInt(binaryReader);
				TargetType = ReadHelper.ReadUInt(binaryReader);
				TargetParameter1 = ReadHelper.ReadUInt(binaryReader);
				TargetParameter2 = ReadHelper.ReadUInt(binaryReader);
				TargetParameter3 = ReadHelper.ReadUInt(binaryReader);
				TargetParameter4 = ReadHelper.ReadUInt(binaryReader);
				TargetParameter5 = ReadHelper.ReadUInt(binaryReader);
				TargetParameter6 = ReadHelper.ReadUInt(binaryReader);
				TargetParameter7 = ReadHelper.ReadUInt(binaryReader);
				TaskOpenUiId = ReadHelper.ReadUInt(binaryReader);
				TaskOpenGuideId = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				WayfindingTips = ReadHelper.ReadUInt(binaryReader);
				EscortNpc = shareData.GetShareData<List<uint>>(binaryReader, 1);
				MoveSpeed = ReadHelper.ReadUInt(binaryReader);
				EscortNpcLocation = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				EscortNpcOrientations = ReadHelper.ReadUInt(binaryReader);
				EscortNpcTriggerDis = ReadHelper.ReadUInt(binaryReader);
				EscortLocation = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				EscortTips = ReadHelper.ReadUInt(binaryReader);
				EscortStartTips = ReadHelper.ReadUInt(binaryReader);
				EscortCompleteTips = ReadHelper.ReadUInt(binaryReader);
				TracingNpc = shareData.GetShareData<List<uint>>(binaryReader, 1);
				TracingMoveSpeed = ReadHelper.ReadUInt(binaryReader);
				TracingNpcLocation = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				TracingNpcOrientations = ReadHelper.ReadUInt(binaryReader);
				TracingNpcTriggerDis = ReadHelper.ReadUInt(binaryReader);
				TracingLocation = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				TracingTips = ReadHelper.ReadUInt(binaryReader);
				TracingAlertScope = ReadHelper.ReadUInt(binaryReader);
				TracingAlertTime = ReadHelper.ReadUInt(binaryReader);
				TracingPunishWay = ReadHelper.ReadUInt(binaryReader);
				PunishNpcMoveSpeed = ReadHelper.ReadUInt(binaryReader);
				PunishNpcMoveAction = shareData.GetShareData<string>(binaryReader, 0);
				TracingFailDistance = ReadHelper.ReadUInt(binaryReader);
				FailDistanceTime = ReadHelper.ReadUInt(binaryReader);
				PunishFight = ReadHelper.ReadUInt(binaryReader);
				TracingStartTips = ReadHelper.ReadUInt(binaryReader);
				TracingCompleteTips = ReadHelper.ReadUInt(binaryReader);
				TracingFailTips = ReadHelper.ReadUInt(binaryReader);
				FollowNpc = shareData.GetShareData<List<uint>>(binaryReader, 1);
				FollowMoveSpeed = ReadHelper.ReadUInt(binaryReader);
				FollowNpcLocation = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				FollowNpcOrientations = ReadHelper.ReadUInt(binaryReader);
				FollowNpcBubble = shareData.GetShareData<List<uint>>(binaryReader, 1);
				FollowNpcBubbleInterval = shareData.GetShareData<List<uint>>(binaryReader, 1);
				FollowNpcTargetPlace = shareData.GetShareData<List<uint>>(binaryReader, 1);
				FollowNpcTargetScope = ReadHelper.ReadUInt(binaryReader);
				FollowTips = ReadHelper.ReadUInt(binaryReader);
				FollowStartTips = ReadHelper.ReadUInt(binaryReader);
				FollowCompleteTips = ReadHelper.ReadUInt(binaryReader);
				TaskFrontPerform = ReadHelper.ReadUInt(binaryReader);
				TaskAfterPerform = ReadHelper.ReadUInt(binaryReader);
				LimitTime = ReadHelper.ReadUInt(binaryReader);
				LimitOpenNpc = ReadHelper.ReadUInt(binaryReader);
				LimitOpenType = ReadHelper.ReadUInt(binaryReader);
				LimitOpenChoose = ReadHelper.ReadUInt(binaryReader);
				LimitOpenText = ReadHelper.ReadUInt(binaryReader);
				FunctionOpenList = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTaskGoal.bytes";
		}

		private static CSVTaskGoal instance = null;			
		public static CSVTaskGoal Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTaskGoal 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTaskGoal forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTaskGoal();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTaskGoal");

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
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 2, 1);

			return shareData;
		}
	}

#else

    sealed public partial class CSVTaskGoal : FCSVTaskGoal
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTaskGoal.bytes";
		}

		private static CSVTaskGoal instance = null;			
		public static CSVTaskGoal Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTaskGoal 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTaskGoal forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTaskGoal();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTaskGoal");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}